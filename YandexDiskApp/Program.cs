using Disk.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YandexDiskApp
{
    static class Program
    {
        private static string ACCESS_TOKEN = "AQAAAAAP1aPWAAe_lES2qOcrBEM9vkU5vPmxuX8";

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var sdk = new DiskSdkClient(ACCESS_TOKEN);

            Application.Run(new mainForm(sdk));
        }
    }
}
