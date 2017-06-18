using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using FileMonitorHook;

namespace PhotoShopBackUpC
{
    public class SaiListener:Listener
    {

        class SaiMessage
        {
            public string Operate;
            public string file1;
            public string file2;
            public string targetFile;
        }

        class SaiProcess
        {
            public Process systemProcess;
            public string processName;
            public int Id;
            public List<string> files = new List<string>();
            public string errorInfo = "";
            public delegate void FileCopy(string src, string dst);

            public FileMonitor fileMonitor;


            public FileCopy copy;

            private int _keepFileCount = 5;

            public SaiProcess(string name)
            {
                processName = name;
                Id = -1;
            }

            void HandleSaiMessage(string msg)
            {
                SaiMessage saiMessage = DecodeMsg(msg);
                if (saiMessage != null)
                {
                    string targetFile = saiMessage.targetFile;
                    if (saiMessage.Operate == ServerInterface.MOVEW
                        || saiMessage.Operate == ServerInterface.MOVEA)
                    {
                        if (targetFile.EndsWith(".psd"))
                        {
                            copy(targetFile, Path.GetFileName(saiMessage.targetFile));
                        }
                    }
                    else
                    {
                        if (targetFile.EndsWith(".psd"))
                        {
                            if (!files.Contains(targetFile))
                            {
                                files.Add(targetFile);
                                if (files.Count > _keepFileCount)
                                {
                                    int deleteFileCount = files.Count - _keepFileCount;
                                    for (int i = 0; i < deleteFileCount; i++)
                                    {
                                        files.RemoveAt(0);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            SaiMessage DecodeMsg(string msg)
            {
                SaiMessage saiMessage = new SaiMessage();
                errorInfo += msg + "\n";
                Logger.Log(msg);
                string[] msgs = msg.Split('|');
                int len = msgs.Length;
                if (len < 2) return null;
                saiMessage.Operate = msgs[0];
                saiMessage.file1 = msgs[1];
                saiMessage.targetFile = saiMessage.file1;
                if (len >= 3 && (saiMessage.Operate == ServerInterface.MOVEW
                                || saiMessage.Operate == ServerInterface.MOVEA))
                {
                    //only move msg
                    saiMessage.file2 = msgs[2];
                    saiMessage.targetFile = saiMessage.file2;
                }
                return saiMessage;
            }

            public void StartInjector()
            {
                if (Id > 0)
                {
                    fileMonitor = new FileMonitor();
                    fileMonitor.StartMonitor(Id, HandleSaiMessage);
                }
            }

            public bool CheckProcessChanged()
            {
                Process[] processes = Process.GetProcessesByName(processName);
                if (processes.Length > 0)
                {
                    systemProcess = processes[0];
                    int tPId = systemProcess.Id;
                    if (tPId != Id)
                    {
                        Logger.Log("Not same:" + tPId + " " + Id);
                        Id = tPId;
                        return true;
                    }
                }
                else
                {
                    errorInfo = "没有找到进程：" + processName;
                    Id = -1;
                    fileMonitor = null;
                }
                return false;
            }
        }


        private SaiProcess sai;
        private SaiProcess sai2;
        public SaiListener(string path, FileOperater fileOperater) : base(path, fileOperater)
        {
            sai = new SaiProcess("sai");
            sai2 = new SaiProcess("sai2");
            sai.copy = CopyFile;
            sai2.copy = CopyFile;
        }

        public override void Check()
        {
            if (sai.CheckProcessChanged())
            {
                sai.StartInjector();
            }
            if (sai2.CheckProcessChanged())
            {
                sai2.StartInjector();
            }
        }

        public void CopyFile(string src,string dstName)
        {
            string backUpfile = path + dstName + "_backup.psd";
            _fileOperater.Copy(src, backUpfile);
        }
    
        public override void Save()
        {
            Check();
            Logger.Log("Send save message");
            //todo:click save
            if(sai.Id > 0) SendSaveMessageToProcess(sai.systemProcess.MainWindowHandle);
            if (sai2.Id > 0) SendSaveMessageToProcess(sai2.systemProcess.MainWindowHandle);
        }

        private void SendSaveMessageToProcess(IntPtr mainWindowHandle)
        {
            IntPtr CTRL_KEY = new IntPtr(0x11);
            uint KEY_DOWN = 0x0100;
            uint KEY_UP = 0x0101;
            IntPtr S_KEY = new IntPtr(0x53);
            //SetForegroundWindow(p.MainWindowHandle);
            Win32.PostMessage(mainWindowHandle, KEY_DOWN, CTRL_KEY, IntPtr.Zero);
            Win32.PostMessage(mainWindowHandle, KEY_DOWN, S_KEY, IntPtr.Zero);
            Win32.PostMessage(mainWindowHandle, KEY_UP, S_KEY, IntPtr.Zero);
            Win32.PostMessage(mainWindowHandle, KEY_UP, CTRL_KEY, IntPtr.Zero);
        }


        private string _info;
        public override string GetRunInfo()
        {
            _info = "Sai:";
            if (sai.Id < 0) _info += "没有打开." ;
            else foreach (var saiFile in sai.files)
            {
                _info += "\n" + saiFile;
            }
            _info += "\nSai2:";
            if (sai2.Id < 0) _info += "没有打开.";
            else foreach (var saiFile in sai2.files)
            {
                _info += "\n" + saiFile;
            }
//            _info += sai.errorInfo+"\n";
//            _info += sai2.errorInfo+ "\n";
            return _info;
        }

        public override void Stop()
        {
            if (sai.fileMonitor != null)
            {
                sai.fileMonitor.StopMonitor();
                sai.fileMonitor = null;
            }
            if (sai2.fileMonitor != null)
            {
                sai2.fileMonitor.StopMonitor();
                sai2.fileMonitor = null;
            }
        }
    }
}