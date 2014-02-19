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

namespace Luscinia
{
    /// <summary>
    /// Interaction logic for Receiver.xaml
    /// </summary>
    public partial class receiver : Page
    {
        private Thread verifyThrd;
        private Thread volumeThrd;
        private string IP;

        private _deviceListReceiver deviceListReceiver;
        private _passCheckReceiver passCheckReceiver;
        private _soundReceive soundReceive;
        private _volumeController volumeController;
        
        public receiver()
        {
            InitializeComponent();
            _Grid.Height = Sizes.Height;
            _Grid.Width = Sizes.Width;
            volumeController = new _volumeController();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Luscinia.Receiver.receiverListener receiverListener = new Luscinia.Receiver.receiverListener(Password.GetLineText(0));
            this.NavigationService.Navigate(receiverListener);
        }

        private void ButtonStartPage(object sender, RoutedEventArgs e)
        {

            //Luscinia.Start startpage = new Luscinia.Start();	//Old return back button
            //this.NavigationService.Navigate(startpage);
        }

    }
}
