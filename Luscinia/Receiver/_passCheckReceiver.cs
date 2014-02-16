using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Luscinia
{
    public class _passCheckReceiver
    {
        public Thread Thrd;
        public string IP;
        private string Password;
        private _deviceListReceiver deviceListReceiver;

        public _passCheckReceiver(_deviceListReceiver __deviceListReceiver, string _Password)
        {
            deviceListReceiver = __deviceListReceiver;
            Password = _Password;
            Thrd = new Thread(checkAccess);
            Thrd.IsBackground = true;
            Thrd.Name = "checkAccessReceiver";
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

        private void checkAccess()
        {

            while (deviceListReceiver.Thrd.IsAlive)
            {
                IP = deviceListReceiver.IP;
            }
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);     //create new socket

            IPEndPoint receiverIP = new IPEndPoint(IPAddress.Any, 810);     //end point to listen 810
            socket.Bind(receiverIP);        //listen  810 port

            IPEndPoint senderIP = new IPEndPoint(IPAddress.Any, 0);     //end point to answer 
            EndPoint Remote = (EndPoint)(senderIP);
            int size = 0;
            do
            {
                if (socket.Available > 0)
                {
                    byte[] data = new byte[256];      //array that will save ping message
                    size = socket.ReceiveFrom(data, ref Remote);       //read message to data
                    Array.Resize(ref data, size);
                    if (System.Text.Encoding.Default.GetString(data) == Password)
                    {
                        _volumeController volumeController = new _volumeController();
                        byte[] checkFlag = Encoding.ASCII.GetBytes(volumeController.Volume.ToString());
                        socket.SendTo(checkFlag, checkFlag.Length, SocketFlags.None, _getHost(Remote.ToString(), 811));    //send answer to 811 port
                        break;
                    }
                    else
                    {
                        byte[] checkFlag = Encoding.ASCII.GetBytes("false");
                        socket.SendTo(checkFlag, checkFlag.Length, SocketFlags.None, _getHost(Remote.ToString(), 811));    //send answer to 811 port
                    }
                }
                Thread.CurrentThread.Join(0);
            } while (true);
            socket.Dispose();
            socket.Close();
        }

    }
}
