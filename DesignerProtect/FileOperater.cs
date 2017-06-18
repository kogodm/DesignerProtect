using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace PhotoShopBackUpC
{
    public class FileOperater
    {
        Object lockCopyObject = new Object();
        private UInt64 tick = 0;
        private UInt64 count = 0;

        void AddCopyTime(UInt64 addTick)
        {
            lock (lockCopyObject)
            {
                tick += addTick;
                count++;
            }
        }

        public UInt64 GetTick()
        {
            return tick;
        }

        public UInt64 GetCount()
        {
            return count;
        }

        public void Copy(string src, string des)
        {
            Thread thread = null;
            //为了不让界面死掉，要将该操作放在一个线程中
            thread = new Thread
            (() => {
                
                Stopwatch sp = new Stopwatch();
                sp.Start();
                FileStream fsRead = new FileStream(src, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                FileStream fsWrite = new FileStream(des, FileMode.OpenOrCreate);
                
                byte[] buffer = new byte[2 * 1024 * 1024];
                int readLength = fsRead.Read(buffer, 0, buffer.Length);
                long readCount = 0;
                while (readLength != 0)
                {
                    fsWrite.Write(buffer, 0, readLength);
                    readCount += readLength;
                    readLength = fsRead.Read(buffer, 0, buffer.Length);
                }
                fsRead.Close();
                fsWrite.Close();
                buffer = null;
                GC.Collect();
                sp.Stop();
                AddCopyTime((UInt64)sp.ElapsedTicks);
            }
            );
            thread.Start();
        }
    }
}