using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ChinookApp.Repositories;
using ChinookApp.Models;

namespace ChinookApp
{
    class Program
    {
        /// <summary>
        /// The main entry point for the application. This method:
        /// 1. Sets up the configuration and dependency injection.
        /// 2. Creates an instance of the CustomerRepository.
        /// 3. Runs the main menu loop, allowing the user to interact with the database.
        /// </summary>
        /// <param name="args">Command line arguments (not used in this application)</param>
        static void Main(string[] args)
        {
            // Set up configuration
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            // Set up dependency injection
            var services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(configuration);
            services.AddScoped<ICustomerRepository, CustomerRepository>(sp =>
                new CustomerRepository(configuration.GetConnectionString("ChinookDatabase")));

            var serviceProvider = services.BuildServiceProvider();

            // Get an instance of the CustomerRepository
            var customerRepository = serviceProvider.GetRequiredService<ICustomerRepository>();

            // Main menu loop
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
                        ListAllCustomers(customerRepository);
                        break;
                    case "2":
                        FindCustomerById(customerRepository);
                        break;
                    case "3":
                        FindCustomerByName(customerRepository);
                        break;
                    case "4":
                        AddNewCustomer(customerRepository);
                        break;
                    case "5":
                        UpdateCustomer(customerRepository);
                        break;
                    case "6":
                        CustomerCountByCountry(customerRepository);
                        break;
                    case "7":
                        TopSpenders(customerRepository);
                        break;
                    case "8":
                        MostPopularGenreForCustomer(customerRepository);
                        break;
                    case "9":
                        GetCustomerPage(customerRepository);
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
        /// Lists all customers in the database. This method:
        /// 1. Retrieves all customers from the repository.
        /// 2. Displays each customer's ID, first name, last name, country, postal code, phone number, and email.
        /// </summary>
        /// <param name="repository">The customer repository used to retrieve customer data</param>
        static void ListAllCustomers(ICustomerRepository repository)
        {
            // Retrieve all customers
            var customers = repository.GetAllCustomers();

            // Display header
            Console.WriteLine("\nAll Customers:\n");

            // Display each customer's information
            foreach (var customer in customers)
            {
                Console.WriteLine($"Customer ID:{customer.Id}");
                Console.WriteLine($"Customer name: {customer.FirstName} {customer.LastName}");
                Console.WriteLine($"Email: {customer.Email}");
                Console.WriteLine($"Country: {customer.Country}");
                Console.WriteLine($"Postal Code: {customer.PostalCode}");
                Console.WriteLine($"Phone: {customer.Phone}");
                Console.WriteLine(new string('-', 80));  // Separator line between entries
            }
        }

        /// <summary>
        /// Finds a customer by their ID. This method:
        /// 1. Prompts the user to enter a customer ID.
        /// 2. Attempts to retrieve the customer with the given ID.
        /// 3. Displays the customer's information if found, or a "not found" message if not found.
        /// </summary>
        /// <param name="repository">The customer repository used to retrieve customer data</param>
        static void FindCustomerById(ICustomerRepository repository)
        {
            Console.Write("Enter customer ID: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                // Attempt to retrieve the customer
                var customer = repository.GetCustomerById(id);
                if (customer != null)
                {
                    // Customer found, display information
                    Console.WriteLine($"Customer found: {customer.FirstName} {customer.LastName}");
                    Console.WriteLine($"Email: {customer.Email}");
                    Console.WriteLine($"Country: {customer.Country}");
                    Console.WriteLine($"Postal Code: {customer.PostalCode}");
                    Console.WriteLine($"Phone: {customer.Phone}");
                }
                else
                {
                    // Customer not found
                    Console.WriteLine("Customer not found.");
                }
            }
            else
            {
                // Invalid input
                Console.WriteLine("Invalid ID. Please enter a number.");
            }
        }

        /// <summary>
        /// Finds customers by their name. This method:
        /// 1. Prompts the user to enter a customer name.
        /// 2. Searches for customers with matching names.
        /// 3. Displays the information of all matching customers.
        /// </summary>
        /// <param name="repository">The customer repository used to retrieve customer data</param>
        static void FindCustomerByName(ICustomerRepository repository)
        {
            Console.Write("Enter customer name: ");
            var name = Console.ReadLine();

            // Search for customers with matching names
            var customers = repository.GetCustomerByName(name);

            // Display matching customers
            foreach (var customer in customers)
            {
                Console.WriteLine($"Customer ID:{customer.Id}");
                Console.WriteLine($"Customer name: {customer.FirstName} {customer.LastName}");
                Console.WriteLine($"Email: {customer.Email}");
                Console.WriteLine($"Country: {customer.Country}");
                Console.WriteLine($"Postal Code: {customer.PostalCode}");
                Console.WriteLine($"Phone: {customer.Phone}");
            }
        }

        /// <summary>
        /// Adds a new customer to the database. This method:
        /// 1. Prompts the user to enter customer details.
        /// 2. Creates a new Customer object with the entered data.
        /// 3. Adds the new customer to the database and displays the new customer's ID.
        /// </summary>
        /// <param name="repository">The customer repository used to add the new customer</param>
        static void AddNewCustomer(ICustomerRepository repository)
        {
            var customer = new Customer();

            // Prompt for customer details
            Console.Write("First Name: ");
            customer.FirstName = Console.ReadLine();
            Console.Write("Last Name: ");
            customer.LastName = Console.ReadLine();
            Console.Write("Email: ");
            customer.Email = Console.ReadLine();
            Console.Write("Country: ");
            customer.Country = Console.ReadLine();
            Console.Write("Postal Code: ");
            customer.PostalCode = Console.ReadLine();
            Console.Write("Phone: ");
            customer.Phone = Console.ReadLine();

            // Add the new customer to the database
            var id = repository.AddCustomer(customer);
            Console.WriteLine($"New customer added with ID: {id}");
        }

        /// <summary>
        /// Updates an existing customer in the database. This method:
        /// 1. Prompts the user to enter the ID of the customer to update.
        /// 2. Retrieves the customer with the given ID.
        /// 3. If found, prompts for updated information and applies the changes.
        /// 4. If not found, displays a "not found" message.
        /// </summary>
        /// <param name="repository">The customer repository used to update customer data</param>
        static void UpdateCustomer(ICustomerRepository repository)
        {
            Console.Write("Enter customer ID to update: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                // Attempt to retrieve the customer
                var customer = repository.GetCustomerById(id);
                if (customer != null)
                {
                    // Prompt for updated information
                    Console.Write($"New First Name ({customer.FirstName}): ");
                    var input = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(input)) customer.FirstName = input;

                    Console.Write($"New Last Name ({customer.LastName}): ");
                    input = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(input)) customer.LastName = input;

                    Console.Write($"New Email ({customer.Email}): ");
                    input = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(input)) customer.Email = input;

                    Console.Write($"New Country ({customer.Country}): ");
                    input = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(input)) customer.Country = input;

