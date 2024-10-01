using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using ChinookApp.Models;

namespace ChinookApp.Repositories
{
    /// <summary>
    /// Implements the ICustomerRepository interface to perform customer-related database operations.
    /// This class is responsible for all data access operations related to customers in the Chinook database.
    /// It uses ADO.NET with SqlClient to interact with the SQL Server database.
    /// </summary>
    public class CustomerRepository : ICustomerRepository
    {
        private readonly string _connectionString;

        /// <summary>
        /// Initializes a new instance of the CustomerRepository class.
        /// </summary>
        /// <param name="connectionString">The connection string to the Chinook database.</param>
        public CustomerRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Retrieves all customers from the database.
        /// </summary>
        /// <returns>A list of all customers.</returns>
        public List<Customer> GetAllCustomers()
        {
            var customers = new List<Customer>();

            // Create a new connection using the connection string
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // Create a command to select all customers
                var command = new SqlCommand("SELECT * FROM Customer", connection);

                // Execute the command and process the results
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        customers.Add(ReadCustomer(reader));
                    }
                }
            }

            return customers;
        }

        /// <summary>
        /// Retrieves a specific customer by their ID.
        /// </summary>
        /// <param name="id">The ID of the customer to retrieve.</param>
        /// <returns>The customer with the specified ID, or null if not found.</returns>
        public Customer GetCustomerById(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // Create a command to select a specific customer by ID
                var command = new SqlCommand("SELECT * FROM Customer WHERE CustomerId = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);

                // Execute the command and process the result
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadCustomer(reader);
                    }
                }
            }

            // Return null if no customer was found
            return null;
        }

        /// <summary>
        /// Searches for customers by name.
        /// </summary>
        /// <param name="name">The name (or part of the name) to search for.</param>
        /// <returns>A list of customers matching the search criteria.</returns>
        public List<Customer> GetCustomerByName(string name)
        {
            var customers = new List<Customer>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // Create a command to search for customers by name
                var command = new SqlCommand("SELECT * FROM Customer WHERE FirstName LIKE @Name OR LastName LIKE @Name", connection);
                command.Parameters.AddWithValue("@Name", $"%{name}%");

                // Execute the command and process the results
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        customers.Add(ReadCustomer(reader));
                    }
                }
            }

            return customers;
        }

        /// <summary>
        /// Retrieves a page of customers from the database.
        /// </summary>
        /// <param name="limit">The maximum number of customers to return.</param>
        /// <param name="offset">The number of customers to skip before starting to return results.</param>
        /// <returns>A list of customers for the specified page.</returns>
        public List<Customer> GetCustomerPage(int limit, int offset)
        {
            var customers = new List<Customer>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // Create a command to select a page of customers
                var command = new SqlCommand("SELECT * FROM Customer ORDER BY CustomerId OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY", connection);
                command.Parameters.AddWithValue("@Offset", offset);
                command.Parameters.AddWithValue("@Limit", limit);

                // Execute the command and process the results
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        customers.Add(ReadCustomer(reader));
                    }
                }
            }

            return customers;
        }

        /// <summary>
        /// Adds a new customer to the database.
        /// </summary>
        /// <param name="customer">The customer object to add.</param>
        /// <returns>The ID of the newly added customer.</returns>
        public int AddCustomer(Customer customer)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // Create a command to insert a new customer
                var command = new SqlCommand(@"
                    INSERT INTO Customer (FirstName, LastName, Company, Address, City, State, Country, PostalCode, Phone, Fax, Email, SupportRepId)
                    VALUES (@FirstName, @LastName, @Company, @Address, @City, @State, @Country, @PostalCode, @Phone, @Fax, @Email, @SupportRepId);
                    SELECT SCOPE_IDENTITY();", connection);

                AddCustomerParameters(command, customer);

                // Execute the command and get the new customer ID
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        /// <summary>
        /// Updates an existing customer in the database.
        /// </summary>
        /// <param name="customer">The customer object with updated information.</param>
        public void UpdateCustomer(Customer customer)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // Create a command to update an existing customer
                var command = new SqlCommand(@"
                    UPDATE Customer 
                    SET FirstName = @FirstName, LastName = @LastName, Company = @Company, Address = @Address, 
                        City = @City, State = @State, Country = @Country, PostalCode = @PostalCode, 
                        Phone = @Phone, Fax = @Fax, Email = @Email, SupportRepId = @SupportRepId
                    WHERE CustomerId = @CustomerId", connection);

                command.Parameters.AddWithValue("@CustomerId", customer.CustomerId);
                AddCustomerParameters(command, customer);

                // Execute the command
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Retrieves the count of customers in each country, ordered by count descending.
        /// </summary>
        /// <returns>A list of CustomerCountry objects containing country names and customer counts.</returns>
        public List<CustomerCountry> GetCustomerCountByCountry()
        {
            var customerCountries = new List<CustomerCountry>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // Create a command to get customer counts by country
                var command = new SqlCommand("SELECT Country, COUNT(*) as CustomerCount FROM Customer GROUP BY Country ORDER BY CustomerCount DESC", connection);

                // Execute the command and process the results
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        customerCountries.Add(new CustomerCountry
                        {
                            Country = reader.GetString(0),
                            CustomerCount = reader.GetInt32(1)
                        });
                    }
                }
            }

            return customerCountries;
        }

        /// <summary>
        /// Retrieves customers sorted by their total spending, in descending order.
        /// </summary>
        /// <returns>A list of CustomerSpender objects containing customer information and total spent.</returns>
        public List<CustomerSpender> GetTopSpenders()
        {
            var topSpenders = new List<CustomerSpender>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // Create a command to get top spenders
                var command = new SqlCommand(@"
                    SELECT c.CustomerId, c.FirstName + ' ' + c.LastName AS CustomerName, SUM(i.Total) AS TotalSpent
                    FROM Customer c
                    JOIN Invoice i ON c.CustomerId = i.CustomerId
                    GROUP BY c.CustomerId, c.FirstName, c.LastName
                    ORDER BY TotalSpent DESC", connection);

                // Execute the command and process the results
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        topSpenders.Add(new CustomerSpender
                        {
                            CustomerId = reader.GetInt32(0),
                            CustomerName = reader.GetString(1),
                            TotalSpent = reader.GetDecimal(2)
                        });
                    }
                }
            }

            return topSpenders;
        }

        /// <summary>
        /// Retrieves the most popular genre(s) for a specific customer.
        /// </summary>
        /// <param name="customerId">The ID of the customer to analyze.</param>
        /// <returns>A list of CustomerGenre objects representing the customer's most popular genre(s).</returns>
        public List<CustomerGenre> GetMostPopularGenreForCustomer(int customerId)
        {
            var customerGenres = new List<CustomerGenre>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // Create a command to get the most popular genre(s) for a customer
                var command = new SqlCommand(@"
                    WITH CustomerGenreCounts AS (
                        SELECT 
                            c.CustomerId,
                            c.FirstName + ' ' + c.LastName AS CustomerName,
                            g.Name AS GenreName,
                            COUNT(*) AS PurchaseCount,
                            ROW_NUMBER() OVER (PARTITION BY c.CustomerId ORDER BY COUNT(*) DESC) AS Rank
                        FROM Customer c
                        JOIN Invoice i ON c.CustomerId = i.CustomerId
                        JOIN InvoiceLine il ON i.InvoiceId = il.InvoiceId
                        JOIN Track t ON il.TrackId = t.TrackId
                        JOIN Genre g ON t.GenreId = g.GenreId
                        WHERE c.CustomerId = @CustomerId
                        GROUP BY c.CustomerId, c.FirstName, c.LastName, g.Name
                    )
                    SELECT CustomerId, CustomerName, GenreName, PurchaseCount
                    FROM CustomerGenreCounts
                    WHERE Rank = 1", connection);

                command.Parameters.AddWithValue("@CustomerId", customerId);

                // Execute the command and process the results
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        customerGenres.Add(new CustomerGenre
                        {
                            CustomerId = reader.GetInt32(0),
                            CustomerName = reader.GetString(1),
                            GenreName = reader.GetString(2),
                            PurchaseCount = reader.GetInt32(3)
                        });
                    }
                }
            }

            return customerGenres;
        }

        /// <summary>
        /// Reads a Customer object from a SqlDataReader.
        /// </summary>
        /// <param name="reader">The SqlDataReader containing customer data.</param>
        /// <returns>A Customer object populated with data from the reader.</returns>
        private Customer ReadCustomer(SqlDataReader reader)
        {
            return new Customer
            {
                CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                Company = reader.IsDBNull(reader.GetOrdinal("Company")) ? null : reader.GetString(reader.GetOrdinal("Company")),
                Address = reader.IsDBNull(reader.GetOrdinal("Address")) ? null : reader.GetString(reader.GetOrdinal("Address")),
                City = reader.IsDBNull(reader.GetOrdinal("City")) ? null : reader.GetString(reader.GetOrdinal("City")),
                State = reader.IsDBNull(reader.GetOrdinal("State")) ? null : reader.GetString(reader.GetOrdinal("State")),
                Country = reader.IsDBNull(reader.GetOrdinal("Country")) ? null : reader.GetString(reader.GetOrdinal("Country")),
                PostalCode = reader.IsDBNull(reader.GetOrdinal("PostalCode")) ? null : reader.GetString(reader.GetOrdinal("PostalCode")),
                Phone = reader.IsDBNull(reader.GetOrdinal("Phone")) ? null : reader.GetString(reader.GetOrdinal("Phone")),
                Fax = reader.IsDBNull(reader.GetOrdinal("Fax")) ? null : reader.GetString(reader.GetOrdinal("Fax")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                SupportRepId = reader.IsDBNull(reader.GetOrdinal("SupportRepId")) ? null : (int?)reader.GetInt32(reader.GetOrdinal("SupportRepId"))
            };
        }

        /// <summary>
        /// Adds customer parameters to a SqlCommand.
        /// </summary>
        /// <param name="command">The SqlCommand to add parameters to.</param>
        /// <param name="customer">The Customer object containing the parameter values.</param>
        private void AddCustomerParameters(SqlCommand command, Customer customer)
        {
            command.Parameters.AddWithValue("@FirstName", customer.FirstName);
            command.Parameters.AddWithValue("@LastName", customer.LastName);
            command.Parameters.AddWithValue("@Company", (object)customer.Company ?? DBNull.Value);
            command.Parameters.AddWithValue("@Address", (object)customer.Address ?? DBNull.Value);
            command.Parameters.AddWithValue("@City", (object)customer.City ?? DBNull.Value);
            command.Parameters.AddWithValue("@State", (object)customer.State ?? DBNull.Value);
            command.Parameters.AddWithValue("@Country", (object)customer.Country ?? DBNull.Value);
            command.Parameters.AddWithValue("@PostalCode", (object)customer.PostalCode ?? DBNull.Value);
            command.Parameters.AddWithValue("@Phone", (object)customer.Phone ?? DBNull.Value);
            command.Parameters.AddWithValue("@Fax", (object)customer.Fax ?? DBNull.Value);
            command.Parameters.AddWithValue("@Email", customer.Email);
            command.Parameters.AddWithValue("@SupportRepId", (object)customer.SupportRepId ?? DBNull.Value);
        }
    }
}