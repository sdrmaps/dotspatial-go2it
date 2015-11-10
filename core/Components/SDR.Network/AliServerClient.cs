using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SDR.Network
{
    public delegate void PacketReceievedEventHandler(object sender, AliServerDataPacket packet);

    public class AliServerClient
    {
        public event PacketReceievedEventHandler PacketReceieved;

        protected virtual void OnPacketReceieved(AliServerDataPacket packet)
        {
            if (PacketReceieved != null)
            {
                PacketReceieved(this, packet);
            }
        }

        private readonly UdpClient _udpClient;
        private readonly IPEndPoint _endPoint;
        
        private string _udpHost;
        private int _udpPort;

        private bool _ping;
        private bool _loggedIn;

        public bool LoggedIn
        {
            get { return _loggedIn; }
        }

        public string UdpHost
        {
            get { return _udpHost; }
            set
            {
                _udpHost = value;
                Reconnect();
            }
        }

        public int UdpPort
        {
            get
            {
                return _udpPort;
            }
            set
            {
                _udpPort = value;
                Reconnect();
            }
        }

        private void Reconnect()
        {
            if (_loggedIn)
            {
                Logout();
                Connect();
                Login();
            }
            else
            {
                Connect();
            }
        }

        public AliServerClient(string udpHost, int udpPort)
        {
            _udpHost = udpHost;
            _udpPort = udpPort;

            // set up a remote udp host to communicate with
            _udpClient = new UdpClient();
            _endPoint = new IPEndPoint(IPAddress.Any, 0);

            // setup the connection with the host and port information
            Connect();
        }

        private void Connect()
        {
            var ip = ConvertToIpAddress(_udpHost);
            if (ip == null) // in this case we were given a host name
            {
                _udpClient.Connect(_udpHost, _udpPort);
            }
            else // and in this case we have been given an ip address
            {
                _udpClient.Connect(ip, _udpPort);
            }
        }

        private class UdpState
        {
            public readonly UdpClient Udpclient;
            public readonly IPEndPoint Endpoint;

            public UdpState(UdpClient udpclient, IPEndPoint endpoint)
            {
                Udpclient = udpclient;
                Endpoint = endpoint;
            }
        }

        private IPAddress ConvertToIpAddress(string ipString)
        {
            try
            {
                return IPAddress.Parse(ipString);
            }
            catch
            {
                return null;
            }
        }

        public void Close()
        {
            Logout();
            _udpClient.Close();
        }

        public void Logout()
        {
            if (_loggedIn)
            {
                // send a logout command to the server
                var sendLogout = new AliServerDataPacket
                {
                    Message = null,
                    Name = _udpHost,
                    Command = Command.Logout
                };
                byte[] message = sendLogout.ToByte();
                _udpClient.Send(message, message.Length);
                _loggedIn = false;
            }
        }

        public void Login()
        {
            if (!_loggedIn)
            {
                var sendLogin = new AliServerDataPacket
                {
                    Message = null,
                    Name = _udpHost,
                    Command = Command.Login
                };
                byte[] message = sendLogin.ToByte();
                _udpClient.Send(message, message.Length);

                var state = new UdpState(_udpClient, _endPoint);
                _udpClient.BeginReceive(OnMessageReceieved, state);
                _loggedIn = true;
            }
        }

        private void OnPing(IAsyncResult ar)
        {
            _ping = true;
        }

        public bool Ping()
        {
            var sendPing = new AliServerDataPacket
            {
                Message = "Are you available?",
                Name = _udpHost,
                Command = Command.Ping
            };
            byte[] message = sendPing.ToByte();
            _udpClient.Send(message, message.Length);

            var state = new UdpState(_udpClient, _endPoint);
            _udpClient.BeginReceive(OnPing, state);

            if (_ping)
            {
                _ping = false;  // reset ping to false;
                return true;
            }
            Thread.Sleep(100);  // wait for the async to complete
            var ping = _ping;
            _ping = false;  // reset ping to false;
            return ping;
        }

        private void OnMessageReceieved(IAsyncResult ar)
        {
            if (!_loggedIn) return;

            UdpClient u = ((UdpState)(ar.AsyncState)).Udpclient;
            IPEndPoint e = ((UdpState)(ar.AsyncState)).Endpoint;

            byte[] message = u.EndReceive(ar, ref e);
            var receiveMsg = new AliServerDataPacket(message);

            OnPacketReceieved(receiveMsg);
            // setup a new listener and start the process anew
            var state = new UdpState(_udpClient, _endPoint);
            _udpClient.BeginReceive(OnMessageReceieved, state);
        }
    }
}
