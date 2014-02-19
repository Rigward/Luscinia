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
	enum Requests 
	{ 
		PingRequest = 1, 
		ConnectionRequest = 2, 
		VolumeChangeRequest = 3, 
		DisconnectionRequest = 4,
		PingResponse = 5
	}
	class Networker
	{
		private UdpClient DataSender;
		private UdpClient DataReceiver;
		private byte[] data;
		private IPEndPoint Remote;
		private Thread ListeningThread;
		public Networker()
		{
			DataSender = new UdpClient();
			DataReceiver = new UdpClient();
			data = new byte[8];
			IPEndPoint Remote = new IPEndPoint(IPAddress.Any, 811);     //SAVES REAL PING RESPONSER`S ADRESS, CAN WRITE ANYTHING
			ListeningThread = new Thread(Listen);
		}
		public void Listen()
		{
			try
			{
				DataReceiver.Connect(IPAddress.Any, 811);
			}
			catch (Exception e)
			{
				return;
			}
			while(true)
			{
				if (DataReceiver.Available > 0)
				{
					data = DataReceiver.Receive(ref Remote);
					ParseInput(data, Remote);
				}
			}
		}
		public void SendData(string Message, IPEndPoint Remote)
		{
			data = Encoding.ASCII.GetBytes(Message);
			DataSender.Send(data, data.Length, Remote);
		}
		public void ParseInput(byte[] data, IPEndPoint Remote)
		{
			string Message = Encoding.ASCII.GetString(data);
			switch (Message[0])
			{
				case '1': { ResponseToPingRequest(Remote); break; }
			}
		}
		public void ResponseToPingRequest(IPEndPoint Remote)
		{
			SendData(((int)Requests.PingResponse).ToString(), Remote);
		}
	}
}
