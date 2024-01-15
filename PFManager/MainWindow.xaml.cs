using PFManager.View;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace PFManager
{
    public partial class MainWindow : Window
    {
        public bool enabled = false;
        public bool autoPurge = false;
        public ListBox allExceptions = new();
        public MainWindow()
        {
            DataContext = this;
            processEntries = [];
            InitializeComponent();
        }
        private ObservableCollection<string> processEntries;
        public ObservableCollection<string> ProcessEntries
        {
            get { return processEntries; }
            set { processEntries = value; }
        }
        /// <summary>
        /// Object to Integer converter, created using help from https://stackoverflow.com/a/56868985
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public object ConverttoInt(object value)
        {
            return int.TryParse((string)value, out int ret) ? ret : 0; 
        }
        /// <summary>
        /// Process list function, created using help from https://www.youtube.com/watch?v=F2RG5NDmgBU
        /// </summary>
        /// <param name="purge"></param>
        public void ListAllProcesses(bool purge)
        {
            Process[] AllProcesses = Process.GetProcesses();
            ComboBoxItem selectedItem = (ComboBoxItem)(PurgeComboBox.SelectedValue);
            string value = (string)selectedItem.Content;
            value = value.Remove(value.Length - 1, 1);
            int newValue = (int)ConverttoInt(value);
            ReadSettings();
            foreach (Process process in AllProcesses)
            {
                if (Math.Floor((decimal)process.WorkingSet64 / 1024 * newValue) < Math.Floor((decimal)process.PagedMemorySize64 / 1024))
                {
                    if (!purge && !autoPurge)
                    {
                        bool exceptionFound = false;
                        foreach (string exception in allExceptions.Items)
                        {
                            if (process.ProcessName.ToLower() != exception)
                            {
                                continue;
                            }
                            ProcessEntries.Add($"{process.ProcessName}.exe | {Math.Floor((decimal)process.WorkingSet64 / 1024)} K | {Math.Floor((decimal)process.PagedMemorySize64 / 1024)} K");
                            exceptionFound = true;
                        }
                        if (!exceptionFound)
                        {
                            ProcessEntries.Add($"[EXCEEDED LIMIT] {process.ProcessName}.exe | {Math.Floor((decimal)process.WorkingSet64 / 1024)} K | {Math.Floor((decimal)process.PagedMemorySize64 / 1024)} K");
                        }
                    }
                    else
                    {
                        bool exceptionFound = false;
                        foreach (string exception in allExceptions.Items)
                        {;
                            if (process.ProcessName.ToLower() != exception){
                                continue;
                            }
                            exceptionFound = true;
                        }
                        if (!exceptionFound)
                        {
                            process.Kill();
                        }
                    }
                }
                else
                {
                    ProcessEntries.Add($"{process.ProcessName}.exe | {Math.Floor((decimal)process.WorkingSet64 / 1024)} K | {Math.Floor((decimal)process.PagedMemorySize64 / 1024)} K");
                }
                ProcessList.Items.SortDescriptions.Add(new SortDescription("", ListSortDirection.Ascending)); // obtained from https://stackoverflow.com/a/56868985
            }
        }
        private async void Start_Click(object sender, RoutedEventArgs e)
        {
            this.StopButton.IsEnabled = true;
            this.StartButton.IsEnabled = enabled;
            enabled = !enabled;
            while (enabled)
            {
                ComboBoxItem selectedPollingRate = (ComboBoxItem)(PollingRateBox.SelectedValue);
                string pollingRate = (string)selectedPollingRate.Content;
                pollingRate = pollingRate.Remove(pollingRate.Length - 2, 2);
                int newPollingRate = (int)ConverttoInt(pollingRate);
                PollingRateText.Text = $"Polling Rate: {newPollingRate}ms";
                ListAllProcesses(false);
                await Task.Delay(newPollingRate);
                ProcessEntries.Clear();
            }
        }
        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Your PC is gonna explode!", "Oh no!", MessageBoxButton.OK, MessageBoxImage.Error);
            this.StartButton.IsEnabled = enabled;
            this.StopButton.IsEnabled = false;
            enabled = !enabled;
        }
        private void Exceptions_Click(object sender, RoutedEventArgs e)
        {
            Exceptions execeptions = new(this);
            execeptions.ShowDialog();
        }
        private void Purge_Click(object sender, RoutedEventArgs e)
        {
            if (!enabled)
            {
                return;
            }
            ListAllProcesses(true);
        }
        private void PurgeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.PurgeText.Text = $"Purge items exceeding a commit size of {(string)((ComboBoxItem)((ComboBox)sender).SelectedValue).Content} of physical memory.";
        }
        private void AutoPurgeCheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }
        /// <summary>
        /// Controls the topmost checkbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TopMostCheckBox_Click(object sender, RoutedEventArgs e)
        {
            this.Topmost = !this.Topmost;
        }
        /// <summary>
        /// Maximizes the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        ///private void Maximize_Click(object sender, RoutedEventArgs e)
        ///{
        ///    if (this.WindowState != WindowState.Maximized)
        ///    {
        ///        this.WindowState = WindowState.Maximized;
        ///        this.Maximize.Content = "🗗";
        ///    }
        ///    else
        ///    {
        ///        this.WindowState = WindowState.Normal;
        ///        this.Maximize.Content = "🗖";
        ///    }
        ///}

        /// <summary>
        /// Minimizes the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void X_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void Rectangle_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void InverseExceptions_Click(object sender, RoutedEventArgs e)
        {

        }
        /// <summary>
        /// Maximizes the window when double clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void Window_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        //{
        //    Maximize_Click(sender, e);
        //}
        /// <summary>
        /// Read settings function, created with help from https://www.youtube.com/watch?v=1HpcedPme0U
        /// </summary>
        void ReadSettings()
        {
            try
            {
                NameValueCollection appSettings = ConfigurationManager.AppSettings;
                ListBox exceptions = new();
                foreach (string appSetting in appSettings)
                {
                    if (!appSetting.EndsWith(".EXE"))
                    {
                        continue;
                    }
                    string settingValue = appSetting.Remove(appSetting.Length - 4, 4);
                    exceptions.Items.Add(settingValue.ToLower());
                }
                allExceptions = exceptions;
            }
            catch (ConfigurationErrorsException) { return; }
        }
        /// <summary>
        /// Write settings function, created with help from https://www.youtube.com/watch?v=1HpcedPme0U
        /// </summary>
        void WriteSettings(string key, string value)
        {
            try
            {
                Configuration configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                KeyValueConfigurationCollection settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch(ConfigurationErrorsException) { return; }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ReadSettings();
        }
    }
}