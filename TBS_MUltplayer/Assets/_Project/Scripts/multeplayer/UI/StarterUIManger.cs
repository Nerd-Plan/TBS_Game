using UnityEngine;
using UnityEngine.SceneManagement;
using TBS.NetWork;
using TMPro;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System;

namespace TBS.UI
{
    public class StarterUIManger : MonoBehaviour
    {
        [SerializeField] GameObject client_prefab;
        [SerializeField] GameObject Server_prefab;
        [SerializeField] GameObject Game_Manger;


        [SerializeField] Server Server;
        [SerializeField] Client client;
        [SerializeField] GameObject G_M;


        [SerializeField] TMP_InputField ip;
        [SerializeField] TMP_InputField port;
        [SerializeField] TMP_Text ShowPort;

        [SerializeField] GameObject MultiplayerUI;
        [SerializeField] GameObject ClientConnectedToServerUI;

        [SerializeField] GameObject LoadingScreen;
        #region Start and Stop            
        public void StartClient()
        {

            client = Instantiate(client_prefab, Vector3.zero, Quaternion.identity).GetComponent<Client>();
            client.ConnectToServer(ip.text, int.Parse(port.text));
            if (Server != null)
            {               
               client.GetGameClient().IsOwner = true;   
            }
            LoadingScreen.SetActive(true);
            G_M= Instantiate(Game_Manger, Vector3.zero, Quaternion.identity);
            try
            {
                if (client.GetGameClient().IsConnected())
                {
                    MultiplayerUI.SetActive(false);
                    ClientConnectedToServerUI.SetActive(true);
                    LoadingScreen.SetActive(false);
                }
            }
            catch
            {               
                    ClientCancel();              
            }           
        }
        
        public void StartServer()
        {
            Server = Instantiate(Server_prefab, Vector3.zero, Quaternion.identity).GetComponent<Server>();
            LoadingScreen.SetActive(true);
            Server.StartServer(0);
            ShowPort.text = Server.PortNumber().ToString();
            port.text = Server.PortNumber().ToString();
            Invoke("StartClient", .2f);

        }

    
        public void Back()
        {
            if ( Server != null)
            { StopServer(); }
            if (client != null && Server != null)
            { StopClient(); StopServer(); }
            else if (client != null)
                StopClient();
        }
        public void StopServer()
        {
            Destroy(Server.gameObject);
        }
        public void StopClient()
        {
            Destroy(client.gameObject);
            Destroy(G_M);
        }
        public void LocalHostPlay()
        {
            StartServer();
        }
        #endregion
        public void ClientReady() => client.GetGameClient().SendMessage("Ready");
        public void ClientCancel() => client.GetGameClient().SendMessage("Cancel");


        public void StartSinglePlayer()
        {
            SceneManager.LoadScene(1);
        }
    }
}