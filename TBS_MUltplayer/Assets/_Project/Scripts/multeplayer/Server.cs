using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TBS.NetWork
{
    public class Server : MonoBehaviour
    {
        GameServer gameServer;

        private void Awake()
        {
            SceneManager.sceneLoaded+=OnSceneLoaded;    
            DontDestroyOnLoad(gameObject);
        }
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != "MultplayerGameScene") return;
            Invoke(nameof(SetBoard), .4f);
           
        }

        private void SetBoard()
        {
            gameServer.SetBoard();
        }

        public void StartServer(int port)
        {
            DontDestroyOnLoad(gameObject);
            gameServer = new GameServer();
            GameServer.Instance.SetPort(port);
            GameServer.Instance.StartServer();
        }

        private void OnApplicationQuit()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            GameServer.Instance.Stop();
            GC.SuppressFinalize(this);

        }
        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            GameServer.Instance.Stop();
            GC.SuppressFinalize(this);
        }

        public int PortNumber()
        {
            return GameServer.Instance.GetPort();
        }
    }
}