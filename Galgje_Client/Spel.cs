using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galgje_Client
{
    class Spel
    {
        private string Woord;
        private int aantalleters;
        private List<char> lettersGeweest;
        private int aantalfout;
        private char laatstechar;
        private List<char> geraden;

        Buzzer bzr;

        public int Aantalfout
        {
            get
            {
                return aantalfout;
            }

            set
            {
                aantalfout = value;
            }
        }

        public List<char> LettersGeweest
        {
            get
            {
                return lettersGeweest;
            }

            
        }

        public char Laatstechar
        {
            get
            {
                return laatstechar;
            }

            set
            {
                laatstechar = value;
            }
        }

        public List<char> Geraden
        {
            get
            {
                return geraden;
            }

            set
            {
                geraden = value;
            }
        }

        public Spel(int aantallettters1)
        {
            aantalleters = aantallettters1;
            lettersGeweest = new List<char>();
            aantalfout = 0;
            bzr = new Buzzer(21);
            geraden = new List<char>();
            int i = 0;
            while (i< aantalleters)
            {
                geraden.Add('-');
                i++;
            }
            
        }

        public void VoegLetterToe(char letter)
        {
            lettersGeweest.Add(letter);
        }
        //zodra de speler een fout maakt zal er een fout opgeteld worden bij het aantal fouten en vervolgens zal de buzzer buzzen.
        public void updatefout()
        {
            aantalfout++;
            bzr.Buzz(1500);
        }

       //Update het woord
        public void updateWoord(string data)
        {
            string[] readdata = data.Split('|');
            int i = 0;
            string updatewoord = "";
            Debug.WriteLine(readdata.Length);
            Debug.WriteLine(readdata);

            //Update het te raden woord
            while (i < (readdata.Length - 1))
            {
                geraden[Convert.ToInt16(readdata[i])] = laatstechar;
                i++;
            }
        }

    }
}
