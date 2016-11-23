using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using NAudio.Wave;

namespace music.local.Bussiness
{
    public class WaveFormProcessing
    {
        public static FileContentResult DemoDraw()
        {
            var physPath = WebConfigurationManager.AppSettings["PhysicalPath"];
            physPath = physPath + @"\Music\Millenario - Elisa.mp3";
            //var n = new NAudio.Wave.AudioFileReader(physPath);
            //using (AudioFileReader aFileReader = new AudioFileReader(physPath))
            //{
            //     //aFileReader.re
            //}
            return WriteToFile("", physPath, new byte[500]);
        }

        private static FileContentResult WriteToFile(string SongID, string strPath, byte[] Buffer)
        {
            try
            {
                int samplesPerPixel = 128;
                long startPosition = 0;
                //FileStream newFile = new FileStream(GeneralUtils.Get_SongFilePath() + "/" + strPath, FileMode.Create);
                float[] data = FloatArrayFromByteArray(Buffer);

                Bitmap bmp = new Bitmap(1170, 200);

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
                    Pen pen1 = new Pen(Color.Gray);
                    int size = data.Length;

                    string hexValue1 = "#009adf";
                    Color colour1 =ColorTranslator.FromHtml(hexValue1);
                    pen1.Color = colour1;

                    Stream wavestream = new Mp3FileReader(strPath, wf => new NAudio.FileFormats.Mp3.DmoMp3FrameDecompressor(wf));

                    wavestream.Position = 0;
                    int bytesRead1;
                    byte[] waveData1 = new byte[samplesPerPixel * bytesPerSample];
                    wavestream.Position = startPosition + (width * bytesPerSample * samplesPerPixel);

                    for (float x = 0; x < width; x++)
                    {
                        short low = 0;
                        short high = 0;
                        bytesRead1 = wavestream.Read(waveData1, 0, samplesPerPixel * bytesPerSample);
                        if (bytesRead1 == 0)
                            break;
                        for (int n = 0; n < bytesRead1; n += 2)
                        {
                            short sample = BitConverter.ToInt16(waveData1, n);
                            if (sample < low) low = sample;
                            if (sample > high) high = sample;
                        }
                        float lowPercent = ((((float)low) - short.MinValue) / ushort.MaxValue);
                        float highPercent = ((((float)high) - short.MinValue) / ushort.MaxValue);
                        float lowValue = height * lowPercent;
                        float highValue = height * highPercent;
                        g.DrawLine(pen1, x, lowValue, x, highValue);

                    }
                    wavestream.Close();
                    wavestream.Dispose();
                }
                FileContentResult image = null;
                using (var ms = new MemoryStream())
                {
                     bmp.Save(ms,ImageFormat.Png);
                     image = new FileContentResult(ms.ToArray(), "image/png");
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
    
    }
}