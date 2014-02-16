using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NAudio.Wave;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Luscinia.Sender
{
	/// <summary>
	/// Interaction logic for SenderController.xaml
	/// </summary>


	public partial class SenderCapture : Page
	{
		private IPEndPoint ReceiverAudioAddress;
		private IPEndPoint ReceiverVolumeControlAddress;
		private IWaveIn waveIn; //Device for capturing (in our case - WASAPI loopback)
		private UdpClient udpSender;
		private UdpClient udpVolumeControlSender;

		public double Level;		//Level of sound from another pc, displayed on the slider 

		public SenderCapture(string IP, double _Level)
		{
			InitializeComponent();
			ReceiverAudioAddress = new IPEndPoint(IPAddress.Parse(IP), 810);	//magic number of port
			ReceiverVolumeControlAddress = new IPEndPoint(IPAddress.Parse(IP), 811);	//magic number of port
			_Grid.Height = Sizes.Height;
			_Grid.Width = Sizes.Width;
			udpVolumeControlSender = new UdpClient();
			udpVolumeControlSender.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);	//TODO: understand
			speakerLevel.Value = _Level * 100;//TODO: optimize
			Level = _Level;

		}

		private void Cleanup()
		{
			if (waveIn != null) // working around problem with double raising of RecordingStopped
			{
				waveIn.Dispose();
				waveIn = null;
			}
		}

		void OnDataAvailable(object sender, WaveInEventArgs e)
		{
			if (!(this.Dispatcher.CheckAccess()))		//hecking thread avaiability
			{
				this.Dispatcher.BeginInvoke(new EventHandler<WaveInEventArgs>(OnDataAvailable), sender, e);
			}
			else
			{
				udpSender.Send(e.Buffer, e.BytesRecorded);
			}
		}

		void OnRecordingStopped(object sender, StoppedEventArgs e)
		{
			if (!(this.Dispatcher.CheckAccess()))
			{
				this.Dispatcher.BeginInvoke(new EventHandler<StoppedEventArgs>(OnRecordingStopped), sender, e);
			}
			else
			{
				Cleanup();
				if (e.Exception != null)
				{
					//MessageBox.Show(String.Format("A problem was encountered during recording {0}", e.Exception.Message));
				}
			}
		}


		private void mute(bool flag)
		{
			try
			{
				NAudio.CoreAudioApi.MMDeviceEnumerator MMDE = new NAudio.CoreAudioApi.MMDeviceEnumerator();
				NAudio.CoreAudioApi.MMDeviceCollection DevCol = MMDE.EnumerateAudioEndPoints(NAudio.CoreAudioApi.DataFlow.All, NAudio.CoreAudioApi.DeviceState.All);
				foreach (NAudio.CoreAudioApi.MMDevice dev in DevCol)
				{
					try
					{
						System.Diagnostics.Debug.Print(dev.FriendlyName);
						dev.AudioEndpointVolume.Mute = flag;
					}
					catch { }
				}
			}
			catch { }
		}


		private void ButtonStartCapture(object sender, RoutedEventArgs e)
		{

			mute(true);
			udpSender = new UdpClient();
			waveIn = new WasapiLoopbackCapture();       //assigning waveIn
			waveIn.DataAvailable += OnDataAvailable;    //TODO: understand what this line does
			waveIn.RecordingStopped += OnRecordingStopped;  //TODO: understand, what this line does
			waveIn.StartRecording();
			udpSender.Connect(ReceiverAudioAddress);
		}

		void StopRecording()
		{
			//Debug.WriteLine("StopRecording");
			waveIn.StopRecording();
		}

		private void ButtonStopCapture(object sender, RoutedEventArgs e)
		{
			if (waveIn != null)
			{
				StopRecording();
			}
			mute(false);
			try
			{
				//udpSender.Close();        invokation exception
			}
			catch { }
		}

		private void ChangeReceiverVolume(double Level)
		{
			byte[] vol = Encoding.ASCII.GetBytes((Level / 100).ToString());
			udpVolumeControlSender.Send(vol, vol.Length, ReceiverVolumeControlAddress);		//send needed volume level
		}
		private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)		//If volume changed, send sound level to another pc(this is remote sound control)
		{
			ChangeReceiverVolume(e.NewValue);
		}
		~SenderCapture()
		{
			try
			{
				udpSender.Close();
				udpVolumeControlSender.Close();
			}
			catch { }	//TODO: sometimes udpSender is already null(when closing)
		}
	}
}
