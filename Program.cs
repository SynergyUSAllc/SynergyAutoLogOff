using System;

using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;

using System.Threading;
using System.Xml;
using System.Xml.Linq;

namespace AutoLogoutConsole
{
    public static class InputTimer
    {
        //
        //
        internal struct LASTINPUTINFO
        {
            public uint cbSize;

            public uint dwTime;
        }

        //
        private static BackgroundWorker worker = new BackgroundWorker();

        //
        //
        [DllImport("User32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        [DllImport("Kernel32.dll")]
        private static extern uint GetLastError();

        public static uint GetIdleTime()
        {
            LASTINPUTINFO lastInPut = new LASTINPUTINFO();
            lastInPut.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(lastInPut);
            GetLastInputInfo(ref lastInPut);

            return ((uint)Environment.TickCount - lastInPut.dwTime);
        }

        public static long GetTickCount()
        {
            return Environment.TickCount;
        }

        public static long GetLastInputTime()
        {
            LASTINPUTINFO lastInPut = new LASTINPUTINFO();
            lastInPut.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(lastInPut);
            if (!GetLastInputInfo(ref lastInPut))
            {
                throw new Exception(GetLastError().ToString());
            }

            return lastInPut.dwTime;
        }

        //
        //

        //

        //
        //
        //CODE FOR LOG OFF
        //
        //
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ExitWindowsEx(uint uFlags, uint dwReason);

        public static bool WindowsLogOff()
        {
            // Console.WriteLine("debugger");
            return ExitWindowsEx(0 | 0x00000004, 0);
        }

        //
        //
        //

        //
        //
        //WORKER CODE
        //
        //
        //
        private static void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // Console.WriteLine(e.ProgressPercentage.ToString());
        }

        private static void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            //   Console.WriteLine("Starting to do some work now...");
            //  double idlelogout = 10;
            //    Console.WriteLine("*****IDLE MONITOR*****");

            uint wrkGetIdleTime;

            while (true)
            {
                wrkGetIdleTime = GetIdleTime() / 1000;

                //  Console.WriteLine(wrkGetIdleTime);
                Thread.Sleep(200);

                int getTimeFromXmlInSeconds = GetTimeFromXML();

                if (wrkGetIdleTime > getTimeFromXmlInSeconds) WindowsLogOff();
                // if (wrkGetIdleTime > 20) WindowsLogOff();                //Log off after 20 seconds

                //if (InputTimer.GetInputIdleTime().Seconds <= idlelogout)
                //{
                //    Console.WriteLine("Nothing, nevermind, IDLE time: " + InputTimer.GetInputIdleTime().Seconds + " seconds");
                //}
                //else
                //{
                //    Console.WriteLine("Logout, IDLE time: " + InputTimer.GetInputIdleTime().Seconds + " seconds");
                //    WindowsLogOff();
                //}
            }
        }

        private static void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //  Console.WriteLine("Done...");
            //   Console.WriteLine("All inactive users are logged off");
        }

        public static int GetTimeFromXML()
        {
            try
            {
                String RunPath = AppDomain.CurrentDomain.BaseDirectory;

                XmlDocument doc = new XmlDocument();
                doc.Load(RunPath + "Time.xml");

                XmlElement root = doc.DocumentElement;
                XmlNodeList nodes = root.SelectNodes("LogOffTime");

                int minutes = 15; //set a default time just in case
                int seconds = 20 * 60;

                foreach (XmlNode node in nodes)
                {
                    foreach (XmlNode childNode in node.ChildNodes)
                    {
                        if (childNode.Name == "minutes")
                        {
                            minutes = Convert.ToInt32(childNode.InnerText);
                            seconds = minutes * 60;
                            return seconds;
                        }
                    }
                }
                return seconds;
            }
            catch { return 20 * 60; }
        }

        public static void Main()
        {
            //  int x = GetTimeFromXML();
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.ProgressChanged += worker_ProgressChanged;

            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;

            // Console.WriteLine("Starting Application...");

            worker.RunWorkerAsync();
            while (true)
            {
                Thread.Sleep(1000);
            }
            //  Console.ReadKey();
        }
    }
}