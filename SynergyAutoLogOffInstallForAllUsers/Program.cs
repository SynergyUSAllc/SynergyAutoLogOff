using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynergyAutoLogOffInstallForAllUsers
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                String RunPath = AppDomain.CurrentDomain.BaseDirectory;

                string ProgramPath = "\"" + RunPath + "SynergyAutoLogOff.exe" + "\"";
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Run", "SynergyAutoLogOff", ProgramPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Success. Please Log Off / Log On to start the application.");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}