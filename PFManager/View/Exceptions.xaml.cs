using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;
using System.Windows;

namespace PFManager.View
{
    public partial class Exceptions : Window
    {
        public Exceptions(Window parentWindow)
        {
            Owner = parentWindow;
            DataContext = this;
            entries = [];
            InitializeComponent();
        }
        private ObservableCollection<string> entries;
        public ObservableCollection<string> Entries
        {
            get { return entries; }
            set { entries = value; }
        }
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddException modalWindow = new(this);
            modalWindow.ShowDialog();
            if (modalWindow.Success)
            {
                Entries.Add(modalWindow.Input);
            }
        }
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var item = (string)ExceptionsList.SelectedItem;
            if (item != null)
            {
                DeleteException modalWindow = new(this, item);
                modalWindow.ShowDialog();
                if (modalWindow.Yes)
                {
                    Entries.Remove((string)item);
                    DeleteSettings(item.ToUpper());
                }
            }
        }
        private void X_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void Rectangle_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }
        /// <summary>
        /// Read settings function, created with help from https://www.youtube.com/watch?v=1HpcedPme0U
        /// </summary>
        void ReadSettings()
        {
            try
            {
                NameValueCollection appSettings = ConfigurationManager.AppSettings;
                foreach (string appSetting in appSettings)
                {
                    if (!appSetting.EndsWith(".EXE"))
                    {
                        continue;
                    }
                    string settingValue = appSetting.ToLower();
                    MeowText.Text = settingValue;
                    Entries.Add(settingValue);
                }
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
            catch (ConfigurationErrorsException) { return; }
        }
        void DeleteSettings(string key)
        {
            try
            {
                Configuration configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                KeyValueConfigurationCollection settings = configFile.AppSettings.Settings;
                if (settings[key] != null)
                {
                    settings.Remove(key);
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException) { return; }
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (string item in Entries)
            {
                WriteSettings(item.ToUpper(), item);
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ReadSettings();
        }
    }
}
