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

        private void Edit_Book(object sender, RoutedEventArgs args)
        {
            Edit_Book(allBooksDataGrid);
        }

        private void Edit_Found_Book(object sender, RoutedEventArgs args)
        {
            Edit_Book(searchResultGrid);
        }

        private void Edit_Book(DataGrid dataGrid)
        {
            if (dataGrid.SelectedItem is null)
                return;

            BookDto bookDto = (BookDto)dataGrid.SelectedItem;
            BookWindow editBookWindow = new BookWindow
            {
                Book = BookService.FindById(bookDto.Id),
                BookService = BookService,
                AuthorService = AuthorService,
                BookCounter = BookCounter
            };


            editBookWindow.ShowDialog();
            dataGrid.Items.Refresh();
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
            BookCounter.Remove(dto.Id);
        }

        private void Update_All_Books_Data_Grid(object sender, RoutedEventArgs args)
        {
            Update(allBooksDataGrid);
        }

        private void Update_Search_Result_Grid(object sender, RoutedEventArgs args)
        {
            Search();
        }

        private void Update(DataGrid dataGrid)
        {
            BookDtoConverter dtoConverter = new BookDtoConverter {
                BookCounter = BookCounter
            };

            dataGrid.ItemsSource = dtoConverter.ConvertBooks(BookService.FindAll());
            dataGrid.Items.Refresh();
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
            searchByAuthor.ItemsSource = AuthorService.FindAll();
        }

        private void Search()
        {
            var ratingFromStr = searchByRatingFrom.Text;
            var ratingToStr = searchByRatingTo.Text;
            var title = searchByTitle.Text;
            var section = searchBySection.Text;
            var author = (Author)searchByAuthor.SelectedItem;
            float ratingFrom;
            float ratingTo;
            IList<Book> temp;
            ISet<BookDto> result = new HashSet<BookDto>();
            BookDtoConverter converter = new BookDtoConverter
            {
                BookCounter = BookCounter
            };

            if (ratingFromStr.Length != 0 && ratingToStr.Length != 0)
            {
                if (!float.TryParse(ratingFromStr, out ratingFrom) || !float.TryParse(ratingToStr, out ratingTo))
                {
                    MessageBox.Show("Enter the valid value of rating");
                    return;
                }
                temp = BookService.FindByRating(ratingFrom, ratingTo);

                foreach (var book in temp)
                    result.Add(converter.ConvertBook(book));
            }



            if (!(author is null))
            {
                foreach (var book in author.Books)
                    result.Add(converter.ConvertBook(book));
            }

            temp = BookService.FindByTitle(title);
            foreach (var book in temp)
                result.Add(converter.ConvertBook(book));


            if (section.Length != 0 && !section.Equals("None"))
            {
                temp = BookService.FindBySection(BookUtils.ParseSection(section));
                foreach (var book in temp)
                    result.Add(converter.ConvertBook(book));
            }

            searchResultGrid.ItemsSource = result;
            searchResultGrid.Items.Refresh();
            searchByAuthor.SelectedItem = null;
        }

        private void Search_Button_Click(object sender, RoutedEventArgs args)
        {
            Search();
        }
    }
}
