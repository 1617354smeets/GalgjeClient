using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            Init();
        }


        private void Init()
        {
            try
            {
                gpio = GpioController.GetDefault();
                gppin = gpio.OpenPin(pinnr);
                gppin.SetDriveMode(GpioPinDriveMode.Output);


            }
            catch
            {
                Debug.Write("Error, kan de gpio pin niet initialiseren.............");
            }
        }

        public void Buzz(int mili)
        {
            gppin.Write(GpioPinValue.High);
            Task.Delay(mili).Wait();
            gppin.Write(GpioPinValue.Low);
        }

    }
}
