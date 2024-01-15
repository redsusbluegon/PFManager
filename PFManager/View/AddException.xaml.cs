using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PFManager.View
{
    public partial class AddException : Window
    {
        public string Input { get; set; }
        public bool Success { get; set; }
        public AddException(Window parentWindow)
        {
            Owner = parentWindow;
            InitializeComponent();
            ExceptionText.Focus();
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Success = true;
            Input = ExceptionText.Text.ToLower();
            Close();
        }
        private void ExceptionText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!ExceptionText.Text.EndsWith(".exe"))
            {
                AddButton.IsEnabled = false;
            } else AddButton.IsEnabled = true;
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
    }
}
