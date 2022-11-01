using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Nagih
{
    public class TutorialManager : MonoBehaviour
    {
        // step is the number that indicate current index of the tutorial. 1001, 1002 is the step. Step in string
        // section is the leading number in step. In 1002 the section is 1. Section in int
        // pace is the back number in step. In 1002 the step is 2. Pace in int

        private const int MULTIPLIER = 1000;

        [HideInInspector] public Transform Parent;
        [HideInInspector] public Action OnChangeStep;

        private Dictionary<int, List<TutorialHighlight>> _highlightObjectDictionary;
        private bool _check;

        public bool IsInitialize { get; private set; }
        public int CurrentStep { get; private set; }
        public int CurrentSection
        {
            get { return GetSection(CurrentStep); }
        }
        public int CurrentPace
        {
            get { return GetPace(CurrentStep); }
        }

        public TutorialArrow Arrow { get; private set; }

        public TutorialBox Box { get; private set; }

        public TutorialOverlay Overlay { get; private set; }

        public void Initialize()
        {
            if (!IsInitialize)
            {
                Parent = Instantiate(DataStatic.GetInstance().GameDataSO.TutorialCanvas, transform).transform;
                Box = Instantiate(DataStatic.GetInstance().GameDataSO.TutorialBox, Parent).GetComponent<TutorialBox>();
                Arrow = Instantiate(DataStatic.GetInstance().GameDataSO.TutorialArrow, Parent).GetComponent<TutorialArrow>();
                Overlay = Instantiate(DataStatic.GetInstance().GameDataSO.TutorialOverlay, Parent).GetComponent<TutorialOverlay>();

                IsInitialize = true;
            }

            PopupBase.HideAllActivePopup();
            FromStart();
        }

        public void FromStart()
        {
            CurrentStep = JoinStep(1, 0);
            NextStep();
        }

        private void Start()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
            SyncHighlightObject();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SyncHighlightObject();
        }

        // cuman dipanggil sama tutorial box. Yg lain panggil CheckNextSpecialStep
        // beberapa button juga panggil ini
        public bool CheckNextStep()
        {
            return CheckNextStep(CurrentStep);
        }

        public bool CheckNextStep(int step)
        {
            bool isNextStep = IsInitialize && !DataStatic.GetInstance().GameDataSO.TutorialExcludeStepArray.Contains(step);
            if (isNextStep)
            {
                NextStep();
            }
            return isNextStep;
        }

        public bool CheckTriggerStep(Enum.TutorialTrigger trigger)
        {
            bool isNextStep = IsInitialize && (int)trigger == CurrentStep;
            if (isNextStep)
            {
                NextStep();
            }
            return isNextStep;
        }

        public void NextStep()
        {
            Hide();

            int nextPace = GetPace(CurrentStep + 1);
            int lastPace = DataStatic.GetInstance().GameDataSO.TutorialSectionStep[CurrentSection - 1];
            CurrentStep = nextPace > lastPace ? JoinStep(CurrentSection + 1, 1) : CurrentStep + 1;

            Invoke($"Step_{CurrentStep}", 0f);
            OnChangeStep?.Invoke();
        }

        public void Hide()
        {
            Box.Hide();
            Arrow.Hide();
            Overlay.Hide();
        }

        public void HideCurrentHighlightObject()
        {
            TutorialHighlight[] currentHighlights = ((TutorialHighlight[])FindObjectsOfType(typeof(TutorialHighlight)))
                                                    .Where(highlight => highlight.Steps.Contains(CurrentStep)).ToArray();
            for (int i = 0; i < currentHighlights.Length; i++)
            {
                currentHighlights[i].SetOriginParent();
            }
        }

        private int JoinStep(int section, int pace)
        {
            return section * MULTIPLIER + pace;
        }

        private int GetSection(int step)
        {
            return step / MULTIPLIER;
        }

        private int GetPace(int step)
        {
            return step % MULTIPLIER;
        }

        private void SyncHighlightObject()
        {
            _highlightObjectDictionary = new Dictionary<int, List<TutorialHighlight>>();
            TutorialHighlight[] highlightObjectArray = ((TutorialHighlight[])FindObjectsOfType(typeof(TutorialHighlight)))
                                                    .Where(highlight => highlight.Steps.Contains(CurrentStep)).ToArray();
            for (int i = 0; i < highlightObjectArray.Length; i++)
            {
                TutorialHighlight highlight = highlightObjectArray[i];
                for (int j = 0; j < highlight.Steps.Length; j++)
                {
                    int step = highlight.Steps[j];
                    if (!_highlightObjectDictionary.ContainsKey(step))
                    {
                        _highlightObjectDictionary[step] = new List<TutorialHighlight> { highlight };
                    }
                    else
                    {
                        _highlightObjectDictionary[step].Add(highlight);
                    }
                }
            }
        }

#region EVERY TUTORIAL STEP
#endregion
    }
}