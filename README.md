# Authentication and Authorization(Permission-based) in ASP.NET Core Project (.NET 7)

This project is an example implementation of authentication and authorization (Permission-based) in ASP.NET Core using the .NET 7 framework. It provides a basic setup for user registration, login, role management, and access control to various parts of the application based on user roles and permissions.

## Features

- User registration: Allows new users to create an account with their email address and password.
- User login: Provides a login functionality to authenticate users and generate JSON Web Tokens(jwt).
- Role management: Administrators can assign roles to users and manage the available roles in the system.
- Permission-based access control: Protects routes and resources based on user roles and permissions.

## Technologies Used

- ASP.NET Core (.NET 7): The latest version of the ASP.NET Core framework.
- Identity Framework: Built-in authentication and authorization system in ASP.NET Core.
- Entity Framework Core: Object-Relational Mapping (ORM) tool for working with the database.
- JWT (JSON Web Tokens): Used for generating and validating access tokens.
- SQL Server (or any preferred database): Database system for storing user and role information.

## Getting Started

### Prerequisites

- .NET 7 SDK: Install the .NET 7 SDK on your development machine. You can download it from the official .NET website.

### Installation

1. Clone the repository or download the source code.
2. Open the project in your preferred IDE (e.g., Visual Studio, Visual Studio Code).
3. Update the connection string in the `appsettings.json` file to point to your SQL Server database or the desired database provider.
4. Open the terminal or command prompt and navigate to the project directory.
5. Run the following commands to create the initial database migration and apply it to the database:

`dotnet restore`
`dotnet ef database update`

6. Build and run the project using the following command:

`dotnet run`

7. Access the application in your web browser at `http://localhost:5000` (or the appropriate URL).

## Configuration

The application can be configured by modifying the `appsettings.json` file. Here are some of the important configuration options:

- `"ConnectionStrings:DefaultConnection"`: The connection string for your database.
- `"Authentication:Schemes:Bearer:Secret"`: The secret key used for signing JWT tokens.
- `"Authentication:Schemes:Bearer:ValidIssuer"`: The issuer of the JWT tokens.
- `"Authentication:Schemes:Bearer:ValidAudiences"`: The audience of the JWT tokens.
- `"Authentication:Schemes:Bearer:Authority"`: The Authority for the access token.

Make sure to secure sensitive configuration values and consider using environment-specific configuration files for different deployment environments.

## Contribution

Contributions to this project are welcome! If you find any issues or have suggestions for improvements, please feel free to open an issue or submit a pull request.

## License

This project is licensed under the [MIT License](LICENSE).
