using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Web.Configuration;
using System.Web.Mvc;
using music.local.Bussiness;
using music.local.Filter;
using WebGrease.Css;
using WebGrease.Css.Extensions;
using System.Collections.Generic;

namespace music.local.Controllers
{
    public class HomeController : Controller
    {
        [CustomAuthFilter]
        public ActionResult Index()
        {
            var listAlbum = TrackProcessing.GetTree();
            ViewBag.Data = listAlbum;
            return View("/Views/Home.cshtml");
        }

        [CustomAuthFilter]
        public ActionResult Demo(string p = "")
        {
            //if (!Common.CheckLogin()) return null;
            return WaveFormProcessing.DemoDraw(p);
        }

        [CustomAuthFilter]
        public ActionResult File(string p)
        {
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

        [CustomAuthFilter]
        public ActionResult Cover(string p)
        {
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

        [CustomAuthFilter]
        public ActionResult Text()
        {

            return View("/Views/Text.cshtml");
        }

        [CustomAuthFilter]
        public ActionResult Test()
        {

            return View("/Views/Test.cshtml");
        }
        [CustomAuthFilter]
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
        [CustomAuthFilter]
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

        #region Image Slider
         public static List<string> ListFileImage = new List<string>();
         [CustomAuthFilter]
         public static string ImageInfor()
         {
             var str =@"<input id='countImage' type='hidden' value='0' />";;

             try
             {
                 var physPath = WebConfigurationManager.AppSettings["ImagePath"].ToLower();
                 var txtFile = physPath + "\\listfile.txt ";
                 if (System.IO.File.Exists(txtFile))
                 {
                     var lines = System.IO.File.ReadAllLines(txtFile, System.Text.UTF8Encoding.UTF8);
                     if (lines == null || lines.Length == 0) goto NotFoundTXT;
                     str = @"<input id='countImage' type='hidden' value='" + lines.Length + "' />";
                     return str;
                 }
             NotFoundTXT:
                 str = @"<input id='countImage' type='hidden' value='" + GenerateListImage() + "' />";
             }
             catch (Exception ex)
             {
                 Common.WriteLog("ImageInfor", ex+ex.StackTrace);
             }
             EndAction:
             return str;
         }

         public ActionResult ImageCover(int id)
         {
             //check static list
             var physPath = WebConfigurationManager.AppSettings["ImagePath"].ToLower();
             if (ListFileImage == null || ListFileImage.Count == 0)
             {
                
                 var txtFile = physPath + "\\listfile.txt ";
                 if (!System.IO.File.Exists(txtFile)) GenerateListImage();
                 var lines = System.IO.File.ReadAllLines(txtFile, System.Text.UTF8Encoding.UTF8);
                 if (lines != null && lines.Length > 0)
                 {
                     ListFileImage = new List<string>();
                     ListFileImage.AddRange(lines);
                 }
             }
             if (id >= ListFileImage.Count) return null;
             var path = ListFileImage[id];
             var mime =path.Split('.').Last();
             return new FilePathResult(physPath+path, MediaUtilities.GetMimeType("." + mime));
         }

         private static int GenerateListImage()
         {
             var physPath = WebConfigurationManager.AppSettings["ImagePath"].ToLower();
             var txtFile = physPath + "\\listfile.txt ";
             var listFile = Directory.EnumerateFiles(physPath, "*", SearchOption.TopDirectoryOnly).ToList();
             if (listFile == null || listFile.Count == 0) return 0; ;
             StreamWriter filetxt = new StreamWriter(txtFile, false);
             var count = 0;
             foreach (var file in listFile)
             {
                 var fileinf = file.ToLower();
                 if (fileinf.EndsWith(".jpeg") || fileinf.EndsWith(".png") || fileinf.EndsWith(".bmp") || fileinf.EndsWith(".jpg"))
                 {
                     filetxt.WriteLine(fileinf.ToLower().Replace(physPath, ""));
                     count++;
                 }
             }
             filetxt.Flush();
             filetxt.Close();
             return count;
         }

        #endregion

    }
}