using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Data.SqlClient;

namespace Bookstore_App
{
    /// <summary>
    /// Interaction logic for CustomerDiaglog.xaml
    /// </summary>
    public partial class CustomerDiaglog : Window
    {
        private const string connectionString = "Data Source=DEVELOPER-966\\SQLEXPRESS;Initial Catalog=projectdb;Integrated Security=True";
        String nameC;
        String emailC;
        String usernameC;
        public CustomerDiaglog(String name , String email, String username)
        {
            InitializeComponent();
             nameC = name;
             emailC = email;
             usernameC = username;
        }

        private void cityTextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void registerBtn_Click(object sender, RoutedEventArgs e)
        {// Retrieve data from input fields
            string city = cityTextBox.Text;
            string contact = contactTexbox.Text;
            string address = addressTextBox.Text;
            string gender = maleRadio.IsChecked == true ? "Male" : "Female";

            // Validate inputs
            if (string.IsNullOrEmpty(city) || string.IsNullOrEmpty(contact) || string.IsNullOrEmpty(address) ||
                string.IsNullOrEmpty(gender))
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            // Generate a random customerID
            Random rand = new Random();
            int customerID = rand.Next(10000, 99999); // Generate a random number between 10000 and 99999

            // Insert into the database
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO customer (customerID, name, email, username, city, contact, address, gender) VALUES (@CustomerID, @Name, @Email, @Username, @City, @Contact, @Address, @Gender)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@CustomerID", customerID);
                    command.Parameters.AddWithValue("@Name", nameC);  
                    command.Parameters.AddWithValue("@Email", emailC); 
                    command.Parameters.AddWithValue("@Username", usernameC); 
                    command.Parameters.AddWithValue("@City", city);
                    command.Parameters.AddWithValue("@Contact", contact);
                    command.Parameters.AddWithValue("@Address", address);
                    command.Parameters.AddWithValue("@Gender", gender);
                    command.ExecuteNonQuery();
                    MessageBox.Show("Customer added successfully!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error adding customer: " + ex.Message);
                }
            }
        }
    }
}
