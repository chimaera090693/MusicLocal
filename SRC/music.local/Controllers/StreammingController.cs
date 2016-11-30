using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;

namespace music.local.Controllers
{
    public class StreammingController : ApiController
    {
        public HttpResponseMessage Get(string filepath)
        {
            if (string.IsNullOrEmpty(filepath) || filepath.Split('&').Length < 2)
            {
                Common.WriteLog(MethodBase.GetCurrentMethod().Name, "Invalid Filepath!");
                return null;
            }
            var key = filepath.Split('&')[1];
            filepath = filepath.Split('&')[0];
            var response = Request.CreateResponse();
            //response.Headers.Connection.Add("keep-alive");
            //response.Headers.Connection.Add("close");
            response.Headers.TransferEncodingChunked = true;
            response.Headers.Add("Accept-Ranges", "bytes");
            response.Headers.Add("Keep-Alive", "timeout=10");

            //response.He
            var appPath = WebConfigurationManager.AppSettings["PhysicalPath"];
            var fileInfo = new FileInfo(appPath.Replace("\\", "/") + filepath);

            //check path
            if (string.IsNullOrEmpty(filepath))
            {
                response.StatusCode = HttpStatusCode.NotFound;
                return response;
            }
            if (!File.Exists(appPath + filepath))
            {
                response.StatusCode = HttpStatusCode.NotFound;
                return response;
            }
            var rangeHeader = Request.Headers.Range;

            var totalLength = fileInfo.Length;

            //không có range hoặc là range gồm toàn bộ file
            if (rangeHeader == null || !rangeHeader.Ranges.Any())
            {
                Common.WriteLog("MediaController, line 100", "Request video range is null or all file! \r\n" + filepath);

                response.Headers.AcceptRanges.Add("bytes");
                //response.StatusCode = HttpStatusCode.PartialContent;
                //edit
                response.StatusCode = HttpStatusCode.OK;

                //check video/audio
                if (fileInfo.Extension.Equals(".oga") || fileInfo.Extension.Equals(".mp3"))
                {
                    response.Content = AudioContent(fileInfo, 0, totalLength - 1);
                    response.Content.Headers.ContentLength = totalLength;
                    response.Headers.TransferEncodingChunked = null;
                }
                
                //check firefox -> không cho content length
                //if (!Request.Headers.UserAgent.ToString().Contains("Firefox"))
                response.Content.Headers.ContentLength = totalLength;
                response.Content.Headers.ContentRange = new ContentRangeHeaderValue(totalLength);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("video/mp4");
                return response;
            }

            long start, end;
            //có range, check range hợp lệ
            if (rangeHeader.Unit != "bytes" || rangeHeader.Ranges.Count > 1 ||
                !MediaUtilities.CheckRangeItem(rangeHeader.Ranges.First(), totalLength, out start, out end))
            {
                Common.WriteLog("MediaController, line 139", "Request video range is invalid! \r\n" + filepath);
                response.StatusCode = HttpStatusCode.RequestedRangeNotSatisfiable;
                response.Content = new StreamContent(Stream.Null);
                response.Content.Headers.ContentRange = new ContentRangeHeaderValue(totalLength);
                response.Content.Headers.ContentType =
                    new MediaTypeHeaderValue(MediaUtilities.GetMimeType(fileInfo.Extension));
                //trả về null stream
                return response;
            }
            //range hợp lệ
            Common.WriteLog("MediaController, line 149", "Request video range is valid, send content! \r\n" + filepath);
            var contentRange = new ContentRangeHeaderValue(start, end, totalLength);
            // status code là partial content.
            response.StatusCode = HttpStatusCode.PartialContent;
            //response.StatusCode = HttpStatusCode.OK;
            response.Headers.TransferEncodingChunked = true;

            if (fileInfo.Extension.Equals(".oga") || fileInfo.Extension.Equals(".mp3"))
            {
                response.Content = AudioContent(fileInfo, start, end);
                response.Content.Headers.ContentLength = end - start + 1;
                response.Headers.TransferEncodingChunked = null;
            }

            //check header, là firefox thì ko thêm tham số content length và range
            response.Content.Headers.ContentRange = contentRange;
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("video/mp4");
            //if (Request.Headers.UserAgent.ToString().Contains("Firefox"))  return response;
            response.Content.Headers.ContentLength = end - start + 1;
            return response;
        }

        public HttpContent AudioContent(FileInfo audio, long start, long end)
        {
            return new PushStreamContent((outputStream, httpContent, transpContext)
                =>
            {
                using (outputStream) // copy file stream vào outputstream theo range trong header
                using (Stream inputStream = audio.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
                    MediaUtilities.CreatePartialContent(inputStream, outputStream, start, end);
                //inputStream.CopyTo(outputStream, MediaUtilities.BufferSize);
            }, MediaUtilities.GetMimeType(audio.Extension));
        }
    }
}
