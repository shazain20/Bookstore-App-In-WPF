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
using Microsoft.Win32;

namespace Bookstore_App
{
    public partial class AddBook : Window
    {
        private const string connectionString = "Data Source=DANISH-HP-LAPTO\\SQLEXPRESS;Initial Catalog=projectdb;Integrated Security=True;";
        private string coverImagePath;
        private string pdfFilePath;

        public AddBook()
        {
            InitializeComponent();
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

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            // Remove event handlers temporarily
            priceTextbox.TextChanged -= PriceTextbox_TextChanged;
            quantityTextbox.TextChanged -= QuantityTextbox_TextChanged;

            if (string.IsNullOrEmpty(titleTextBox.Text) ||
                string.IsNullOrEmpty(genreTextbox.Text) ||
                string.IsNullOrEmpty(descriptionTextbox.Text) ||
                string.IsNullOrEmpty(priceTextbox.Text) ||
                string.IsNullOrEmpty(quantityTextbox.Text) ||
                string.IsNullOrEmpty(coverImagePath) ||
                string.IsNullOrEmpty(pdfFilePath))
            {
                MessageBox.Show("Please fill in all fields and choose appropriate files for cover page and PDF of the book.");

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

            Random rand = new Random();
            int randomId = rand.Next(1000,9999);
            string title = titleTextBox.Text;
            string genre = genreTextbox.Text;
            string description = descriptionTextbox.Text;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO books (BookId,Genre,Price,Quantity,Title,description,imagePath,pdfPath) VALUES (@BookId,@Genre,@Price,@Quantity,@Title,@Description,@imagePath,@pdfPath)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@BookId", randomId);
                    command.Parameters.AddWithValue("@Genre", genre);
                    command.Parameters.AddWithValue("@Price", price);
                    command.Parameters.AddWithValue("@Quantity", quantity);
                    command.Parameters.AddWithValue("@Title", title);
                    command.Parameters.AddWithValue("@Description", description);
                    command.Parameters.AddWithValue("@imagePath", coverImagePath);
                    command.Parameters.AddWithValue("@pdfPath", pdfFilePath);
                    command.ExecuteNonQuery();
                    MessageBox.Show("Book added successfully!");

                    // Clear textboxes
                    titleTextBox.Text = string.Empty;
                    genreTextbox.Text = string.Empty;
                    priceTextbox.Text = string.Empty;
                    quantityTextbox.Text = string.Empty;
                    descriptionTextbox.Text = string.Empty;

                    // Make coverButton and pdfButton visible again
                    coverButton.Visibility = Visibility.Visible;
                    pdfButton.Visibility = Visibility.Visible;
                    pdfAddedLabel.Visibility = Visibility.Collapsed;
                    coverPageAddedLabel.Visibility = Visibility.Collapsed;

                    // Clear image and pdf paths
                    coverImagePath = string.Empty;
                    pdfFilePath = string.Empty;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error adding book: " + ex.Message);
                }
            }

            // Reattach event handlers
            priceTextbox.TextChanged += PriceTextbox_TextChanged;
            quantityTextbox.TextChanged += QuantityTextbox_TextChanged;
        }



        private void coverButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
                Title = "Select a Cover Image"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                try
                {
                    // Validate file format (extension check is already done by the Filter property)
                    string[] validExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };
                    string fileExtension = System.IO.Path.GetExtension(filePath).ToLower();

                    if (validExtensions.Contains(fileExtension))
                    {
                        coverImagePath = filePath;
                        MessageBox.Show("Cover page successfully added.");

                        // Hide the coverButton
                        coverButton.Visibility = Visibility.Collapsed;

                        // Show the coverPageAddedLabel
                        coverPageAddedLabel.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        throw new InvalidOperationException("Invalid file format.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error selecting cover page: " + ex.Message);
                }
            }
        }


        private void pdfButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "PDF Files|*.pdf",
                Title = "Select a PDF File"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                try
                {
                    // Validate file format
                    string fileExtension = System.IO.Path.GetExtension(filePath).ToLower();

                    if (fileExtension == ".pdf")
                    {
                        pdfFilePath = filePath; // Save the PDF file path
                        MessageBox.Show("PDF successfully added.");

                        // Hide the pdfButton
                        pdfButton.Visibility = Visibility.Collapsed;

                        // Show the pdfAddedLabel
                        pdfAddedLabel.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        throw new InvalidOperationException("Invalid file format. Please select a PDF file.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error selecting PDF file: " + ex.Message);
                }
            }
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            bookSideActivity bookSide = new bookSideActivity();
            this.Close();
            bookSide.Show();
        }
    }
}