                    Console.Write($"New Postal Code ({customer.PostalCode}): ");
                    input = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(input)) customer.PostalCode = input;

                    Console.Write($"New Phone ({customer.Phone}): ");
                    input = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(input)) customer.Phone = input;

                    // Update the customer in the database
                    repository.UpdateCustomer(customer);
                    Console.WriteLine("Customer updated successfully.");
                }
                else
                {
                    // Customer not found
                    Console.WriteLine("Customer not found.");
                }
            }
            else
            {
                // Invalid input
                Console.WriteLine("Invalid ID. Please enter a number.");
            }
        }

        /// <summary>
        /// Displays the count of customers in each country, ordered by count descending.
        /// This method:
        /// 1. Retrieves the customer count by country from the repository.
        /// 2. Displays each country and its corresponding customer count in descending order.
        /// </summary>
        /// <param name="repository">The customer repository used to retrieve customer data</param>
        /// <summary>
        /// Displays the count of customers in each country, ordered by count descending.
        /// This method:
        /// 1. Retrieves the customer count by country from the repository.
        /// 2. Displays each country (or "Unknown" for NULL values) and its corresponding customer count in descending order.
        /// </summary>
        /// <param name="repository">The customer repository used to retrieve customer data</param>
        static void CustomerCountByCountry(ICustomerRepository repository)
        {
            // Retrieve customer count by country
            var customerCountries = repository.GetCustomerCountByCountry();

            // Display header
            Console.WriteLine("\nCustomer Count by Country (Descending Order):");
            Console.WriteLine("Country                 | Customer Count");
            Console.WriteLine(new string('-', 40));  // Separator line

            // Display each country and its customer count
            foreach (var cc in customerCountries)
            {
                Console.WriteLine($"{cc.Country,-22} | {cc.CustomerCount,5}");
            }
        }

        /// <summary>
        /// Displays the top spenders among customers, ordered by total spent descending.
        /// </summary>
        /// <param name="repository">The customer repository used to retrieve customer data</param>
        static void TopSpenders(ICustomerRepository repository)
        {
            var topSpenders = repository.GetTopSpenders();
            foreach (var spender in topSpenders)
            {
                Console.WriteLine($"Customer ID: {spender.CustomerId,-10} Customer name: {spender.CustomerName,-25} Amount spent: {spender.TotalSpent,-10:C}");


            }
        }

        /// <summary>
        /// Displays the most popular genre(s) for a specific customer.
        /// </summary>
        /// <param name="repository">The customer repository used to retrieve customer data</param>
        static void MostPopularGenreForCustomer(ICustomerRepository repository)
        {
            Console.Write("Enter customer ID: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var popularGenres = repository.GetMostPopularGenreForCustomer(id);
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
        /// 2. Calculates the offset based on the page number and limit.
        /// 3. Retrieves the specified page of customers from the repository.
        /// 4. Displays the customer information for the retrieved page, including all relevant fields.
        /// </summary>
        /// <param name="repository">The customer repository used to retrieve customer data</param>
        static void GetCustomerPage(ICustomerRepository repository)
        {
            // Prompt for page size
            Console.Write("Enter page size (limit): ");
            if (!int.TryParse(Console.ReadLine(), out int limit))
            {
                Console.WriteLine("Invalid limit. Please enter a number.");
                return;
            }

            // Prompt for page number
            Console.Write("Enter page number: ");
            if (!int.TryParse(Console.ReadLine(), out int page))
            {
                Console.WriteLine("Invalid page number. Please enter a number.");
                return;
            }

            // Calculate offset
            int offset = (page - 1) * limit;

            // Retrieve the specified page of customers
            var customers = repository.GetCustomerPage(limit, offset);

            // Display results
            Console.WriteLine($"\nPage {page} (Limit: {limit}, Offset: {offset})");
            Console.WriteLine("ID | First Name     | Last Name      | Country        | Postal Code  | Phone              | Email");
            Console.WriteLine(new string('-', 100));  // Separator line

            foreach (var customer in customers)
            {
                Console.WriteLine($"{customer.Id,-3}| {customer.FirstName,-15}| {customer.LastName,-15}| " +
                                  $"{(customer.Country ?? "N/A"),-15}| {(customer.PostalCode ?? "N/A"),-13}| " +
                                  $"{(customer.Phone ?? "N/A"),-19}| {customer.Email}");
            }

            Console.WriteLine($"\nShowing {customers.Count} customers on page {page}");
        }
    }
}