using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TBS.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using TBS.NetWork;
using System.Collections.Generic;
using Random = System.Random;

public class GameClient 
{
    #region Prop's

    private TcpClient client;
    private NetworkStream stream;
    private byte[] buffer = new byte[1024];
    string ipAddress;
    int port;

    public void SetIpAddress(string ip)=>ipAddress = ip;
    public void SetPort(int Port) => port = Port;

    public event Action<byte> OnGameStart;
    Thread receivemessagethread;
    #endregion
    #region Game Prop's 

    public event Action<int> OnSpawnUnitsOnSide;
    public event Action<byte> OnSpawnLevelGrid;
    //public event Action<Tuple <string,Tuple<string, string> > > OnActionHasBeenDone;

    public event Action<bool> OnPlayerStartGameMove;
    Random r = new Random();
    #endregion
    #region Client only
    public void Connect()
    {
        try
        {
            client = new TcpClient(ipAddress, port);
            stream = client.GetStream();
            receivemessagethread = new Thread(new ThreadStart(ReceiveMessage));
            receivemessagethread.Start();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        catch (Exception e)
        {
            Debug.Log("Error connecting to server: " + e.Message);
            return;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene loaded: " + scene.name);
        SendMessage("Scene loaded: " + scene.name);

        ListenToGameEvents();
    }

    private void ListenToGameEvents()
    {
        //send to to other and cancel the abilty to play
        //UnitActionSystem.Instance.OnActionStarted+=
    }

    public void Disconnect()
    {
        try
        {
            if (client.Connected)
                SendMessage("Log Out");
            stream.Dispose();
            client.Dispose();
        }
        catch (Exception e)
        {
            Debug.Log("Error disconnecting from server: " + e.Message);
        }
        Debug.Log("Disconnected from server.");
    }

    public void SendMessage(string message)
    {
        try
        {
            stream.Write(Encoding.ASCII.GetBytes(message), 0, message.Length);
        }
        catch (Exception e)
        {
            Debug.Log("Error sending message: " + e.Message);
        }
    }

    public void ReceiveMessage()
    {
        try
        {
            while (true)
            {
                // read data from the server into the buffer
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string response = Encoding.ASCII.GetString(buffer);
                Debug.Log(response);
                DoAsTheServerCommends(response);
            }
        }
        catch (Exception e)
        {
            Debug.Log("Error receiving message: " + e.Message);
            if (!client.Connected)
            {
                Disconnect();
            }
        }
    }

    #endregion

    #region Game contect

    public void StartGame()
    {
        MainThreadDispatcher.ExecuteOnMainThread(OnGameStart,byte.MinValue);
    }
    #endregion

    private void DoAsTheServerCommends(string servercommend)
    {
        if (StartBoardCommands(servercommend)) { return; }
        if (GameActionsCommands(servercommend)){return;}
    }

    private bool GameActionsCommands(string servercommend)
    {
        return true;
    }

    private bool StartBoardCommands(string servercommend)
    {
        if (servercommend.Contains("Game Has Been Started"))
        {
            StartGame();
            return true;
        }
        else if (servercommend.Contains("SetBoard"))
        {
            MainThreadDispatcher.ExecuteOnMainThread(OnSpawnLevelGrid, byte.MinValue);
            return true;
        }
        else if (servercommend.Contains("Instantiate Units"))
        { 
            int i = r.Next(0, 2);
            if (servercommend.Contains("Side One"))
            {
                MainThreadDispatcher.ExecuteOnMainThread(OnSpawnUnitsOnSide,1);
                MainThreadDispatcher.ExecuteOnMainThread(OnPlayerStartGameMove, i == 0 ? true : false);

            }
            else
            {
                MainThreadDispatcher.ExecuteOnMainThread(OnSpawnUnitsOnSide,2);
                MainThreadDispatcher.ExecuteOnMainThread(OnPlayerStartGameMove, i == 0 ? true : false);
            }
            return true;
        }
        return false;
    }

    
}
