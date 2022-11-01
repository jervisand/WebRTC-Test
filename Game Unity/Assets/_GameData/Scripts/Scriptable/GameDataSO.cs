using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Nagih
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GameDataSO", order = 1)]
    public class GameDataSO : ScriptableObject
    {
        [Header ("=== Scene Data ===")]
        public SceneData[] SceneData;
        public float MinimumLoadingDuration = 2f;
        public AudioStruct[] AudioTypeData;
        
        [Header("=== Localization ===")]
        public GameObject LeanLocalization;

        [Header("=== Loading ===")]
        public GameObject SceneLoading;
        public GameObject FrontLoading;
        public GameObject BackLoading;

        [Header("=== Popup ===")]
        public GameObject PopupCanvas;
        public GameObject PopupConsent;
        public GameObject PopupInfo;
        public GameObject PopupNotification;
        public GameObject PopupSelection;

        [Header("=== Tutorial ===")]
        public int[] TutorialSectionStep; // how many step in each section
        public int[] TutorialExcludeStepArray; // step apa saja yg tidak akan langsung pindah ke next step berikutnya jika click box, atau overlay
        public GameObject TutorialCanvas;
        public GameObject TutorialBox;
        public GameObject TutorialArrow;
        public GameObject TutorialMask;
        public GameObject TutorialOverlay;


        public void Initialize()
        {

        }

        public Enum.AudioSource GetAudioType(Enum.Sound sound)
        {
            return AudioTypeData.First(x => x.Sounds.Contains(sound)).AudioSource;
        }

        [Serializable]
        public struct AudioStruct
        {
            public Enum.AudioSource AudioSource;
            public Enum.Sound[] Sounds;
        }
    }
}