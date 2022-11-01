using UnityEngine;
using Proyecto26;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Threading.Tasks;
using UnityEngine.Events;
using Newtonsoft.Json.Linq;
using Lean.Localization;
using System.Collections;

namespace Nagih
{
    public class RequestManager : MonoBehaviour
    {
        private string _userId;
        private string _token;

        private Dictionary<Loading, int> _loadingCountDict;
        private Dictionary<Error.Scope, bool> _popupWarningDict;
        private GameObject _frontLoading;
        private GameObject _backLoading;
        private bool _alreadyRefreshToken = false;

        private void Awake()
        {
            _loadingCountDict = new Dictionary<Loading, int>
            {
                { Loading.Front, 0 },
                { Loading.Back, 0 }
            };
            _popupWarningDict = new Dictionary<Error.Scope, bool>();
            foreach (Error.Scope scope in System.Enum.GetValues(typeof(Error.Scope)))
            {
                _popupWarningDict[scope] = false;
            }
        }

        public IEnumerator LoadAsset(GameObject frontLoading, YieldInstruction instruction = null)
        {
            _frontLoading = frontLoading;
            _backLoading = Instantiate(DataStatic.GetInstance().GameDataSO.BackLoading, transform);
            yield return instruction;
        }

        public void Initialize(string userId)
        {
            _userId = userId;
            _token = string.Empty;
        }

        public void SetServerToken(string token)
        {
            _token = token;
        }

