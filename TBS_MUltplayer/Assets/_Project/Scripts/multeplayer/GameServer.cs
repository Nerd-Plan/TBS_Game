using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using Random = System.Random;
using TBS.Threading;
using System.Collections.Generic;
using System.IO;

public class GameServer : IDisposable
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
                SendMessageToPlayer(1, ("Game Has Been Started"));
                SendMessageToPlayer(2, ("Game Has Been Started"));
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
        SendMessageToPlayer(player, ("First Move"));
        SendMessageToPlayer(player == 0 ? 1 : 2, ("Not You'r Move"));
    }
    #endregion

    #region GameProp's

    Dictionary<int, Tuple<int, string>> moves_history = new Dictionary<int, Tuple<int, string>>();
    #endregion

    #region encryption detals
    EncryptionKeys encryptionKeys;

    string[] players_public_key;
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
            players_public_key = new string[2];
            encryptionKeys = new EncryptionKeys();  
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            listenerThread = new Thread(new ThreadStart(ListenForTCPClients));
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

            MainThreadDispatcher.ExecuteOnMainThread(OnServerEnded, byte.MinValue);
            // stop the listener and dispose of resources
            listener?.Stop();


            // stop the client threads and dispose of resources
            clientThread1?.Abort();
            clientThread2?.Abort();
            clientStream1?.Close();
            clientStream2?.Close();
            client1?.Close();
            client2?.Close();
        }
        catch (Exception e)
        {
            Debug.Log("Error stopping server: " + e.Message);
        }

        Debug.Log("Server stopped.");
    }

    private void ListenForTCPClients()
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
    private void ListenForTCPClient()
    {
        if (client1.Connected)
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
            if (player == 1)
            {
                client1 = client;
                clientStream1 = stream;
            }
            else
            {
                client2 = client;
                clientStream2 = stream;
            }

            ExchangePublicKeys(client, player);

            Thread thread = new Thread(() => HandleClientComm(player, stream, client));
            thread.Start();
        }
        catch (Exception e)
        {
            Debug.Log("Error accepting client connection: " + e.Message);
        }
    }

  
    private void DisconnectPlayer(int player)
    {
        if (player == 1)
        {
            client1.Close();
            listenerThread = new Thread(new ThreadStart(ListenForTCPClient));
            listenerThread.Start();
        }
        else
        {
            client2.Close();
            listenerThread = new Thread(new ThreadStart(ListenForTCPClient));
            listenerThread.Start();
        }
        Debug.Log($"player {player} Disconnected ");
    }

    public void ExchangePublicKeys(TcpClient client,int player)
    {
        Stream stream=  client.GetStream();
        //Send Pulbic Server Key
        stream.Write(Encoding.UTF8.GetBytes(encryptionKeys.public_key), 0, Encoding.UTF8.GetBytes(encryptionKeys.public_key).Length);
        stream.Write(Encoding.UTF8.GetBytes(string.Empty), 0, Encoding.UTF8.GetBytes(string.Empty).Length);
        // read data from the client
        byte[] data = new byte[client.ReceiveBufferSize];
        int bytesRead = stream.Read(data, 0, data.Length);
        if (bytesRead <= 0)
        {
            Debug.Log("Player " + player + " disconnected.");
            DisconnectPlayer(player);
            return;
        }
        string message = Encoding.ASCII.GetString(data, 0, bytesRead);
        players_public_key[player - 1] = message;
        Debug.Log("Player " + player + " Public Key: "+players_public_key[player-1]);
        data=new byte[] {byte.MinValue};
        bytesRead = 0;
    }
    #endregion

    #region Server Client Communiction

    private void HandleClientComm(int player, NetworkStream stream, TcpClient client)
    {
        try
        {
            SendMessageToPlayer(player, $"Hello Player {player}");
            bool shouldListen = true;
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
                byte[] encryptedData = new byte[bytesRead];
                Array.Copy(data,encryptedData, bytesRead);
                string message = EncryptionHelper.Decrypt(encryptedData, encryptionKeys.private_key);
                Debug.Log("Player " + player + " sent: " + message);
                                
                if (message.Contains("Log Out"))
                {
                    Debug.Log("Player " + player + " disconnected.");
                    DisconnectPlayer(player);
                    shouldListen = false;
                    return;
                }
                    HandleMessagesFromClient(player, message);
               
            }
        }
        catch (Exception e)
        {
            if (!client.Connected)
            {
                client.Close();
                if (ready >= 2)
                {
                    //get back to loby
                    return;
                }
            }
            Debug.Log($"Error handling client {player} communication: " + e.Message);
        }
    }
    private void SendMessageToPlayer(int player, string data)
    {
        NetworkStream stream = (player == 1 ? clientStream1 : clientStream2);
        if (stream != null && stream.CanWrite)
        {
            try
            {
                byte[] encryptedmessg = EncryptionHelper.Encrypt(data, players_public_key[player - 1]);
                Debug.Log("Server Sending encoded message : "+Encoding.UTF8.GetString(encryptedmessg));
                stream.Write(encryptedmessg, 0, encryptedmessg.Length);
            }
            catch (IOException ex)
            {
               Debug.Log("Error sending message: " + ex.Message);
            }
        }
        else
        {
            Debug.Log("Error sending message: stream is null or not writable");
        }
    }

    #endregion


    #region Handle Client messages

    //other class
    private void HandleMessagesFromClient(int player, string message)
    {
        if (Ready != 2)
        {
            HandleStartGameMessage(message);                       
        }
        if (HandleGameMessage(player, message)) { return; }
        if (HandleSwitchTurnsMessage(player, message)) { return; }

        //Brodcast
    }
    private bool HandleSwitchTurnsMessage(int player, string message)
    {
        if (!message.Contains("Switch Turns"))
            return false;
        SendMessageToPlayer(player == 1 ? 2 : 1, (message));
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
<<<<<<< HEAD
        SendMessageToPlayer(player == 1 ? 2 : 1, (message));
=======
        SendMessageToPlayer(player == 1 ? 2 : 1, Encoding.ASCII.GetBytes(message));
>>>>>>> 6dd6e3b580ac93c7563388730948302c7d5d09ad
        return true;
    }

    #endregion

    #region Game
    public void SetBoard()
    {
        SendMessageToPlayer(1, ($"SetBoard"));
        SendMessageToPlayer(2, ($"SetBoard"));
        SendMessageToPlayer(1, ($"Instantiate Units On Side One"));
        SendMessageToPlayer(2, ("Instantiate Units On Side Two"));

    }


    public void Dispose()
    {
        Stop();
    }

    public int GetPort()
    {
        if (listener != null && listener.LocalEndpoint is IPEndPoint localEndpoint)
        {
            return localEndpoint.Port;
        }
        return -1;
    }
    #endregion
}