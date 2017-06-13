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
using Windows.UI.Xaml.Media.Imaging;

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

        private int testint;

        private Spel game;


        public MainPage()
        {
            this.InitializeComponent();
            server = new Server(raspPort, GameHost);

            //Koppel OnDataOntvangen aan de methode die uitgevoerd worden:
            server.OnDataOntvangen += DataOntvangenVanServer;

            

           
            
        }

        //Verwerkt de binnenkomende data
        public void DataOntvangenVanServer(string data)
        {
            //De client is succesvol met de server verbonden en krijgt een id meegestuurt, dit id wordt bij opdrachten meegestuurd zodat de server weet welke client iets heeft opgevraagd.
            if (data.StartsWith("SuccesfullAdded|"))
            {
                data = data.Replace("SuccesfullAdded|","");
                GameHost.Id = Convert.ToInt16(data);

                Debug.Write("connection gelukt id:" + Convert.ToString(GameHost.Id));
            }
            
            //Start een nieuw spel.
            else if(data.StartsWith("START"))
            {
                
                data = data.Replace("START|", "");
                //Debug.Write(data);
                //maak een nieuw spel aan
                int aantal = Convert.ToInt16(data);
                game = new Spel(aantal);

                int ii = 0;
                string tekst = "";
                while (ii < aantal)
                {
                    tekst = tekst + " - ";
                    ii++;
                }

                updatewoord(tekst);
                //updatetextb(Convert.ToString(aantal));
                //Debug.WriteLine("NICEONE");
                //button.Visibility = Visibility.Collapsed;
            }
            //De controle van het spel
            else if (data.StartsWith("checkresponse|"))
            {
                Debug.WriteLine("Checkrespons data: "+data);
                data = data.Replace("checkresponse|", "");
                if (data.StartsWith("goed|"))
                {
                    
                    data = data.Replace("goed|", "");
                    game.updateWoord(data);

                    Task.Delay(150).Wait();

                    int i = 0;
                    string woord = "";
                    while (i < game.Geraden.Count)
                    {
                        woord = woord + game.Geraden[i];
                        i++;
                    }

                    updatewoord(woord);
                    
                    

                }
                else if (data.StartsWith("fout|"))
                {
                    data = data.Replace("fout|", "");
                    if (data.StartsWith("gameover|"))
                    {
                        data = data.Replace("gameover|", "");

                        updatetextb("GAME OVER!!!!\nHet woord was: " + data);
                    }
                    else
                    {
                        game.updatefout();
                        //Task.Delay(150).Wait();
                        Debug.WriteLine("okkeeee");
                        int x = game.Aantalfout;
                        Task.Delay(150).Wait();
                        updateImg_galg(updateGalg(x));
                    }
                    

                }
                
            }



            else
            {
                Debug.WriteLine("maindata: " + data);
            }
        }


       

        //Start een nieuw spel, verstuurt het command naar de gamemaster
        private void button_Click(object sender, RoutedEventArgs e)
        {
            GameHost.Verstuur("ID" + Convert.ToString(GameHost.Id)+ "|newgame");
        }

        //Update de gebruikte letters
        public void updatetextb(string tekst)
        {
            Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
() =>
{
    tb_letters.Text = tekst;
}
);
        }
        //Update de afbeelding van de galg
        public async void updateImg_galg(string tekst)
        {
            
            Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
() =>
{
    // Your UI update code goes here!
    BitmapImage bpm = new BitmapImage(new Uri(this.BaseUri, tekst));
    img_galg.Source = bpm;
}
);
        }

        //update de tekst van tb1 
        public void updatetextb_1(string tekst)
        {
            Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
() =>
{
    tb_1.Text = tekst;
}
);
        }

        public void updatewoord(string tekst)
        {
            Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
() =>
{
    textBlock.Text = tekst;
}
);
        }

        //Update de galg
        public string updateGalg(int nr)
        {
            string afbeelding;
            switch(nr)
            {
                case 1:
                    afbeelding = "Assets/Hangman/Hn1.png";
                    break;
                case 2:
                    afbeelding = "Assets/Hangman/Hn2.png";
                    break;
                case 3:
                    afbeelding = "Assets/Hangman/Hn3.png";
                    break;
                case 4:
                    afbeelding = "Assets/Hangman/Hn4.png";
                    break;
                case 5:
                    afbeelding = "Assets/Hangman/Hn5.png";
                    break;
                case 6:
                    afbeelding = "Assets/Hangman/Hn6.png";
                    break;
                case 7:
                    afbeelding = "Assets/Hangman/Hn7.png";
                    break;
                case 8:
                    afbeelding = "Assets/Hangman/Hn8.png";
                    break;
                case 9:
                    afbeelding = "Assets/Hangman/Hn9.png";
                    break;
                case 10:
                    afbeelding = "Assets/Hangman/Hn10.png";
                    break;
                case 11:
                    afbeelding = "Assets/Hangman/Hn11.png";
                    break;
                default:
                    afbeelding = "Assets/Hangman/Hn1.png";
                    break;

                
            }
            return afbeelding;
        }
        //Controlleer of het ingevoerde character wel in het alfabet voorkomt
        private bool isalfabet(char chr)
        {
            if (chr >= 'a' && chr <='z')
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //Verstuur de letter naar de gamemaster voor controle, de letter wordt ook toegevoegd aan de lijst met gebruikte letters.
        private void btn_check_Click(object sender, RoutedEventArgs e)
        {
            /*
            testint++;
            updateImg_galg(updateGalg(testint));
            */

            
            string tekst = tb_1.Text;
            tekst = tekst.ToLower();
            
            

            
            if (tekst.Length == 1)
            {
                char chr = Convert.ToChar(tekst);
                if (game.LettersGeweest.Contains(chr))
                {
                    updatetextb_1("Letter is al gebruikt!");
                }

                else
                {
                    if (isalfabet(chr))
                    {
                        GameHost.Verstuur("ID" + Convert.ToString(GameHost.Id) + "|checkchar|" + tekst);
                        game.Laatstechar = chr;
                        game.VoegLetterToe(chr);

                        string lgw = "";
                        int i = 0;
                        while (i < game.LettersGeweest.Count)
                        {
                            lgw = lgw + "-" + Convert.ToString(game.LettersGeweest[i]);
                            i++;
                        }
                        updatetextb(lgw);

                    }
                    else
                    {
                        updatetextb_1("Voer één letter in");
                    }
                }

                
            }
            else
            {
                updatetextb_1("Voer één letter in");
            }
            
        }
    }
}
