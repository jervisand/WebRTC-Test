using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nagih
{
    public class SequenceAction
    {
        private int _progress;
        private Action[] _sequenceList;
        private HashSet<Action> _skipList;
        private Action _onComplete;
        private Action _onEverySequence;
        private MonoBehaviour _mono;
        private IEnumerator _routine;

        public SequenceAction(MonoBehaviour mono)
        {
            _mono = mono;
        }

        public void StartSequence(Action[] sequenceList, Action onComplete, Action onEverySequence = null)
        {
            _sequenceList = sequenceList;
            _onComplete = onComplete;
            _onEverySequence = onEverySequence;
            _progress = 0;
            _skipList = new HashSet<Action>();

            NextSequence();
        }

        public void NextSequence()
        {
            if (_routine == null)
            {
                _routine = DelayedNextSequence();
                _mono.StartCoroutine(_routine);
            }
        }

        private IEnumerator DelayedNextSequence()
        {
            yield return new WaitForSeconds(0.2f);

            Action currentSequence = null;

            while (currentSequence == null && _progress < _sequenceList.Length)
            {
                currentSequence = _sequenceList[_progress++];
                if (_skipList.Contains(currentSequence))
                {
                    currentSequence = null;
                }
            }

            if (currentSequence != null)
            {
                _routine = null;
                currentSequence();
                _onEverySequence?.Invoke();
            }
            else
            {
                _routine = null;
                _sequenceList = null;
                _skipList = null;
                Util.ExecuteCallback(ref _onComplete);
            }
        }
    }
}
