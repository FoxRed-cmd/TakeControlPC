using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TakeControlPC
{
    internal class CommandServer
    {
        private string _ip;
        private string _port;
        private Socket _listener;
        private Socket _client;
        private IPEndPoint _ipPoint;
        private StringBuilder _builder = new StringBuilder();
        private byte[] _data;
        public string Answer { get; set; } = string.Empty;
        public bool IsListening { get; private set; }
        public CommandServer(string ip, string port)
        {
            _ip = ip;
            _port = port;
        }

        public void StartListening()
        {
            if (_ip == null || _port == null)
                return;
            IsListening = true;
            if (IPAddress.TryParse(_ip, out IPAddress address) && int.TryParse(_port, out int port))
            {
                using (_listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    if (_ipPoint == null)
                        _ipPoint = new IPEndPoint(address, port);

                    _listener.Bind(_ipPoint);
                    _listener.Listen(10);
                    try
                    {
                        _client = _listener.Accept();
                    }
                    catch (Exception)
                    {
                        return;
                    }
                    _data = new byte[64];
                    _builder.Clear();
                    int bytes = 0;
                    do
                    {
                        bytes = _client.Receive(_data);
                        _builder.Append(Encoding.UTF8.GetString(_data, 0, bytes));
                    } while (_client.Available > 0);
                    _client.Shutdown(SocketShutdown.Both);
                    _client.Close();
                    Answer = _builder.ToString();
                }
            }
        }

        public async Task StartListeningAsync()
        {
           await Task.Run(() => StartListening());
        }

        public void StopListening()
        {
            if (IsListening == true)
            {
                try
                {
                    IsListening = false;
                    _listener.Close();
                    _listener.Dispose();
                }
                catch (Exception)
                {

                }
            }
        }
    }
}
