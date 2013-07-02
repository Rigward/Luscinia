using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AudioCloud
{
    public class _deviceListSender
    {
        public Dictionary<string,string> List;
        public Thread Thrd;

        UdpClient UDP;
        byte[] data;
        Socket socket;
        IPEndPoint senderIP;
        IPEndPoint receiverIP;
        EndPoint Remote;

        public _deviceListSender()
        {

            // Listen for answer
            data = new byte[8];      //array that will save ping message 
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);     //create new socket

            receiverIP = new IPEndPoint(IPAddress.Any, 811);     //end point to listen 811
            try 
            { 
                socket.Bind(receiverIP); 
            }        //listen 811 port
            catch 
            { }

            senderIP = new IPEndPoint(IPAddress.Any, 0);     //end point to answer
            Remote = (EndPoint)(senderIP);

            List = new Dictionary<string, string>();
            
            Thrd = new Thread(Listen);
            Thrd.Name = "FindDevices";
            
            Thrd.Start();
            
            this.PingSend();
        }

        private void _getHostName(string IP, Dictionary<string,string> DataList)
        {
            string host = IP.Remove(IP.IndexOf(":"), IP.Length - IP.IndexOf(":"));
            IPAddress hostIPAddress = IPAddress.Parse(host);
            IPHostEntry hostName = Dns.GetHostEntry(hostIPAddress);
            DataList.Add(hostName.HostName, hostIPAddress.ToString());
        }

        public void PingSend()
        {
            // Create UDP client and send message to broadcast
            UDP = new UdpClient();
            UDP.EnableBroadcast = true;     //enable broadcast send
            IPEndPoint IP = new IPEndPoint(IPAddress.Broadcast, 810);       //create end point
            UDP.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            byte[] pingTest = Encoding.ASCII.GetBytes("ping");
            UDP.Send(pingTest, pingTest.Length, IP);        //send message
            UDP.Close(); 
        }

        private void Listen()
        { 
            int notAvailable = 0;

            Thread.Sleep(100);

            while(notAvailable < 10)
            {
                try
                {
                    if (socket.Available > 0)
                    {
                        socket.ReceiveFrom(data, ref Remote);       //read message to data
                        _getHostName(Remote.ToString(), List);
                    }
                    else notAvailable++;

                }
                catch (SocketException e)
                {
                    if (e.SocketErrorCode == SocketError.TimedOut)
                    {
                        notAvailable++;
                    }
                }
            } 
            socket.Dispose();
            socket.Close();
        }

    }
}
