using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web.Configuration;
using System.Web.Mvc;
using NAudio.Wave;

namespace music.local.Bussiness
{
    public class WaveFormProcessing
    {
        public static float zoomper = (float)1.0;
        public static ActionResult DemoDraw(string fn = "")
        {
            var physPath = WebConfigurationManager.AppSettings["PhysicalPath"];
            var mp3Path = physPath + (fn==""? @"\Music\Millenario - Elisa.mp3": fn);
            var imgPath = physPath + "\\image\\" + GetMd5Hash(mp3Path) + ".png";

            if (!Directory.Exists(physPath + "\\image"))
            {
                Directory.CreateDirectory(physPath + "\\image");
            }

            if (File.Exists(imgPath))
            {
                //using(var str  = new FileStream(imgPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                //{
                //    byte[] data = new byte[str.Length];
                //    int br = str.Read(data, 0, data.Length);
                //    if (br != str.Length)
                //        throw new System.IO.IOException(imgPath);
                //    //var ms = str.
                //    //return new FileContentResult(str, "image/png");
                //    return new FileContentResult(data, "image/png");
                //}
                return new FilePathResult(imgPath, "image/png");
            }
            return WriteToFile(mp3Path);
        }

        private static FileContentResult WriteToFile(string strPath)
        {
            try
            {
                long startPosition = 0;
                //FileStream newFile = new FileStream(GeneralUtils.Get_SongFilePath() + "/" + strPath, FileMode.Create);
                //float[] data = FloatArrayFromByteArray(Buffer);

                Bitmap bmp = new Bitmap(3256, 400);

                int BORDER_WIDTH = 1;
                int width = bmp.Width - (2 * BORDER_WIDTH);
                int height = bmp.Height - (2 * BORDER_WIDTH);

                Mp3FileReader reader = new Mp3FileReader(strPath, wf => new NAudio.FileFormats.Mp3.DmoMp3FrameDecompressor(wf));
                WaveChannel32 channelStream = new WaveChannel32(reader);
                int bytesPerSample = (reader.WaveFormat.BitsPerSample / 8) * channelStream.WaveFormat.Channels;
                channelStream.Dispose();
                reader.Dispose();
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.Transparent);
                    Pen pen1 = new Pen(Color.Gray) {Width = (float) 0.1};

                    Stream wavestream = new Mp3FileReader(strPath, wf => new NAudio.FileFormats.Mp3.DmoMp3FrameDecompressor(wf));
                    var samplesPerPixel = (int)(wavestream.Length / bytesPerSample) / width;
                    wavestream.Position = 0;
                    byte[] waveData1 = new byte[samplesPerPixel * bytesPerSample];
                    wavestream.Position = startPosition;//+ (width * bytesPerSample * samplesPerPixel);

                    for (float x = 0; x < width; x++)
                    {
                        short low = 0;
                        short high = 0;
                        var bytesRead1 = wavestream.Read(waveData1, 0, samplesPerPixel * bytesPerSample);
                        if (bytesRead1 == 0)
                            break;
                        for (int n = 0; n < bytesRead1; n += 2)
                        {
                            short sample = BitConverter.ToInt16(waveData1, n);
                            if (sample < low) { low = sample; }
                            if (sample > high) { high = sample; }
                        }
                        float lowPercent = ((Zoom(low) - short.MinValue) / ushort.MaxValue);
                        float highPercent = ((Zoom(high) - short.MinValue) / ushort.MaxValue);
                        float lowValue = (height * lowPercent);
                        float highValue = (height * highPercent);
                        g.DrawLine(pen1, x, lowValue, x, highValue);

                    }
                    wavestream.Close();
                    wavestream.Dispose();
                }
                FileContentResult image;
                using (var ms = new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Png);
                    image = new FileContentResult(ms.ToArray(), "image/png");
                }
                //var mp3FileInfor = Path.GetFileName(strPath);
                var physPath = WebConfigurationManager.AppSettings["PhysicalPath"];
                string hash = GetMd5Hash(strPath);

                if (!string.IsNullOrEmpty(hash))
                {
                    bmp.Save(physPath + "\\image\\" + hash + ".png", ImageFormat.Png);
                }
                bmp.Dispose();
                return image;
            }
            catch (Exception e)
            {
                Common.WriteLog(MethodBase.GetCurrentMethod().Name, e.Message + e.StackTrace);
            }
            return null;
        }
        #region common
        public static float[] FloatArrayFromStream(MemoryStream stream)
        {
            return FloatArrayFromByteArray(stream.GetBuffer());
        }

        public static float[] FloatArrayFromByteArray(byte[] input)
        {
            float[] output = new float[input.Length / 4];
            for (int i = 0; i < output.Length; i++)
            {
                output[i] = BitConverter.ToSingle(input, i * 4);
            }
            return output;
        }

        public static float Zoom(float pecent)
        {
            var rtn = zoomper * pecent;
            return rtn;
        }


        public static string GetMd5Hash(string input)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder sBuilder = new StringBuilder();
                foreach (byte t in data)
                {
                    sBuilder.Append(t.ToString("x2"));
                }
                return sBuilder.ToString();
            }
        }
        #endregion


    }
}