using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace Galgje_Client
{
    class Buzzer
    {
        private int pinnr;
        private GpioPin gppin;
        private GpioController gpio;

        public Buzzer(int pin)
        {
            pinnr = pin;
        }



        public void Buzz(int mili)
        {
            //buzz voor een aantal milliseconde
        }

    }
}
