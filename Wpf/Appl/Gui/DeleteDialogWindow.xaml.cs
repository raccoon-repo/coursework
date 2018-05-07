using System.Windows;

namespace Wpf.Appl.Gui
{
    public partial class DeleteDialogWindow : Window
    {
        public DeleteDialogWindow()
        {
            InitializeComponent();
        }

        private void Yes_Button_Click(object sender, RoutedEventArgs args)
        {
            DialogResult = true;
            Close();
        }

        private void No_Button_Click(object sender, RoutedEventArgs args)
        {
            DialogResult = false;
            Close();
        }
    }
}
