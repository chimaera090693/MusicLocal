using System.Reflection;
using log4net;

namespace music.local
{
    public class Common
    {
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
    }
}