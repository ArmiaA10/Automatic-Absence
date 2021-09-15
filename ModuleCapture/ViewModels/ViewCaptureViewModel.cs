using AutomaticAbsence.Core.Bases;
using AutomaticAbsence.Core.Configs;
using DirectShowLib;
using Microsoft.Win32;
using OpenCvSharp;
using OpenCvSharp.Face;
using OpenCvSharp.WpfExtensions;
using Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ModuleCapture.ViewModels
{
    public class ViewCaptureViewModel : ViewModelBase
    {
        #region Private Variable
        private readonly VideoCapture _videoCapture;
        private readonly BackgroundWorker _background;
        private readonly Application _application;
        private readonly CascadeClassifier _cascadeClassifier;
        #endregion Private Variable

        #region Constructor
        public ViewCaptureViewModel()
        {
            Title = "Module Capture";
            ExecuteRefreshDevice();

            _videoCapture = new();
            _background = new BackgroundWorker { WorkerSupportsCancellation = true };
            _cascadeClassifier = new(PathConfig.CascadeClassifierPath);

            _application = Application.Current;
            _background.DoWork += Background_DoWork;
        }
        #endregion Constructor

        #region Property
        private WriteableBitmap _captureImage;
        public WriteableBitmap CaptureImage
        {
            get { return _captureImage; }
            set { SetProperty(ref _captureImage, value); }
        }

        private bool _isCaptureRunning;
        public bool IsCaptureRunning
        {
            get { return _isCaptureRunning; }
            set { SetProperty(ref _isCaptureRunning, value); }
        }

        private ObservableCollection<string> _listDevice = new();
        public ObservableCollection<string> ListDevice
        {
            get { return _listDevice; }
            set { SetProperty(ref _listDevice, value); }
        }

        private string _nameDevice;
        public string NameDevice
        {
            get { return _nameDevice; }
            set { SetProperty(ref _nameDevice, value); }
        }

        private int _selectedDevice;
        public int SelectedDevice
        {
            get { return _selectedDevice; }
            set { SetProperty(ref _selectedDevice, value); }
        }
        #endregion Property

        #region Command
        private DelegateCommand _startCapture;
        public DelegateCommand StartCapture =>
            _startCapture ??= new DelegateCommand(ExecuteStartCapture, CanExecuteStartCapture).ObservesProperty(() => IsCaptureRunning);

        private bool CanExecuteStartCapture()
        {
            return !IsCaptureRunning;
        }

        private void ExecuteStartCapture()
        {
            if (ListDevice.Count == 0 || NameDevice == null)
            {
                return;
            }
            RunStartCapture(SelectedDevice);
            IsCaptureRunning = true;
        }

        private DelegateCommand _stopCapture;
        public DelegateCommand StopCapture =>
            _stopCapture ??= new DelegateCommand(ExecuteStopCapture).ObservesCanExecute(() => IsCaptureRunning);

        private void ExecuteStopCapture()
        {
            RunStopCapture();
            IsCaptureRunning = false;
        }

        private DelegateCommand _openVideoFile;
        public DelegateCommand OpenVideoFile =>
            _openVideoFile ??= new DelegateCommand(ExecuteOpenVideoFile, CanExecuteStartCapture).ObservesProperty(() => IsCaptureRunning);

        private void ExecuteOpenVideoFile()
        {
            try
            {
                var openFile = new OpenFileDialog()
                {
                    Filter = "Video file (*.mp4)|*.mp4",
                    InitialDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos)),
                    Title = "Select Video File",
                    Multiselect = false
                };
                openFile.FileOk += OpenFile_FileOk;
                if (openFile.ShowDialog() == true)
                {
                    RunStartCapture(openFile.FileName);
                    IsCaptureRunning = true;
                }
            }
            catch (Exception)
            {
                MessageBox.Show(new ArgumentException("Please select Video File").ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private DelegateCommand _refreshDevice;
        public DelegateCommand RefreshDevice =>
            _refreshDevice ??= new DelegateCommand(ExecuteRefreshDevice);

        private void ExecuteRefreshDevice()
        {
            if (ListDevice.Count > 0)
            {
                ListDevice.Clear();
                NameDevice = null;
            }
            foreach (DsDevice device in DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice))
            {
                ListDevice.Add(device.Name);
            }
            if (ListDevice.Count > 0)
            {
                NameDevice = ListDevice[0];
                SelectedDevice = 0;
            }
        }
        #endregion Command

        #region Private Method
        private void RunStartCapture(int index)
        {
            _videoCapture.Open(index);
            _background.RunWorkerAsync();
        }

        private void RunStartCapture(string file)
        {
            _videoCapture.Open(file);
            _background.RunWorkerAsync();
        }

        private void RunStopCapture()
        {
            if (!_videoCapture.IsOpened())
            {
                return;
            }
            _background.CancelAsync();
            _videoCapture.Release();
            _application.Dispatcher.Invoke(() => CaptureImage = new(1200, 1600, 96, 96, PixelFormats.Bgra32, null));
        }
        #endregion Private Method

        #region Grabber
        private void Background_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = (BackgroundWorker)sender;
            Mat grayImage = null;
            while (!worker.CancellationPending)
            {
                var fps = (int)_videoCapture.Fps;
                using (var model = LBPHFaceRecognizer.Create())
                using (var mat = _videoCapture.RetrieveMat())
                {
                    if (mat.Empty())
                    {
                        break;
                    }
                    if (grayImage == null)
                    {
                        grayImage = mat.CvtColor(ColorConversionCodes.BGR2GRAY);
                    }
                    model.Train(new[] { grayImage }, new[] { 1 });
                    string text;
                    foreach (var rect in _cascadeClassifier.DetectMultiScale(mat, 1.2, 5, HaarDetectionTypes.ScaleImage, new OpenCvSharp.Size(30, 30)))
                    {
                        using (Mat face = mat.CvtColor(ColorConversionCodes.BGR2GRAY)[rect].Clone())
                        {
                            Cv2.Resize(face, face, new OpenCvSharp.Size(256, 256));
                            model.Predict(face, out int label, out double predict);
                            text = $"Fadhil Ilham {DateTime.Now.ToShortDateString()} | {DateTime.Now.ToShortTimeString()}";
                            Cv2.PutText(mat, text, new OpenCvSharp.Point(rect.X, rect.Y - 5), HersheyFonts.HersheyTriplex, 0.3, Scalar.Red);
                        }
                        Cv2.Rectangle(mat, rect, Scalar.Red);
                    }
                    _application.Dispatcher.Invoke(() => CaptureImage = mat.ToWriteableBitmap()); // Must create and use WriteableBitmap in the same thread(UI Thread).
                }
                Thread.Sleep(fps);
            }
        }

        /// <summary>
        /// Fix file name with special character issue
        /// </summary>
        /// <param name="sender">sender from open file dialog</param>
        /// <param name="e">Event from open file dialog</param>
        private void OpenFile_FileOk(object sender, CancelEventArgs e)
        {
            var openFile = (OpenFileDialog)sender;
            if (openFile.SafeFileName.Contains("#") || openFile.SafeFileName.Contains("+") || openFile.SafeFileName.Contains("_"))
            {
                e.Cancel = true;
                MessageBox.Show(new FileFormatException("Please select file name without special character").ToString(), "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        #endregion Grabber
    }
}
