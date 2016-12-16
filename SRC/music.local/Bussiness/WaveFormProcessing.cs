using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Web.Configuration;
using System.Web.Mvc;
using NAudio.Wave;

namespace music.local.Bussiness
{
    public class WaveFormProcessing
    {
        public static float Zoommer = (float)1.2;
        public static ActionResult DemoDraw(string fn = "")
        {
            var physPath = WebConfigurationManager.AppSettings["PhysicalPath"];
            var mp3Path = physPath + (fn==""? @"\Music\Millenario - Elisa.mp3": fn);
            var imgPath = physPath + "\\_image\\" + Common.GetMd5Hash(mp3Path) + ".png";

            if (!Directory.Exists(physPath + "\\_image"))
            {
                Directory.CreateDirectory(physPath + "\\_image");
            }

            if (File.Exists(imgPath))
            {
                return new FilePathResult(imgPath, "image/png");
            }
            return WriteToFile(mp3Path);
        }

        private static FileContentResult WriteToFile(string strPath)
        {
            try
            {
                const int ConstMinVal = short.MinValue;
                const int Denominator = ushort.MaxValue ;

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
                    Pen pen1 = new Pen(Color.FromArgb(255, 110, 110, 110)) { Width = (float)0.1 };
                    Pen pen2 = new Pen(Color.FromArgb(255,150,150,150)) {Width = (float) 0.1};
                    Pen pen3 = new Pen(Color.FromArgb(255, 170, 170, 170)) { Width = (float)0.1 };

                    Stream wavestream = new Mp3FileReader(strPath, wf => new NAudio.FileFormats.Mp3.DmoMp3FrameDecompressor(wf));
                    var samplesPerPixel = (int)(wavestream.Length / bytesPerSample) / width;
                    wavestream.Position = 0;
                    byte[] waveData1 = new byte[samplesPerPixel * bytesPerSample];
                    //wavestream.Position =  (width * bytesPerSample * samplesPerPixel);

                    for (float x = 0; x < width-1; x++)
                    {
                        int low = 0;
                        int high = 0;
                        var bytesRead1 = wavestream.Read(waveData1, 0, samplesPerPixel * bytesPerSample);
                        if (bytesRead1 == 0)
                            break;
                        int totalLow = 0, countLow=0;
                        int totalHigh = 0, countHigh=0;
                        for (int n = 0; n <= bytesRead1-2; n +=2)
                        {
                            int sample = BitConverter.ToInt16(waveData1, n); 
                            if (sample > 0)
                            {
                                if (sample > high) { high = sample; }

                                totalHigh += sample;
                                countHigh++;
                            }
                            else
                            {
                                    if (sample < low) { low = sample; }
                                    totalLow += sample;
                                    countLow++;
                            }
                            
                        }
                        low =(low + totalLow/(countLow == 0 ? 1 : countLow))/2;
                        high =(high + totalHigh / (countHigh == 0 ? 1 : countHigh))/2;

                        float lowPercent = ((float)(low - ConstMinVal) / Denominator);
                        float highPercent = ((float)(high - ConstMinVal) / Denominator);
                        float lowValue = (height * lowPercent);
                        float highValue = (height * highPercent);
                        g.DrawLine(pen3, x, lowValue, x, highValue);
                        var dif = 0.2*(highValue - lowValue);
                        g.DrawLine(pen2, x, (int)(lowValue + dif), x, (int)(highValue - dif));
                        dif =dif+ dif/1.5;
                        g.DrawLine(pen1, x, (int)(lowValue + dif), x, (int)(highValue - dif));

                    }
                    wavestream.Close();
                    wavestream.Dispose();
                    pen1.Dispose();
                    pen2.Dispose();
                    pen3.Dispose();
                }
                FileContentResult image;
                using (var ms = new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Png);
                    image = new FileContentResult(ms.ToArray(), "image/png");
                }
                //var mp3FileInfor = Path.GetFileName(strPath);
                var physPath = WebConfigurationManager.AppSettings["PhysicalPath"];
                string hash = Common.GetMd5Hash(strPath);

                if (!string.IsNullOrEmpty(hash) && !Common.IsTesting())
                {
                   bmp.Save(physPath + "\\_image\\" + hash + ".png", ImageFormat.Png);
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

        public static float Zoom(float pecent, bool isNegative = false)
        {
            var rtn = !isNegative ? Zoommer * pecent : pecent / Zoommer;
            return rtn;
        }
        
        #endregion
    }
}