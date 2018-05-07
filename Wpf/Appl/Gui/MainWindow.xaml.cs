using System.Collections.Generic;
using System.Windows;
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

            bookWindow.Show();
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
    }
}
