//#if !UNITY_WEBGL
//using UnityEngine;
//using UnityEngine.AddressableAssets;
//using System.Collections.Generic;
//using UnityEngine.ResourceManagement.AsyncOperations;
//using System.Collections;

//namespace Nagih
//{
//    public class AddressableManager : MonoBehaviour
//    {
//        private Dictionary<string, AsyncOperationHandle<object>> _handleDictionary;
//        private Dictionary<string, Sprite> _spriteDictionary;

//        private void Awake()
//        {
//            _handleDictionary = new Dictionary<string, AsyncOperationHandle<object>>();
//            _spriteDictionary = new Dictionary<string, Sprite>();
//        }

//        public IEnumerator LoadAssetsFromSceneData(SceneData data)
//        {
//            if (data == null) yield break;

//            if (data.IsLoadBundle)
//            {
//                string key = data.GetAddressableKey();
//                yield return StartCoroutine(LoadLabel(key));
//            }
//        }

//        public IEnumerator ReleaseAssetsFromSceneData(SceneData data)
//        {
//            if (data == null) yield break;

//            if (data.IsLoadBundle)
//            {
//                string key = data.GetAddressableKey();
//                yield return StartCoroutine(ReleaseLabel(key));
//            }
//        }

//        public IEnumerator LoadLabel(string labelName)
//        {
//            var locHandle = Addressables.LoadResourceLocationsAsync(labelName);
//            yield return locHandle;

//            foreach (var loc in locHandle.Result)
//            {
//                var objHandle = Addressables.LoadAssetAsync<object>(loc);
//                yield return objHandle;

//                if (objHandle.Status == AsyncOperationStatus.Succeeded)
//                {
//                    string key = loc.PrimaryKey;
//                    ReleaseIfContains(key);
//                    _handleDictionary.Add(key, objHandle);
//                }
//            }
//            Addressables.Release(locHandle);
//        }

//        public IEnumerator ReleaseLabel(string labelName)
//        {
//            var locHandle = Addressables.LoadResourceLocationsAsync(labelName);
//            yield return locHandle;

//            foreach (var loc in locHandle.Result)
//            {
//                string key = loc.PrimaryKey;
//                ReleaseIfContains(key);
//            }
//            Addressables.Release(locHandle);
//        }

//        // only use GetAsset when you are sure the object already loaded in memory
//        public T GetAsset<T>(string filename)
//        {
//            return (T)_handleDictionary[filename].Result;
//        }

//        // only use this once when initialization, otherwise it will costly
//        public Sprite GetSprite(string filename)
//        {
//            if (!_spriteDictionary.ContainsKey(filename))
//            {
//                Texture2D texture = GetAsset<Texture2D>(filename);
//                _spriteDictionary[filename] = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
//            }

//            return _spriteDictionary[filename];
//        }

//        public void ReleaseAsset(string filename)
//        {
//            if (_handleDictionary.ContainsKey(filename))
//            {
//                Addressables.Release(_handleDictionary[filename]);
//                _handleDictionary.Remove(filename);
//            }
//        }

//        public IEnumerator LoadAsset(string filename)
//        {
//            var handle = Addressables.LoadAssetAsync<object>(filename);
//            yield return handle;

//            if(handle.Status == AsyncOperationStatus.Succeeded)
//            {
//                ReleaseIfContains(filename);
//                Debug.Log(handle.Result.GetType());
//                _handleDictionary.Add(filename, handle);
//            }
//        }

//        private void ReleaseIfContains(string key)
//        {
//            if (_handleDictionary.ContainsKey(key))
//            {
//                var hdl = _handleDictionary[key];
//                _handleDictionary.Remove(key);
//                Addressables.Release(hdl);
//            }
//        }
//    }
//}
//#endif