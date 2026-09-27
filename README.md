# 🎓 Educational Center API

A robust, enterprise-grade RESTful Web API designed to manage the core operations of an educational facility. Built with **C#** and **ASP.NET Core**, this system manages course creation, class scheduling, student enrollments, instructor assignments, and financial transactions.

This project was built to demonstrate professional backend engineering practices, focusing heavily on **Clean Architecture**, data integrity, and centralized error handling.

---

## 🛠️ Tech Stack

* **Framework:** .NET 8 / ASP.NET Core Web API
* **Language:** C#
* **Database:** Microsoft SQL Server
* **ORM:** Entity Framework Core (Code-First Migrations)
* **Object Mapping:** AutoMapper v13+
* **API Documentation:** Swagger / OpenAPI

---

## 🏛️ Architectural Design: Clean Architecture

The solution is divided into four distinct layers to enforce the separation of concerns and the Dependency Inversion Principle.

1. **Core Layer (`EducationalCenter.Core`):** The heart of the application containing pure domain logic. It has zero dependencies on any other project or framework. Contains domain entities (`Student`, `Class`, etc.), enums, and repository abstractions (`IUnitOfWork`).
2. **Shared Layer (`EducationalCenter.Shared`):** Acts as a communication bridge. Contains Request/Response DTOs and custom domain exceptions (`NotFoundException`, `ConflictException`).
3. **Infrastructure Layer (`EducationalCenter.Infrastructure`):** Handles all external data persistence. Implements EF Core `DbContext`, SQL Server configurations, and Unit of Work data access logic.
4. **Web Layer (`EducationalCenter.Web`):** The presentation tier containing API Controllers, AutoMapper profiles, and the global exception-handling middleware.

---

## ✨ Advanced Business Logic & Data Integrity

The API goes beyond basic CRUD functionality to enforce strict business rules:

* **Capacity-Guarded Registrations:** The `EnrollmentService` actively tracks active enrollments, preventing duplicate student registrations and strictly enforcing maximum seat capacities for every class.
* **Global Exception Handling:** A centralized middleware intercepts all server exceptions, preventing raw crashes. It maps domain errors to standardized JSON responses:
  * `NotFoundException` $\rightarrow$ `404 Not Found`
  * `BadRequestException` $\rightarrow$ `400 Bad Request`
  * `ConflictException` $\rightarrow$ `409 Conflict`
* **Relational Data Reporting (LINQ):** Custom endpoints aggregate relational data across multiple tables. Clients can retrieve unified views, such as a complete class schedule (with course names, instructor names, and calculated available seats) or a student's full payment history.
* **Safe Data Encapsulation:** Uses AutoMapper to seamlessly translate internal Entity Framework Core database models into flat Data Transfer Objects (DTOs), preventing over-posting attacks and JSON circular reference errors.

---

## 🗄️ Database Schema & Entity Relationships

The relational database is structured to support complex queries and reporting:
* **Course $\leftrightarrow$ Class (1:N):** A single course curriculum can be taught across multiple scheduled classes.
* **Instructor $\leftrightarrow$ Class (1:N):** An instructor can be assigned to teach multiple classes.
* **Student $\leftrightarrow$ Enrollment (1:N):** A student can register for multiple classes.
* **Class $\leftrightarrow$ Enrollment (1:N):** A class contains multiple student enrollments.
* **Enrollment $\leftrightarrow$ Payment (1:1):** Every enrollment record generates a specific payment transaction.

---

## 📋 Comprehensive API Endpoint Matrix

### 🎓 Students
| Method | Route | Description |
| :--- | :--- | :--- |
| `GET` | `/api/Students` | Retrieves a list of all students. |
| `GET` | `/api/Students/{id}` | Retrieves details of a specific student. |
| `POST` | `/api/Students` | Registers a new student in the system. |
| `GET` | `/api/Students/{id}/payments` | **Reporting:** Aggregates a student's full payment history. |

### 📚 Courses
| Method | Route | Description |
| :--- | :--- | :--- |
| `GET` | `/api/Courses` | Retrieves the course catalog. |
| `GET` | `/api/Courses/{id}` | Retrieves details of a specific course. |
| `POST` | `/api/Courses` | Creates a new course (validates price $> 0$). |
| `PUT` | `/api/Courses/{id}` | Updates existing course details. |
| `DELETE` | `/api/Courses/{id}` | Removes a course from the catalog. |

### 🏫 Classes
| Method | Route | Description |
| :--- | :--- | :--- |
| `GET` | `/api/Classes` | Retrieves all scheduled classes. |
| `GET` | `/api/Classes/{id}` | Retrieves details of a specific class. |
| `POST` | `/api/Classes` | Schedules a new class, assigning a course, instructor, and capacity. |
| `PUT` | `/api/Classes/{id}` | Updates class schedules or capacities. |
| `DELETE` | `/api/Classes/{id}` | Cancels a scheduled class. |
| `GET` | `/api/Classes/schedule` | **Reporting:** Returns a master schedule dynamically calculating available seats. |
| `GET` | `/api/Classes/{id}/students` | **Reporting:** Retrieves a full roster of students enrolled in a specific class. |

### 👨‍🏫 Instructors
| Method | Route | Description |
| :--- | :--- | :--- |
| `GET` | `/api/Instructors` | Retrieves a list of all instructors. |
| `GET` | `/api/Instructors/{id}` | Retrieves details of a specific instructor. |
| `POST` | `/api/Instructors` | Adds a new instructor (validates required fields). |
| `PUT` | `/api/Instructors/{id}` | Updates instructor contact details. |
| `DELETE` | `/api/Instructors/{id}` | Removes an instructor from the system. |

### 📝 Enrollments
| Method | Route | Description |
| :--- | :--- | :--- |
| `GET` | `/api/Enrollments` | Retrieves a master list of all registration records. |
| `POST` | `/api/Enrollments/register` | Attempts to enroll a student in a class, enforcing capacity limits. |

### 💳 Payments
| Method | Route | Description |
| :--- | :--- | :--- |
| `GET` | `/api/Payments` | Retrieves all financial transactions. |
| `GET` | `/api/Payments/{id}` | Retrieves a specific transaction receipt. |
| `POST` | `/api/Payments` | Processes a payment linked to a specific enrollment. |
| `DELETE` | `/api/Payments/{id}` | Voids a transaction. |

---

## 🚀 Getting Started

### Prerequisites
* [.NET SDK](https://dotnet.microsoft.com/download) (v8.0 or newer)
* [SQL Server](https://www.microsoft.com/en-us/sql-server/) (Express, LocalDB, or Developer Edition)

### Installation & Setup

1. **Clone the repository:**
   ```bash
   git clone https://github.com/eiad20/EducationalCenter
   cd EducationalCenter

   ## 📄 License & Copyright

© 2026 Eiad Salama. All rights reserved.

This project is licensed under the **MIT License**. See the [LICENSE](LICENSE) file for full details.
