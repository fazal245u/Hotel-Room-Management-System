# STAYEASE – Hotel Room Management System

A desktop-based **Hotel Room Management System** developed using **C# Windows Forms**, **SQL Server**, and **.NET Framework 4.0**. STAYEASE provides separate Guest/User and Administrator panels to manage room inventory, reservations, and guest records.

## Screenshots

### 1. Login Screen
Guests and administrators can sign in using their respective account types. New guests can create an account.

![STAYEASE login screen](images/login-screen.png)

### 2. Guest Dashboard
The guest dashboard displays the number of available rooms and the guest's bookings, with navigation to booking and room information.

![Guest dashboard](images/guest-dashboard.png)

### 3. Administrator – Manage Rooms
Administrators can add, update, and delete room records. The room list displays room number, type, price per night, capacity, and availability.

![Administrator manage rooms screen](images/admin-manage-rooms.png)

### 4. SQL Server Management Studio
SQL Server Management Studio can be used to execute the database setup script and inspect the database.

![SQL Server Management Studio](images/sql-server-management-studio.png)

## Features

### Guest / User Panel
- Create a user account and sign in.
- View available rooms and nightly prices.
- Book a room by entering the guest name, phone number, check-in date, and check-out date.
- Prevent overlapping active bookings for the same room and date range.
- View personal bookings.
- Cancel an active booking.

### Administrator Panel
- View dashboard totals.
- Add, update, and delete room records (CRUD).
- View all bookings.
- Update booking status:
  - `Booked`
  - `Checked-In`
  - `Checked-Out`
  - `Cancelled`
- View registered guest records.

## Technology Stack

| Component | Technology |
|---|---|
| Programming language | C# |
| Desktop UI | Windows Forms |
| Framework | .NET Framework 4.0 |
| Database | Microsoft SQL Server |
| IDE | Visual Studio 2010 |
| Database management | SQL Server Management Studio (SSMS) |

## Requirements

- Visual Studio 2010
- .NET Framework 4.0
- SQL Server Express or another SQL Server instance
- SQL Server Management Studio (SSMS) is recommended for running the setup script

## Setup and Installation

1. Download or clone this repository.
2. Extract the ZIP file if you downloaded the project as a ZIP.
3. Open `HotelRoomManagement.sln` in Visual Studio 2010.
4. Open `Database_Setup.sql` in SQL Server Management Studio and execute the script.
5. Open `HotelRoomManagement/Db.cs` and check the database connection string.

   Default connection string:

   ```csharp
   @"Data Source=.\SQLEXPRESS;Initial Catalog=HotelRoomDB;Integrated Security=True"
   ```

   Common alternatives:

   ```csharp
   // Local default instance
   @"Data Source=.;Initial Catalog=HotelRoomDB;Integrated Security=True"

   // Named SQL Server instance
   @"Data Source=YOUR-PC-NAME\SQLEXPRESS;Initial Catalog=HotelRoomDB;Integrated Security=True"
   ```

6. Make sure the SQL Server service is running and Windows Authentication is enabled.
7. In Visual Studio, select **Build → Build Solution**.
8. Press **F5** to run the application.

## Demo Administrator Account

| Field | Value |
|---|---|
| Username | `admin` |
| Password | `Admin@123` |

> **Security note:** These are demo credentials. Change or remove them before sharing or deploying the application.

## Create a Guest Account

On the login screen, select **Guest / User**, choose **CREATE USER ACCOUNT**, register a user, and then sign in with that account.

## Troubleshooting

- **Database connection error:** Verify the SQL Server instance name and update `Db.ConnectionString` in `Db.cs`.
- **SQL Server Express is not installed:** Install SQL Server Express or use another available SQL Server instance, then run `Database_Setup.sql` on that instance.
- **A room cannot be deleted:** The room may have booking history. This protects referential integrity. Mark the room as unavailable instead.
- **No users are listed:** Register a guest account before creating a booking.

## Important Security Note

This project is a **mini-project/demo starter**, not production-ready hotel software. In this starter version, passwords are stored as plain text for simplicity. A production system should use salted password hashing, secure administrator account management, input validation, audit logs, and stronger booking concurrency controls.

## Project Structure

The solution is expected to include the Visual Studio solution and project files, the database setup script, and the C# source files. The screenshots used in this README are stored in the `images/` directory.

```text
STAYEASE/
├── HotelRoomManagement.sln
├── Database_Setup.sql
├── HotelRoomManagement/
│   ├── Db.cs
│   └── ...
├── images/
│   ├── login-screen.png
│   ├── guest-dashboard.png
│   ├── admin-manage-rooms.png
│   └── sql-server-management-studio.png
└── README.md
```

## Author

Add your name, institution, and contact details here if you want to display project ownership.

---

**STAYEASE – Manage your stay in one place.**
