using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace ServerApp.Services
{
    public class FileReceiveResult
    {
        public bool Success { get; set; }
        public string SavedFilePath { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class FileService
    {
        private const int BUFFER_SIZE = 8192; // 8KB buffer - CLO2.2
        
        public event Action<string> OnLogMessage;

        public FileReceiveResult ReceiveFile(NetworkStream stream, string fileName, long fileSize, string clientIP)
        {
            try
            {
                Log($"[{clientIP}] Receiving: {fileName} ({FormatFileSize(fileSize)})");

                // Tạo thư mục lưu file nếu chưa có
                string saveFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ReceivedFiles");
                if (!Directory.Exists(saveFolder))
                {
                    Directory.CreateDirectory(saveFolder);
                }

                // Tạo tên file unique nếu đã tồn tại
                string savePath = Path.Combine(saveFolder, fileName);
                int counter = 1;
                while (File.Exists(savePath))
                {
                    string nameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
                    string ext = Path.GetExtension(fileName);
                    savePath = Path.Combine(saveFolder, $"{nameWithoutExt}_{counter}{ext}");
                    counter++;
                }

                // Gửi xác nhận sẵn sàng nhận file
                byte[] readyResponse = Encoding.UTF8.GetBytes("READY\n");
                stream.Write(readyResponse, 0, readyResponse.Length);

                Log($"[{clientIP}] Starting file transfer...");

                // Nhận file theo buffer và ghi bằng FileStream
                byte[] buffer = new byte[BUFFER_SIZE];
                using (FileStream fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write))
                {
                    long totalReceived = 0;
                    int bytesRead;

                    while (totalReceived < fileSize)
                    {
                        int toRead = (int)Math.Min(buffer.Length, fileSize - totalReceived);
                        bytesRead = stream.Read(buffer, 0, toRead);

                        if (bytesRead == 0)
                        {
                            throw new Exception("Connection closed unexpectedly");
                        }

                        fileStream.Write(buffer, 0, bytesRead);
                        totalReceived += bytesRead;
                    }
                }

                // Gửi xác nhận hoàn thành
                byte[] okResponse = Encoding.UTF8.GetBytes("OK\n");
                stream.Write(okResponse, 0, okResponse.Length);

                Log($"[{clientIP}] File saved: {Path.GetFileName(savePath)}");
                Log($"[{clientIP}] Transfer completed successfully!");

                return new FileReceiveResult
                {
                    Success = true,
                    SavedFilePath = savePath
                };
            }
            catch (Exception ex)
            {
                Log($"[{clientIP}] Error: {ex.Message}");
                return new FileReceiveResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
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
    }
}
