using System;
using System.Data.SqlClient;
using System.Windows;

namespace Bookstore_App
{
    public partial class MainWindow : Window
    {
        private const string ConnectionString = "Data Source=DANISH-HP-LAPTO\\SQLEXPRESS;Initial Catalog=projectdb;Integrated Security=True;";
        string name;
        string email;
        string usernamee;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void signupButton_Click(object sender, RoutedEventArgs e)
        {
            // Get user input
            string name = nameTextBox.Text;
            string email = emailTextBox.Text;
            string username = usernameTextBox.Text;
            string password = passwordPasswordBox.Password;
            bool isAdminChecked = adminRadioButton.IsChecked ?? false;
            bool isUserChecked = userRadioButton.IsChecked ?? false;
            string role = isAdminChecked ? "Admin" : isUserChecked ? "Customer" : "";

            // Validate input
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please fill in all fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!isAdminChecked && !isUserChecked)
            {
                MessageBox.Show("Please select a role.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!IsValidEmail(email))
            {
                MessageBox.Show("Please enter a valid email address.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Generate a random 4-digit integer for userID
            Random rand = new Random();
            int userID = rand.Next(1000, 9999);

            // Check if username already exists
            if (UsernameExists(username))
            {
                MessageBox.Show("Username already exists. Please choose a different username.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Insert data into the users table
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    // Prepare SQL statement
                    string sql = "INSERT INTO users (userID, name, email, username, password, role) " +
                                 "VALUES (@UserID, @Name, @Email, @Username, @Password, @Role)";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        // Add parameters to prevent SQL injection
                        command.Parameters.AddWithValue("@UserID", userID);
                        command.Parameters.AddWithValue("@Name", name);
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@Password", password);
                        command.Parameters.AddWithValue("@Role", role);

                        // Execute SQL statement
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Sign up successful!");
                        }
                        else
                        {
                            MessageBox.Show("Sign up failed!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = loginUsernameTextBox.Text;
            string password = loginPasswordPasswordBox.Password;
            bool isAdminChecked = loginAdminRadioButton.IsChecked ?? false;
            bool isCustomerChecked = loginUserRadioButton.IsChecked ?? false;

            if (!isAdminChecked && !isCustomerChecked)
            {
                MessageBox.Show("Please select a role.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    // Prepare SQL statement
                    string sql = "SELECT COUNT(*), role, name, email FROM users WHERE username = @Username AND password = @Password GROUP BY role, name, email";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@Password", password);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int count = reader.GetInt32(0);
                                string role = reader.GetString(1).Trim(); // Ensure to trim any whitespace characters
                                string name = reader.IsDBNull(2) ? string.Empty : reader.GetString(2).Trim();
                                string email = reader.IsDBNull(3) ? string.Empty : reader.GetString(3).Trim();

                                if (count > 0)
                                {
                                    if (role == "Admin" && isAdminChecked)
                                    {
                                        MessageBox.Show("Login successful!");

                                        // Open AdminMenu window
                                        AdminMenu adminMenu = new AdminMenu();
                                        adminMenu.Show();
                                        // Close MainWindow
                                        this.Close();
                                    }
                                    else if (role == "Customer" && isCustomerChecked)
                                    {
                                        MessageBox.Show("Login successful!");

                                        // Open CustomerDiaglog window
                                        CustomerDiaglog customerDiaglog = new CustomerDiaglog(name, email, username);
                                        customerDiaglog.Show();
                                        // Close MainWindow
                                        this.Close();
                                    }
                                    else
                                    {
                                        MessageBox.Show("Role mismatch. Please select the correct role.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Invalid username or password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                            else
                            {
                                MessageBox.Show("Invalid username or password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        // Helper method to validate email address
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        // Helper method to check if username already exists
        private bool UsernameExists(string username)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string sql = "SELECT COUNT(*) FROM users WHERE username = @Username";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        int count = (int)command.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return true; // Assume username exists if there's an error checking
            }
        }
    }
}
