using FileMonitor.BackgroundService;
using FileMonitor.Front.Handlers;
using FileMonitor.Services.Common;
using FileMonitor.Services.Services;
using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace FileMonitor.Front
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly InformationService _informationService;
        private readonly MainWindowFileHandler _fileHandlerService;
        private readonly ControlBackgroundService _controlBackgroundService;

        public ObservableCollection<string> Paths { get; set; }

        public SolidColorBrush StatusColor { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            _informationService = new InformationService();
            _fileHandlerService = new MainWindowFileHandler();
            _controlBackgroundService = new ControlBackgroundService();

            this.DataContext = this;

            Paths = new ObservableCollection<string>();

            SetupEventHandlers();

            StatusColor = new SolidColorBrush(Color.FromRgb(231, 76, 60));

            SetupTimer();

            UpdateBackgroundServiceStatus(default, default);
            Autostart.IsChecked = _informationService.IsSetStartup();

            InitializePathSystem();
        }

        private void InitializePathSystem()
        {
            var paths = _fileHandlerService.InitializePaths();

            foreach (var path in paths)
            {
                Paths.Add(path);
            };
        }

        private void SetupTimer()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Tick += UpdateBackgroundServiceStatus;
            timer.Start();

        }

        private void SetupEventHandlers()
        {
            AddPath.Click += AddPath_Click;
            RemoveLast.Click += RemoveLast_Click;
            StopBackgroundApplication.Click += StopBackgroundApplication_Click;
            StartBackgroundApplication.Click += StartBackgroundApplication_Click;
            Autostart.Click += Autostart_Click;
        }

        public void UpdateBackgroundServiceStatus(object sender, EventArgs e)
        {
            var isRunning = _informationService.BackgroundServiceIsRunning(Constraints.BackgroundAssemblyName);

            if (isRunning)
            {
                StatusColor.Color = Color.FromRgb(29, 188, 96);
            }
            else
            {
                StatusColor.Color = Color.FromRgb(231, 76, 60);
            }
        }

        public void AddPath_Click(object sender, EventArgs e)
        {
            var path = _fileHandlerService.AddNewPathClick();

            if (path != string.Empty)
            {
                Paths.Add(path);
            }
        }

        public void Autostart_Click(object sender, EventArgs e)
        {
            if (Autostart.IsChecked.HasValue && Autostart.IsChecked.Value)
            {
                var fileLocation = $"{LocationHelper.GetLocation(Assembly.GetExecutingAssembly().Location)}\\{Constraints.BackgroundApplicationNestedFolder}";

                _controlBackgroundService.SetStartup($"{fileLocation}\\{Constraints.BackgroundAssemblyName}.exe");

                return;
            }

            _controlBackgroundService.DeleteStartup();
        }

        public void RemoveLast_Click(object sender, EventArgs e)
        {
            if (_fileHandlerService.RemoveLastPath())
            {
                Paths.RemoveAt(Paths.Count - 1);
            }
        }

        public void StopBackgroundApplication_Click(object sender, EventArgs e)
        {
            _controlBackgroundService.StopBackgroundApplication(Constraints.BackgroundAssemblyName);
        }

        public void StartBackgroundApplication_Click(object sender, EventArgs e)
        {
            _controlBackgroundService.StartBackgroundApplication(Constraints.BackgroundAssemblyName, Constraints.BackgroundApplicationNestedFolder);
        }
    }
}
