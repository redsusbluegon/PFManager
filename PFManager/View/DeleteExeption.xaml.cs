using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PFManager.View
{
    public partial class DeleteException : Window
    {
        public bool Yes { get; set; }
        public DeleteException(Window parentWindow, string item)
        {
            Owner = parentWindow;
            InitializeComponent();
            DeleteText.Text = $"Are you sure you want to delete {item}?";
        }
        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            Yes = true;
            Close();
        }
        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void X_Click(object sender, RoutedEventArgs e)
        {
            Close();
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
