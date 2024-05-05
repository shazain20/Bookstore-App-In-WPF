using System;
using System.Data.SqlClient; // Added missing namespace
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
    /// Interaction logic for AddBook.xaml
    /// </summary>
    public partial class AddBook : Window
    {
        private const string connectionString = "Data Source=DEVELOPER-966\\SQLEXPRESS;Initial Catalog=projectdb;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";
        public AddBook()
        {
            InitializeComponent();
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            // Get values from the text boxes
            string bookId = bookIdTextBox.Text;
            string title = titleTextBox.Text;
            string genre = genreTextbox.Text;
            decimal price = Convert.ToDecimal(priceTextbox.Text);
            int quantity = Convert.ToInt32(quantityTextbox.Text);

            // Insert into the database
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO YourTableName (BookId, Title, Genre, Price, Quantity) VALUES (@BookId, @Title, @Genre, @Price, @Quantity)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@BookId", bookId);
                    command.Parameters.AddWithValue("@Title", title);
                    command.Parameters.AddWithValue("@Genre", genre);
                    command.Parameters.AddWithValue("@Price", price);
                    command.Parameters.AddWithValue("@Quantity", quantity);
                    command.ExecuteNonQuery();
                    MessageBox.Show("Book added successfully!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error adding book: " + ex.Message);
                }
            }
        }
    }
}
