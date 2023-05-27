using UnityEngine;
using UnityEngine.SceneManagement;
using TBS.NetWork;
using TMPro;
using System.Net;
using System.Net.Sockets;
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
        [SerializeField] TMP_Text ShowHost;

        [SerializeField] GameObject MultiplayerUI;
        [SerializeField] GameObject ClientConnectedToServerUI;

        [SerializeField] GameObject LoadingScreen;
        #region Start and Stop            
        public void StartClient()
        {

            client = Instantiate(client_prefab, Vector3.zero, Quaternion.identity).GetComponent<Client>();
            client.ConnectToServer(GetLocalIPAddressFromCode(int.Parse(ip.text)), int.Parse(port.text));
            ShowPort.text = int.Parse(port.text).ToString();
            ShowHost.text= int.Parse(ip.text).ToString();
            if (Server != null)
            {               
               client.GetGameClient().IsOwner = true;   
            }
            LoadingScreen.SetActive(true);
            MultiplayerUI.SetActive(false);
            G_M = Instantiate(Game_Manger, Vector3.zero, Quaternion.identity);
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
            MultiplayerUI.SetActive(false);
            LoadingScreen.SetActive(true);
            Server.StartServer(0);
            ShowPort.text = Server.PortNumber().ToString();
            ShowHost.text = GetLocalIPAddressInCode().ToString();
            port.text = ShowPort.text;
            ip.text = ShowHost.text;
            Invoke("StartClient", .2f);

        }
        public int GetLocalIPAddressInCode()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    string myip= ip.ToString();

                    Debug.Log(ip.ToString());
                    IPAddress ipAddress = IPAddress.Parse(myip);

                    byte[] ipAddressBytes = ipAddress.GetAddressBytes();
                    int ipAddressInt = BitConverter.ToInt32(ipAddressBytes, 0);
                    return ipAddressInt;
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
        public string GetLocalIPAddressFromCode(int ipAddressInt)
        {
            byte[] ipAddressBytes = BitConverter.GetBytes(ipAddressInt);
            IPAddress ipAddress = new IPAddress(ipAddressBytes);

            string ipAddressString = ipAddress.ToString();

            Debug.Log(ipAddressString);
            return ipAddressString;
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