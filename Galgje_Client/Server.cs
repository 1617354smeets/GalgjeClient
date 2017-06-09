using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace Galgje_Client
{
    class Server
    {
        private readonly int _port;

        private StreamSocketListener listener;
        private Client server;

        public delegate void DataOntvangenDelegate(string data);
        public DataOntvangenDelegate OnDataOntvangen;

        public Server(int port, Client host)
        {
            server = host;
            _port = port;
            Start();
        }

        public async void Start()
        {
            listener = new StreamSocketListener();
            listener.ConnectionReceived += Listener_ConnectionReceived;

            await listener.BindServiceNameAsync(_port.ToString());
        }

        public void Server_OnDataOntvangen(string data)
        {
            Debug.WriteLine("Data ontvangen van server: " + data);
        }

        public async void Listener_ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            var reader = new DataReader(args.Socket.InputStream);
            try
            {
                while (true)
                {
                    uint sizeFieldCount = await reader.LoadAsync(sizeof(uint));
                    if (sizeFieldCount != sizeof(uint)) return; //Disconnect
                    uint stringLength = reader.ReadUInt32();
                    uint actualStringLength = await reader.LoadAsync(stringLength);
                    if (stringLength != actualStringLength) return; //Disconnect

                    //Zodra data binnen is en er is een functie gekoppeld aan het event:                    
                    if (OnDataOntvangen != null)
                    {
                        //Trigger het event, zodat er iets gedaan wordt met de ontvangen data
                        OnDataOntvangen(reader.ReadString(actualStringLength));
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
