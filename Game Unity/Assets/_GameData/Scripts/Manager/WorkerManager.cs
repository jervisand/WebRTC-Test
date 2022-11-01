using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Nagih
{
    internal class WorkerManager : MonoBehaviour
    {
        internal static WorkerManager Worker;
        private Queue<Action> _jobs;

        void Awake()
        {
            Worker = this;
            _jobs = new Queue<Action>();
        }

        void Update()
        {
            while (_jobs.Count > 0)
                _jobs.Dequeue().Invoke();
        }

        internal void AddJob(Action newJob)
        {
            _jobs.Enqueue(newJob);
        }
    }
}