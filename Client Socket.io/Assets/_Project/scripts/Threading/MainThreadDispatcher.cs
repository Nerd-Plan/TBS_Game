using UnityEngine;
using System.Collections.Generic;
using System.Threading;

namespace TBS.Threading
{
    public class MainThreadDispatcher : MonoBehaviour
    {
        private static MainThreadDispatcher Instance = null;
        private static readonly Queue<System.Action> executionQueue = new Queue<System.Action>();
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

        public static void ExecuteOnMainThread(System.Action action)
        {
            lock (queueLock)
            {
                executionQueue?.Enqueue(action);
            }
        }

    }
}