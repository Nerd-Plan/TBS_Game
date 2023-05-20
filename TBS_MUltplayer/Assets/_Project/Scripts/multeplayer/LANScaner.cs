using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

public class LANScanner
{
    private const int Timeout = 100;

    public string ScanLANForApplication(int targetPort)
    {
        string localIP = GetLocalIPAddress();
        string[] ipParts = localIP.Split('.');

        for (int i = 1; i <= 255; i++)
        {
            string ipAddress = ipParts[0] + "." + ipParts[1] + "." + ipParts[2] + "." + i;
            if (IsPortOpen(ipAddress, targetPort))
            {
                Console.WriteLine("Found active device with application running: " + ipAddress);
                // Open the application or perform further actions here
                return ipAddress;
            }
        }
        return null;
    }

    private bool IsPortOpen(string ipAddress, int port)
    {
        try
        {
            using (TcpClient client = new TcpClient())
            {
                var result = client.BeginConnect(ipAddress, port, null, null);
                bool success = result.AsyncWaitHandle.WaitOne(Timeout);
                if (success)
                {
                    client.EndConnect(result);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        catch (Exception)
        {
            return false;
        }
    }

    private string GetLocalIPAddress()
    {
        string localIP = "";
        foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
            {
                foreach (UnicastIPAddressInformation ipInfo in networkInterface.GetIPProperties().UnicastAddresses)
                {
                    if (ipInfo.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        localIP = ipInfo.Address.ToString();
                        break;
                    }
                }
            }
        }
        return localIP;
    }
}
