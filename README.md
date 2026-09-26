# TaskManager

TaskManager is a full-stack project and task management web application built with ASP.NET Core MVC.

The application allows users to create projects, collaborate with project members, manage tasks, assign priorities and deadlines, communicate through comments, and track overall progress.

The project also demonstrates the integration of ASP.NET Core MVC with a REST API and JavaScript `fetch()` requests for asynchronous UI updates.

---

## Features

### Authentication and Authorization

- User registration and login with ASP.NET Core Identity
- Custom `ApplicationUser` with first name and last name
- Role-based authorization
- `User` and `Admin` roles
- Automatic `User` role assignment after registration
- Seeded administrator account
- Protected Admin Area

### Project Management

- Create projects
- Edit projects
- Delete projects
- View project details
- Set project deadlines
- View project statistics
- Project Owner and Project Member access control

### Project Members

- Add registered users to projects by email
- Remove project members
- Prevent duplicate members
- Owner-only member management
- Members can view projects they participate in

### Task Management

- Create tasks inside projects
- Edit tasks
- Delete tasks
- Assign tasks to project members
- Set task priorities:
  - Low
  - Medium
  - High
  - Critical
- Track task statuses:
  - To Do
  - In Progress
  - Completed
- Set task deadlines
- Automatic `CompletedOn` management
- Permission-based task modification

### Comments

- Add comments to tasks
- View task discussions
- Delete own comments
- Users cannot delete comments created by other users

### Dashboard

The user dashboard displays:

- Number of accessible projects
- Total number of tasks
- Completed tasks
- Overall task completion percentage
- Visual task progress indicator

### Admin Area

Administrators can:

- View registered users
- View user roles
- Change users between `User` and `Admin`
- Access a dedicated Admin Dashboard

The application prevents removing the final administrator role, ensuring that the system cannot be left without an Admin.

---

## REST API and JavaScript

TaskManager includes an API endpoint used together with JavaScript for asynchronous task status updates.

### Endpoint

```http
PUT /api/tasks/status
```

Example request:

```json
{
  "taskId": 5,
  "status": 2
}
```

JavaScript uses the Fetch API to communicate with the ASP.NET Core API controller.

This allows task statuses to be updated without reloading the page.

The UI updates dynamically by:

- changing the task status
- updating the task card styling
- marking completed tasks visually
- displaying update feedback
- restoring the previous value if the API request fails

---

## Roles and Permissions

### User

A registered user can:

- create projects
- manage projects they own
- participate in projects as a member
- view project tasks
- create tasks in accessible projects
- add comments

### Project Owner

The project owner can additionally:

- edit and delete the project
- add and remove project members
- modify tasks inside the project

### Task Creator

A task creator can:

- edit the task
- delete the task
- change its status

Being assigned to a task alone does not grant modification permissions.

### Admin

An Admin can:

- access the Admin Area
- view registered users
- manage application roles

---

## Technologies

### Backend

- C#
- .NET 8
- ASP.NET Core MVC
- ASP.NET Core Web API
- ASP.NET Core Identity
- Entity Framework Core
- SQL Server

### Frontend

- Razor Views
- HTML5
- CSS3
- Bootstrap
- JavaScript
- Fetch API

### Testing

- NUnit
- Entity Framework Core InMemory
- Moq
- Coverlet

### Development Tools

- Visual Studio
- SQL Server Express
- Git
- GitHub

---

## Architecture

The project follows a layered structure with separation between presentation, business logic, and data access.

```text
TaskManager
│
├── Areas
│   ├── Admin
│   └── Identity
│
├── Common
│
├── Controllers
│   └── Api
│
├── Data
│   └── Seed
│
├── Migrations
│
├── Models
│   └── Enums
│
├── Services
│   └── Interfaces
│
├── ViewModels
│
├── Views
│
└── wwwroot
    ├── css
    └── js
```

The application uses:

- Controllers for handling HTTP requests
- ViewModels for communication between Views and Controllers
- Services for business logic
- Entity Framework Core for database access
- Identity for authentication and authorization
- API Controllers for JSON-based endpoints
- JavaScript for asynchronous frontend interactions

---

## Main Entities

### ApplicationUser

Extends ASP.NET Core Identity and stores additional user information.

### Project

Represents a project owned by a registered user.

### ProjectMember

Represents the relationship between users and projects.

### TaskItem

Represents a task belonging to a project.

### Comment

Represents a comment created by a user on a task.

---

## Testing

The project contains unit tests for the main service layer.

Covered services include:

- `TaskService`
- `ProjectService`
- `ProjectMemberService`
- `CommentService`
- `HomeService`
- `AdminUserService`

The tests cover important business scenarios such as:

- creating, editing and deleting projects
- creating, editing and deleting tasks
- task status updates
- project and task permissions
- project member management
- comment creation and deletion
- dashboard statistics
- Admin role management
- protection against removing the final administrator

Tests use an EF Core InMemory database where appropriate and Moq for `UserManager` testing.

Run the tests with:

```bash
dotnet test
```

Code coverage can be collected with:

```bash
dotnet test --collect:"XPlat Code Coverage"
```

---

## Getting Started

### Prerequisites

Make sure the following are installed:

- .NET 8 SDK
- SQL Server or SQL Server Express
- Visual Studio 2022 or another compatible IDE

### 1. Clone the repository

```bash
git clone https://github.com/DniPr/TaskManager.git
```

Navigate to the project folder:

```bash
cd TaskManager
```

### 2. Configure the database

Update the connection string in `appsettings.json` if necessary.

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=TaskManagerDB;Trusted_Connection=True;MultipleActiveResultSets=true;Encrypt=False"
  }
}
```

### 3. Configure the administrator account

The administrator credentials are not stored in the repository.

Use .NET User Secrets:

```bash
dotnet user-secrets set "AdminUser:Email" "admin@example.com"
dotnet user-secrets set "AdminUser:Password" "YourSecurePassword123!"
```

### 4. Apply database migrations

```bash
dotnet ef database update
```

### 5. Run the application

```bash
dotnet run
```

Or start the application directly from Visual Studio.

---

## Highlights

This project demonstrates practical usage of:

- ASP.NET Core MVC
- Entity Framework Core
- Identity authentication
- Role-based authorization
- Service layer architecture
- ViewModels
- REST API development
- JavaScript Fetch API
- asynchronous UI updates
- relational database design
- authorization and permission rules
- unit testing
- mocking
- dependency injection

---

## Future Improvements

Possible future additions include:

- task filtering and searching
- pagination
- notifications
- richer dashboard charts
- email invitations
- API expansion
- deployment to a cloud platform

---

## Author

Developed by **DniPr**

GitHub: https://github.com/DniPr

---

## License

This project is created for educational and portfolio purposes.
