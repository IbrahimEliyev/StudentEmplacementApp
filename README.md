# StudentEmplacementApp

**StudentEmplacementApp** is an ASP.NET Core Web API that automates the competition and placement process for student admissions into universities, based on the centralized exam scores managed by the State Examination Center (DİM) of Azerbaijan.  
The placement of students is determined by an algorithm that evaluates each student's university program choices and their exam scores.

## Features

### Student Management
- Create, read, update, and delete student records
- Manage student placement preferences

### Student Placement Algorithm
- Implements logic to place students into programs based on their scores and selected preferences
- Resolves tie cases and respects quota constraints

### JWT Authentication
- Secures API endpoints using JSON Web Tokens

## Role-Based Access
The API implements role-based access using JWT authentication with two roles: `Admin` and `Student`.

### Swagger Documentation
- Provides API documentation and testing interface with Swagger UI

### Entity Framework
- Handles all database operations
- Supports migrations for schema versioning and updates

## Technology Stack

- ASP.NET Core Web API  
- Entity Framework Core  
- SQL Server  
- ASP.NET Core Identity (for JWT authentication)  
- AutoMapper (for DTO mapping)  
- Swagger/OpenAPI
