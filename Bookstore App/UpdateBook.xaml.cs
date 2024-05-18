using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data.SqlClient;

namespace Bookstore_App
{
    public partial class UpdateBook : Window
    {
        private BookDetails bookDetails;
        private string connectionString = "Data Source=DANISH-HP-LAPTO\\SQLEXPRESS;Initial Catalog=projectdb;Integrated Security=True;"; // Adjust your connection string as needed

        public UpdateBook(BookDetails bookDetails)
        {
            InitializeComponent();

            // Set the bookDetails field to the passed BookDetails object
            this.bookDetails = bookDetails;

            // Populate the textboxes with the details from the BookDetails object
            titleTextBox.Text = bookDetails.Title;
            genreTextbox.Text = bookDetails.Genre;
            priceTextbox.Text = bookDetails.Price.ToString();
            quantityTextbox.Text = bookDetails.Quantity.ToString();
            descriptionTextbox.Text = bookDetails.Description;
        }

        private void PriceTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateNumericInput(priceTextbox);
        }

        private void QuantityTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateNumericInput(quantityTextbox);
        }

        private void ValidateNumericInput(TextBox textBox)
        {
            if (!decimal.TryParse(textBox.Text, out _))
            {
                int selectionStart = textBox.SelectionStart - 1;
                textBox.Text = new string(textBox.Text.Where(char.IsDigit).ToArray());
                textBox.SelectionStart = Math.Max(0, selectionStart);
                MessageBox.Show("Please enter only numeric values.");
            }
        }

        private void updateButton_Click(object sender, RoutedEventArgs e)
        {
            // Remove event handlers temporarily
            priceTextbox.TextChanged -= PriceTextbox_TextChanged;
            quantityTextbox.TextChanged -= QuantityTextbox_TextChanged;

            if (string.IsNullOrEmpty(titleTextBox.Text) ||
                string.IsNullOrEmpty(genreTextbox.Text) ||
                string.IsNullOrEmpty(descriptionTextbox.Text) ||
                string.IsNullOrEmpty(priceTextbox.Text) ||
                string.IsNullOrEmpty(quantityTextbox.Text))
            {
                MessageBox.Show("Please fill in all fields.");

                // Reattach event handlers
                priceTextbox.TextChanged += PriceTextbox_TextChanged;
                quantityTextbox.TextChanged += QuantityTextbox_TextChanged;

                return;
            }

            if (!decimal.TryParse(priceTextbox.Text, out decimal price))
            {
                MessageBox.Show("Please enter a valid numeric value for price.");

                // Reattach event handlers
                priceTextbox.TextChanged += PriceTextbox_TextChanged;
                quantityTextbox.TextChanged += QuantityTextbox_TextChanged;

                return;
            }

            if (!int.TryParse(quantityTextbox.Text, out int quantity))
            {
                MessageBox.Show("Please enter a valid numeric value for quantity.");

                // Reattach event handlers
                priceTextbox.TextChanged += PriceTextbox_TextChanged;
                quantityTextbox.TextChanged += QuantityTextbox_TextChanged;

                return;
            }

            string title = titleTextBox.Text;
            string genre = genreTextbox.Text;
            string description = descriptionTextbox.Text;
            int bookId = bookDetails.ID;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE books SET Title = @Title, Genre = @Genre, Price = @Price, Quantity = @Quantity, Description = @Description WHERE BookId = @BookId";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Title", title);
                    command.Parameters.AddWithValue("@Genre", genre);
                    command.Parameters.AddWithValue("@Price", price);
                    command.Parameters.AddWithValue("@Quantity", quantity);
                    command.Parameters.AddWithValue("@Description", description);
                    command.Parameters.AddWithValue("@BookId", bookId);
                    command.ExecuteNonQuery();
                    titleTextBox.Text = string.Empty;
                    genreTextbox.Text = string.Empty;
                    priceTextbox.Text = string.Empty;
                    quantityTextbox.Text = string.Empty;
                    descriptionTextbox.Text = string.Empty;
                    MessageBox.Show("Book updated successfully!");

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating book: " + ex.Message);
                }
            }

            // Reattach event handlers
            priceTextbox.TextChanged += PriceTextbox_TextChanged;
            quantityTextbox.TextChanged += QuantityTextbox_TextChanged;
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            bookSideActivity obj = new bookSideActivity();
            this.Close();
            obj.Show();
        }
    }
}
