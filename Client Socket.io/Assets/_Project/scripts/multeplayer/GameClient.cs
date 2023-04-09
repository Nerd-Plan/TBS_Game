using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TBS.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public event Action<byte> OnSwitchTurns;
    Thread receivemessagethread;
    #endregion

    #region Game Prop's 

    public event Action<int> OnSpawnUnitsOnSide;
    public event Action<byte> OnSpawnLevelGrid;
    public event Action<string>OnUnitDoAction;

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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene loaded: " + scene.name);
        SendMessage("Scene loaded: " + scene.name);

        ListenToGameEvents();

    }

    private void ListenToGameEvents()
    {
        //send to to other and cancel the abilty to play
        BaseAction.OnAnyActionStarted += Action_ActionStarted;
        TurnSystem.OnTurnSwitched += OnTurnChanged;
    }


    private void OnTurnChanged()
    {
        if (TurnSystem.Instance.IsPlayerTurn()) { return; }
        SendMessage("Switch Turns");
    }

    private void Action_ActionStarted(object sender, EventArgs e)
    {
        if (UnitManager.Instance.GetEnemyUnitList().Contains(((BaseAction)sender).GetUnit()))
            return;
        SendMessage(((BaseAction)sender).GetActionAsString());
    }
    #endregion

    private void DoAsTheServerCommends(string servercommend)
    {
        if (StartBoardCommands(servercommend)){return;}
        if (GameActionsCommands(servercommend)){return;}
        if(GameSwitchTurnsCommands(servercommend)){return;}
    }

    private bool GameSwitchTurnsCommands(string servercommend)
    {
        if (!servercommend.Contains("Switch"))
            return false;
        MainThreadDispatcher.ExecuteOnMainThread(OnSwitchTurns, byte.MinValue);
        return true;
    }

    private bool GameActionsCommands(string servercommend)
    {
        if (!servercommend.Contains("Action"))
        return false;
        MainThreadDispatcher.ExecuteOnMainThread(OnUnitDoAction, servercommend);
        return true;
        
    }

    private bool StartBoardCommands(string servercommend)
    {
        if (servercommend.Contains("Game Has Been Started"))
        {
            MainThreadDispatcher.ExecuteOnMainThread(OnGameStart, byte.MinValue);
            return true;
        }
        else if (servercommend.Contains("SetBoard"))
        {
            MainThreadDispatcher.ExecuteOnMainThread(OnSpawnLevelGrid, byte.MinValue);
            return true;
        }
        else if (servercommend.Contains("Instantiate Units"))
        { 
            if (servercommend.Contains("Side One"))
            {
                MainThreadDispatcher.ExecuteOnMainThread(OnSpawnUnitsOnSide,1);
            }
            else
            {
                MainThreadDispatcher.ExecuteOnMainThread(OnSpawnUnitsOnSide,2);
            }
            return true;
        }
        else if(servercommend.Contains("First Move"))
        {
            MainThreadDispatcher.ExecuteOnMainThread(OnPlayerStartGameMove, true);
            return true;
        }
        else if(servercommend.Contains("Not You'r Move"))
        {
            MainThreadDispatcher.ExecuteOnMainThread(OnPlayerStartGameMove, false);
            return true;
        }
        return false;
    }

    
}
