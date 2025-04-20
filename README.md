# BlogApp: ASP.NET Core MVC Project

## Overview

Welcome to BlogApp! This is a simple but functional blog website built using the ASP.NET Core MVC framework. It was developed as a learning project during the Dogus Tech training program to practice and demonstrate key web development concepts.

The application allows users to register, log in, and manage blog posts (create, read, update, delete). It features categorization, basic user roles, and a commenting system.

## Features

*   **User Management:** Register, log in, and log out using ASP.NET Core Identity.
*   **Blog Post Management:** Authenticated users can create, edit, and delete their own blog posts (CRUD).
*   **Authorization:** Only logged-in users can create/edit/delete posts. Guests can only read.
*   **Categorization:** Blog posts are organized by categories.
*   **Homepage Listing:** Displays the latest blog posts.
*   **Detailed View:** Users can view the full content of a single blog post.
*   **Commenting:** Logged-in users can leave comments on posts.
*   **Responsive Design:** Built with Bootstrap for adaptability across different screen sizes.
*   **Form Validation:** User input is validated both on the client and server-side.

## Technologies Used

*   **.NET (ASP.NET Core MVC)**
*   **Entity Framework Core** (Object-Relational Mapper)
*   **ASP.NET Core Identity** (Authentication & Authorization)
*   **MS SQL Server** (Database - Can be adapted for others like PostgreSQL)
*   **C#**
*   **Razor Views** (HTML Generation)
*   **Bootstrap** (Front-end Framework)
*   **HTML / CSS / JavaScript**
*   **AutoMapper** (Object-to-Object Mapping)
*   **Design Patterns:** Repository Pattern, Unit of Work, Dependency Injection

## Screenshots

Here's a look at the application in action:

**Homepage (Post Listing):**
![Homepage showing blog posts](https://raw.githubusercontent.com/Mens1s/BlogApp-DogusTech/main/in-site-images/main-page.png)

**Blog Post Detail View:**
![Detailed view of a single blog post](https://raw.githubusercontent.com/Mens1s/BlogApp-DogusTech/main/in-site-images/post-view.png)

**Creating a New Blog Post:**
![Form for creating a new blog post](https://raw.githubusercontent.com/Mens1s/BlogApp-DogusTech/main/in-site-images/create-new-blog.png)

**Login & Register Page:**
![Login and Register pages](https://raw.githubusercontent.com/Mens1s/BlogApp-DogusTech/main/in-site-images/register-login-page.png)

**Account Management/Details:**
![User account management page](https://raw.githubusercontent.com/Mens1s/BlogApp-DogusTech/main/in-site-images/account-detail-view.png)

**Adding a Comment:**
![Interface for adding a comment to a post](https://raw.githubusercontent.com/Mens1s/BlogApp-DogusTech/main/in-site-images/comment-add-view.png)

## Getting Started

To run this project locally:

1.  **Prerequisites:**
    *   .NET SDK (Check `global.json` or `.csproj` for version, likely .NET 6 or later)
    *   SQL Server (Express, Developer, or other editions) or another compatible database.
    *   A code editor like Visual Studio or VS Code.
2.  **Clone the repository:**
    ```bash
    git clone https://github.com/Mens1s/BlogApp-DogusTech.git
    cd BlogApp-DogusTech/BlogApp.Web # Navigate to the main web project directory
    ```
3.  **Configure Database Connection:**
    *   Open the `appsettings.json` (or `appsettings.Development.json`) file.
    *   Update the `ConnectionStrings["DefaultConnection"]` value to point to your local database instance.
4.  **Apply Migrations:**
    *   Open a terminal or Package Manager Console in the project directory (`BlogApp.Web`).
    *   Run the command: `dotnet ef database update`
    *   This will create the database and apply the necessary schema based on the migrations.
5.  **Run the Application:**
    *   Run the command: `dotnet run`
    *   Or, open the solution in Visual Studio and press F5 or Ctrl+F5.
6.  **Access the Site:** Open your web browser and navigate to the URL provided (usually `https://localhost:xxxx` or `http://localhost:xxxx`).

---

*This project was created by [Mens1s](https://github.com/Mens1s) as part of the Dogus Tech training.*
