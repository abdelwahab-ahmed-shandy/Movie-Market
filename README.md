# 🎬 Movie Market

[![Visit My Blog](https://img.shields.io/badge/Visit%20My%20Blog-2962FF?style=flat-square&logo=hashnode&logoColor=white)](https://abdelwahabshandy.hashnode.dev)

## 📖 Overview

**Movie Market** is a full-featured web application built using **ASP.NET Core** and **Entity Framework Core**. It allows users to explore a curated library of movies, manage their personal watchlists, and enables administrators to control content and user access through a powerful admin dashboard.

---

## 🚀 Features

- 🏷️ **Movie Management**  
  Create, update, and remove movie entries with full CRUD support.

- 🔍 **Search & Filter**  
  Quickly find movies by title, genre, or release year.

- 👥 **User Accounts**  
  Users can register, log in, and maintain personal watchlists.

- 📊 **Admin Dashboard**  
  Role-based access to manage movies, users, and content efficiently.

- 🌐 **(Upcoming)** External API Integration  
  Integrate third-party APIs to auto-fetch movie data and updates.

---

## 🧱 Project Architecture
```

Movie-Market/
│
├── 📂 Presentation Layer         # Responsible for the User Interface (UI)
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
│ └── GloubalUsing/               # GloubalUsing
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
├── 📂 Business Logic Layer (BLL) # Business logic (services, rules, transformations)
│ ├── Services/
│ │ ├── Interfaces/                # Service interfaces
│ │ └── Implementations/           # Service implementations
│ ├── Utilities/                   # Any logic-specific helper functions
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
---

## 🛠️ Technologies Used

- **ASP.NET Core MVC** – For structuring the application.
- **Entity Framework Core** – For ORM and database operations.
- **SQL Server** – Backend database.
- **Identity Framework** – User authentication and authorization.
- **Bootstrap** **Html** **CSS** **JS** – Responsive front-end UI.
               

---
## 📞 Contact

If you have any questions or feedback, feel free to reach out:

[![LinkedIn](https://img.shields.io/badge/Followers-4000-blue?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/abdelwahab-ahmed-shandy/)
[![Medium](https://img.shields.io/badge/Followers-25-brightgreen?style=for-the-badge&logo=medium&logoColor=white)](https://medium.com/@abdelwahabshandy)
[![GitHub](https://img.shields.io/badge/GitHub-333333?style=for-the-badge&logo=github&logoColor=white)](https://github.com/abdelwahab-ahmed-shandy)

---

If you get bored , Make up a programming problem ").
