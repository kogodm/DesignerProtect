using System;
using System.Diagnostics;
using System.Windows.Forms;
using Photoshop;

namespace PhotoShopBackUpC
{
    public class PhotoShopLisener : Listener
    {
        public PhotoShopLisener(string path, FileOperater fileOperater) : base(path, fileOperater)
        {
        }

        private ApplicationClass app;
        private string processName = "PhotoShop";
        private int Id = -1;
        string _infoString = "";
        public override void Check()
        {
            if (CheckProcessChanged())
            {
                if (Id > 0)
                {
                    _infoString = "PhotoShop:\n";
                    try
                    {
                        app = new ApplicationClass();
                        int c = app.Documents.Count;
                        for (int i = 0; i < c; i++)
                        {
                            var doc = app.Documents[i + 1];
                            if (doc.Name.EndsWith("psd"))
                            {
                                string name =doc.Name;
                                if (name.Length >= 60)
                                {
                                    int remain = name.Length - 60;
                                    name = name.Remove(30, remain);
                                    name = name.Insert(30, "...");
                                }
                                _infoString += i + "." + name + "\n";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex.Message + "\n" + ex.StackTrace);
                        _infoString = "PhotoShop:Busy\n";
                    }
                }
            }
            else
            {
                _infoString = "PhotoShop:没有打开.";
            }
        }
        public bool CheckProcessChanged()
        {
            Process[] processes = Process.GetProcessesByName(processName);
            if (processes.Length > 0)
            {
                int tPId = processes[0].Id;
                if (tPId != Id)
                {
                    Debug.WriteLine("Not same:" + tPId + " " + Id);
                    Id = tPId;
                    return true;
                }
                return true;
            }
            else Id = -1;
            return false;
        }

        public override string GetRunInfo()
        {
            return _infoString;
        }
        public override void Save()
        {
            Check();
            if (Id < 0)
            {
                return;
            }
            try
            {
                int c = app.Documents.Count;
                for (int i = 0; i < c; i++)
                {
                    var doc = app.Documents[i + 1];
                    if (doc.Name.EndsWith("psd"))
                    {
                        string backUpfile = path + "/ps/" + doc.Name + "_ps_backup.psd";
                        if (doc.Saved)
                        {
                            _fileOperater.Copy(doc.FullName, backUpfile);
                            continue;
                        }
                        PhotoshopSaveOptions psdSaveOptions = new PhotoshopSaveOptions
                        {
                            EmbedColorProfile = true,
                            AlphaChannels = true
                        };
                        doc.SaveAs(backUpfile, psdSaveOptions, true, PsExtensionType.psLowercase);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message + "\n" + ex.StackTrace);
            }
        }

        public override void Stop()
        {
            app = null;
        }
    }
}