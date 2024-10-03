using System;
using System.Collections.Generic;
using ChinookApp.Repositories;
using ChinookApp.Models;

namespace ChinookApp
{
    /// <summary>
    /// The main application class that handles user interaction and business logic.
    /// This class is responsible for all data access operations related to customers in the Chinook database.
    /// It uses the ICustomerRepository interface to perform customer-related database operations.
    /// </summary>
    public class Application
    {
        private readonly ICustomerRepository _customerRepository;

        /// <summary>
        /// Initializes a new instance of the Application class.
        /// </summary>
        /// <param name="customerRepository">The customer repository to use for data operations.</param>
        public Application(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        /// <summary>
        /// Runs the main application loop, presenting a menu to the user and handling their choices.
        /// This method continuously displays options to the user and processes their input
        /// until they choose to exit the application.
        /// </summary>
        public void Run()
        {
            while (true)
            {
                Console.WriteLine("\nChoose an operation:");
                Console.WriteLine("1. List all customers");
                Console.WriteLine("2. Find customer by ID");
                Console.WriteLine("3. Find customer by name");
                Console.WriteLine("4. Add new customer");
                Console.WriteLine("5. Update customer");
                Console.WriteLine("6. Customer count by country");
                Console.WriteLine("7. Top spenders");
                Console.WriteLine("8. Most popular genre for a customer");
                Console.WriteLine("9. Get page of customers");
                Console.WriteLine("10. Exit");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ListAllCustomers();
                        break;
                    case "2":
                        FindCustomerById();
                        break;
                    case "3":
                        FindCustomerByName();
                        break;
                    case "4":
                        AddNewCustomer();
                        break;
                    case "5":
                        UpdateCustomer();
                        break;
                    case "6":
                        CustomerCountByCountry();
                        break;
                    case "7":
                        TopSpenders();
                        break;
                    case "8":
                        MostPopularGenreForCustomer();
                        break;
                    case "9":
                        GetCustomerPage();
                        break;
                    case "10":
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        /// <summary>
        /// Lists all customers in the database.
        /// This method:
        /// 1. Retrieves all customers from the repository.
        /// 2. Displays each customer's ID, first name, last name, country, postal code, phone number, and email.
        /// 3. Shows the total count of customers at the end.
        /// </summary>
        private void ListAllCustomers()
        {
            var customers = _customerRepository.GetAllCustomers();

            Console.WriteLine("\nAll Customers:");
            Console.WriteLine($"{"ID",-3} | {"First Name",-15} | {"Last Name",-15} | {"Country",-15} | {"Postal Code",-13} | {"Phone",-19} | {"Email"}");
            Console.WriteLine(new string('-', 100));

            foreach (var customer in customers)
            {
                Console.WriteLine($"{customer.Id,-3} | {customer.FirstName,-15} | {customer.LastName,-15} | " +
                                  $"{(customer.Country ?? "N/A"),-15} | {(customer.PostalCode ?? "N/A"),-13} | " +
                                  $"{(customer.Phone ?? "N/A"),-19} | {customer.Email}");
            }

            Console.WriteLine($"\nTotal customers: {customers.Count}");
        }

        /// <summary>
        /// Finds a customer by their ID.
        /// This method:
        /// 1. Prompts the user to enter a customer ID.
        /// 2. Attempts to retrieve the customer with the given ID.
        /// 3. Displays the customer's information if found, or a "not found" message if not found.
        /// </summary>
        private void FindCustomerById()
        {
            Console.Write("Enter customer ID: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var customer = _customerRepository.GetCustomerById(id);
                if (customer != null)
                {
                    Console.WriteLine("\nCustomer Details:");
                    Console.WriteLine($"{"ID",-3} | {"First Name",-15} | {"Last Name",-15} | {"Country",-15} | {"Postal Code",-13} | {"Phone",-19} | {"Email"}");
                    Console.WriteLine(new string('-', 100));
                    Console.WriteLine($"{customer.Id,-3} | {customer.FirstName,-15} | {customer.LastName,-15} | " +
                                      $"{(customer.Country ?? "N/A"),-15} | {(customer.PostalCode ?? "N/A"),-13} | " +
                                      $"{(customer.Phone ?? "N/A"),-19} | {customer.Email}");
                }
                else
                {
                    Console.WriteLine("Customer not found.");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID. Please enter a number.");
            }
        }

        /// <summary>
        /// Finds customers by their name.
        /// This method:
        /// 1. Prompts the user to enter a customer name.
        /// 2. Searches for customers with matching names.
        /// 3. Displays the information of all matching customers.
        /// </summary>
        private void FindCustomerByName()
        {
            Console.Write("Enter customer name: ");
            var name = Console.ReadLine();
            var customers = _customerRepository.GetCustomerByName(name);

            if (customers.Any())
            {
                Console.WriteLine("\nMatching Customers:");
                Console.WriteLine($"{"ID",-3} | {"First Name",-15} | {"Last Name",-15} | {"Country",-15} | {"Postal Code",-13} | {"Phone",-19} | {"Email"}");
                Console.WriteLine(new string('-', 100));

                foreach (var customer in customers)
                {
                    Console.WriteLine($"{customer.Id,-3} | {customer.FirstName,-15} | {customer.LastName,-15} | " +
                                      $"{(customer.Country ?? "N/A"),-15} | {(customer.PostalCode ?? "N/A"),-13} | " +
                                      $"{(customer.Phone ?? "N/A"),-19} | {customer.Email}");
                }
            }
            else
            {
                Console.WriteLine("No customers found with that name.");
            }
        }


        /// <summary>
        /// Adds a new customer to the database.
        /// This method:
        /// 1. Prompts the user to enter customer details.
        /// 2. Validates the input to ensure required fields are not null or empty.
        /// 3. Handles optional fields consistently, setting them to null if input is empty.
        /// 4. Creates a new Customer object with the entered data.
        /// 5. Adds the new customer to the database and displays the new customer's ID.
        /// </summary>
        private void AddNewCustomer()
        {
            var customer = new Customer();

            while (string.IsNullOrWhiteSpace(customer.FirstName))
            {
                Console.Write("First Name (required): ");
                customer.FirstName = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(customer.FirstName))
                {
                    Console.WriteLine("First Name is required. Please enter a valid name.");
                }
            }

            while (string.IsNullOrWhiteSpace(customer.LastName))
            {
                Console.Write("Last Name (required): ");
                customer.LastName = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(customer.LastName))
                {
                    Console.WriteLine("Last Name is required. Please enter a valid name.");
                }
            }

            while (string.IsNullOrWhiteSpace(customer.Email))
            {
                Console.Write("Email (required): ");
                customer.Email = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(customer.Email))
                {
                    Console.WriteLine("Email is required. Please enter a valid email address.");
                }
                // Note: You might want to add email format validation here
            }

            Console.Write("Country (optional): ");
            var input = Console.ReadLine()?.Trim();
            customer.Country = string.IsNullOrWhiteSpace(input) ? null : input;

            Console.Write("Postal Code (optional): ");
            input = Console.ReadLine()?.Trim();
            customer.PostalCode = string.IsNullOrWhiteSpace(input) ? null : input;

            Console.Write("Phone (optional): ");
            input = Console.ReadLine()?.Trim();
            customer.Phone = string.IsNullOrWhiteSpace(input) ? null : input;

            try
            {
                var id = _customerRepository.AddCustomer(customer);
                Console.WriteLine($"New customer added with ID: {id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding customer: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates an existing customer in the database.
        /// This method:
        /// 1. Prompts the user to enter the ID of the customer to update.
        /// 2. Retrieves the customer with the given ID.
        /// 3. If found, prompts for updated information and applies the changes, ensuring required fields are not set to null.
        /// 4. If not found, displays a "not found" message.
        /// </summary>
        private void UpdateCustomer()
        {
            Console.Write("Enter customer ID to update: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var customer = _customerRepository.GetCustomerById(id);
                if (customer != null)
                {
                    bool isUpdated = false;

                    Console.Write($"New First Name ({customer.FirstName}): ");
                    var input = Console.ReadLine()?.Trim();
                    if (!string.IsNullOrWhiteSpace(input))
                    {
                        customer.FirstName = input;
                        isUpdated = true;
                    }

                    Console.Write($"New Last Name ({customer.LastName}): ");
                    input = Console.ReadLine()?.Trim();
                    if (!string.IsNullOrWhiteSpace(input))
                    {
                        customer.LastName = input;
                        isUpdated = true;
                    }

                    Console.Write($"New Email ({customer.Email}): ");
                    input = Console.ReadLine()?.Trim();
                    if (!string.IsNullOrWhiteSpace(input))
                    {
                        customer.Email = input;
                        isUpdated = true;
                    }

                    Console.Write($"New Country ({customer.Country ?? "N/A"}): ");
                    input = Console.ReadLine()?.Trim();
                    if (input != null)
                    {
                        customer.Country = string.IsNullOrWhiteSpace(input) ? null : input;
                        isUpdated = true;
                    }

                    Console.Write($"New Postal Code ({customer.PostalCode ?? "N/A"}): ");
                    input = Console.ReadLine()?.Trim();
                    if (input != null)
                    {
                        customer.PostalCode = string.IsNullOrWhiteSpace(input) ? null : input;
                        isUpdated = true;
                    }

                    Console.Write($"New Phone ({customer.Phone ?? "N/A"}): ");
                    input = Console.ReadLine()?.Trim();
                    if (input != null)
                    {
                        customer.Phone = string.IsNullOrWhiteSpace(input) ? null : input;
                        isUpdated = true;
                    }

                    if (isUpdated)
                    {
                        try
                        {
                            _customerRepository.UpdateCustomer(customer);
                            Console.WriteLine("Customer updated successfully.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error updating customer: {ex.Message}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("No changes were made to the customer.");
                    }
                }
                else
                {
                    Console.WriteLine("Customer not found.");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID. Please enter a number.");
            }
        }

        /// <summary>
        /// Displays the count of customers in each country.
        /// This method:
        /// 1. Retrieves the customer count by country from the repository.
        /// 2. Displays each country and its corresponding customer count in descending order.
        /// </summary>
        private void CustomerCountByCountry()
        {
            var customerCountries = _customerRepository.GetCustomerCountByCountry();

            Console.WriteLine("\nCustomer Count by Country (Descending Order):");
            Console.WriteLine($"{"Country",-25} | {"Customer Count",-15}");
            Console.WriteLine(new string('-', 40));

            foreach (var cc in customerCountries)
            {
                Console.WriteLine($"{cc.Country,-22} | {cc.CustomerCount,5}");
            }
        }

        /// <summary>
        /// Displays the top spenders among customers.
        /// This method:
        /// 1. Retrieves the list of top spenders from the repository.
        /// 2. Displays each customer's ID, name, and total amount spent, sorted by spending in descending order.
        /// </summary>
        private void TopSpenders()
        {
            var topSpenders = _customerRepository.GetTopSpenders();
            Console.WriteLine("\nTop Spenders:");
            Console.WriteLine($"{"Customer ID",-11} | {"Customer Name",-27} | {"Amount Spent",-12}");
            Console.WriteLine(new string('-', 60));
            foreach (var spender in topSpenders)
            {
                Console.WriteLine($"{spender.CustomerId,-11} | {spender.CustomerName,-27} | {spender.TotalSpent,-12:C}");
            }
        }

        /// <summary>
        /// Displays the most popular genre(s) for a specific customer.
        /// This method:
        /// 1. Prompts the user to enter a customer ID.
        /// 2. Retrieves the most popular genre(s) for the specified customer.
        /// 3. Displays the customer's name, the most popular genre(s), and the number of purchases in that genre.
        /// </summary>
        private void MostPopularGenreForCustomer()
        {
            Console.Write("Enter customer ID: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var popularGenres = _customerRepository.GetMostPopularGenreForCustomer(id);
                foreach (var genre in popularGenres)
                {
                    Console.WriteLine($"Customer: {genre.CustomerName}");
                    Console.WriteLine($"Most popular genre: {genre.GenreName}");
                    Console.WriteLine($"Purchases in this genre: {genre.PurchaseCount}");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID. Please enter a number.");
            }
        }

        /// <summary>
        /// Retrieves and displays a page of customers from the database.
        /// This method:
        /// 1. Prompts the user to enter the page size (limit) and page number.
        /// 2. Validates the input to ensure positive values for both limit and page number.
        /// 3. Calculates the offset based on the page number and limit.
        /// 4. Retrieves the specified page of customers from the repository.
        /// 5. Displays the customer information for the retrieved page, including all relevant fields.
        /// </summary>
        private void GetCustomerPage()
        {
            int limit;
            while (true)
            {
                Console.Write("Enter page size (limit): ");
                if (int.TryParse(Console.ReadLine(), out limit) && limit > 0)
                {
                    break;
                }
                Console.WriteLine("Invalid input. Please enter a positive number for the page size.");
            }

            int page;
            while (true)
            {
                Console.Write("Enter page number: ");
                if (int.TryParse(Console.ReadLine(), out page) && page > 0)
                {
                    break;
                }
                Console.WriteLine("Invalid input. Please enter a positive number for the page number.");
            }

            int offset = (page - 1) * limit;
            var customers = _customerRepository.GetCustomerPage(limit, offset);

            if (customers.Count == 0)
            {
                Console.WriteLine($"\nNo customers found on page {page}.");
                return;
            }

            Console.WriteLine($"\nPage {page} (Limit: {limit}, Offset: {offset})");
            Console.WriteLine($"{"ID",-3} | {"First Name",-15} | {"Last Name",-15} | {"Country",-15} | {"Postal Code",-13} | {"Phone",-19} | {"Email"}");
            Console.WriteLine(new string('-', 100));

            foreach (var customer in customers)
            {
                Console.WriteLine($"{customer.Id,-3} | {customer.FirstName,-15} | {customer.LastName,-15} | " +
                                  $"{(customer.Country ?? "N/A"),-15} | {(customer.PostalCode ?? "N/A"),-13} | " +
                                  $"{(customer.Phone ?? "N/A"),-19} | {customer.Email}");
            }

            Console.WriteLine($"\nShowing {customers.Count} customers on page {page}");

        }
    }
}