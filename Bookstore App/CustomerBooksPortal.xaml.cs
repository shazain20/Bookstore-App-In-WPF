using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Bookstore_App
{
    /// <summary>
    /// Interaction logic for CustomerBooksPortal.xaml
    /// </summary>
    public partial class CustomerBooksPortal : Window
    {

        public ObservableCollection<Book> Books { get; set; }
        public ICommand AddToCartCommand { get; set; }
        public ICommand ShowDetailsCommand { get; set; }

        public CustomerBooksPortal()
        {
            InitializeComponent();
            DataContext = this;

            // Sample data for books
            Books = new ObservableCollection<Book>
            {
                new Book { Name = "Book 1", Price = 19.99, Picture = "book1.jpg" },
                new Book { Name = "Book 2", Price = 29.99, Picture = "book2.jpg" },
                new Book { Name = "Book 3", Price = 29.99, Picture = "book2.jpg" },
                new Book { Name = "Book 4", Price = 29.99, Picture = "book2.jpg" },
                new Book { Name = "Book 5", Price = 29.99, Picture = "book2.jpg" },

                // Add more books as needed
            };

            AddToCartCommand = new RelayCommand<Book>(AddToCart);
            ShowDetailsCommand = new RelayCommand<Book>(ShowDetails);
        }

        private void AddToCart(Book book)
        {
            MessageBox.Show($"{book.Name} added to cart.");
        }

        private void ShowDetails(Book book)
        {
            MessageBox.Show($"Showing details for {book.Name}.");
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Logged out.");
        }

        private void ShowCartButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Showing cart.");
        }

        private void ShowDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Showing details.");
        }
    }

    public class Book
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public string Picture { get; set; }
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute((T)parameter);
        }

        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }



}

