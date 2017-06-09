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

        public MainPage()
        {
            this.InitializeComponent();
            server = new Server(raspPort, GameHost);

            //Koppel OnDataOntvangen aan de methode die uitgevoerd worden:
            server.OnDataOntvangen += server.Server_OnDataOntvangen;


            //Pin 21 initialiseren:
            _gpio = GpioController.GetDefault();
            _pinSend = _gpio.OpenPin(pinnr);
            _pinSend.SetDriveMode(GpioPinDriveMode.InputPullUp);
            _pinSend.DebounceTimeout = TimeSpan.FromMilliseconds(50);
            _pinSend.ValueChanged += _pinSend_ValueChanged;
        }

        private void DataOntvangen(string data)
        {
            Debug.WriteLine(data);
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
    }
}
