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
/*Created by: Rai Vleugels & Jannick Smeets     |
 *B2D2 2016 - 2017 HBO ICT                      | 
 * 
 */
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

        private Spel game;


        public MainPage()
        {
            this.InitializeComponent();
            server = new Server(raspPort, GameHost);

            //Koppel OnDataOntvangen aan de methode die uitgevoerd worden, handelt de inkomende data af:
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

                //Start een nieuw spel, er wordt een verzoek naar de server gestuurd om een nieuw spel te starten
                GameHost.Verstuur("ID" + Convert.ToString(GameHost.Id) + "|newgame");
            }
            
            //Start een nieuw spel.
            else if(data.StartsWith("START"))
            {
                
                data = data.Replace("START|", "");
                //maak een nieuw spel aan
                int aantal = Convert.ToInt16(data);
                game = new Spel(aantal);

                int ii = 0;
                string tekst = "";
                //Zorgt ervoor dat er evenveel streepjes worden weergeven als het aantal letters dat in het woord voorkomt.
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
            //De controle van het spel, 
            else if (data.StartsWith("checkresponse|"))
            {
                Debug.WriteLine("Checkrespons data: "+data);
                data = data.Replace("checkresponse|", "");
                //De ingevoerde letter is door de gamemaster als goed beoordeeld, verwerk de letters in het woord.
                if (data.StartsWith("goed|"))
                {
                    
                    data = data.Replace("goed|", "");
                    //Update het deel van het woord dat geraden is.
                    game.updateWoord(data);

                    Task.Delay(150).Wait();

                    //Update het woord in de gui, er wordt eerst een string gemaakt van het deel van het woord dat al geraden is.
                    int i = 0;
                    string woord = "";
                    while (i < game.Geraden.Count)
                    {
                        woord = woord + game.Geraden[i];
                        i++;
                    }

                    //Voer de update door.
                    updatewoord(woord);


                    //Controleer of het woord geraden is
                    int xxx = 0;
                    int yyy = 0;
                    while (xxx < game.Geraden.Count)
                    {
                        if (game.Geraden[xxx] == '-')
                        {
                            Debug.WriteLine(game.Geraden[xxx]);
                            yyy++;
                        }
                        xxx++;
                    }

                    if (yyy == 0)
                    {
                        counttonewround("Geraden");
                    }

                }
                //De opgestuurde letter is niet goed, er zijn nu twee mogelijkheden:
                //1. De gebruiker heeft teveel fouten gemaakt en is gameover.
                //2. De gebruiker heeft de letter verkeerd geraden, maar is nog niet gameover.
                else if (data.StartsWith("fout|"))
                {
                    data = data.Replace("fout|", "");
                    //De gebruiker is gameover, het woord wordt nu getoond en er zal een nieuwe ronde gestart worden.
                    if (data.StartsWith("gameover|"))
                    {
                        data = data.Replace("gameover|", "");
                        updatetextb("GAME OVER!!!!\nHet woord was: " + data);
                        //start nieuwe ronde
                        counttonewround("GAME OVER");
                    }

                    //De gebruiker had het fout maar is nog niet gameover.
                    else
                    {
                        //Voeg de fout toe aan het spel
                        game.updatefout();
                        int x = game.Aantalfout;
                        Task.Delay(150).Wait();
                        //Update de galg, eerst wordt de juiste afbeelding gezocht om vervolgens de galg te updaten.
                        updateImg_galg(updateGalg(x));
                    }
                    

                }
                
            }


            //De gestuurde data is geen command, doe er niks mee
            else
            {
                Debug.WriteLine("De ontvangen data is geen command, ontvangen data --> : " + data);
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

        //Update het woord
        public void updatewoord(string tekst)
        {
            Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
() =>
{
    textBlock.Text = tekst;
}
);
        }

        //Countdown to new round
        public void counttonewround(string tekst)
        {
            int i = 20;
            while (i>0)
            {
                updatetextb(tekst +"\nDe volgende ronde start over: " + Convert.ToString(i) + " Seconden");
                Task.Delay(1000).Wait();
                i = i - 1;
            }
            updatetextb("");
            GameHost.Verstuur("ID" + Convert.ToString(GameHost.Id) + "|newgame");

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
                    afbeelding = "Assets/Hangman/TransparentHangmanPart0.png";
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
            //zet de tekst van de tekstbox om naar een string en maak er kleine letters van            
            string tekst = tb_1.Text;
            tekst = tekst.ToLower();
            
            

            //Controlleer of er maar een letter is ingevuld, is dit niet het geval dan krijgt de speler een melding om maar één letter in te vullen.
            if (tekst.Length == 1)
            {
                char chr = Convert.ToChar(tekst);
                //Controleer of de letter al is gebruikt.
                if (game.LettersGeweest.Contains(chr))
                {
                    updatetextb_1("Letter is al gebruikt!");
                }
                //De letter is nog niet gebruikt
                else
                {
                    //Controleer of het ingevoerde character wel in het alfabet voorkomt
                    if (isalfabet(chr))
                    {
                        //Verstuur de letter naar de gamemaster
                        GameHost.Verstuur("ID" + Convert.ToString(GameHost.Id) + "|checkchar|" + tekst);
                        game.Laatstechar = chr;
                        game.VoegLetterToe(chr);

                        updatetextb_1("");

                        //Zet de letter in de lijst met gebruikte letters
                        string lgw = "";
                        int i = 0;
                        while (i < game.LettersGeweest.Count)
                        {
                            lgw = lgw + "-" + Convert.ToString(game.LettersGeweest[i]);
                            i++;
                        }
                        updatetextb(lgw);

                    }
                    //Het ingevoerde character komt niet voor in het alfabet
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
