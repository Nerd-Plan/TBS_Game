using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TBS.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using TBS.NetWork;
using System.Collections.Generic;

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

    public event Action OnGameStart;
    Thread receivemessagethread;
    #endregion
    #region Game Prop's 

    public event Action OnSpawnUnitsOnSideOne;
    public event Action OnSpawnUnitsOnSideTwo;
    public event Action OnSpawnLevelGrid;
    public event Action<Tuple <string,Tuple<string, string> > > OnActionHasBeenDone;


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
        MainThreadDispatcher.ExecuteOnMainThread(OnGameStart);
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
            MainThreadDispatcher.ExecuteOnMainThread(OnGameStart);
            return true ;
        }
        else if (servercommend.Contains("SetBoard"))
        {
            MainThreadDispatcher.ExecuteOnMainThread(OnSpawnLevelGrid);
            return true;
        }
        else if (servercommend.Contains("Instantiate Units"))
        {
            if (servercommend.Contains("Side One"))
            {
                MainThreadDispatcher.ExecuteOnMainThread(OnSpawnUnitsOnSideOne);
                return true;
            }
            else
            {
                MainThreadDispatcher.ExecuteOnMainThread(OnSpawnUnitsOnSideTwo);
                return true;
            }
        }
        else if (servercommend.Contains("First Move Is your's"))
        {
            //the first who start's the game 
            //TODO: Enable Player to do actions
            return true;
        }
            return false;
    }

    public  Vector3 StringToVector3(string sVector)
    {
        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }

        // split the items
        string[] sArray = sVector.Split(',');

        // store as a Vector3
        Vector3 result = new Vector3(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]));

        return result;
    }
}
