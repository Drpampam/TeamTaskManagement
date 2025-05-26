# Team Task Management API

A secure RESTful API for managing team-based tasks and collaboration.

## Tech Stack

- .NET 7.0
- Entity Framework Core
- SQL Server
- JWT Authentication
- Swagger/OpenAPI
- Serilog

## Setup Instructions

1. Clone the repository
2. Update the connection string in `appsettings.json` to point to your SQL Server instance
3. Update the JWT secret key in `appsettings.json`
4. Open a terminal in the project directory
5. Run the following commands:
   ```bash
   dotnet restore
   dotnet ef migrations add InitialCreate
   dotnet run
   ```

The API will be available at `https://localhost:7001` (or the port shown in your console).

## API Documentation

### Authentication

#### Register User
```http
POST /api/auth/register
Content-Type: application/json

{
    "email": "user@example.com",
    "password": "password123",
    "firstName": "John",
    "lastName": "Doe"
}
```

#### Login
```http
POST /api/auth/login
Content-Type: application/json

{
    "email": "user@example.com",
    "password": "password123"
}
```

### Teams

#### Create Team
```http
POST /api/team
Authorization: Bearer {token}
Content-Type: application/json

{
    "name": "Development Team",
    "description": "Main development team"
}
```

#### Add User to Team
```http
POST /api/team/{teamId}/users
Authorization: Bearer {token}
Content-Type: application/json

{
    "userEmail": "user@example.com",
    "role": "Member"
}
```

### Tasks

#### Get Team Tasks
```http
GET /api/task/teams/{teamId}/tasks
Authorization: Bearer {token}
```

#### Create Task
```http
POST /api/task/teams/{teamId}/tasks
Authorization: Bearer {token}
Content-Type: application/json

{
    "title": "Implement authentication",
    "description": "Add JWT authentication to the API",
    "dueDate": "2024-03-20T00:00:00Z",
    "assignedToUserId": 1
}
```

#### Update Task
```http
PUT /api/task/{taskId}
Authorization: Bearer {token}
Content-Type: application/json

{
    "title": "Updated task title",
    "description": "Updated description",
    "dueDate": "2024-03-25T00:00:00Z",
    "assignedToUserId": 2
}
```

#### Update Task Status
```http
PATCH /api/task/{taskId}/status
Authorization: Bearer {token}
Content-Type: application/json

{
    "status": "InProgress"
}
```

#### Delete Task
```http
DELETE /api/task/{taskId}
Authorization: Bearer {token}
```

## Assumptions

1. Users can belong to multiple teams
2. Tasks belong to a single team
3. Tasks can be assigned to team members
4. Team admins can add users to their teams
5. Only team members can view and manage their team's tasks
6. JWT tokens expire after 7 days
7. Passwords are hashed using BCrypt
8. SQL Server is used as the database
9. The API is deployed in a secure environment with HTTPS

## Security Features

1. JWT-based authentication
2. Password hashing with BCrypt
3. Role-based access control
4. Team-based data isolation
5. Input validation
6. HTTPS enforcement
7. Secure password storage
8. Token-based session management 