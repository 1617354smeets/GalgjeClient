using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.WiFi;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace Galgje_Client
{
    class Client
    {


        private const string IP = "192.168.0.103";

        private readonly string _ip;
        private readonly int _port;
        private StreamSocket _socket;
        private DataWriter _writer;

        private int id;

        public delegate void Error(string message);
        public delegate void DataRecived(string data);

        public string Ip { get { return _ip; } }
        public int Port { get { return _port; } }

        public int Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        public Client(string ip, int port)
        {
            _ip = ip;
            _port = port;
            Task.Run(() => Connect()).Wait();
        }

        public async void Connect()
        {
            try
            {
                var hostName = new HostName(Ip);
                _socket = new StreamSocket();
                await _socket.ConnectAsync(hostName, Port.ToString());
                _writer = new DataWriter(_socket.OutputStream);
                Task.Delay(50).Wait();

                //Verstuur een bericht naar de server met eigen IP-Adres
                Verstuur(IP);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        //Verstuur data naar de server
        public async void Verstuur(string message)
        {

            if (_writer != null)
            {
                _writer.WriteUInt32(_writer.MeasureString(message));
                _writer.WriteString(message);

                try
                {
                    await _writer.StoreAsync();
                    await _writer.FlushAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
            else
            {
                Debug.WriteLine("no connection");
            }
        }

        public IPAddress GetIpAddress()
        {
            var hosts = NetworkInformation.GetHostNames();
            foreach (var host in hosts)
            {
                IPAddress addr;
                if (IPAddress.TryParse(host.DisplayName, out addr))
                    if ((host.DisplayName != IP) && (addr.AddressFamily == AddressFamily.InterNetwork))
                        return addr;
            }
            return null;
        }
    }
}
