# Student Registration System

A comprehensive student management system built with **Blazor WebAssembly** and **.NET 8.0**, following a **3-Tier Architecture** pattern with SQL Server database backend.

## 📋 Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Features](#features)
- [Technologies Used](#technologies-used)
- [Project Structure](#project-structure)
- [Installation](#installation)
- [Configuration](#configuration)
- [Usage](#usage)
- [Database Schema](#database-schema)
- [Contributing](#contributing)
- [License](#license)

## 🎯 Overview

The Student Registration System is a modern web-based application that facilitates the management of student information, course registration, and academic records. Built using Blazor WebAssembly for a responsive single-page application experience, it provides an intuitive interface for both administrators and students to manage educational data efficiently.

## 🏗 Architecture

This project follows a **3-Tier Architecture** pattern, ensuring separation of concerns, maintainability, and scalability:

### Presentation Layer
- **StudentRegistrationSystem.Client** (Blazor WebAssembly)
  - Client-side UI components and pages
  - Handles user interactions and presentation logic
  - Communicates with the server via HTTP/API calls

### Business Logic Layer
- **BusinessLogic** Project
  - Contains core business rules and validation logic
  - Service implementations for business operations
  - Interfaces defining service contracts
  - Orchestrates data flow between presentation and data layers

### Data Access Layer
- **DataAccess** Project
  - Entity Framework Core data context
  - Repository pattern implementations
  - Database entity models
  - CRUD operations and data persistence
  - SQL Server database integration

### Shared Layer
- **Shared** Project
  - DTOs (Data Transfer Objects)
  - Common models and entities
  - Shared interfaces and constants
  - Used across all layers

### Server Layer
- **StudentRegistrationSystem** (ASP.NET Core Web API)
  - Hosts the Blazor WebAssembly application
  - RESTful API endpoints
  - Dependency injection configuration
  - Middleware and authentication

## ✨ Features

### Student Management
- Add, update, and delete student records
- View comprehensive student profiles
- Search and filter students by various criteria
- Track student enrollment status
- Manage student academic history

### Course Management
- Create and manage course offerings
- Set course prerequisites and capacity limits
- View course schedules and availability
- Manage course instructors and departments
- Track course enrollment statistics

### Registration System
- Student course enrollment and drop functionality
- Real-time availability checking
- Conflict detection for course schedules
- Registration approval workflow
- Waitlist management

### Administrative Features
- User authentication and authorization
- Role-based access control (Admin, Staff, Student)
- Generate reports and statistics
- Export data in various formats
- Dashboard with analytics

## 🛠 Technologies Used

- **Frontend**: Blazor WebAssembly
- **Backend**: ASP.NET Core 8.0 Web API
- **Framework**: .NET 8.0
- **Database**: Microsoft SQL Server (SSMS)
- **ORM**: Entity Framework Core 8.0
- **Architecture**: 3-Tier Architecture with Repository Pattern
- **Dependency Injection**: Built-in ASP.NET Core DI Container

### Key NuGet Packages

```xml
- Microsoft.AspNetCore.Components.WebAssembly.Server (8.0.20)
- Microsoft.EntityFrameworkCore (8.0.20)
- Microsoft.EntityFrameworkCore.SqlServer (8.0.20)
- Microsoft.EntityFrameworkCore.Tools (8.0.20)
- Microsoft.EntityFrameworkCore.Design (8.0.20)
```

## 📁 Project Structure

```
StudentRegistrationSystem/
├── StudentRegistrationSystem/              # Server project (API Host)
│   ├── Controllers/                        # API Controllers
│   ├── Program.cs                          # Application entry point
│   ├── appsettings.json                    # Configuration settings
│   └── StudentRegistrationSystem.csproj
│
├── StudentRegistrationSystem.Client/      # Blazor WebAssembly Client
│   ├── Pages/                              # Razor pages
│   ├── Components/                         # Reusable UI components
│   ├── Services/                           # Client-side services
│   └── StudentRegistrationSystem.Client.csproj
│
├── BusinessLogic/                          # Business Logic Layer
│   ├── Services/                           # Service implementations
│   ├── Interfaces/                         # Service contracts
│   └── BusinessLogic.csproj
│
├── DataAccess/                             # Data Access Layer
│   ├── Data/                               # DbContext
│   ├── Repositories/                       # Repository pattern
│   ├── Entities/                           # Database entities
│   └── DataAccess.csproj
│
└── Shared/                                 # Shared resources
    ├── Models/                             # DTOs and shared models
    ├── Interfaces/                         # Shared interfaces
    └── Shared.csproj
```

## 📦 Installation

### Prerequisites

- **.NET 8.0 SDK** or later
- **SQL Server** (SQL Server Management Studio recommended)
- **Visual Studio 2022** or **Visual Studio Code** (with C# extension)
- **Git** for version control

### Setup Steps

1. **Clone the repository**
   ```bash
   git clone https://github.com/Ahmed-0sama/StudentRegisterationSystem.git
   cd StudentRegisterationSystem
   ```

2. **Restore NuGet packages**
   ```bash
   dotnet restore
   ```

3. **Configure SQL Server Database**
   - Open SQL Server Management Studio (SSMS)
   - Create a new database (e.g., `StudentRegistrationDB`)
   - Note your connection string

4. **Update Connection String**
   - Open `appsettings.json` in the server project
   - Update the connection string:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=YOUR_SERVER;Database=StudentRegistrationDB;Trusted_Connection=True;TrustServerCertificate=True;"
     }
   }
   ```

5. **Apply Database Migrations**
   ```bash
   cd StudentRegistrationSystem
   dotnet ef database update
   ```

6. **Build the Solution**
   ```bash
   dotnet build
   ```

7. **Run the Application**
   ```bash
   dotnet run --project StudentRegistrationSystem
   ```

8. **Access the Application**
   - Open your browser and navigate to `https://localhost:7XXX` (check console output for exact port)
   - The Blazor WebAssembly app will load

## ⚙ Configuration

### Database Configuration

The application uses Entity Framework Core with SQL Server. Configure your connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=StudentRegistrationDB;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### Service Registration

Services are registered in `Program.cs` using dependency injection:

```csharp
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
```

## 📖 Usage

### For Students

1. **Login**: Access the system using your student credentials
2. **Browse Courses**: View available courses for the current semester
3. **Register for Courses**: Select and register based on program requirements
4. **View Schedule**: Check registered courses and timetable
5. **Drop Courses**: Remove courses before the deadline
6. **View Grades**: Access academic records and transcripts

### For Administrators

1. **Dashboard**: View system statistics and recent activities
2. **Manage Students**: Add, edit, or remove student records
3. **Manage Courses**: Create and configure course offerings
4. **Monitor Registrations**: View and approve student registrations
5. **Generate Reports**: Create enrollment and performance reports
6. **User Management**: Manage system users and permissions

## 🗄 Database Schema

### Main Entities

- **Students**: Student information (ID, Name, Email, Program, EnrollmentDate)
- **Courses**: Course details (Code, Name, Credits, Capacity, Department)
- **Registrations**: Links students to courses with enrollment status and grades
- **Users**: Authentication and role management
- **Departments**: Academic departments and programs
- **Instructors**: Faculty information linked to courses

### Relationships

- Students ↔ Courses (Many-to-Many through Registrations)
- Courses → Departments (Many-to-One)
- Courses → Instructors (Many-to-One)
- Users ↔ Students (One-to-One)

### Entity Framework Migrations

Create a new migration:
```bash
dotnet ef migrations add MigrationName --project DataAccess
```

Update database:
```bash
dotnet ef database update --project StudentRegistrationSystem
```

## 🤝 Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/YourFeature`)
3. Follow the 3-tier architecture pattern
4. Implement interfaces in the BusinessLogic layer
5. Add repository methods in DataAccess layer
6. Create UI components in the Client project
7. Commit your changes (`git commit -m 'Add some feature'`)
8. Push to the branch (`git push origin feature/YourFeature`)
9. Open a Pull Request

### Coding Standards

- Follow C# coding conventions and .NET naming standards
- Use async/await for database operations
- Implement proper error handling and logging
- Write XML documentation comments for public APIs
- Use dependency injection for service resolution
- Follow the repository pattern for data access
- Keep business logic separate from data access and presentation

## 📧 Contact

**Ahmed Osama**
- GitHub: [@Ahmed-0sama](https://github.com/Ahmed-0sama
