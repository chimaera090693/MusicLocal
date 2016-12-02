using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;

namespace music.local
{
    public class VideoStream
    {
        private  string _filename;
        private  long _start;
        private  long _end;
        /// <summary>
        /// khởi tạo với filepath và start và end của request
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public VideoStream(string filePath, long start, long end)
        {
            _filename = filePath;
            _start = start;
            _end = end;
        }

        /// <summary>
        /// Action để ghi vào response stream
        /// </summary>
        /// <param name="outputStream"></param>
        /// <param name="content"></param>
        /// <param name="context"></param>
        public async void WriteToStream(Stream outputStream, HttpContent content, TransportContext context)
        {
            try
            {
                const int bufferSize = 1024*1024;
                var buffer = new byte[bufferSize];

                using (var video = File.Open(_filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    //bỏ ép kiểu video.length thành int để fix lỗi video size> 2gb.
                    //var length = (int)video.Length;
                    var length = video.Length;
                    var bytesRead = 1;
                    video.Position = _start;
                    while (length > 0 && bytesRead > 0 && video.Position<=_end)
                    {
                        try
                        {
                            var countReadByte = 0;
                            countReadByte = (length > bufferSize) ? bufferSize : (int)length;
                            bytesRead = video.Read(buffer, 0, countReadByte);
                            await outputStream.WriteAsync(buffer, 0, bytesRead);
                            length -= bytesRead;
                        }
                        catch
                        {
                            return;
                        }
                    }
                }
            }
            catch (HttpException ex)//!important
            {
                //khi bắt exception, thoát khỏi hàm WriteToStream để giải phóng bộ nhớ và tài nguyên.
                //Common.Writelog("Error", ex + ex.StackTrace);
                // ReSharper disable once RedundantJumpStatement
                return;
            }
            finally
            {
                outputStream.Close();
            }
        }
    }
}