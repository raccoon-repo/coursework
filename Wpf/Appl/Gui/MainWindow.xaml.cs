using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using BookLibrary;
using BookLibrary.Core.Service;
using BookLibrary.Entities;
using BookLibrary.Xml.Utils;
using Wpf.Appl.DTO;

namespace Wpf.Appl.Gui
{
    public partial class MainWindow : Window
    {
        private readonly IBookService BookService;
        private readonly IAuthorService AuthorService;
        private readonly IBookCounter BookCounter;

        public MainWindow()
        {
            InitializeComponent();

            BookService = ApplicationContext.BookService;
            AuthorService = ApplicationContext.AuthorService;
            BookCounter = ApplicationContext.BookCounter;
        }

        private void Data_Grid_Loaded(object sender, RoutedEventArgs args)
        {
            BookDtoConverter converter = new BookDtoConverter()
            {
                BookCounter = ApplicationContext.BookCounter
            };

            IList<BookDto> data = converter.ConvertBooks(BookService.FindAll());
            allBooksDataGrid.ItemsSource = data;  
        }

        private void Edit_Button_Click(object sender, RoutedEventArgs args)
        {
            if (allBooksDataGrid.SelectedItem is null)
            {
                MessageBox.Show("Select book");
                return;
            }

            BookDto bookDto = (BookDto)allBooksDataGrid.SelectedItem;
            BookWindow editBookWindow = new BookWindow
            {
                Book = BookService.FindById(bookDto.Id),
                BookService = BookService,
                AuthorService = AuthorService,
                BookCounter = BookCounter
            };


            editBookWindow.ShowDialog();
            allBooksDataGrid.Items.Refresh();
        }

        private void New_Book_Button_Click(object sender, RoutedEventArgs args)
        {
            BookWindow bookWindow = new BookWindow
            {
                Book = new Book(),
                BookService = BookService,
                AuthorService = AuthorService,
                BookCounter = BookCounter
            };

            BookDtoConverter dtoConverter = new BookDtoConverter {
                BookCounter = BookCounter
            };

            bookWindow.ShowDialog();
            if (bookWindow.BookIsSaved)
            {
                ((IList<BookDto>)allBooksDataGrid.ItemsSource).Add(dtoConverter.ConvertBook(bookWindow.Book));
                allBooksDataGrid.Items.Refresh();
            }
        }

        private void Remove_Button_Click(object sender, RoutedEventArgs args)
        {
            if (allBooksDataGrid.SelectedItem is null)
            {
                MessageBox.Show("Select book");
                return;
            }

            BookDto dto = (BookDto) allBooksDataGrid.SelectedItem;
            BookService.Delete(dto.Id);
        }

        private void Update_Button_Click(object sender, RoutedEventArgs args)
        {
            BookDtoConverter dtoConverter = new BookDtoConverter {
                BookCounter = BookCounter
            };
            allBooksDataGrid.ItemsSource = dtoConverter.ConvertBooks(BookService.FindAll());
        }



        private void Section_Selection_Changed(object sender, RoutedEventArgs args)
        {
            ComboBox box = (ComboBox)sender;
            BookSection section = BookUtils.ParseSection(box.SelectedValue.ToString());

            BookDtoConverter dtoConverter = new BookDtoConverter
            {
                BookCounter = BookCounter
            };

            allBooksBySection.ItemsSource = dtoConverter.ConvertBooks(BookService.FindBySection(section));
            allBooksBySection.Items.Refresh();
        }

        private void Search_By_Author_Loaded(object sender, RoutedEventArgs args)
        {

        }

        private void Search_Button_Click(object sender, RoutedEventArgs args)
        {

        }
    }
}
