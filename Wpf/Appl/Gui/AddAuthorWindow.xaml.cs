using BookLibrary.Core.Service;
using BookLibrary.Entities;
using System.Windows;
using System.Windows.Controls;

namespace Wpf.Appl.Gui
{
    public partial class AddAuthorWindow : Window
    {
        public IAuthorService AuthorService;

        public Author Author { get; set; }

        public AddAuthorWindow()
        {
            InitializeComponent();
        }

        private void Data_Grid_Loaded(object sender, RoutedEventArgs args)
        {
            allAuthorsDataGrid.ItemsSource = AuthorService.FindAll();
        }

        private void Done_Button_Click(object sender, RoutedEventArgs args)
        {
            if (newAuthorRadioButton.IsChecked.Value)
            {
                Author = new Author() {
                    FirstName = firstNameTextBox.Text,
                    LastName = lastNameTextBox.Text
                };
            }
            else if (selectedAuthorRadioButton.IsChecked.Value)
            {
                Author = (Author) allAuthorsDataGrid.SelectedItem;
            }

            DialogResult = true;
            Close();
        }
    }
}
