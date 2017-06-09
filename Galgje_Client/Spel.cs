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


        public Spel(int aantallettters1)
        {
            aantalleters = aantallettters1;
            lettersGeweest = new List<char>();
            aantalfout = 0;
        }

        public void VoegLetterToe(char letter)
        {
            lettersGeweest.Add(letter);
        }

        public void updatefout()
        {
            aantalfout++;
        }

        public void toonWoord()
        {

        }

    }
}
