using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TBS.NetWork
{
    public class Client : MonoBehaviour
    {
        GameClient gameClient;
        public static Client Instance;
        public  Transform NormalUnit;
        public GameClient GetGameClient() => gameClient;

        private void OnDestroy()
        {
            gameClient.Stop();
            gameClient.Dispose();
        }
        private void OnApplicationQuit()
        {
            gameClient.Stop();
            gameClient.Dispose();
        }
        public void ConnectToServer(string ip,int port)
        {
            DontDestroyOnLoad(gameObject);
            gameClient = new GameClient();
            gameClient.SetPort(port);
            gameClient.SetIpAddress(ip);
            gameClient.Connect();
            gameClient.OnGameStart += StartGame;
            Instance = this;

        }



        private void StartGame(byte a)
        {
            SceneManager.LoadScene(2);
        }



    }
}