using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Core;
using Windows.Devices.Gpio;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Galgje_Client
{
   

    public sealed partial class MainPage : Page
    {
        //Naam of ip-adress van de server:
        private const string raspConnect = "192.168.0.101";
        //Poort waar de server naar luistert (moet dezelfde zijn als de server!):
        private const int raspPort = 9000;

        private Client GameHost = new Client(raspConnect, raspPort);
        private Server server;

        //Om te testen of het verzenden / versturen werkt -> 1 knop aansluiten op pin 21
        private int i = 0;
        private const int pinnr = 21;
        private GpioPin _pinSend;
        private GpioController _gpio;


        private Spel game;


        public MainPage()
        {
            this.InitializeComponent();
            server = new Server(raspPort, GameHost);

            //Koppel OnDataOntvangen aan de methode die uitgevoerd worden:
            server.OnDataOntvangen += DataOntvangenVanServer;


            //Pin 21 initialiseren:
            _gpio = GpioController.GetDefault();
            _pinSend = _gpio.OpenPin(pinnr);
            _pinSend.SetDriveMode(GpioPinDriveMode.InputPullUp);
            _pinSend.DebounceTimeout = TimeSpan.FromMilliseconds(50);
            _pinSend.ValueChanged += _pinSend_ValueChanged;
        }

        
        public void DataOntvangenVanServer(string data)
        {
            if (data.StartsWith("SuccesfullAdded|"))
            {
                data = data.Replace("SuccesfullAdded|","");
                GameHost.Id = Convert.ToInt16(data);

                Debug.Write("connection gelukt id:" + Convert.ToString(GameHost.Id));
            }

            else if(data.StartsWith("START"))
            {
                //Debug.Write(data);
                data = data.Replace("START|", "");
                //Debug.Write(data);
                int aantal = Convert.ToInt16(data);
                game = new Spel(aantal);


                updatetextb(Convert.ToString(aantal));
                //Debug.WriteLine("NICEONE");
                //button.Visibility = Visibility.Collapsed;
            }
            else if (data.StartsWith("checkresponse|"))
            {
                data = data.Replace("checkresponse|", "");
                if (data != "")
                {

                }
                else
                {
                    //updategalg
                }
            }


            else
            {
                Debug.WriteLine("maindata: " + data);
            }
        }


        private void _pinSend_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs e)
        {
            if (e.Edge == GpioPinEdge.FallingEdge)
            {
                GameHost.Verstuur("ButtonPressed " + i.ToString());
                Debug.WriteLine("ButtonPressed " + i.ToString());
                i++;
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            GameHost.Verstuur("ID" + Convert.ToString(GameHost.Id)+ "|newgame");
        }

        public void updatetextb(string tekst)
        {
            Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
() =>
{
    textBlock.Text = tekst;
}
);
        }
        public void updatetextb_1(string tekst)
        {
            Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
() =>
{
    tb_1.Text = tekst;
}
);
        }


        private void btn_check_Click(object sender, RoutedEventArgs e)
        {
            string tekst = tb_1.Text;
            if(tekst.Length == 1)
            {
                GameHost.Verstuur("ID" + Convert.ToString(GameHost.Id) + "|checkchar|" + tekst);
            }
            else
            {
                updatetextb_1("Voer één letter in");
            }
        }
    }
}
