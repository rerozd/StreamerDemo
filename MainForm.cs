using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using Flir.Atlas.Live.Device;
using Flir.Atlas.Live.Discovery;

namespace ThermalImageStreamerDemo
{
    public partial class MainForm : Form
    {
        private Camera _camera;
        bool CancelClicked = false;

        readonly System.Windows.Forms.Timer _timerRefreshUi = new System.Windows.Forms.Timer();
        public MainForm()
        {
            InitializeComponent();
            //Changing Trigger
            _timerRefreshUi.Interval = 20;
            _timerRefreshUi.Tick += _timerRefreshUi_Tick;
        }

        void _timerRefreshUi_Tick(object sender, EventArgs e)
        {
            if (!IsDirty) return;
            IsDirty = false;
            if (_camera == null) return;

            _camera.GetImage().EnterLock();
            try
            {
                pictureBox1.Image = _camera.GetImage().Image;
            }
            catch (Exception exception)
            {
                Trace.TraceError(exception.Message);
            }
            finally
            {
                _camera.GetImage().ExitLock();
            }
        }

        private void discoveryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var discoveryDlg = new DiscoveryForm();
            if (discoveryDlg.ShowDialog() == DialogResult.OK)
            {
                ConnectCamera(discoveryDlg.SelectedCameraDevice);
            }
        }

        private void ConnectCamera(CameraDeviceInfo cameraDeviceInfo)
        {
            DisposeCamera();
            switch (cameraDeviceInfo.SelectedStreamingFormat)
            {
                case ImageFormat.FlirFileFormat:
                    _camera = new ThermalCamera();
                    break;
                case ImageFormat.Argb:
                    _camera = new VideoOverlayCamera();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            _camera.ConnectionStatusChanged += _camera_ConnectionStatusChanged;
            _camera.GetImage().Changed += Image_Changed;
            _camera.Connect(cameraDeviceInfo);
            if (_camera.Recorder == null && _recorder != null)
            {
                _recorder.Dispose();
                _recorder = null;
            }
            if (_recorder != null)
            {
                if (!_recorder.IsDisposed)
                    _recorder.Initialize(_camera);
            }
            recorderToolStripMenuItem.Enabled = _camera.Recorder != null;
            _timerRefreshUi.Start();
        }

        private void DisposeCamera()
        {
            _timerRefreshUi.Stop();
            if (_camera == null) return;
            if (_recorder != null)
            {
                _recorder.UnInitialize();
                _recorder.Dispose();
            }
            _camera.ConnectionStatusChanged -= _camera_ConnectionStatusChanged;
            _camera.GetImage().Changed -= Image_Changed;
            _camera.Dispose();
        }

        private bool IsDirty { get; set; }
        void Image_Changed(object sender, Flir.Atlas.Image.ImageChangedEventArgs e)
        {
            IsDirty = true;
        }

        void _camera_ConnectionStatusChanged(object sender, Flir.Atlas.Live.ConnectionStatusChangedEventArgs e)
        {
            BeginInvoke((Action) (()=> toolStripConnectionStatus.Text = e.Status.ToString()));
            BeginInvoke((Action)(() => cameraToolStripMenuItem.Enabled = e.Status == ConnectionStatus.Connected)); 
        }

        private void DisconnectCamera()
        {
            if (_camera == null) return;
            
            _camera.Disconnect();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            
        }

        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisconnectCamera();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_recorder != null && _recorder.IsDisposed == false)
            {
                _recorder.Dispose();    
            }
            
            DisposeCamera();
        }

        private RecorderForm _recorder;
        private void recorderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_recorder == null || _recorder.IsDisposed)
            {
                _recorder = new RecorderForm();
                _recorder.Initialize(_camera);
                _recorder.SelectedFileMouseDoubleClick += _recorder_SelectedFileMouseDoubleClick;
            }
            _recorder.Show();
            _recorder.Focus();
        }

        void _recorder_SelectedFileMouseDoubleClick(object sender, SelectedFileEventArgs e)
        {
            var playback = new PlaybackForm(e.FilePath);
            playback.Show();
        }

        private const string FileFilter = "IR Image Files(*.jpg;*.img;*.seq;*.fcf)|*.jpg;*.img;*.seq;*.fcf|All files (*.*)|*.*";

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + @"\FLIR";
                dialog.Filter = FileFilter;
                if (dialog.ShowDialog() != DialogResult.OK) return;
                var playback = new PlaybackForm(dialog.FileName);
                playback.Show();
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            
        }


        private void button1_Click(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CancelClicked = true;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(100);
            do
            {
                //Loop for continuous taking pictures
                _camera.RemoteControl.SetValue(".power.states.digin1", true);
                Thread.Sleep(1000);
                _camera.RemoteControl.SetValue(".power.states.digin1", false);

                if (CancelClicked == true)
                {
                    CancelClicked = false;
                    MessageBox.Show("Stopped saving pictures");
                    break;
                }
            }
            while (true);
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            CancelClicked = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //Taking only 1 picture
            _camera.RemoteControl.SetValue(".power.states.digin1", true);
            Thread.Sleep(1000);
            _camera.RemoteControl.SetValue(".power.states.digin1", false);
        }
    }
}