        public void Post<T>(string type, IRequestData data, Action<T> callback) where T : IReturnData, new()
        {
            bool canRequest = !string.IsNullOrEmpty(Const.GAME_ID)
                            && (!Const.REQUEST_CHECK_ID.Contains(type) || !string.IsNullOrEmpty(_userId));
            if (!canRequest)
            {
                Debug.Log($"Attempt to Request but UserId is Null. Type:{type} Data:{JsonConvert.SerializeObject(data)}");
                ResponseData response = new ResponseData((int)Error.Type.RequestNotValid);
                callback?.Invoke(response.data.ToObject<T>());
                return;
            }

            // connection data yg dikirim ke server
            ConnectionData<T> connData = new ConnectionData<T>(type, _userId, _token, data, callback);
            Loading loadingType = callback != null ? Loading.Front : Loading.Back;
            SetActiveLoading(loadingType, true);

            string raw = JsonConvert.SerializeObject(connData, Formatting.None,
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            string reqJson = raw;

            if (Const.UseCypher())
            {
                string cypher = Encryption.EncryptAES(raw, Const.SERVER_KEY_SECRET);
                RequestEncryptedData reqCypher = new RequestEncryptedData(cypher);
                reqJson = JsonConvert.SerializeObject(reqCypher, Formatting.None);
            }

            byte[] reqByte = new System.Text.UTF8Encoding().GetBytes(reqJson);
            RequestHelper request = GetGeneralRequest(Const.URL_REQUEST);
            request.BodyRaw = reqByte;
            request.Headers["Authorization"] = GetAuthString(Const.AUTH_USERNAME, Const.AUTH_PASSWORD);

            Debug.Log($"[SEND] Raw:{raw} \nCypher:{reqJson}");
            RestClient.Post(request).Then(responseHelper =>
            {
                ResponseData response = null;
                if (Const.UseCypher())
                {
                    var responseEncrypt = JsonConvert.DeserializeObject<ResponseEncryptedData>(responseHelper.Text,
                                    new JsonSerializerSettings { FloatParseHandling = FloatParseHandling.Double });
                    JToken tokenData = string.IsNullOrEmpty(responseEncrypt.data) ? new JObject()
                                        : JToken.Parse(Encryption.DecryptAES(responseEncrypt.data, Const.SERVER_KEY_SECRET));
                    response = new ResponseData(
                        responseEncrypt.error,
                        responseEncrypt.message,
                        tokenData
                    );
                }
                else
                {
                    response = JsonConvert.DeserializeObject<ResponseData>(responseHelper.Text,
                        new JsonSerializerSettings { FloatParseHandling = FloatParseHandling.Double });
                }

                CheckError(response, connData);
                _alreadyRefreshToken = false;

                Debug.Log($"[RETURN] ReqType:{type} Data:{response.data}");
                Error.Scope scopeError = Error.GetScope(response.error);
                if(scopeError == Error.Scope.Success || scopeError == Error.Scope.Expected)
                {
                    SetActiveLoading(loadingType, false);
                    connData.ExecuteCallback(response.data);
                }
            })
            .Catch(err =>
            {
                Error.Type errorType = CatchError(err);
                ResponseData response = new ResponseData((int)errorType);
                HandleError(errorType, loadingType, () => connData.ExecuteCallback(response.data));
            });
        }

        // for get privacy policy
        public void Post<T>(string type, Action<T> callback) where T : IReturnData, new()
        {
            FormData<T> formData = new FormData<T>(type, callback);
            Loading loadingType = callback != null ? Loading.Front : Loading.Back;
            SetActiveLoading(loadingType, true);

            string raw = JsonConvert.SerializeObject(formData, Formatting.None,
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            byte[] reqByte = new System.Text.UTF8Encoding().GetBytes(raw);

            RequestHelper request = GetGeneralRequest(Const.URL_GET_CONTENT);
            request.BodyRaw = reqByte;
            request.Headers["Authorization"] = GetAuthString(Const.AUTH_FIRA_USERNAME, Const.AUTH_FIRA_PASSWORD);

            Debug.Log($"[SEND] Raw:{raw}");
            RestClient.Post(request).Then(responseHelper =>
            {
                ResponseData response = JsonConvert.DeserializeObject<ResponseData>(responseHelper.Text);
                CheckError(response, formData);

                Debug.Log($"[RETURN] ReqType:{type} Data:{response.data}");
                Error.Scope scopeError = Error.GetScope(response.error);
                if (scopeError == Error.Scope.Success)
                {
                    SetActiveLoading(loadingType, false);
                    formData.ExecuteCallback(response.data);
                }
            })
            .Catch(err =>
            {
                Error.Type errorType = CatchError(err);
                ResponseData response = new ResponseData((int)errorType);
                HandleError(errorType, loadingType, () => formData.ExecuteCallback(response.data));
            });
        }

        private void CheckError<T>(ResponseData response, ConnectionData<T> connData) where T : IReturnData, new()
        {
            if (response.data.Type == JTokenType.Null)
            {
                response.data = new JObject() { ["Error"] = response.error };
            }
            response.data["Error"] = response.error;

            if (response.error != (int)Error.Type.NoError)
            {
                Error.Scope scopeError = Error.GetScope(response.error);
                if (scopeError != Error.Scope.Expected)
                {
                    Debug.LogWarning($"Unexpected error. Scope:{scopeError} Error:{response.error}");
                    Loading type = connData.Callback != null ? Loading.Front : Loading.Back;
                    HandleError((Error.Type)response.error, type, () => connData.ExecuteCallback(response.data));
                }
                else if (!_alreadyRefreshToken && response.error == (int)Error.Type.TokenExpired)
                {
                    ConnectionData<T> prevConnectionData = connData.Clone();
                    TokenRequestData reqData = new TokenRequestData(DataSelf.GetInstance().Device.ServerToken);

                    Post<TokenReturnData>(Const.AUTH_REFRESH_TOKEN, reqData, (returnData) =>
                    {
                        if (returnData.Error == (int)Error.Type.NoError)
                        {
                            SetServerToken(returnData.token);
                            DataSelf.GetInstance().Device.ServerToken = returnData.token;
                            StartCoroutine(Manager.GetInstance().SaveManager.SaveData<UserData>());

                            // ulang request yg sebelumnya
                            _alreadyRefreshToken = true;
                            Post(prevConnectionData.req_type, prevConnectionData.data, prevConnectionData.Callback);
                        }
                    });
                }
            }
        }

        private void CheckError<T>(ResponseData response, FormData<T> formData) where T : IReturnData, new()
        {
            if (response.error != (int)Error.Type.NoError)
            {
                Error.Scope scopeError = Error.GetScope(response.error);
                if (scopeError != Error.Scope.Expected)
                {
                    Debug.LogWarning($"Unexpected error. Scope:{scopeError} Error:{response.error}");
                    Loading type = formData.Callback != null ? Loading.Front : Loading.Back;
                    HandleError((Error.Type)response.error, type, () => formData.ExecuteCallback(response.data));
                }

                if (response.data.Type == JTokenType.Null)
                {
                    response.data = new JObject() { ["Error"] = response.error };
                }
            }

            response.data["Error"] = response.error;
        }

        private string GetAuthString(string username, string password)
        {
            string auth = username + ":" + password;
            auth = Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(auth));
            auth = "Basic " + auth;
            return auth;
        }

        private void HandleError(Error.Type errorType, Loading type, UnityAction callback)
        {
            if (Const.USE_POPUP_WARNING && type == Loading.Front)
            {
                Error.Scope errorScope = Error.GetScope(errorType);
                if(!Manager.GetInstance().Login.IsLoginSequence || !_popupWarningDict[errorScope])
                {
                    _popupWarningDict[errorScope] = true;
                    switch (errorScope)
                    {
                        case Error.Scope.Connection:
                            Manager.GetInstance().Popup.Info
                                .SetLeanText("NoConnection")
                                .SetButtonListener(callback)
                                .Show();
                            break;
                        case Error.Scope.AuthServer:
                            Manager.GetInstance().Popup.Info
                                .SetLeanText("ServerLoginError")
                                .SetButtonListener(callback)
                                .Show();
                            break;
                        case Error.Scope.Unexpected:
                            string errorText = LeanLocalization.GetTranslationText("Popup/RequestError")
                                    .Replace("{err}", ((int)errorType).ToString());
                            Manager.GetInstance().Popup.Info
                                .SetLeanText("RequestError")
                                .SetCustomInfoText(errorText)
                                .SetButtonListener(callback)
                                .Show();
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    callback?.Invoke();
                }
            }
            else
            {
                callback?.Invoke();
            }

            SetActiveLoading(type, false);
        }

        private Error.Type CatchError(Exception err)
        {
            Error.Type errorType = Error.Type.Unexpected;
            string message = $"[UNEXPECTED ERROR] Message:{err.Message}.";
            if (err is RequestException)
            {
                RequestException exception = (RequestException)err;
                if (exception.IsHttpError || exception.IsNetworkError)
                {
                    message = $"[NO CONNECTION] StatusCode: {exception.StatusCode} Message:{exception.Message}";
                    errorType = Error.Type.NoConnection;
                }
            }

            Debug.LogWarning(message);
            return errorType;
        }

        private void SetActiveLoading(Loading type, bool isActive)
        {
            int increment = isActive ? 1 : -1;
            _loadingCountDict[type] = Mathf.Max(_loadingCountDict[type] + increment, 0);

            GameObject loading = type == Loading.Front ? _frontLoading : _backLoading;
            loading.SetActive(_loadingCountDict[type] > 0);
        }

        private RequestHelper GetGeneralRequest(string url)
        {
            return new RequestHelper
            {
                Uri = url,
                Timeout = Const.TIMEOUT_DURATION,
                Headers = new Dictionary<string, string>(),
                EnableDebug = false
            };
        }

        public void CanConnectToServer(Action<bool> callback)
        {
            RestClient.Get("https://gamesmanager.nagihgames.com/app-status").Then(responseHelper =>
            {
                callback(responseHelper.Error == null);
            })
            .Catch(err =>
            {
                callback(false);
            });
        }

        private enum Loading
        {
            Front = 0,
            Back = 1
        }
    }
}