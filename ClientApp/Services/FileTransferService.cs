using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace ClientApp.Services
{
    public class FileTransferResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class FileTransferService : IDisposable
    {
        private TcpClient _client;
        private NetworkStream _stream;
        private bool _isSending;
        
        private const int BUFFER_SIZE = 8192;
        
        public bool IsConnected => _client?.Connected ?? false;
        public bool IsSending => _isSending;
        
        public event Action<int, long, long> OnProgressChanged;
        public event Action<string> OnLogMessage;

        public bool Connect(string serverIP, int port)
        {
            try
            {
                _client = new TcpClient(serverIP, port);
                _stream = _client.GetStream();
                Log($"Đã kết nối đến {serverIP}:{port}");
                return true;
            }
            catch (Exception ex)
            {
                Log($"Lỗi kết nối: {ex.Message}");
                return false;
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
            Log("Đã ngắt kết nối");
        }

        /// <summary>
        /// CLO2.1: Sử dụng FileStream để đọc file và NetworkStream để gửi
        /// CLO2.2: Đọc và gửi theo buffer 8KB thay vì đọc toàn bộ file
        /// CLO3.1: Truyền file hoàn chỉnh, không bị lỗi
        /// CLO3.2: Gửi thông tin file (tên, kích thước) trước khi truyền dữ liệu
        /// CLO3.3: Hỗ trợ mọi định dạng file (binary mode)
        /// </summary>
        public FileTransferResult SendFile(string filePath)
        {
            if (!IsConnected)
            {
                return new FileTransferResult { Success = false, ErrorMessage = "Chưa kết nối đến server" };
            }

            _isSending = true;
            
            try
            {
                FileInfo fileInfo = new FileInfo(filePath);
                string fileName = fileInfo.Name;
                long fileSize = fileInfo.Length;

                Log($"Bắt đầu gửi file: {fileName}");

                // CLO3.2: Gửi metadata (tên file và kích thước) trước
                string metadata = $"FILE|{fileName}|{fileSize}";
                byte[] metadataBytes = Encoding.UTF8.GetBytes(metadata);
                _stream.Write(metadataBytes, 0, metadataBytes.Length);

                Log("Đã gửi thông tin file, chờ server xác nhận...");

                // Đọc phản hồi từ server
                byte[] responseBuffer = new byte[1024];
                int responseLength = _stream.Read(responseBuffer, 0, responseBuffer.Length);
                string response = Encoding.UTF8.GetString(responseBuffer, 0, responseLength).Trim();

                if (response != "READY")
                {
                    return new FileTransferResult { Success = false, ErrorMessage = $"Server từ chối: {response}" };
                }

                Log("Server sẵn sàng nhận file. Bắt đầu truyền...");

                // CLO2.1 & CLO2.2: Đọc file bằng FileStream và gửi theo buffer
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    byte[] buffer = new byte[BUFFER_SIZE];
                    int bytesRead;
                    long totalSent = 0;

                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        _stream.Write(buffer, 0, bytesRead);
                        totalSent += bytesRead;

                        int progress = (int)((totalSent * 100) / fileSize);
                        OnProgressChanged?.Invoke(progress, totalSent, fileSize);
                    }
                }

                // Đọc xác nhận hoàn thành từ server
                responseLength = _stream.Read(responseBuffer, 0, responseBuffer.Length);
                response = Encoding.UTF8.GetString(responseBuffer, 0, responseLength).Trim();

                if (response == "OK")
                {
                    Log($"Gửi file thành công! Tổng: {FormatFileSize(fileSize)}");
                    return new FileTransferResult { Success = true };
                }
                
                return new FileTransferResult { Success = false, ErrorMessage = $"Lỗi từ server: {response}" };
            }
            catch (Exception ex)
            {
                Log($"Lỗi: {ex.Message}");
                return new FileTransferResult { Success = false, ErrorMessage = ex.Message };
            }
            finally
            {
                _isSending = false;
            }
        }

        private void Log(string message)
        {
            OnLogMessage?.Invoke(message);
        }

        public static string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            double size = bytes;
            while (size >= 1024 && order < sizes.Length - 1)
            {
                order++;
                size /= 1024;
            }
            return $"{size:0.##} {sizes[order]}";
        }

        public void Dispose()
        {
            Disconnect();
        }
    }
}
