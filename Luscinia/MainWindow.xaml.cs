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

namespace Luscinia
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
		private SenderView Sender;
		private receiver Receiver;
		private Page[] Pages;
		int lol = 5;
        public MainWindow()
        {
			Sender = new SenderView();
			Receiver = new receiver();
			Pages = new Page[] { Sender, Receiver };
			InitializeComponent();
        }

		private void MenuItem_Click(object sender, RoutedEventArgs e)
		{
			Page NextPage;
			switch ((e.Source as MenuItem).Name)
			{
				case "SenderButton": { NextPage = Pages[0]; break; }
				case "ReceiverButton": { NextPage = Pages[1]; break; }
				default: { return; }
			}
			MainFrame.Navigate(NextPage);
		}
		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			App.Current.Shutdown();
		}
    }
}
