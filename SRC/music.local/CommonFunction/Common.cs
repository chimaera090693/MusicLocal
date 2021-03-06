﻿using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;
using log4net;

namespace music.local
{
    public class Common
    {
        public static string txtLogAccess = HostingEnvironment.ApplicationPhysicalPath + "\\Logs\\AccessLog.txt";

        private static readonly ILog Logger =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static void WriteLog(string title, string exception)
        {
            {
                Logger.Error("");
                Logger.Error("--------------------------------");
                Logger.Error(title + ": " + exception);
                Logger.Error("--------------------------------");
                Logger.Error("");
            }
        }

        public static void WriteDebug(string title, string exception)
        {
            {
                Logger.Debug("");
                Logger.Debug("--------------------------------");
                Logger.Debug(title + ": " + exception);
                Logger.Debug("--------------------------------");
                Logger.Debug("");
            }
        }
        

        /// <summary>
        /// ghi log access ra file text
        /// </summary>
        public static void WriteLogAccess()
        {
            var ipAddress = HttpContext.Current.Request.UserHostAddress;
            if (!("127.0.0.1").Equals(ipAddress))
            {
                var tr = HttpContext.Current.Request.Headers["User-Agent"];
                var msg = (DateTime.Now.ToString("s") + ": " + ipAddress + "    ||  User-Agent:" + tr);
                using (
                    StreamWriter file =
                        new StreamWriter(txtLogAccess, true))
                {
                    try
                    {
                        file.WriteLine(msg);
                        file.WriteLine("=================================================================");
                        file.Flush();
                        file.Close();
                    }
                    catch (Exception ex)
                    {
                        WriteLog(MethodBase.GetCurrentMethod().Name, ex + ex.StackTrace);
                    }
                }
            }
        }

        public static string GetFileName(string oldValue)
        {
            if (string.IsNullOrEmpty(oldValue)) return "";
            return oldValue.Replace(" ", "").Replace("(", "").Replace(")", "");
        }

        public static string GetMd5Hash(string input)
        {
            using (var md5Hash = MD5.Create())
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

        public static bool IsTesting()
        {
            var chk = WebConfigurationManager.AppSettings["DeployType"];
            return "Testing".Equals(chk);
        }
    }
}