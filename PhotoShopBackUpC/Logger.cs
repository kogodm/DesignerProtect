using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms.VisualStyles;

namespace PhotoShopBackUpC
{
    public class Logger
    {
        private static FileStream logFile;
        private static bool Closed = false;
        private string path="Log";
        public static void Log(string log)
        {
            if (Closed) return;
#if DEBUG
            Debug.WriteLine(log);
#endif
            if (logFile == null)
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                if (!Directory.Exists(path+"/DesignerProtect"))
                {
                    Directory.CreateDirectory(path + "/DesignerProtect");
                }

                logFile = new FileStream(path + "/DesignerProtect/log.txt", FileMode.OpenOrCreate,FileAccess.ReadWrite,FileShare.Read);
            }
            log += "\n";
            byte[] bytes = System.Text.Encoding.Default.GetBytes(log);
            logFile.Write(bytes, 0, bytes.Length);
            logFile.Flush();

        }

        public static void Log(string log, params object[] ps)
        {
            string s = string.Format(log, ps);
            Log(s);
        }

        public static void Close()
        {
            Closed = true;
            if (logFile != null)
            {
                logFile.Close();
            }
        }
    }
}