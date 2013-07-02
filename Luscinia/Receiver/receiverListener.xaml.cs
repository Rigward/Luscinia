using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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

namespace AudioCloud.Receiver
{
    /// <summary>
    /// Interaction logic for receiverListener.xaml
    /// </summary>
    public partial class receiverListener : Page
    {
        private _sizes Sizes;
        private Thread verifyThrd;
        private Thread volumeThrd;
        private string IP;
        private string Password;
        private _deviceListReceiver deviceListReceiver;
        private _passCheckReceiver passCheckReceiver;
        private _soundReceive soundReceive;
        private _volumeController volumeController;

        public receiverListener(string _Password)
        {
            InitializeComponent();

            Sizes = new _sizes();
            _Grid.Height = Sizes.Height;
            _Grid.Width = Sizes.Width;

            messageView.Text = "waiting for connect";
            Password = _Password;
            volumeController = new _volumeController();

            deviceListReceiver = new _deviceListReceiver(Password);
            passCheckReceiver = new _passCheckReceiver(deviceListReceiver, Password);

            verifyThrd = new Thread(verifyingCompleted);
            verifyThrd.IsBackground = true;
            verifyThrd.Name = "verifyingCheck";
            verifyThrd.Start();
        }

        private void verifyingCompleted()
        {
            bool Flag = false;
            while (!Flag)
            {
                if (!passCheckReceiver.Thrd.IsAlive) 
                    Flag = true;
                Thread.CurrentThread.Join(0);
            }
            IP = passCheckReceiver.IP;
            Dispatcher.BeginInvoke((Action)(() => messageView.Text = "verifying completed"));
            Thread.Sleep(300);
            Dispatcher.BeginInvoke((Action)(() => messageView.Text = "enjoy music"));
            Dispatcher.BeginInvoke((Action)(() =>  Slonik.Visibility = Visibility.Visible));
            //MessageBox.Show("Verifying completed from", IP);
            soundReceive = new _soundReceive();
            volumeThrd = new Thread(volumeChanged);
            volumeThrd.IsBackground = true;
            volumeThrd.Name = "volumeChange";
            volumeThrd.Start();
        }

        private void volumeChanged()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);     //create new socket

            IPEndPoint bindIP = new IPEndPoint(IPAddress.Parse(IP), 811);     //end point to listen 810
            try
            {
                socket.Bind(bindIP);        //listen  811 port
            }
            catch { }


            IPEndPoint senderIP = new IPEndPoint(IPAddress.Parse(IP), 0);     //end point to answer 
            EndPoint Remote = (EndPoint)(senderIP);

            byte[] data = new byte[512];      //array that will save ping message
            int size = 0;

            do
            {
                if (socket.Available > 0)
                {
                    socket.ReceiveFrom(data, ref Remote);       //read message to data
                    volumeController.setSystemVolume(Convert.ToSingle(System.Text.Encoding.Default.GetString(data)));
                }
                Thread.Sleep(200);
                Thread.CurrentThread.Join(0);
            } while (true);

            socket.Dispose();
            socket.Close();
        }
    }
}
