using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using Random = UnityEngine.Random;
using TBS.Threading;
using System.Collections.Generic;
using System.Windows.Input;

public class GameServer 
{
    #region Prop's
    int port;
    public void SetPort(int p) => port = p;
    public event Action OnServerEnded;
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
                int player = Random.Range(0, 1);
                SendMessageToPlayer(player, Encoding.ASCII.GetBytes("you starting"));
                // player  with playerid starts
            }
        }
    }
    #endregion
    #region GameProp's
    LevelGrid current_LevelGrid;
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
        Ready = 0;
        try
        {          
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            listenerThread = new Thread(new ThreadStart(ListenForClients));
            listenerThread.Start();
            Debug.Log("Server started and listening on port " + port);
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
            MainThreadDispatcher.ExecuteOnMainThread(OnServerEnded);
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
            // accept the first client connection
            client1 = listener.AcceptTcpClient();
            Debug.Log("Player 1 connected.");

            // get the network stream for the client connection 
            clientStream1 = client1.GetStream();

            // start a new thread to handle communication with the first client
            clientThread1 = new Thread(new ParameterizedThreadStart(HandleClientComm));
            clientThread1.Start(1);

            // accept the second client connection
            client2 = listener.AcceptTcpClient();
            Debug.Log("Player 2 connected.");

            // get the network stream for the client connection 
            clientStream2 = client2.GetStream();

            // start a new thread to handle communication with the second client
            clientThread2 = new Thread(new ParameterizedThreadStart(HandleClientComm));
            clientThread2.Start(2);
        }
        catch (Exception e)
        {
            Debug.Log("Error accepting client connection: " + e.Message);
        }
    }

    private void ListenForClient()
    {
        try
        {
            if (!client1.Connected)
            {
                // accept the first client connection
                client1 = listener.AcceptTcpClient();
                Debug.Log("Player 1 connected.");

                // get the network stream for the client connection 
                clientStream1 = client1.GetStream();

                // start a new thread to handle communication with the first client
                clientThread1 = new Thread(new ParameterizedThreadStart(HandleClientComm));
                clientThread1.Start(1);
            }
            else if (!client2.Connected)
            {
                // accept the second client connection
                client2 = listener.AcceptTcpClient();
                Debug.Log("Player 2 connected.");

                // get the network stream for the client connection 
                clientStream2 = client2.GetStream();

                // start a new thread to handle communication with the second client
                clientThread2 = new Thread(new ParameterizedThreadStart(HandleClientComm));
                clientThread2.Start(2);
            }
        }
        catch (Exception e)
        {
            Debug.Log("Error accepting client connection: " + e.Message);
        }
    }


    private void HandleClientComm(object playerObj)
    {
        int player = (int)playerObj;
        NetworkStream stream = (player == 1 ? clientStream1 : clientStream2);
        Thread playerthread = (player == 1 ? clientThread1 : clientThread2);
        try
        {
            // send a welcome message to the client
            SendMessageToPlayer(player, Encoding.ASCII.GetBytes($"Hello Player {player} welcome to the server"));
            while (true)
            {
                // read data from the client
                byte[] data = new byte[client1.ReceiveBufferSize];
                int bytesRead = stream.Read(data, 0, data.Length);
                if (bytesRead > 0)
                {
                    string message = Encoding.ASCII.GetString(data);
                    Debug.Log("Player " + player + " sent: " + message);
                    if (message.Contains("Log Out"))
                    {
                        DisconnectPlayer(player);
                        return;
                    }
                    HandleMessagesFromClient(message);
                    //if (client2 != null)                                //TODO: send to 2 players if we have 2 and we are in the game state
                    //    SendMessageToPlayer(player, data);
                }
                else
                {
                    if (playerthread != null) return;
                    // the client has disconnected, so exit the loop
                    Debug.Log("Player " + player + " disconnected.");
                    DisconnectPlayer(player);
                    return;
                }
            }
        }
        catch (Exception e)
        {
            if (!(player == 1 ? client1 : client2).Connected)
                DisconnectPlayer(player);
            Debug.Log("Error handling client communication: " + e.Message);
        }
    }

    private void DisconnectPlayer(int player)
    {
        if (player == 1)
        {
            client1.Close();
            listenerThread = new Thread(new ThreadStart(ListenForClient));
            listenerThread.Start();
        }
        else
        {
            client2.Close();
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
    private void HandleMessagesFromClient(string message)
    {
        if (Ready != 2)
        {
            HandleStartGameMessage(message);
        }
        HandleGameMessage(message);

        //Brodcast
    }

    private void HandleStartGameMessage(string message)
    {
        if (message.Contains("Ready"))
            Ready++;
        else if (message.Contains("Cancel"))
            Ready--;

    }

    private bool HandleGameMessage(string message)
    {
        if (message.Contains("Move"))
        {
            return true;
        }
        else if (message.Contains("Attack"))
        {
            return true;
        }

        return false;
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
