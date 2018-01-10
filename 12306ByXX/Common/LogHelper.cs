using System;

namespace _12306ByXX.Common
{
    public static class LogHelper
    {
        private static readonly log4net.ILog Loginfo = log4net.LogManager.GetLogger("Info");
        private static readonly log4net.ILog Logerror = log4net.LogManager.GetLogger("Error");
        private static readonly log4net.ILog Logedebug = log4net.LogManager.GetLogger("Debug");
        public static void Info(string info)
        {
            if (Loginfo.IsInfoEnabled)
            {
                Loginfo.Info(info);
            }
        }

        public static void Error(string info, Exception se)
        {
            if (Logerror.IsErrorEnabled)
            {
                Logerror.Error(info, se);
            }
        }

        public static void Debug(string info)
        {
            if (Logedebug.IsDebugEnabled)
            {
                Logedebug.Debug(info);
            }
        }
    }
}
