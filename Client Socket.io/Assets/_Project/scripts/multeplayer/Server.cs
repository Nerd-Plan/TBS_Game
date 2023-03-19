using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TBS.NetWork
{
    public class Server : MonoBehaviour
    {
        GameServer gameServer;
        [SerializeField] Transform levelGrid;

        private void Awake()
        {
            SceneManager.sceneLoaded+=OnSceneLoaded;    
        }
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != "NetWorkGame")return;
            SetBoard();
           
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
            GameServer.Instance.Stop();
        }
        private void OnDestroy()
        {
            GameServer.Instance.Stop();

        }
    }
}