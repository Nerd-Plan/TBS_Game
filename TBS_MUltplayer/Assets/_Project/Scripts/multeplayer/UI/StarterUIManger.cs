
using UnityEngine;
using UnityEngine.SceneManagement;
using TBS.NetWork;
namespace TBS.UI
{
    public class StarterUIManger : MonoBehaviour
    {
        [SerializeField] GameObject client_prefab;
        [SerializeField] GameObject Server_prefab;
        [SerializeField] GameObject Game_Manger;


        [SerializeField] Server Server;
        [SerializeField] Client client;


        [SerializeField] string ip;
        [SerializeField] int port;

        #region Start and Stop
        public void StartClient()
        {
            client = Instantiate(client_prefab, Vector3.zero, Quaternion.identity).GetComponent<Client>();
            client.ConnectToServer(ip,port);
             Instantiate(Game_Manger, Vector3.zero, Quaternion.identity);
        }
        public void StartServer()
        {
            Server = Instantiate(Server_prefab, Vector3.zero, Quaternion.identity).GetComponent<Server>();
            Server.StartServer(port);            
            return;
        }
        public void Back()
        {
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
        }
        public void LocalHostPlay()
        {
            StartServer();
            StartClient();
        }
        #endregion
        public void ClientReady() => client.GetGameClient().SendMessage("Ready");
        public void ClientCancel() => client.GetGameClient().SendMessage("Cancel");

    }
}