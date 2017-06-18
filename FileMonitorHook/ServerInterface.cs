// RemoteFileMonitor (File: FileMonitorHook\ServerInterface.cs)
//
// Copyright (c) 2017 Justin Stenning
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
// Please visit https://easyhook.github.io for more information
// about the project, latest updates and other tutorials.

using System;
using System.Diagnostics;

namespace FileMonitorHook
{
    /// <summary>
    /// Provides an interface for communicating from the client (target) to the server (injector)
    /// </summary>
    public class ServerInterface : MarshalByRefObject
    {

        /*
         * CREATE
         * MOVEA
         * MOVEW
         * GETATTRA
         * GETATTRW
         */

           public const string  CREATE      = "CREATE";
           public const string  MOVEA       = "MOVEA";
           public const string  MOVEW       = "MOVEW";
           public const string  GETATTRA    = "GETATTRA";
           public const string  GETATTRW    = "GETATTRW";
        public bool running = false;
        public delegate void HandleMessage(string message);
        public HandleMessage messageHandler;
        public void IsInstalled(int clientPID)
        {
            string s = $"FileMonitor has injected FileMonitorHook into process {clientPID}.\r\n";
            if (messageHandler != null) messageHandler(s);
            Debug.WriteLine(s);
        }

        /// <summary>
        /// Output the message to the console.
        /// </summary>
        /// <param name="fileNames"></param>
        public void ReportMessages(string[] messages)
        {
            for (int i = 0; i < messages.Length; i++)
            {
                Debug.WriteLine(messages[i]);
                if (messageHandler != null) messageHandler(messages[i]);
            }
        }

        public void ReportMessage(string message)
        {
            Debug.WriteLine(message);
                if (messageHandler != null) messageHandler(message);
        }

        /// <summary>
        /// Report exception
        /// </summary>
        /// <param name="e"></param>
        public void ReportException(Exception e)
        {
            Debug.WriteLine("The target process has reported an error:\r\n" + e.ToString());
        }

        int count = 0;
        /// <summary>
        /// Called to confirm that the IPC channel is still open / host application has not closed
        /// </summary>
        public void Ping()
        {
        }
    }
}
