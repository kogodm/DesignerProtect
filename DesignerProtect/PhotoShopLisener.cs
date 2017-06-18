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
                    app = new ApplicationClass();
                    int c = app.Documents.Count;
                    for (int i = 0; i < c; i++)
                    {
                        var doc = app.Documents[i + 1];
                        if (doc.Name.EndsWith("psd"))
                        {
                            _infoString += i + "." + doc.Name + "\n";
                        }
                    }
                }
                else
                {
                    _infoString = "PhotoShop:没打开.";
                }
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
            int c = app.Documents.Count;
            for (int i = 0; i < c; i++)
            {
                var doc = app.Documents[i + 1];
                if (doc.Name.EndsWith("psd"))
                {
                    string backUpfile = path + doc.Name + "_backup.psd";
                    if (doc.Saved)
                    {
                        _fileOperater.Copy(doc.FullName, backUpfile);
                        continue;
                    }
                    PhotoshopSaveOptions psdSaveOptions = new PhotoshopSaveOptions();
                    psdSaveOptions.EmbedColorProfile = true;
                    psdSaveOptions.AlphaChannels = true;
                    doc.SaveAs(backUpfile, psdSaveOptions, true, PsExtensionType.psLowercase);
                }
            }
        }

        public override void Stop()
        {
            app = null;
        }
    }
}