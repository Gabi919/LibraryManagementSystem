#  Library Management System

A modern, desktop-based solution for managing library operations, built with **C#** and **WPF (Windows Presentation Foundation)**. This application follows the **MVVM (Model-View-ViewModel)** architectural pattern to ensure a clean separation of concerns and maintainability.

---

##   Features

* **Inventory Management:** Add, update, and remove books, including details like ISBN, author, and genre.
* **Member Tracking:** Manage library members, their contact info, and membership status.
* **Search & Filters:** Real-time search functionality for books and members using LINQ.
* **Modern UI:** A responsive and intuitive interface leveraging WPF's data binding and control templating.
* **Data Persistence:** Integrated with **PostgreSQL** for reliable data storage and management.

---

##  Tech Stack

* **Language:** C#
* **Framework:** .NET 8.0 / .NET Framework
* **UI Framework:** WPF (Windows Presentation Foundation)
* **Pattern:** MVVM (Model-View-ViewModel)
* **DB:** PostgreSQL
* **Styling:** XAML (Custom Styles & Control Templates)

---

##  Architecture

The project is structured for MVVM arhitecture:

* **Models:** Data structures representing the business entities (Book, Member, Loan).
* **ViewModels:** Business logic handling, command binding, and UI state management.
* **Views:** XAML-based declarations of the user interface.
* **Services:** Abstraction layer for database operations and business logic.

---



This project was developed for a college course to illustrate desktop application architecture in WPF and C#.
