using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace UBA.Redis.SaveFileTaskV1
{
    public static class LogHelperNet
    {
        private static ILog _log;
        private static void InitializeLog()
        {
            if (_log == null)
            {
                _log = LogManager.GetLogger("DataFile");

            }
        }
        public static void Error(object message, Exception exception)
        {
            InitializeLog();
            if (_log.IsErrorEnabled)
                _log.Error(message, exception);
        }
        public static void Fatal(object message, Exception exception)
        {
            InitializeLog();
            if (_log.IsFatalEnabled)
                _log.Fatal(message, exception);
        }
        public static void Info(object message, Exception exception)
        {
            InitializeLog();
            if (_log.IsInfoEnabled)
                _log.Info(message, exception);
        }
        public static void Warn(object message, Exception exception)
        {
            InitializeLog();
            if (_log.IsWarnEnabled)
                _log.Warn(message, exception);
        }
        public static void Debug(object message, Exception exception)
        {
            InitializeLog();
            if (_log.IsDebugEnabled)
                _log.Debug(message, exception);
        }
    }
}
