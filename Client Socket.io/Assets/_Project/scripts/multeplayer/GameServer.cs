using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using Random = System.Random;
using TBS.Threading;
using System.Collections.Generic;

public class GameServer 
{
    #region Prop's
    int port;
    public void SetPort(int p) => port = p;
    public event Action<byte> OnServerEnded;
    public event Action OnServerStarted;

    private TcpListener listener;
    private Thread listenerThread;


    private TcpClient client1;
    private Thread clientThread1;
    private NetworkStream clientStream1;


    private TcpClient client2;
    private Thread clientThread2;
    private NetworkStream clientStream2;

    public static GameServer Instance;
    int ready = 0;

    public int Ready
    {
        get { return ready; }
        set
        {
            ready = value;
            if (ready == 2)
            {
                SendMessageToPlayer(0, Encoding.ASCII.GetBytes("Game Has Been Started"));
                SendMessageToPlayer(1, Encoding.ASCII.GetBytes("Game Has Been Started"));
                RandomPlayerStart();
                // player  with playerid starts
            }
        }
    }

    private void RandomPlayerStart()
    {
        Random random = new Random();
        int odds = random.Next(0, 101);
        int player = odds >= 50 ? 0 : 1;
        Debug.Log(player);
        SendMessageToPlayer(player, Encoding.ASCII.GetBytes("First Move"));
        SendMessageToPlayer(player == 0 ? 1 : 0, Encoding.ASCII.GetBytes("Not You'r Move"));
    }
    #endregion
    #region GameProp's
    LevelGrid current_LevelGrid;

    Dictionary<int,Tuple<int,string>> moves_history=new Dictionary<int,Tuple<int,string>>();
    #endregion

    #region Start and Stop Server 
    public GameServer()
    {
        if (Instance == null && Instance != this)
        {
            Instance = this;
        }
        else
        {
            return;
        }
    }


    public void StartServer()
    {
        try
        {          
            
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            listenerThread = new Thread(new ThreadStart(ListenForClients));
            listenerThread.Start();
            Debug.Log("Server started and listening on port " + port);
            Ready = 0;
        }
        catch (Exception e)
        {
            Debug.Log("Error starting server: " + e.Message);
        }
    }
    public void Stop()
    {
        try
        {
            MainThreadDispatcher.ExecuteOnMainThread(OnServerEnded,byte.MinValue);
            // stop the listener and dispose of resources
            listener?.Stop();


            // stop the client threads and dispose of resources
            clientThread1?.Abort();
            clientThread2?.Abort();
            clientStream1?.Dispose();
            clientStream2?.Dispose();
            client1?.Close();
            client2?.Close();
        }
        catch (Exception e)
        {
            Debug.Log("Error stopping server: " + e.Message);
        }

        Debug.Log("Server stopped.");
    }
    #endregion

    #region Server Client Communiction

    private void ListenForClients()
    {
        try
        {
            ListenForClient(1);
            ListenForClient(2);
        }
        catch (Exception e)
        {
            Debug.Log("Error accepting client connection: " + e.Message);
        }
    }
    private void ListenForClient()
    {
        if(client1.Connected)
        {
            ListenForClient(2);
        }
        else
        {
            ListenForClient(1);
        }

    }
    private void ListenForClient(int player)
    {
        try
        {
            TcpClient client = listener.AcceptTcpClient();
            client.ReceiveBufferSize = 1024;
            Debug.Log($"Player {player} connected.");

            NetworkStream stream = client.GetStream();
            if(player == 1)
            {
                client1 = client;
                clientStream1 = stream;
            }
            else
            {
                client2= client;
                clientStream2 = stream;
            }
            Thread thread = new Thread(() => HandleClientComm(player,stream,client));
            thread.Start();
        }
        catch (Exception e)
        {
            Debug.Log("Error accepting client connection: " + e.Message);
        }
    }
    private void HandleClientComm(int player, NetworkStream stream, TcpClient client)
    {
        try
        {
            SendMessageToPlayer(player, Encoding.ASCII.GetBytes($"Hello Player {player}, welcome to the server"));
            bool shouldListen=true;
            while (shouldListen)
            {
                // read data from the client
                byte[] data = new byte[client.ReceiveBufferSize];
                int bytesRead = stream.Read(data, 0, data.Length);
                if (bytesRead <= 0)
                {
                    Debug.Log("Player " + player + " disconnected.");
                    DisconnectPlayer(player);
                    shouldListen = false;
                    return;
                }

                string message = Encoding.ASCII.GetString(data, 0, bytesRead);
                Debug.Log("Player " + player + " sent: " + message);
                if (message.Contains("Log Out"))
                {
                    Debug.Log("Player " + player + " disconnected.");
                    DisconnectPlayer(player);
                    shouldListen = false;
                    return;
                }
                HandleMessagesFromClient(player,message);
            }
        }
        catch (Exception e)
        {
            if (!client.Connected)
                client.Close();           
            Debug.Log("Error handling client communication: " + e.Message);            
        }
    }
    private void DisconnectPlayer(int player)
    {
        if (player == 1)
        {
            client1.Close();
            listener.Stop();
            listenerThread = new Thread(new ThreadStart(ListenForClient));
            listenerThread.Start();
        }
        else
        {
            client2.Close();
            listener.Stop();
            listenerThread = new Thread(new ThreadStart(ListenForClient));
            listenerThread.Start();
        }
        Debug.Log($"player {player} Disconnected ");
    }
    private void SendMessageToPlayer(int player, byte[] data)
    {
        NetworkStream Stream = (player == 1 ? clientStream1 : clientStream2);
        Stream.Write(data, 0, data.Length);
    }

    #endregion


    #region Handle Client messages

    //other class
    private void HandleMessagesFromClient(int player,string message)
    {
        if (Ready != 2)
        {
           HandleStartGameMessage(message);
        }
       if(HandleGameMessage(player, message)) { return; }
       if(HandleSwitchTurnsMessage(player, message)) { return; }

        //Brodcast
    }
    private bool HandleSwitchTurnsMessage(int player,string message)
    {
        if(!message.Contains("Switch Turns"))
            return false;
        SendMessageToPlayer(player == 1 ? 2 : 1, Encoding.UTF8.GetBytes(message));
        return true;
    }
    private void HandleStartGameMessage(string message)
    {
        if (message.Contains("Ready"))
            Ready++;
        else if (message.Contains("Cancel"))
            Ready--;
    }
    private bool HandleGameMessage(int player, string message)
    {
        if (!message.Contains("Action"))
            return false;
        if (!message.Contains("key123"))
            return false;
        message = message.Replace("key123", "");
        SendMessageToPlayer(player == 1 ? 2 : 1, Encoding.ASCII.GetBytes(message));
        return true;
    }

    #endregion

    #region Game

    public void SetBoard()
    {
        SendMessageToPlayer(1, Encoding.UTF8.GetBytes($"SetBoard"));
        SendMessageToPlayer(2, Encoding.UTF8.GetBytes($"SetBoard"));
        SendMessageToPlayer(1,Encoding.UTF8.GetBytes($"Instantiate Units On Side One"));  
        SendMessageToPlayer(2,Encoding.UTF8.GetBytes("Instantiate Units On Side Two"));

    }
    #endregion
}
