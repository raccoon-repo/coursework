using BookLibrary;
using BookLibrary.Core.Service;
using BookLibrary.Entities;
using BookLibrary.Xml.Utils;
using System.Collections.Generic;
using System.Globalization;
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

        public bool BookIsSaved { get; set; }

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
            countTextBox.Text = BookCounter.Count(Book.Id).ToString();
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
                MessageBox.Show("The value of rating is invalid. Enter it again");
                return;
            }

            if (!int.TryParse(countTextBox.Text, out _))
            {
                MessageBox.Show("The value of count is invalid. Enter it again");
                return;
            }

            Book.Title = titleTextBox.Text;
            Book.Rating = float.Parse(ratingTextBox.Text);
            Book.Description = descriptionTextBox.Text;
            Book.Section = BookUtils.ParseSection(sectionComboBox.Text);
            Book.Authors = (bookAuthors.ItemsSource as IList<Author>);

            BookService.SaveOrUpdate(Book);

            if (!BookCounter.IsPresent(Book.Id)) {
                BookCounter.AddNew(Book.Id, int.Parse(countTextBox.Text));
            }
            else
            {
                BookCounter.SetCount(Book.Id, int.Parse(countTextBox.Text));
            }

            // if book is saved
            // it will be displayed onto main datagrid
            BookIsSaved = true;
            MessageBox.Show("Done");
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
            DeleteDialogWindow deleteDialog = new DeleteDialogWindow();

            if (deleteDialog.ShowDialog() == true)
            {
                BookService.Delete(Book);
                Close();
            }
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
