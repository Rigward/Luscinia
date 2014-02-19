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

namespace Luscinia
{
    public class _deviceListSender
    {
		//ping sending variables
		UdpClient PingSender;   // Create UDP client and send message to broadcast
		UdpClient PingReceiver;
		byte[] PingMessage;
		//End of ping sending variables
        public Dictionary<string,string> List;
        public Thread Thrd;
        byte[] data;
        IPEndPoint Remote;
        public _deviceListSender()
        {
			//Broadcast message sender configurations

			PingMessage = Encoding.ASCII.GetBytes("ping");
			//End of broadcast sender settings
			//response receiver configuration
			
			//end of response receiver configuration
            // Listen for answer
            data = new byte[8];      //array that will save ping message 
            Remote = new IPEndPoint(IPAddress.Any, 811);     //SAVES REAL PING RESPONSER`S ADRESS, CAN WRITE ANYTHING
            List = new Dictionary<string, string>();
           
            PingSend();
        }

        private void getHostName(string IP)
        {
			//string test = Remote.ToString();
            string host = IP.Remove(IP.IndexOf(":"), IP.Length - IP.IndexOf(":"));
            IPAddress hostIPAddress = IPAddress.Parse(host);
            IPHostEntry hostName = Dns.GetHostEntry(hostIPAddress);
            List.Add(hostName.HostName, hostIPAddress.ToString());
        }

        public void PingSend()
        {
			List.Clear();
			PingSender = new UdpClient();
			PingSender.EnableBroadcast = true;
			PingSender.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);//TODO: REMAKE WHOLE CLASS TO HANDLE BUSY PORT
			try
			{
				PingSender.Connect(IPAddress.Broadcast, 810);	//magic number - receiver`s port
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.ToString());
			}
            PingSender.Send(PingMessage, PingMessage.Length);        //send message
			Thrd = new Thread(Listen);
			Thrd.Start(); 
            PingSender.Close();
        }
		private void Listen()
		{
			try
			{
				PingReceiver = new UdpClient(811);
			}
			catch (Exception)
			{
			}

			PingReceiver.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
			int notAvailable = 0;
			Thread.Sleep(100);
			while (notAvailable < 10)
			{
				try
				{
					if (PingReceiver.Available > 0)
					{
						data = PingReceiver.Receive(ref Remote);      //read message to data
						getHostName(Remote.ToString());
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
			PingReceiver.Close();
		}

    }
}
