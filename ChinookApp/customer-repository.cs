using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using ChinookApp.Models;

namespace ChinookApp.Repositories
{
    /// <summary>
    /// Implements the ICustomerRepository interface to perform customer-related database operations.
    /// This class is responsible for all data access operations related to customers in the Chinook database.
    /// It uses SqlClient to interact with the SQL Server database.
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
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand(
                    "SELECT CustomerId, FirstName, LastName, Country, PostalCode, Phone, Email FROM Customer",
                    connection);
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
                var command = new SqlCommand(
                    "SELECT CustomerId, FirstName, LastName, Country, PostalCode, Phone, Email FROM Customer WHERE CustomerId = @Id",
                    connection);
                command.Parameters.AddWithValue("@Id", id);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadCustomer(reader);
                    }
                }
            }
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
                var command = new SqlCommand(
                    "SELECT CustomerId, FirstName, LastName, Country, PostalCode, Phone, Email FROM Customer WHERE FirstName LIKE @Name OR LastName LIKE @Name",
                    connection);
                command.Parameters.AddWithValue("@Name", $"%{name}%");
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
                var command = new SqlCommand(
                    "SELECT CustomerId, FirstName, LastName, Country, PostalCode, Phone, Email FROM Customer ORDER BY CustomerId OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY",
                    connection);
                command.Parameters.AddWithValue("@Offset", offset);
                command.Parameters.AddWithValue("@Limit", limit);
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
                var command = new SqlCommand(@"
                    INSERT INTO Customer (FirstName, LastName, Country, PostalCode, Phone, Email)
                    VALUES (@FirstName, @LastName, @Country, @PostalCode, @Phone, @Email);
                    SELECT SCOPE_IDENTITY();", connection);

                AddCustomerParameters(command, customer);

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
                var command = new SqlCommand(@"
                    UPDATE Customer 
                    SET FirstName = @FirstName, 
                        LastName = @LastName, 
                        Country = @Country, 
                        PostalCode = @PostalCode, 
                        Phone = @Phone, 
                        Email = @Email
                    WHERE CustomerId = @Id", connection);

                command.Parameters.AddWithValue("@Id", customer.Id);
                AddCustomerParameters(command, customer);

                command.ExecuteNonQuery();
            }
        }


        /// <summary>
        /// Retrieves the count of customers in each country, ordered by count descending.
        /// </summary>
        /// <returns>A list of CustomerCountry objects containing country names (or "Unknown" for NULL) and customer counts.</returns>
        public List<CustomerCountry> GetCustomerCountByCountry()
        {
            var customerCountries = new List<CustomerCountry>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand(
                    @"SELECT COALESCE(Country, 'Unknown') as Country, COUNT(*) as CustomerCount 
              FROM Customer 
              GROUP BY Country 
              ORDER BY CustomerCount DESC",
                    connection);
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
                var command = new SqlCommand(@"
                    SELECT c.CustomerId, c.FirstName + ' ' + c.LastName AS CustomerName, SUM(i.Total) AS TotalSpent
                    FROM Customer c
                    JOIN Invoice i ON c.CustomerId = i.CustomerId
                    GROUP BY c.CustomerId, c.FirstName, c.LastName
                    ORDER BY TotalSpent DESC", connection);
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
                // This query identifies the most frequently purchased music genre for a specific customer.
                // It does this by:
                // 1. Calculating the number of purchases for each genre by the customer.
                // 2. Ranking these genres based on the purchase count, with the highest count receiving the top rank.
                // 3. Selecting the genre with the highest purchase count for the customer.
                command.Parameters.AddWithValue("@CustomerId", customerId);
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
                Id = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                Country = reader.IsDBNull(reader.GetOrdinal("Country")) ? null : reader.GetString(reader.GetOrdinal("Country")),
                PostalCode = reader.IsDBNull(reader.GetOrdinal("PostalCode")) ? null : reader.GetString(reader.GetOrdinal("PostalCode")),
                Phone = reader.IsDBNull(reader.GetOrdinal("Phone")) ? null : reader.GetString(reader.GetOrdinal("Phone")),
                Email = reader.GetString(reader.GetOrdinal("Email"))
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
            command.Parameters.AddWithValue("@Country", (object)customer.Country ?? DBNull.Value);
            command.Parameters.AddWithValue("@PostalCode", (object)customer.PostalCode ?? DBNull.Value);
            command.Parameters.AddWithValue("@Phone", (object)customer.Phone ?? DBNull.Value);
            command.Parameters.AddWithValue("@Email", customer.Email);
        }
    }
}