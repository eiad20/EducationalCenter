# Educational Center API

A modular, production-ready RESTful Web API built with **ASP.NET Core** and **C#**, adhering strictly to **Clean Architecture** and **Domain-Driven Design (DDD)** principles. The system manages core operations of an educational training center, covering student enrollments, course tracking, class scheduling, instructor assignments, and payment transaction histories.

---

## 🏛️ Architecture Overview

The solution follows a 4-layer Clean Architecture approach to maintain separation of concerns, testability, and framework independence:

* **Core Layer (`EducationalCenter.Core`)**: Contains domain entities (`Student`, `Course`, `Instructor`, `Class`, `Enrollment`, `Payment`), domain enums (`PaymentStatus`, `PaymentMethod`), and core repository/unit-of-work abstractions (`IUnitOfWork`, `IGenericRepository<T>`).
* **Shared Layer (`EducationalCenter.Shared`)**: Contains Data Transfer Objects (DTOs) for incoming requests and outgoing responses, as well as domain-agnostic custom exceptions (`NotFoundException`, `BadRequestException`, `ConflictException`).
* **Infrastructure Layer (`EducationalCenter.Infrastructure`)**: Manages data access, Microsoft SQL Server integration, Entity Framework Core `DbContext`, migrations, and repository/Unit of Work implementations.
* **Presentation Layer (`EducationalCenter.Web`)**: ASP.NET Core Web API controllers, dependency injection setups, AutoMapper configuration profiles, and custom global exception handling middleware.

---

## ✨ Key Features

* **Full CRUD Operations**: Complete resource management for Students, Instructors, Courses, Classes, Enrollments, and Payments.
* **Business-Logic Validated Enrollment**: Prevents duplicate class registrations, verifies active student/class IDs, and enforces strict class capacity constraints.
* **Dynamic Reporting Endpoints**:
  * `/api/classes/schedule`: Aggregates class schedules alongside course names, assigned instructors, enrolled counts, and available capacity calculations.
  * `/api/classes/{id}/students`: Lists all students enrolled in a specific class.
  * `/api/students/{id}/payments`: Resolves full payment histories for a student, including course names and completion statuses.
* **Global Exception Middleware**: Intercepts unhandled errors across the HTTP pipeline and maps custom exceptions to standardized JSON problem details:
  * `NotFoundException` $\rightarrow$ `404 Not Found`
  * `BadRequestException` $\rightarrow$ `400 Bad Request`
  * `ConflictException` $\rightarrow$ `409 Conflict`
  * Unhandled server exceptions $\rightarrow$ `500 Internal Server Error`
* **AutoMapper Integration**: Safe DTO-to-entity bidirectional mapping preventing over-posting and circular serialization references.

---

## 🛠️ Tech Stack

* **Framework**: .NET 8 / .NET 9 ASP.NET Core Web API
* **Language**: C#
* **ORM**: Entity Framework Core
* **Database**: Microsoft SQL Server
* **Object Mapping**: AutoMapper v13+
* **API Documentation**: Swagger / OpenAPI (Swashbuckle)
* **Architecture**: Clean Architecture, Repository Pattern, Unit of Work

---

## 🚀 Getting Started

### Prerequisites

* [.NET SDK](https://dotnet.microsoft.com/download) (Version 8.0 or newer)
* [SQL Server](https://www.microsoft.com/en-us/sql-server/) (LocalDB, Express, or standard edition)
* Any modern IDE (Visual Studio Code, JetBrains Rider, or Visual Studio)

### Installation & Setup

1. **Clone the Repository**
   ```bash
   git clone https://github.com/eiad20/EducationalCenter
   cd EducationalCenter
