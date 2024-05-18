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
            RefreshAndSort(); // Call method to fetch book titles when the window is initialized
        }

        private void RefreshAndSort()
        {
            try
            {
                List<string> bookTitles = new List<string>();

                // Connect to the database and fetch book titles
                using (SqlConnection connection = new SqlConnection("Data Source=DANISH-HP-LAPTO\\SQLEXPRESS;Initial Catalog=projectdb;Integrated Security=True;"))
                {
                    connection.Open();
                    string query = "SELECT Title FROM books";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    // Add fetched book titles to the list
                    while (reader.Read())
                    {
                        string title = reader.GetString(0);
                        bookTitles.Add(title);
                    }

                    reader.Close();
                }

                // Sort the book titles in ascending order
                bookTitles.Sort();

                // Clear existing items in the ListView and add sorted book titles
                bookListView.Items.Clear();
                foreach (string title in bookTitles)
                {
                    bookListView.Items.Add(title);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error refreshing book titles: " + ex.Message);
            }
        }
        private void FetchBookTitles()
        {
            try
            {
                // Connect to the database and fetch book titles
                using (SqlConnection connection = new SqlConnection("Data Source=DEVELOPER-966\\SQLEXPRESS;Initial Catalog=projectdb;Integrated Security=True;"))
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

                // Initialize variables to store book details
                int price = 0;
                string imagePath = string.Empty;

                // Connect to the database and fetch the details of the selected book
                using (SqlConnection connection = new SqlConnection("Data Source=DANISH-HP-LAPTO\\SQLEXPRESS;Initial Catalog=projectdb;Integrated Security=True;"))
                {
                    try
                    {
                        connection.Open();
                        string query = "SELECT Price, imagePath FROM books WHERE Title = @Title";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@Title", selectedTitle);

                        SqlDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            // Fetch price as integer
                            price = reader.GetInt32(0);
                            // Fetch image path
                            imagePath = reader.GetString(1);
                        }

                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error fetching book details: " + ex.Message);
                    }
                }

                // Display the price in the priceLabel
                priceLabel.Content = $"Price:\t${price}";

                // Display the selected title in the titleLabel
                nameLabel.Content = $"Title:\t{selectedTitle}";

                // Set the image source if an image path is available
                if (!string.IsNullOrEmpty(imagePath))
                {
                    try
                    {
                        Uri imageUri = new Uri(imagePath, UriKind.Absolute);
                        booksImages.Source = new BitmapImage(imageUri);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error setting image source: " + ex.Message);
                    }
                }
                else
                {
                    // Clear the image source if no image path is found
                    booksImages.Source = null;
                }
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
                            bookdetails.Price = Convert.ToInt32(reader["Price"]);
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

                
            }
            else
            {
                MessageBox.Show("Please select a book to delete.");
            }
            RefreshAndSort();
        }
    }
}
