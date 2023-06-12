using UnityEngine;
using UnityEngine.SceneManagement;
using TBS.NetWork;
using TMPro;
using System.Net;
using System.Net.Sockets;
using System;
using System.Text;
using System.Linq;

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
            if(ip.text==string.Empty|| port.text == string.Empty ||port.text.Any(char.IsLetter)|| ip.text.Any(char.IsLetter))
            {
                return;
            }
            MultiplayerUI.SetActive(false);
            client = Instantiate(client_prefab, Vector3.zero, Quaternion.identity).GetComponent<Client>();
            client.ConnectToServer(GetLocalIPAddressFromCode(int.Parse(ip.text)), int.Parse(port.text));                     
            ShowPort.text = int.Parse(port.text).ToString();
            ShowHost.text= int.Parse(ip.text).ToString();
            if (Server != null)
            {               
               client.GetGameClient().IsOwner = true;   
            }
            G_M = Instantiate(Game_Manger, Vector3.zero, Quaternion.identity);
            if(client.GetGameClient().HasClinet())
            {
                ClientConnectedToServerUI.SetActive(true); 
            }
            else
            {
                Destroy(G_M);
                MultiplayerUI.SetActive(true);
                Back();
            }    
        }
        
        public void StartServer()
        {
            Server = Instantiate(Server_prefab, Vector3.zero, Quaternion.identity).GetComponent<Server>();
            Server.StartServer(0);
            ShowPort.text = Server.PortNumber().ToString();
            ShowHost.text = GetLocalIPAddressInCode().ToString();
            port.text = ShowPort.text;
            ip.text = ShowHost.text;
            StartClient();

        }
        public int GetLocalIPAddressInCode()
        {
            string localIP;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
            }
            string[] octets = localIP.Split('.');
            int decimalValue = 0;
            for (int i = 0; i < 4; i++)
            {
                decimalValue += int.Parse(octets[i]) * (int)Math.Pow(256, (3 - i));
            }
            return decimalValue;
        }
        public string GetLocalIPAddressFromCode(int ipAddressInt)
        {
            byte[] ipAddressBytes = BitConverter.GetBytes(ipAddressInt);
            Array.Reverse(ipAddressBytes); // Reverse the byte order
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
            AudioManger.Instance.PlayMusic("GameMainMelody");
        }
        public void Quit()
        {
            Application.Quit();
        }
    }
}