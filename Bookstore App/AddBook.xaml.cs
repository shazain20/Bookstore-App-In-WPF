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
            // Check if any of the fields are empty
            if (string.IsNullOrEmpty(titleTextBox.Text) ||
                string.IsNullOrEmpty(genreTextbox.Text) ||
                string.IsNullOrEmpty(priceTextbox.Text) ||
                string.IsNullOrEmpty(quantityTextbox.Text))
            {
                MessageBox.Show("Please fill in all fields.");
                return; // Exit the method if any field is empty
            }

            // Generate a random ID
            Random rand = new Random();
            int randomId = rand.Next(10000); // Generate a random number between 0 and 9999

            // Get values from the text boxes
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
                    command.Parameters.AddWithValue("@BookId", randomId);
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


