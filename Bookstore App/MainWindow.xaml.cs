using System;
using System.Data.SqlClient;
using System.Windows;

namespace Bookstore_App
{
    public partial class MainWindow : Window
    {
        private const string ConnectionString = "Data Source=DANISH-HP-LAPTO\\SQLEXPRESS;Initial Catalog=projectdb;Integrated Security=True;";

        public MainWindow()
        {
            InitializeComponent();
            loginButton.Click += loginButton_Click; // Attach event listener
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

            // Determine the table name based on radio button selection
            string tableName = isAdminChecked ? "admins" : "customer";

            // Check if username already exists
            if (UsernameExists(username, tableName))
            {
                MessageBox.Show("Username already exists. Please choose a different username.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Insert data into the appropriate table
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    // Prepare SQL statement
                    string sql = $"INSERT INTO {tableName} (name, email, username, password) " +
                                 "VALUES (@Name, @Email, @Username, @Password)";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        // Add parameters to prevent SQL injection
                        command.Parameters.AddWithValue("@Name", name);
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@Password", password);

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
            bool isUserChecked = loginUserRadioButton.IsChecked ?? false;

            if (!isAdminChecked && !isUserChecked)
            {
                MessageBox.Show("Please select a role.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string tableName = isAdminChecked ? "admins" : "customer";

            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    // Prepare SQL statement
                    string sql = $"SELECT COUNT(*) FROM {tableName} WHERE username = @Username AND password = @Password";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@Password", password);

                        int count = (int)command.ExecuteScalar();

                        if (count > 0)
                        {
                            MessageBox.Show("Login successful!");
                            // Add code to navigate to the appropriate page or perform other actions after successful login
                        }
                        else
                        {
                            MessageBox.Show("Invalid username or password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool UsernameExists(string username, string tableName)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string sql = $"SELECT 1 FROM {tableName} WHERE username = @Username";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            return reader.HasRows;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while checking username: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return true; // Assume username exists to prevent signup due to unknown error
            }
        }

        private bool IsValidEmail(string email)
        {
            return email.Contains("@") && email.Contains(".");
        }
    }
}
