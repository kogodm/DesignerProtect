using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms.VisualStyles;

namespace PhotoShopBackUpC
{
    public class Logger
    {
        private static bool Closed = false;
        private static Object lockObj = new object();
        public static void Log(string log)
        {
#if DEBUG
            Debug.WriteLine(log);
#endif
            if (Closed) return;
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            if (!Directory.Exists(path+"/DesignerProtect"))
            {
                Directory.CreateDirectory(path + "/DesignerProtect");
            }
            lock (lockObj)
            {
                var logFile = new FileStream(path + "/DesignerProtect/log.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
                log += "\n";
                byte[] bytes = System.Text.Encoding.Default.GetBytes(log);
                logFile.Write(bytes, 0, bytes.Length);
                logFile.Flush();
                logFile.Close();
            }
        }

        public static void Log(string log, params object[] ps)
        {
            string s = string.Format(log, ps);
            Log(s);
        }

        public static void Close()
        {
            Closed = true;
        }
    }
}