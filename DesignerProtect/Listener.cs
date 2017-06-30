using System;
using System.Diagnostics;
using System.IO;

namespace PhotoShopBackUpC
{
    public class Listener
    {
        protected string path;
        protected FileOperater _fileOperater;

        public Listener() { }

        public Listener(string path, FileOperater fileOperater)
        {
            this.path = path;
            this._fileOperater = fileOperater;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (!Directory.Exists(path+"/ps"))
            {
                Directory.CreateDirectory(path + "/ps");
            }
            if (!Directory.Exists(path + "/sai"))
            {
                Directory.CreateDirectory(path + "/sai");
            }
        }

        protected Process[] FindProcess(string name)
        {
            return Process.GetProcessesByName(name);
        }

        public virtual void Save()
        {
            throw new NotImplementedException();
        }

        public virtual void Check()
        {
            throw new NotImplementedException();
        }

        public virtual string GetRunInfo()
        {
            throw new NotImplementedException();
        }

        public virtual void Stop()
        {
            throw new NotImplementedException();
        }
    }
}