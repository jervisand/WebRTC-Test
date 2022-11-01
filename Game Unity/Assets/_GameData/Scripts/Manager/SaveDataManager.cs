using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Nagih
{
    public class SaveDataManager : MonoBehaviour
    {
        private DataSelf _dataSelf;
        private string FileDirPath;
        private string FileType = ".json";

        private void Awake()
        {
            FileDirPath = Application.persistentDataPath;
            Debug.Log("Data Save on : " + FileDirPath);
        }

        public IEnumerator Initialize()
        {
            WaitForSeconds waitDelay = new WaitForSeconds(Const.ROUTINE_DURATION);
            ExampleClass example = new ExampleClass();
            yield return waitDelay;
            string json = JsonConvert.SerializeObject(example);
            yield return waitDelay;
        }

        public IEnumerator SaveData<T>() where T : ISaveData
        {
            if (_dataSelf == null) _dataSelf = DataSelf.GetInstance();

            string varName = GetKey<T>();
            string fullPath = Path.Combine(FileDirPath, varName + FileType);

            PropertyInfo field = _dataSelf.GetType().GetProperty(varName);
            T obj = (T)field.GetValue(_dataSelf);

            yield return StartCoroutine(SaveFileData(fullPath,varName, obj));
        }

        public IEnumerator LoadData<T>(Action<T> Callback) where T : ISaveData, new()
        {
            WaitForSeconds waitDelay = new WaitForSeconds(Const.ROUTINE_DURATION);

            string varName = GetKey<T>();
            string fullPath = Path.Combine(FileDirPath, varName + FileType);
            yield return waitDelay;

            bool HaveData = false;

#if UNITY_WEBGL
            if (PlayerPrefs.HasKey(varName))HaveData = true;
#else
            if (File.Exists(fullPath)) HaveData = true;
#endif

            if (HaveData)
            {
                int fileVersion = 0;
                JObject ObjectData = null;
#if UNITY_WEBGL
                string content = Encryption.Decrypt(PlayerPrefs.GetString(varName)); 
                ObjectData = JObject.Parse(content);
                yield return waitDelay;
#else
                using (StreamReader file = new StreamReader(fullPath))
                {
                    //READ FILE TEXT
                    var jsonData = file.ReadToEnd();
                    yield return waitDelay;

                    //DECRYPT DATA
                    var decryptData = Encryption.Decrypt(jsonData);
                    yield return waitDelay;

                    //PARSE STRING TO JOBJECT
                    ObjectData = JObject.Parse(decryptData);
                    yield return waitDelay;
                }
#endif
                if(ObjectData != null)
                {
                    //GET DATA VERSION
                    JToken token = ObjectData["Version"];
                    yield return waitDelay;
                    if (token != null)
                    {
                        fileVersion = ObjectData["Version"].ToObject<int>();
                        yield return waitDelay;
                    }

                    //CHECK VERSION FILE
                    if (fileVersion != 0)
                    {
                        var data = new T();
                        yield return waitDelay;

                        if (fileVersion == data.Version)
                        {
                            data = ObjectData.ToObject<T>();
                            yield return waitDelay;
                            Callback?.Invoke(data);
                        }
                        else
                        {
                            //MIGRATE VERSION
                            Debug.Log($"Migrating Save Data From v{fileVersion} to v{data.Version}");

                            ObjectData = data.Migrate(ObjectData, fileVersion);
                            yield return waitDelay;

                            data = ObjectData.ToObject<T>();
                            yield return waitDelay;

                            yield return StartCoroutine(SaveFileData(fullPath, varName, data));
                            yield return waitDelay;

                            Callback?.Invoke(data);
                        }
                    }
                    else
                    {
                        Debug.Log("Save Data Version Not Found");
                        Callback?.Invoke(default(T));
                    }
                }
                else
                {
                    Debug.Log("Save Data Is Error");
                    Callback?.Invoke(default(T));
                }
            }
            else
            {
                Debug.Log("Save Data Not Found");
                Callback?.Invoke(default(T));
            }

            yield return waitDelay;
        }

        private IEnumerator SaveFileData(string fullPath,string filename, ISaveData SaveData)
        {
            WaitForSeconds waitDelay = new WaitForSeconds(Const.ROUTINE_DURATION);

            //CONVERT TO JSON
            string JsonString = JsonConvert.SerializeObject(SaveData);
            yield return waitDelay;

            //ENCRYPT DATA
            string EncryptData = Encryption.Encrypt(JsonString);
            yield return waitDelay;

            //Use PlayerPrefs for WEBGL
#if UNITY_WEBGL
            PlayerPrefs.SetString(filename, EncryptData);
#else
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            yield return waitDelay;
            using (StreamWriter writer = new StreamWriter(fullPath))
            {
                writer.Write(EncryptData);
                yield return waitDelay;
            }
#endif
            yield return waitDelay;
        }

        private string GetKey<T>()
        {
            return typeof(T).Name.Replace("Data", string.Empty);
        }

        public class ExampleClass
        {
        }
    }
}
