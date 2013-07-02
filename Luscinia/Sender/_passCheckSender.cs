using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace AudioCloud
{
    class _passCheckSender
    {
        public Thread Thrd;

        public bool checkFlag;
        public float Volume;
        byte[] Password;
        private string IP;

        public _passCheckSender(string _IP, string _Password)
        {
            IP = _IP;
            Password = Encoding.ASCII.GetBytes(_Password);
            Thrd = new Thread(checkAccess);
            Thrd.Name = "checkAccessSender";
            Thrd.Start();
        }

        private void checkAccess()
        {
            UdpClient UDP = new UdpClient();
            IPEndPoint receiverIP = new IPEndPoint(IPAddress.Parse(IP), 810);       //create end point

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);     //create new socket
            IPEndPoint bindIP = new IPEndPoint(IPAddress.Any, 811);     //end point to listen response

            UDP.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            UDP.Send(Password, Password.Length, receiverIP);        //send message
            UDP.Close();

            try
            {
                socket.Bind(bindIP);        //listen  811 port
            }
            catch { }

            IPEndPoint senderIP = new IPEndPoint(IPAddress.Any, 0);     //end point to answer 
            EndPoint Remote = (EndPoint)(senderIP);

            byte[] data = new byte[256];      //array that will save ping message
            int size = 0;

            do
            {
                if (socket.Available > 0)
                {
                    size = socket.ReceiveFrom(data, ref Remote);       //read message to data
                    Array.Resize(ref data, size);
                    if (System.Text.Encoding.Default.GetString(data) != "false")
                    {
                        Volume = Convert.ToSingle(System.Text.Encoding.Default.GetString(data));    //mute sound on sender
                        checkFlag = true;
                        break;
                    }
                    else
                    {
                        checkFlag = false;
                        break;
                    }
                }
                Thread.CurrentThread.Join(0);
            } while (true);
            socket.Dispose();
            socket.Close();
            Thrd.Abort();
        }
    }
}
