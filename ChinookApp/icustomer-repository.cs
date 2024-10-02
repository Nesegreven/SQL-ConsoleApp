using System.Collections.Generic;
using ChinookApp.Models;

namespace ChinookApp.Repositories
{
    /// <summary>
    /// Defines the contract for customer-related database operations.
    /// This interface outlines all the methods that any customer repository must implement.
    /// It provides a clear separation between the data access logic and the business logic.
    /// </summary>
    public interface ICustomerRepository
    {
        /// <summary>
        /// Retrieves all customers from the database.
        /// This method should:
        /// 1. Query the database for all customer records.
        /// 2. Convert each record into a Customer object.
        /// 3. Return a list of all Customer objects.
        /// </summary>
        /// <returns>A list of all customers in the database.</returns>
        List<Customer> GetAllCustomers();

        /// <summary>
        /// Retrieves a specific customer by their ID.
        /// This method should:
        /// 1. Query the database for a customer with the specified ID.
        /// 2. If found, convert the record into a Customer object.
        /// 3. If not found, return null.
        /// </summary>
        /// <param name="id">The unique identifier of the customer to retrieve.</param>
        /// <returns>The Customer object if found; otherwise, null.</returns>
        Customer GetCustomerById(int id);

        /// <summary>
        /// Searches for customers by name.
        /// This method should:
        /// 1. Query the database for customers whose first name or last name contains the search string.
        /// 2. Convert each matching record into a Customer object.
        /// 3. Return a list of matching Customer objects.
        /// </summary>
        /// <param name="name">The name (or part of the name) to search for.</param>
        /// <returns>A list of customers whose name matches the search criteria.</returns>
        List<Customer> GetCustomerByName(string name);

        /// <summary>
        /// Retrieves a page of customers from the database.
        /// This method implements pagination and should:
        /// 1. Calculate the correct offset based on the page number and limit.
        /// 2. Query the database for a specific "page" of customer records.
        /// 3. Convert each record into a Customer object.
        /// 4. Return a list of Customer objects for the specified page.
        /// </summary>
        /// <param name="limit">The maximum number of customers to return (page size).</param>
        /// <param name="offset">The number of customers to skip before starting to return results.</param>
        /// <returns>A list of customers for the specified page.</returns>
        List<Customer> GetCustomerPage(int limit, int offset);

        /// <summary>
        /// Adds a new customer to the database.
        /// This method should:
        /// 1. Validate the customer object to ensure all required fields are provided.
        /// 2. Insert a new record into the database using the provided customer information.
        /// 3. Return the ID of the newly created customer record.
        /// </summary>
        /// <param name="customer">The Customer object containing the information for the new customer.</param>
        /// <returns>The ID of the newly added customer.</returns>
        int AddCustomer(Customer customer);

        /// <summary>
        /// Updates an existing customer in the database.
        /// This method should:
        /// 1. Validate the customer object to ensure all required fields are provided.
        /// 2. Check if the customer with the given ID exists in the database.
        /// 3. If exists, update the customer record with the new information.
        /// 4. If not exists, throw an exception or handle the error appropriately.
        /// </summary>
        /// <param name="customer">The Customer object containing the updated information.</param>
        void UpdateCustomer(Customer customer);

        /// <summary>
        /// Retrieves the count of customers in each country, ordered by count descending.
        /// This method should:
        /// 1. Query the database to count customers grouped by country.
        /// 2. Order the results by the customer count in descending order.
        /// 3. Convert each result into a CustomerCountry object.
        /// 4. Return a list of CustomerCountry objects.
        /// </summary>
        /// <returns>A list of CustomerCountry objects containing country names and customer counts.</returns>
        List<CustomerCountry> GetCustomerCountByCountry();

        /// <summary>
        /// Retrieves customers sorted by their total spending, in descending order.
        /// This method should:
        /// 1. Query the database to calculate the total amount spent by each customer.
        /// 2. Order the results by the total spent in descending order.
        /// 3. Convert each result into a CustomerSpender object.
        /// 4. Return a list of CustomerSpender objects.
        /// </summary>
        /// <returns>A list of CustomerSpender objects containing customer information and total spent.</returns>
        List<CustomerSpender> GetTopSpenders();

        /// <summary>
        /// Retrieves the most popular genre(s) for a specific customer.
        /// This method should:
        /// 1. Query the database to find the genre(s) with the most purchases for the specified customer.
        /// 2. In case of a tie, return all genres with the highest purchase count.
        /// 3. Convert each result into a CustomerGenre object.
        /// 4. Return a list of CustomerGenre objects (usually containing just one item, unless there's a tie).
        /// </summary>
        /// <param name="customerId">The ID of the customer to analyze.</param>
        /// <returns>A list of CustomerGenre objects representing the customer's most popular genre(s).</returns>
        List<CustomerGenre> GetMostPopularGenreForCustomer(int customerId);
    }
}