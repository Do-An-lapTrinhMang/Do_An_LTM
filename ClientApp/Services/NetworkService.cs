using System;
using System.Net.Sockets;
using System.Text;

namespace ClientApp.Services
{
    public class NetworkService : IDisposable
    {
        private TcpClient _client;
        private NetworkStream _stream;
        
        public string ServerIP { get; set; } = "127.0.0.1";
        public int ServerPort { get; set; } = 12345;
        public bool IsConnected => _client?.Connected ?? false;
        
        public NetworkStream Stream => _stream;

        public bool Connect()
        {
            try
            {
                _client = new TcpClient(ServerIP, ServerPort);
                _stream = _client.GetStream();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Connect(string ip, int port)
        {
            ServerIP = ip;
            ServerPort = port;
            return Connect();
        }

        public string SendRequest(string request)
        {
            try
            {
                using (var client = new TcpClient(ServerIP, ServerPort))
                {
                    var stream = client.GetStream();
                    
                    byte[] requestBytes = Encoding.UTF8.GetBytes(request);
                    stream.Write(requestBytes, 0, requestBytes.Length);
                    
                    byte[] buffer = new byte[1024];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    return Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                }
            }
            catch (Exception ex)
            {
                return $"ERROR|{ex.Message}";
            }
        }

        public void Disconnect()
        {
            try
            {
                _stream?.Close();
                _client?.Close();
            }
            catch { }
            finally
            {
                _stream = null;
                _client = null;
            }
        }

        public void Dispose()
        {
            Disconnect();
        }
    }
}
