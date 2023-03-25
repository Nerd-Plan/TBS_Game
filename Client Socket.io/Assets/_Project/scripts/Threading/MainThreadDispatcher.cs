using UnityEngine;
using System.Collections.Generic;
using System;

namespace TBS.Threading
{
    public class MainThreadDispatcher : MonoBehaviour
    {
        private static MainThreadDispatcher Instance = null;
        private static readonly Queue<Action> executionQueue = new Queue<Action>();
        private static readonly object queueLock = new object();

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(Instance);
        }

        private void Update()
        {
            lock (queueLock)
            {
                while (executionQueue.Count > 0)
                {
                    executionQueue.Dequeue()?.Invoke();
                }
            }
        }

        public static void ExecuteOnMainThread<T>(Action<T> action, T arg)
        {
            lock (queueLock)
            {
                executionQueue.Enqueue(() =>
                {
                    if(arg != null&&action!=null)
                        action(arg); 
                });

            }
        }
    }
}
