using System;
using Microsoft.Extensions.Configuration;
using ChinookApp.Repositories;

namespace ChinookApp
{
    /// <summary>
    /// The main entry point for the Chinook application.
    /// This class is responsible for setting up the application's configuration,
    /// creating necessary dependencies, and starting the main application loop.
    /// </summary>
    class Program
    {
        /// <summary>
        /// The main method that starts the application.
        /// It sets up the configuration, creates the CustomerRepository,
        /// and runs the main Application loop.
        /// </summary>
        /// <param name="args">Command line arguments (not used in this application)</param>
        static void Main(string[] args)
        {
            // Set up configuration 
            // this project stores the connection string in a file so the string can be kept out of the source code repository, and easy for the user to add their own, 
            // although it is still included in this repository as the login to the server is handled by windows running a local server, so the string does not contain passwords or usernames
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json") //make sure to set the build action for content and copy always when testing the application with the included file
                .Build();

            // Get connection string from configuration
            var connectionString = configuration.GetConnectionString("ChinookDatabase");

            // Create CustomerRepository instance
            ICustomerRepository customerRepository = new CustomerRepository(connectionString);

            // Create Application instance
            var application = new Application(customerRepository);

            // Run the main application loop
            application.Run();
        }
    }
}

//note to self https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/documentation-comments