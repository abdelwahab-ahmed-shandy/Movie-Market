# 🎬 Movie Market

[![Visit My Blog](https://img.shields.io/badge/Visit%20My%20Blog-2962FF?style=flat-square&logo=hashnode&logoColor=white)](https://abdelwahabshandy.hashnode.dev)

# 🎬 Movie Market - Cinema Ticketing Platform

A full-featured web platform for cinema ticket booking built with ASP.NET Core MVC, following a clean architecture with layered separation (Presentation, BLL, DAL).
This system supports a complete movie experience: cinema management, series and seasons, user and admin areas, and flexible service-based logic.

🚀 This project was built completely from scratch, including the authentication and user management systems.

---

## 📌 Project Overview

Movie Market is a **cinema ticket booking system** that allows:
- Users to browse and book movie tickets.
- Admins to manage cinemas, movies, users, and more.
- A global search functionality for both users and admins.
- Managing both **Movies** and **TV Series**, where:
  - Series contain Seasons.
  - Seasons contain Episodes.
- Each movie is linked with:
  - **Category**
  - **Characters**
  - **Cinema & Showtimes**

---

## 🛠️ Tech Stack

- **ASP.NET Core MVC**
- **Entity Framework Core** (Code First)
- **SQL Server**
- **Razor Views**
- **Stripe Integration** for payment handling, including redirect to Stripe payment gateway after booking
- **Session-based authentication & Authorization**
- **Service and Repository Pattern**
- **Pagination and File Upload Services**

---

## 🎯 Key Features

### 🎟️ Ticket Booking & Cinema Management
- Users can book tickets directly for available movies in specific cinemas.
- Admins can manage:
  -    Category
  - 🎬 Movies
  - 🏢 Cinemas
  - 📺 Series, Seasons, and Episodes
  - 🎭 Characters

### 👥 User Management System
- Complete CRUD operations for users.
- Role-based access (Customer / Admin / Super Admin)
- Block & Unblock users with reason tracking.

### 📂 File Upload System
- Centralized upload service that handles both **single and multiple file uploads**, used across various parts of the application.

### 🌐 Global Search
- Available in both Admin and Customer areas.
- Search across movies, series, episodes, and characters.

### 📑 Pagination
- Implemented using a reusable **PaginatedList** helper.
- Applied to all listings: movies, users, cinemas, etc.

---

### 🔐 Authentication & Authorization

### The system follows Area-based architecture:
- /Admin/ → for Admin and Super Admin users.
- /Customer/ → for regular users (Customers).

### Custom Authentication System:
- A **fully custom-built login, registration, password reset, and account management system** (without using ASP.NET Core Identity scaffolding).
- Includes:
  - Manual login and registration logic
  - Password reset with email verification
  - New password creation
  - Edit account details
  - View account information
  - Full account deletion flow

### Role-based access using claims and policies:
- Regular users (Customers) can fully manage their accounts, including deleting them.
- Admin and Super Admin accounts **cannot be deleted** for security and integrity.
- Super Admin has full privileges and cannot be modified by other roles.

### Access Restrictions:
- Admin and Super Admin accounts cannot be deleted for security purposes.
- Regular users (Customers) can fully manage their accounts including profile picture updates and account deletion.

### Global Shared Pages:
- Access Denied, Not Found, Error, Unauthorized, Maintenance, and Coming Soon pages are included to handle various states across the app.

---

## 🖼️ Series & Characters System

- Movies and Series contain linked **Characters**.
- Series consist of:
  - Multiple **Seasons**
  - Each season includes **Episodes**
- All relationships managed through the admin interface.

---

## 🧱 Project Architecture
```

Movie-Market/
│
├── 📂 Presentation Layer         # UI - MVC Areas (Admin, Customer, Identity)
│ └── Areas/
│ ├── Admin/                      # Admin Dashboard
│ │ ├── Controllers/
│ │ ├── ViewModels/               # Admin-specific view models
│ ├── Customer/                   # Regular User Interface
│ │ ├── Controllers/
│ │ ├── Views/
│ │ └── ViewModels/               # View models for Customer
│ └── Identity/                   # Login and registration
│ ├── Controllers/
│ └── Views/
│ └── GloubalUsing/              # GloubalUsing
|     ├── BaseController.cs
|     └── BaseController.cs
│ └── Views/
|    └── Shared/
|      ├── AccessDenied.cshtml
|      ├── Error.cshtml
|      ├── NotFound.cshtml
|      ├── Unauthorized.cshtml
|      ├── GenericError.cshtml
|      ├── Maintenance.cshtml
|      └── ComingSoon.cshtml
|
├── 📂 Business Logic Layer (BLL) # Services & Logic (with interfaces)
│ ├── Services/
│ │ ├── Interfaces/                # Service interfaces
│ │ └── Implementations/           # Service implementations
│ ├── Utilities/                   # Any logic-specific helper functions
|      ├── PaginatedList.cs
|      └──  StripeSettings.cs
│ └── GloubalUsing/                # GloubalUsing
│
├── 📂 Data Access Layer (DAL)    # Data access layer
│ │ ├── Context/                  # Main context for EF Core
│ │ └── ApplicationDbContext.cs/
│ ├── Models/                     # Entities
│ ├── Repositories/
│ │ ├── Interfaces/               # Repository Interfaces
│ │ └── Implementations/          # Repository Implementations
│ ├── Migrations/                 # Database Migration Files
│ ├── Enums/                      # Enums
│ ├── GloubalUsing/               # GloubalUsing
│ └── ViewModels/                 # ViewModels
│
└── Movie-Market.sln              # Solution File

```
> 🔹 Each layer is modular and loosely coupled.  
> 🔹 The project uses **Generic Services** and **Generic Repositories** for reusability and scalability.

---

## 🛠️ Technologies Used

- **ASP.NET Core MVC** – For structuring the application.
- **Entity Framework Core** – For ORM and database operations.
- **SQL Server** – Backend database.
- **Custom Authentication System** – Built manually without using Identity scaffolding, based on claims and role-based access.
- **Bootstrap** **Html** **CSS** **JS** – Responsive front-end UI.
               
---

## 🚀 Getting Started

1. Clone the repository:
```bash
git clone https://github.com/abdelwahab-ahmed-shandy/Movie-Market
```

2. Apply Migrations and Seed Data:
```bash
Update-Database
```

3. Run the project using Visual Studio or:
```bash
dotnet run
```

---

## 🤝 Contribution

Feel free to fork the project and submit PRs. Contributions are welcome!

---

## 📞 Contact

If you have any questions or feedback, feel free to reach out:

[![LinkedIn](https://img.shields.io/badge/Followers-4000-blue?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/abdelwahab-ahmed-shandy/)
[![Medium](https://img.shields.io/badge/Followers-25-brightgreen?style=for-the-badge&logo=medium&logoColor=white)](https://medium.com/@abdelwahabshandy)
[![GitHub](https://img.shields.io/badge/GitHub-333333?style=for-the-badge&logo=github&logoColor=white)](https://github.com/abdelwahab-ahmed-shandy)

---

If you get bored , Make up a programming problem ").
