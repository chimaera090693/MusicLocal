using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Web.Configuration;
using System.Web.Http;
using music.local.Filter;

namespace music.local.Controllers
{
    [CustomAuthFilter]
    public class StreammingController : ApiController
    {
        private static readonly string appPath = WebConfigurationManager.AppSettings["PhysicalPath"];
        public HttpResponseMessage Get(string p)
        {
            try
            {
                //không check login vì ảnh hưởng đến performance
                //if (!Common.CheckLogin()) return null;
                var response = Request.CreateResponse();
                response.Headers.TransferEncodingChunked = true;
                response.Headers.Add("Accept-Ranges", "bytes");
                response.Headers.Add("Keep-Alive", "timeout=10");

                //path.combine error??
                //var appPath = WebConfigurationManager.AppSettings["PhysicalPath"];
                var fileInfo = new FileInfo((appPath + p).Replace("\\", "/"));

                //check path
                if (string.IsNullOrEmpty(p))
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    return response;
                }
                if (!File.Exists(appPath + p))
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    return response;
                }
                var rangeHeader = Request.Headers.Range;
                var totalLength = fileInfo.Length;

                long start = 0, end = totalLength - 1;
                ContentRangeHeaderValue contentRange = null;
                //không có range hoặc là range gồm toàn bộ file
                if (rangeHeader == null || !rangeHeader.Ranges.Any())
                {
                    Common.WriteDebug("StreammingController, line 49 ", "Request video range is null or all file! \r\n" + p);
                    response.Headers.AcceptRanges.Add("bytes");
                    response.StatusCode = HttpStatusCode.OK;
                }
                else
                {
                    if (rangeHeader.Unit != "bytes" || rangeHeader.Ranges.Count > 1 ||
                    !MediaUtilities.CheckRangeItem(rangeHeader.Ranges.First(), totalLength, out start, out end))
                    {
                        Common.WriteDebug("StreammingController, line 58", "Request video range is invalid! \r\n" + p);
                        response.StatusCode = HttpStatusCode.RequestedRangeNotSatisfiable;
                        response.Content = new StreamContent(Stream.Null);
                        response.Content.Headers.ContentRange = new ContentRangeHeaderValue(totalLength);
                        response.Content.Headers.ContentType =
                            new MediaTypeHeaderValue(MediaUtilities.GetMimeType(fileInfo.Extension));
                        //trả về null stream
                        return response;
                    }
                    //range hợp lệ
                    Common.WriteDebug("StreammingController, line 68", "Request video range is valid, send content! \r\n" + p);
                    contentRange = new ContentRangeHeaderValue(start, end, totalLength);
                    
                }

                // status code là partial content.
                response.StatusCode = HttpStatusCode.PartialContent;
                response.Headers.TransferEncodingChunked = true;
                if (fileInfo.Extension.ToLower().Equals(".oga") 
                    || fileInfo.Extension.ToLower().Equals(".mp3") 
                    || fileInfo.Extension.ToLower().Equals(".flac"))
                {
                    response.Content = AudioContent(fileInfo, start, end);
                    response.Content.Headers.ContentLength = end - start + 1;
                    response.Headers.TransferEncodingChunked = null;
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue(MediaUtilities.GetMimeType(fileInfo.Extension));
                    
                }
                else
                {
                    var vid = new VideoStream(appPath + p, start, end);
                    response.Content = VideoContent(vid, fileInfo.Extension);
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("video/mp4");
                }
                if (contentRange != null)
                {
                    response.Content.Headers.ContentRange = contentRange;
                }
                //if (Request.Headers.UserAgent.ToString().Contains("Firefox"))  return response;
                response.Content.Headers.ContentLength = end - start + 1;
                return response;
            }
            catch (Exception ex)
            {
                Common.WriteLog(MethodBase.GetCurrentMethod().Name, ex + ex.StackTrace);
                return null;
            }
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

        public HttpContent VideoContent(VideoStream video, string extension)
        {
            return new PushStreamContent((Action<Stream, HttpContent, TransportContext>)video.WriteToStream,
                            MediaUtilities.GetMimeType(extension));
        }
    }
}
