# PlanStock

PlanStock is a full-stack event management application that allows users to create, browse, and attend events. It is built with ASP.NET for the frontend, a C# Web API backend, and a SQL Server database. Identity and Entity Framework are used for authentication and data access.

## Technologies Used

- ASP.NET (Frontend)
- C# Web API (Backend)
- SQL Server (Database)
- Entity Framework Core (ORM)
- ASP.NET Identity (Authentication and Authorization)

## Features

- User registration and login
- Secure authentication with ASP.NET Identity
- Create and manage events
- Browse and attend events
- Role-based access control
- Persistent data storage with SQL Server

## Getting Started

### Prerequisites

- .NET SDK 6.0 or later
- SQL Server (LocalDB or full version)
- Visual Studio 2022 or later (recommended)

### Setup Instructions

1. **Clone the repository**

   ```bash
   git clone https://ProjectEventPlanner@dev.azure.com/ProjectEventPlanner/PlanStock/_git/PlanStock
   cd planstock
   ```
2. **Configure the database connection**

   Edit the `appsettings.json` file to match your SQL Server setup:

   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=PlanStockDb;Trusted_Connection=True;"
   }
   ```
3. **Apply migrations and update the database**

    Open a terminal in the project root and run:
    ```bash
    dotnet ef database update
    ```
4. **Run the application**

    Use the following command to run the application:
    ```bash
    dotnet run
    ```

##Project Structure
    
    PlanStock/
    /Backend                C# Web API backend
    /Frontend               ASP.NET frontend application
    PlanStock.sln         Solution file


## Security

- Passwords are hashed using ASP.NET Identity
- Role-based access to protect endpoints
- Input validation and error handling implemented
