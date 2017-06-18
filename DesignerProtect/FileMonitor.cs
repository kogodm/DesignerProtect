using System;
using System.Diagnostics;
using System.IO;
using FileMonitorHook;

namespace PhotoShopBackUpC
{
    public class FileMonitor
    {
        public  FileMonitorHook.ServerInterface serverInterface;
        public  void StopMonitor()
        {
            serverInterface.running = false;
        }

        public  void StartMonitor(Int32 targetPID,ServerInterface.HandleMessage handler)
        {
            // Will contain the name of the IPC server channel
            string channelName = null;

            // Process command line arguments or print instructions and retrieve argument value

            if (targetPID <= 0)
                return;

            // Create the IPC server using the FileMonitorIPC.ServiceInterface class as a singleton
            //                var x  = EasyHook.RemoteHooking.IpcCreateServer<FileMonitorHook.ServerInterface>(ref channelName, System.Runtime.Remoting.WellKnownObjectMode.Singleton);

            serverInterface = new ServerInterface();
            serverInterface.running = true;
            EasyHook.RemoteHooking.IpcCreateServer<FileMonitorHook.ServerInterface>(ref channelName,
                System.Runtime.Remoting.WellKnownObjectMode.Singleton,
                serverInterface);

            serverInterface.messageHandler = handler;

            string d = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            // Get the full path to the assembly we want to inject into the target process
            string injectionLibrary = Path.Combine(d, "FileMonitorHook.dll");

            try
            {
                // Injecting into existing process by Id
                Logger.Log("Attempting to inject into process {0}", targetPID);

                // inject into existing process
                EasyHook.RemoteHooking.Inject(
                    targetPID,          // ID of process to inject into
                    injectionLibrary,   // 32-bit library to inject (if target is 32-bit)
                    injectionLibrary,   // 64-bit library to inject (if target is 64-bit)
                    channelName         // the parameters to pass into injected library
                                        // ...
                );
            }
            catch (Exception e)
            {
                Logger.Log("There was an error while injecting into target:");
                Logger.Log(e.ToString());
            }
        }
    }
}
