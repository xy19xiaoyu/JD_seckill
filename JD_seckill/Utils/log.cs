using System;

namespace JD_seckill
{
    public static class log
    {
        #region 基本日志
        public static log4net.ILog baselog = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        /// <summary>
        /// 记录调试信息
        /// </summary>
        /// <param name="message"></param>
        public static void Debug(object message)
        {
            baselog.Debug(message + Environment.NewLine + "软件版本:" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
        }
        /// <summary>
        /// 记录错误信息
        /// </summary>
        /// <param name="message"></param>
        public static void Error(object message)
        {
            baselog.Error(message + Environment.NewLine + "软件版本:" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
        }
        /// <summary>
        /// 记录失败信息 
        /// </summary>
        /// <param name="message"></param>
        public static void Fatal(object message)
        {
            baselog.Fatal(message + Environment.NewLine + "软件版本:" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
        }
        /// <summary>
        /// 记录信息 如：用户操作信息
        /// </summary>
        /// <param name="message"></param>
        public static void Info(object message)
        {
            baselog.Info(message + Environment.NewLine + "软件版本:" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
        }
        /// <summary>
        /// 记录警告信息
        /// </summary>
        /// <param name="message"></param>
        public static void Warn(object message)
        {
            baselog.Warn(message + Environment.NewLine + "软件版本:" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
        }
        #endregion

        #region 扩展日志 支持日志文件名
        public static log4net.ILog ext_log = log4net.LogManager.GetLogger("ext");

        private static void ExtLog(string message, string filename)
        {
            string fileName = AppDomain.CurrentDomain.BaseDirectory + "\\log\\" + DateTime.Now.ToString("yyyyMM\\\\dd") + "\\" + filename + ".log";

            System.IO.File.AppendAllText(fileName, message + Environment.NewLine + "软件版本:" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + Environment.NewLine);

        }

        /// <summary>
        /// 记录调试信息
        /// </summary>
        /// <param name="message"></param>
        public static void Debug(object message, string filename)
        {
            ExtLog(message.ToString(), filename);
        }
        /// <summary>
        /// 记录错误信息
        /// </summary>
        /// <param name="message"></param>
        public static void Error(object message, string filename)
        {
            ExtLog(message.ToString(), filename);
        }
        /// <summary>
        /// 记录失败信息 
        /// </summary>
        /// <param name="message"></param>
        public static void Fatal(object message, string filename)
        {
            ExtLog(message.ToString(), filename);
        }
        /// <summary>
        /// 记录信息 如：用户操作信息
        /// </summary>
        /// <param name="message"></param>
        public static void Info(object message, string filename)
        {
            ExtLog(message.ToString(), filename);
        }
        /// <summary>
        /// 记录警告信息
        /// </summary>
        /// <param name="message"></param>
        public static void Warn(object message, string filename)
        {
            ExtLog(message.ToString(), filename);
        }
        #endregion


        private static void ChangeLog4netLogFileName(string fileName)
        {
            try
            {
                log4net.Core.LogImpl logImpl = ext_log as log4net.Core.LogImpl;
                if (logImpl != null)
                {
                    log4net.Appender.AppenderCollection ac = ((log4net.Repository.Hierarchy.Logger)logImpl.Logger).Appenders;
                    for (int i = 0; i < ac.Count; i++)
                    {
                        // 这里我只对RollingFileAppender类型做修改 
                        log4net.Appender.RollingFileAppender rfa = ac[i] as log4net.Appender.RollingFileAppender;
                        if (rfa != null)
                        {
                            fileName = AppDomain.CurrentDomain.BaseDirectory + "\\log\\" + DateTime.Now.ToString("yyyyMM\\\\dd") + "\\" + fileName + ".log";

                            if (rfa.File != null)
                            {
                                if (rfa.File == fileName)
                                {
                                    continue;
                                }
                            }
                            rfa.File = fileName;
                            // 更新Writer属性 
                            rfa.Writer = new System.IO.StreamWriter(rfa.File, rfa.AppendToFile, rfa.Encoding);
                        }
                    }
                }
            }
            catch
            { }
            ext_log = log4net.LogManager.GetLogger("ext");
        }
    }
}
