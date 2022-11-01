using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Nagih
{
    public class SceneManager : MonoBehaviour
    {
        private SceneLoading _sceneLoading;

        public Enum.Scene CurrentScene { get; private set; }

        private void Awake()
        {
            // scene pertama
            CurrentScene = (Enum.Scene)UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        }

        private void OnEnable()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            CurrentScene = (Enum.Scene)scene.buildIndex;
        }

        public void LoadAsset(SceneLoading sceneLoading)
        {
            _sceneLoading = sceneLoading;
        }

        public void ChangeScene(Enum.Scene nextScene)
        {
            StartCoroutine(ChangeSceneRoutine(nextScene));
        }

        public IEnumerator ChangeSceneRoutine(Enum.Scene nextScene)
        {
            AudioSource source = Manager.GetInstance().Audio.GetAudioSource(Enum.Sound.MainBGM);
            if (source.isPlaying) source.Pause();

            SceneData[] scenes = DataStatic.GetInstance().GameDataSO.SceneData;
            SceneData currentData = null;
            SceneData nextData = null;
            foreach(SceneData data in scenes)
            {
                if (data.Scene == CurrentScene)
                    currentData = data;
                else if (data.Scene == nextScene)
                    nextData = data;
            }

            if (nextData.ShowLoading)
            {
                _sceneLoading.Show(Const.DUR_LOADING_MINIMUM);
            }

            // load async scene
            AsyncOperation async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync((int)nextScene);
            async.allowSceneActivation = false;

            while (!async.isDone)
            {
                if (async.progress >= 0.9f)
                {
                    async.allowSceneActivation = true;
                    if (nextData.ShowLoading) _sceneLoading.SetMaxProgress(1f);
                }
                else if (nextData.ShowLoading)
                {
                    _sceneLoading.SetMaxProgress(async.progress);
                }
                yield return null;
            }

            CurrentScene = nextScene;
            if (!source.isPlaying) source.Play();
        }
    }
}