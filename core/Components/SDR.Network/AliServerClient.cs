using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SDR.Network
{
    public class AliServerClient
    {
        private readonly UdpClient _udpClient;
        private readonly string _udpHost;
        private bool _ping = false;

        public AliServerClient(string udpHost, int udpPort)
        {
            // set up a remote udp host to communicate with
            _udpHost = udpHost;
            var ip = ConvertToIpAddress(udpHost);
            _udpClient = new UdpClient();
            if (ip == null) // in this case we were given a host name
            {
                _udpClient.Connect(udpHost, udpPort);
            }
            else // and in this case we have been given an ip address
            {
                _udpClient.Connect(ip, udpPort);
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

        private class UdpState
        {
            public UdpClient Udpclient;
            public IPEndPoint Endpoint;

            public UdpState(UdpClient udpclient, IPEndPoint endpoint)
            {
                Udpclient = udpclient;
                Endpoint = endpoint;
            }
        }

        public void Close()
        {
            // handle login and logout here as well
            _udpClient.Close();
        }

        private void PingBack(IAsyncResult ar)
        {
            _ping = true;
        }

        public bool Ping()
        {
            var msgToSend = new AliServerDataPacket { Message = "Are you available?", Name = _udpHost, Command = Command.Ping };
            byte[] message = msgToSend.ToByte();

            _udpClient.Send(message, message.Length);
            
            IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);
            UdpState state = new UdpState(_udpClient, remote);

            _udpClient.BeginReceive(new AsyncCallback(PingBack), state);

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
    }
}
