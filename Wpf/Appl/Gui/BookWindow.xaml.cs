using BookLibrary;
using BookLibrary.Core.Service;
using BookLibrary.Entities;
using BookLibrary.Xml.Utils;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Wpf.Appl.Gui
{

    public partial class BookWindow : Window
    {
        public IBookService BookService { get; set; }

        public IAuthorService AuthorService { get; set; }

        public IBookCounter BookCounter { get; set; }

        public IList<Author> RemovedAuthor = new List<Author>();

        public IList<Author> AddedAuthors = new List<Author>();

        public Book Book { get; set; }

        public BookWindow()
        {
            InitializeComponent();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs args)
        {
            titleTextBox.Text = Book.Title;
            ratingTextBox.Text = Book.Rating.ToString();
            descriptionTextBox.Text = Book.Description;
            sectionComboBox.SelectedItem = (ComboBoxItem)this.FindName(Book.Section.ToString());
            bookAuthors.ItemsSource = Book.Authors;
        }

        private void Undo_Button_Click(object sender, RoutedEventArgs e)
        {
            BookService.Refresh(Book);
            titleTextBox.Text = Book.Title;
            ratingTextBox.Text = Book.Rating.ToString();
            descriptionTextBox.Text = Book.Description;
            sectionComboBox.SelectedItem = (ComboBoxItem)FindName(Book.Section.ToString());
            bookAuthors.ItemsSource = Book.Authors;
            bookAuthors.Items.Refresh();
        }

        private void Save_Button_Click(object sender, RoutedEventArgs e)
        { 
            if (!float.TryParse(ratingTextBox.Text, out _))
            {
                MessageBox.Show("The value of rating is incorrect. Enter it again");
                return;
            }

            Book.Title = titleTextBox.Text;
            Book.Rating = float.Parse(ratingTextBox.Text);
            Book.Description = descriptionTextBox.Text;
            Book.Section = BookUtils.ParseSection(sectionComboBox.Text);
            Book.Authors = (bookAuthors.ItemsSource as IList<Author>);

            BookService.SaveOrUpdate(Book);
        }

        private void Add_Author_Button_Click(object sender, RoutedEventArgs e)
        {
            AddAuthorWindow addAuthorWindow = new AddAuthorWindow {
                AuthorService = AuthorService
            };

            if (addAuthorWindow.ShowDialog() == true && addAuthorWindow.Author != null)
            {
                Book.AddAuthor(addAuthorWindow.Author);
            }

            bookAuthors.Items.Refresh();
        }

        private void Delete_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Remove_Author_Button_Click(object sender, RoutedEventArgs e)
        {
            if (bookAuthors.SelectedItem != null)
            {
                ((IList<Author>)bookAuthors.ItemsSource).Remove((Author)bookAuthors.SelectedItem);
                bookAuthors.Items.Refresh();
            }
        }
    }
}
