using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
    public partial class bookSideActivity : Window
    {
        public bookSideActivity()
        {
            InitializeComponent();
            Update_Book.IsEnabled = false;
            deleteButton.IsEnabled = false;
            FetchBookTitles(); // Call method to fetch book titles when the window is initialized
        }

        private void FetchBookTitles()
        {
            try
            {
                // Connect to the database and fetch book titles
                using (SqlConnection connection = new SqlConnection("Data Source=DANISH-HP-LAPTO\\SQLEXPRESS;Initial Catalog=projectdb;Integrated Security=True;"))
                {
                    connection.Open();
                    string query = "SELECT Title FROM books";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    // Clear existing items in the ListView
                    bookListView.Items.Clear();

                    // Add fetched book titles to the ListView
                    while (reader.Read())
                    {
                        string title = reader.GetString(0);
                        bookListView.Items.Add(title);
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching book titles: " + ex.Message);
            }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // This method can remain empty if it's not needed
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddBook addBook = new AddBook();
            this.Close();
            addBook.Show();
        }

        private void ListView_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            // Check if an item is selected
            if (bookListView.SelectedItem != null)
            {
                Update_Book.IsEnabled = true;
                deleteButton.IsEnabled = true;
                // Get the selected book title
                string selectedTitle = bookListView.SelectedItem.ToString();

                // Connect to the database and fetch the price of the selected book
                using (SqlConnection connection = new SqlConnection("Data Source=DANISH-HP-LAPTO\\SQLEXPRESS;Initial Catalog=projectdb;Integrated Security=True;"))
                {
                    try
                    {
                        connection.Open();
                        string query = "SELECT Price FROM books WHERE Title = @Title";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@Title", selectedTitle);
                        object result = command.ExecuteScalar();

                        if (result != null)
                        {
                            // Display the price in the priceLabel
                            priceLabel.Content = "Price:\t$" + result.ToString();
                        }
                        else
                        {
                            priceLabel.Content = "Price: N/A";
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error fetching book price: " + ex.Message);
                    }
                }

                // Display the selected title in the titleLabel
                nameLabel.Content = "Title:\t" + selectedTitle;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            AdminMenu adminMenu = new AdminMenu();
            adminMenu.Show();
            this.Close();
        }

        private void Update_Book_Click(object sender, RoutedEventArgs e)
        {
            // Check if an item is selected
            if (bookListView.SelectedItem != null)
            {
                string selectedTitle = bookListView.SelectedItem.ToString();

                // Create a BookDetails object to store the details of the selected book
                BookDetails bookdetails = new BookDetails();

                // Connect to the database and fetch the details of the selected book
                using (SqlConnection connection = new SqlConnection("Data Source=DANISH-HP-LAPTO\\SQLEXPRESS;Initial Catalog=projectdb;Integrated Security=True;"))
                {
                    try
                    {
                        connection.Open();
                        string query = "SELECT * FROM books WHERE Title = @Title";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@Title", selectedTitle);

                        SqlDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            // Populate the BookDetails object with the fetched details
                            bookdetails.ID = Convert.ToInt32(reader["BookId"]);
                            bookdetails.Title = reader["Title"].ToString();
                            bookdetails.Genre = reader["Genre"].ToString();
                            bookdetails.Price = Convert.ToDecimal(reader["Price"]);
                            bookdetails.Quantity = Convert.ToInt32(reader["Quantity"]);
                            bookdetails.Description = reader["Description"].ToString();
                        }

                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error fetching book details: " + ex.Message);
                    }
                }

                // Pass the BookDetails object to the UpdateBook window
                UpdateBook updateBook = new UpdateBook(bookdetails);
                this.Close();
                updateBook.Show();
            }
            else
            {
                MessageBox.Show("Please select a book to update.");
            }
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (bookListView.SelectedItem != null)
            {
                string selectedTitle = bookListView.SelectedItem.ToString();
                MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete the book \"{selectedTitle}\" from the catalogue?", "Delete Book", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    using (SqlConnection connection = new SqlConnection("Data Source=DANISH-HP-LAPTO\\SQLEXPRESS;Initial Catalog=projectdb;Integrated Security=True;"))
                    {
                        try
                        {
                            connection.Open();
                            string query = "DELETE FROM books WHERE Title = @Title";
                            SqlCommand command = new SqlCommand(query, connection);
                            command.Parameters.AddWithValue("@Title", selectedTitle);
                            command.ExecuteNonQuery();

                            MessageBox.Show("Book deleted successfully!");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error deleting book: " + ex.Message);
                        }
                    }
                }

                // Reopen the bookSideActivity window using Dispatcher to ensure it happens after the current window is closed
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    bookSideActivity newBookSideActivity = new bookSideActivity();
                    newBookSideActivity.Show();
                });

                this.Close();
            }
            else
            {
                MessageBox.Show("Please select a book to delete.");
            }
        }
    }
}
