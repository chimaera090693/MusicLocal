using System;
using System.IO;
using System.Net.Http.Headers;
using System.Web;

namespace music.local
{
    public class MediaUtilities
    {
        public const int BufferSize = 1024 * 1024 *1024;

        /// <summary>
        /// trả về mime type từ extension của media file
        /// </summary>
        /// <param name="extension">extension của file</param>
        /// <returns>mime type</returns>
        public static string GetMimeType(string extension)
        {
            return MimeMapping.GetMimeMapping(extension);
            //var ext = "";
            //switch (extension.ToLower())
            //{
            //    case ".mp3":
            //        ext = "audio/mpeg";
            //        return ext;
            //    case ".mp4":
            //        ext = "video/mp4";
            //        return ext;
            //    case ".ogg":
            //        ext = "application/ogg";
            //        return ext;
            //    case ".ogv":
            //        ext = "video/ogg";
            //        return ext;
            //    case ".oga":
            //        ext = "audio/ogg";
            //        return ext;
            //    case ".wav":
            //        ext = "audio/x-wav";
            //        return ext;
            //    case ".webm":
            //        ext = "video/webm";
            //        return ext;
            //}
            //return ext;
        }

        /// <summary>
        /// kiểm tra range của request
        /// </summary>
        /// <param name="range"></param>
        /// <param name="contentLength"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static bool CheckRangeItem(RangeItemHeaderValue range, long contentLength,
            out long start, out long end)
        {
            if (range.From != null)
            {
                start = range.From.Value;
                if (range.To != null)
                    end = range.To.Value;
                else
                    end = contentLength - 1;
            }
            else
            {
                end = contentLength - 1;
                if (range.To != null)
                    start = contentLength - range.To.Value;
                else
                    start = 0;
            }
            return (start < contentLength && end < contentLength);
        }

        /// <summary>
        /// tạo partial content cho response
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="outputStream"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public static void CreatePartialContent(Stream inputStream, Stream outputStream,
            long start, long end)
        {
            long remainingBytes = end - start + 1;
            long position;
            byte[] buffer = new byte[BufferSize];
            //byte[] buffer = new byte[65536];

            inputStream.Position = start;
            do
            {
                try
                {
                    var count = remainingBytes > BufferSize ? inputStream.Read(buffer, 0, BufferSize) : inputStream.Read(buffer, 0, (int)remainingBytes);
                    outputStream.Write(buffer, 0, count);
                }
                catch (HttpException error)
                {
                    Common.WriteLog("Error", error + error.StackTrace);
                    Console.WriteLine(error);
                    break;
                }
                catch (Exception ex)
                {
                    Common.WriteLog("Debug", ex + ex.StackTrace);
                    Console.WriteLine(ex);
                    break;
                }
                position = inputStream.Position;
                remainingBytes = end - position + 1;
            } while (position <= end);
            outputStream.Close();
        }
    }
}