using System;
using System.Collections.Generic;
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

        public void updatefout()
        {
            aantalfout++;
            bzr.Buzz(500);
        }

        public void toonWoord()
        {

        }

    }
}
