# ASP.NET Core Web Service with Clean Architecture

This repository contains the implementation of a technical assignment using ASP.NET Core, Clean Architecture, and Domain-Driven Design (DDD).

## Architecture
-   **API**: Entry point, Controllers.
-   **Application**: Business logic, CQRS (Services), Validators.
-   **Domain**: Core Entities.
-   **Infrastructure**: EF Core (PostgreSQL), MongoDB (Logs), Redis (Cache), Authentication.

---

## 📝 Task Requirements

### Task 1: Web Service

Implement an ASP.NET Core Web Service. Request and response information must be logged to a database.

#### Server Side
Develop 2 REST API methods.

**Method 1 (POST): Save Data**
*   **Input**: JSON array of objects.
    ```json
    [
        {"1": "value1"},
        {"5": "value2"},
        {"10": "value32"}
    ]
    ```
*   **Process**:
    1.  Transform to object:
        *   `code` (int): Key from input.
        *   `value` (string): Value from input.
    2.  Sort array by `code`.
    3.  Clear the existing table.
    4.  Save data to DB.
*   **Table Structure**:
    | Field | Type | Description |
    | :--- | :--- | :--- |
    | `id` | int | Serial Number (Primary Key) |
    | `code` | int | Code |
    | `value` | string | Value |

**Method 2 (GET): Retrieve Data**
*   Returns data from the table as JSON.
*   **Output Fields**: `id`, `code`, `value`.
*   **Feature**: Support for filtering returned data.

---

### Task 2: SQL Queries (Clients)

Given the following tables:

**Table: `Clients`**
| Field | Type | Description |
| :--- | :--- | :--- |
| `Id` | bigint | Client ID |
| `ClientName` | nvarchar(200) | Client Name |

**Table: `ClientContacts`**
| Field | Type | Description |
| :--- | :--- | :--- |
| `Id` | bigint | Contact ID |
| `ClientId` | bigint | Client ID (Foreign Key) |
| `ContactType` | nvarchar(255) | Contact Type |
| `ContactValue` | nvarchar(255) | Contact Value |

**Requirements**:
1.  Write a query that returns **Client Name** and **Count of Contacts**.
2.  Write a query that returns a **List of Clients** who have **more than 2 contacts**.

---

### Task 3: SQL Intervals (Optional)

Given the following table:

**Table: `Dates`**
| Field | Type | Description |
| :--- | :--- | :--- |
| `Id` | bigint | Identifier |
| `Dt` | date | Date |

**Goal**: Write a query that returns intervals for identical Ids.

**Example Input**:
| Id | Dt |
| :--- | :--- |
| 1 | 01.01.2021 |
| 1 | 10.01.2021 |
| 1 | 30.01.2021 |
| 2 | 15.01.2021 |
| 2 | 30.01.2021 |

**Expected Output**:
| Id | Sd (Start Date) | Ed (End Date) |
| :--- | :--- | :--- |
| 1 | 01.01.2021 | 10.01.2021 |
| 1 | 10.01.2021 | 30.01.2021 |
| 2 | 15.01.2021 | 30.01.2021 |
