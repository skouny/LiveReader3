using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using CefSharp;
using CefSharp.WinForms;

namespace LiveReader3
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (!Settings.PrevInstance)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                if (WebAuthenticator.Authenticate())
                {
                    Cef.EnableHighDPISupport();
                    var settings = new CefSettings()
                    {
                        CachePath = Path.Combine(Application.LocalUserAppDataPath, "Cache"),
                        IgnoreCertificateErrors = true
                    };
                    if (Cef.Initialize(settings))
                    {
                        Application.Run(new FormMain());
                        Cef.Shutdown();
                    }
                    else
                    {
                        throw new Exception("Unable to Initialize Cef!!");
                    }
                }
            }
        }
    }
}
