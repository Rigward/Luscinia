using System;
using System.Collections.Generic;
using System.Linq;
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

namespace AudioCloud.Sender
{
    /// <summary>
    /// Interaction logic for SenderController.xaml
    /// </summary>
    

    public partial class SenderCapture : Page
    {
        private Thread Thrd;
        private _sizes Sizes;
        private string IP;

        private IWaveIn waveIn; //Device for capturing (in our case - WASAPI loopback)
        UdpClient udpSender;   

        public double Level;

        public SenderCapture(string _IP, double _Level)
        {
            InitializeComponent();

            Sizes = new _sizes();

            IP = _IP;

            _Grid.Height = Sizes.Height;
            _Grid.Width = Sizes.Width;

            speakerLevel.Value = _Level*100;
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
            if (!(this.Dispatcher.CheckAccess()))
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
                    MessageBox.Show(String.Format("A problem was encountered during recording {0}", e.Exception.Message));
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
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(IP), 810);
            waveIn = new WasapiLoopbackCapture();       //assigning waveIn


            waveIn.DataAvailable += OnDataAvailable;    //Need to understand, what this line does
            waveIn.RecordingStopped += OnRecordingStopped;  //Need to understand, what this line does

            waveIn.StartRecording();
            udpSender.Connect(endPoint);
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
            catch{}

        }

        
        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Level = e.NewValue;
            Thrd = new Thread(() => 
                            {
                                UdpClient UDP = new UdpClient();
                                IPEndPoint receiverIP = new IPEndPoint(IPAddress.Parse(IP), 811);       //create end point
                                byte[] vol = Encoding.ASCII.GetBytes((Level/100).ToString());
                                UDP.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                                UDP.Send(vol, vol.Length, receiverIP);        //send message
                                UDP.Close();
                                Thread.CurrentThread.Join(0);
                            }
                                );
            Thrd.IsBackground = true;
            Thrd.Priority = ThreadPriority.Lowest;
            Thrd.Start();
        }
        ~SenderCapture()
        {
            udpSender.Close();
        }
        
    }
}
