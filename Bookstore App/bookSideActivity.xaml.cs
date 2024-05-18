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
    /// <summary>
    /// Interaction logic for bookSideActivity.xaml
    /// </summary>
    public partial class bookSideActivity : Window
    {

        public bookSideActivity()
        {
            InitializeComponent();
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
                    string query = "SELECT title FROM books";
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
                            priceLabel.Content = "Price: $" + result.ToString();
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
                nameLabel.Content = "Selected Book Title: " + selectedTitle;
            }
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            AdminMenu adminMenu = new AdminMenu();
            adminMenu.Show();
            this.Close();
        }
    }
}
