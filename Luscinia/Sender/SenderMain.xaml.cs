using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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

namespace Luscinia
{
    /// <summary>
    /// Interaction logic for Sender.xaml
    /// </summary>
    public partial class SenderMain : Page
    {
        private _deviceListSender _deviceList;
        public SenderMain()
        {
            InitializeComponent();
            _Grid.Height = Sizes.Height;
            _Grid.Width = Sizes.Width;           
            DeviceList.Height = Sizes.Height / 2 + 30;
            DeviceList.Width = Sizes.Width / 1.5;
            NumberColumn.Width = DeviceList.Width / 3 - 90;
            NameColumn.Width = DeviceList.Width / 3 + 45;
            IPColumn.Width = DeviceList.Width / 3 + 45;
            SearchDevice();
        }
        private void SearchDevice()
        {
            DeviceList.Items.Clear();
            try
            {
                _deviceList = new _deviceListSender();
            }
            catch
            {
                MessageBoxResult show = MessageBox.Show("No devices!");
            }

            _deviceList.Thrd.Join();
            _deviceList.Thrd.Abort();
            addToList(_deviceList.List); 
        }

        private void addToList(Dictionary<string, string> List)
        {
            for (int i = 0; i < List.Count; i++)
            {
                DeviceList.Items.Add(new { Number = i + 1, Name = List.Keys.ElementAt(i), IP = List.Values.ElementAt(i) });
            }
        }

//=============CONTROLS=========================================================================================================

        private void buttonSearchDevice(object sender, RoutedEventArgs e)
        {
            SearchDevice();
        }

        private void DeviceList_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
			if (DeviceList.SelectedItem != null)		//Preventing empty item usage
			{
				UdpClient UDP = new UdpClient();
				IPEndPoint IP = new IPEndPoint(IPAddress.Parse(_deviceList.List.Values.ElementAt(DeviceList.SelectedIndex)), 810);       //create end point
				UDP.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
				byte[] pingTest = Encoding.ASCII.GetBytes("stop");
				UDP.Send(pingTest, pingTest.Length, IP);        //send message
				UDP.Close();

				Luscinia.Sender.SenderController _SenderController = new Luscinia.Sender.SenderController(_deviceList.List.Values.ElementAt(DeviceList.SelectedIndex));
				this.NavigationService.Navigate(_SenderController);
			}
        }

        private void ButtonToStartPage(object sender, RoutedEventArgs e)
        {	
            Luscinia.Start startpage = new Luscinia.Start();
            this.NavigationService.Navigate(startpage);
        }

    }
}
