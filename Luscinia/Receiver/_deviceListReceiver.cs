using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Luscinia
{
    public class _deviceListReceiver
    {
        public Thread Thrd;
        public string IP;
        private string Password;
        private byte[] data;

        //public _passCheckReceiver passCheckReceiver;

        public _deviceListReceiver(string _Password)
        {
            Password = _Password;
            Thrd = new Thread(_deviceListListen);
            Thrd.IsBackground = true;
            Thrd.Name = "DeviceListReceiver";
            Thrd.Start();
        }

        private EndPoint _getHost(string IP, int Port)
        {
            string host = IP.Remove(IP.IndexOf(":"), IP.Length - IP.IndexOf(":"));
            IPAddress hostIPAddress = IPAddress.Parse(host);
            IPEndPoint hostIPEndPoint = new IPEndPoint(hostIPAddress, Port);
            EndPoint sendToEndPoint = (EndPoint)(hostIPEndPoint);
            return sendToEndPoint;
        }

        public void _deviceListListen()
        {
            data = new byte[256];      //array that will save ping message
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);     //create new socket
            
            IPEndPoint receiverIP = new IPEndPoint(IPAddress.Any, 810);     //end point to listen 810

            try
            {
                socket.Bind(receiverIP);        //listen  810 port
            }
            catch { }
            
            IPEndPoint senderIP = new IPEndPoint(IPAddress.Any, 0);     //end point to answer 
            EndPoint Remote = (EndPoint)(senderIP);
            double size = 0;
            do
            {
                if (socket.Available > 0)
                {
                    try
                    {
                        size = socket.ReceiveFrom(data, ref Remote);       //read message to data
                    }
                    catch{}
                    
                    Array.Resize(ref data, (int)size);
                    if (System.Text.Encoding.Default.GetString(data) == "ping")
                        socket.SendTo(data, data.Length, SocketFlags.None, _getHost(Remote.ToString(), 811));    //send answer to 811 port
                    else
                    {
                        IP = Remote.ToString().Remove(Remote.ToString().IndexOf(":"), Remote.ToString().Length - Remote.ToString().IndexOf(":"));
                        break;
                    }
                }
                Thread.Sleep(100);
                Thread.CurrentThread.Join(0);
            } while (true);

            socket.Dispose();
            socket.Close();

            //passCheckReceiver = new _passCheckReceiver(IP, Password);

        }


    }
}
