using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Flir.Atlas.Live;
using Flir.Atlas.Live.Device;
using Flir.Atlas.Image;
using Flir.Atlas.Live.Discovery;
using System.Threading;

namespace ThermalImageStreamerDemo
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            /* var device = CameraDeviceInfo.Create("192.168.0.21", Interface.All);
            if (device != null)
            {
                // Select 16-bit FLIR streaming format.
                device.SelectedStreamingFormat = ImageFormat.FlirFileFormat;
                var cam = new ThermalCamera();
                cam.Connect(device);
                while (!cam.ConnectionStatus.Equals(ConnectionStatus.Connected)) ;


                cam.GetImage().SaveSnapshot(@"C:\ftp\flir_ax8\test.jpg");
                cam.Disconnect();

            }
            else
            {
                MessageBox.Show("No camera found");
            }
        */
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
