using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Web.Configuration;
using System.Web.Mvc;
using music.local.Bussiness;
using WebGrease.Css;
using WebGrease.Css.Extensions;

namespace music.local.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (!LoginsProcessing.CheckLogin(true)) return null;
            var listAlbum = TrackProcessing.GetTree();
            ViewBag.Data = listAlbum;
            return View("/Views/Home.cshtml");
        }
        public ActionResult Demo(string p = "")
        {
            //if (!Common.CheckLogin()) return null;
            return WaveFormProcessing.DemoDraw(p);
        }

        public ActionResult File(string p)
        {
            if (!LoginsProcessing.CheckLogin()) return null;
            if (string.IsNullOrEmpty(p))
                return null;
            var physPath = WebConfigurationManager.AppSettings["PhysicalPath"];
            var filePath = physPath + p;
            if (System.IO.File.Exists(filePath))
            {
                FileInfo f = new FileInfo(filePath);
                var fileMine = "audio/mpeg3";
                if (!f.Extension.ToLower().Replace(".", "").Equals("mp3"))
                {
                    fileMine = "image/jpeg";
                    if (f.Extension.ToLower().Equals(".pdf"))
                    {
                        fileMine = "application/pdf";
                    }
                }
                using (var str = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    byte[] data = new byte[str.Length];
                    int br = str.Read(data, 0, data.Length);
                    if (br != str.Length)
                        throw new IOException(filePath);
                    return new FileContentResult(data, fileMine);
                }
            }
            return null;
        }


        public ActionResult Cover(string p)
        {
            if (!LoginsProcessing.CheckLogin()) return null;
            if (string.IsNullOrEmpty(p))
                return null;
            var physPath = WebConfigurationManager.AppSettings["PhysicalPath"];
            var filePath = physPath + p;
            if (System.IO.File.Exists(filePath))
            {
                return Mp3TagReader.GetPicture(filePath);
            }

            var str = "ahihi";
            var bytes = System.Text.UnicodeEncoding.Unicode.GetBytes(str);
            var bite =BitConverter.GetBytes(1);

            return null;
        }


         /// <summary>
         /// renew session
         /// </summary>
         /// <returns></returns>
        public ActionResult CheckSession()
        {
            if (LoginsProcessing.CheckLogin())
            {
                return Content("1");
            }
            return Content("0");
        }


        public ActionResult Text()
        {

            return View("/Views/Text.cshtml");
        }

        public ActionResult Encode(string str)
        {
            if (string.IsNullOrEmpty(str)) return Content("Fail!");
            var bytes = System.Text.Encoding.Unicode.GetBytes(str);
            var txt = "";
            for (int index = 0; index < bytes.Length; index++)
            {
                var VARIABLE = bytes[index];
                txt += Convert.ToString(VARIABLE, 2).PadLeft(8, '0')+" ";
                //txt += index + ": " + VARIABLE+"\r\n";
            }
            txt = txt.Trim() + "\r\n";
            txt += string.Join(" ", bytes.Select(byt => Convert.ToString(byt, 2).PadLeft(8, '0')));
            return Content(txt);
        }
        public ActionResult Decode(string str)
        {
            try
            {
                var text = "";
                 string[] separators = {" "};
                if (string.IsNullOrEmpty(str)) return Content("Fail!");
                var arr = str.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                var bytes = new byte[arr.Length];
                for (int index = 0; index < arr.Length; index++)
                {
                    var byt = arr[index];
                    if (string.IsNullOrEmpty(byt)) continue;
                    if (byt.Length != 8) throw new Exception("Chuỗi nhập vào không đúng chuẩn!");
                    int newBy=0;
                    for (int i = 0; i < 8; i++)
                    {
                        var chr = byt[i];
                        int s = int.Parse(chr+"");
                        newBy = newBy << 1;
                        newBy =  newBy ^ s;
                    }
                    //text +=+index+": "+newBy+ "\r\n";
                    bytes[index] = Convert.ToByte(newBy);
                }
                text += System.Text.Encoding.Unicode.GetString(bytes);
                return Content(text);
            }
            catch (Exception ex)
            {
                  return Content("Fail! \r\n"+ ex.Message+"\r\n"+ex.StackTrace);
            }
        }
    }
}