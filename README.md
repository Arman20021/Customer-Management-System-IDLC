# Customer Management System API

A RESTful Customer Management System built with ASP.NET Core Web API and SQL Server. The application supports customer creation, retrieval, pagination, updating, deletion, multiple mobile numbers, multiple addresses, customer status management, and customer document metadata.


## Table of Contents

- [Project Overview](#project-overview)
- [Main Features](#main-features)
- [Technology Stack](#technology-stack)
- [Solution Structure](#solution-structure)
- [Database Schema](#database-schema)
- [API Endpoints](#api-endpoints)
- [Example Create Customer Request](#example-create-customer-request)
- [Error Handling](#error-handling)
- [Docker Setup](#docker-setup)
- [Assumptions](#assumptions)
- [AI Tools Used](#ai-tools-used)
- [Sample Prompts](#sample-prompts)
- [Modifications Made](#modifications-made)

---

## Project Overview

The solution follows a simple layered architecture:

```text
CMS.API
   ↓
CMS.BLL
   ↓
CMS.DAL
   ↓
SQL Server
```

The Customer Management System provides backend APIs for managing customer records.

Each customer can have:

- Basic personal information
- Multiple mobile numbers
- Multiple addresses
- Active or Inactive status
- Multiple document records

The API includes validation and error handling for invalid requests, missing customers, duplicate email addresses, duplicate National ID numbers, duplicate mobile numbers, unsupported document types, and unexpected server errors.

---

## Main Features

- Create a customer
- Get a customer by ID
- Get all customers
- Pagination, searching, filtering, and sorting
- Update customer information
- Preserve existing mobile and address IDs during updates
- Delete a customer
- Multiple mobile numbers per customer
- Multiple addresses per customer
- Customer document metadata support
- Swagger/OpenAPI documentation
- Layered architecture
- SQL Server database
- Docker support
---

## Technology Stack

| Area | Technology |
|---|---|
| Framework | ASP.NET Core Web API (.NET 9) |
| Language | C# |
| Database | Microsoft SQL Server |
| ORM | Entity Framework Core |
| Database approach | Database First |
| API documentation | Swagger |
| IDE | Visual Studio 2022 |
| Dependency injection | Built-in ASP.NET Core DI |
| Containerization | Docker and Docker Compose |
| Testing | Manual API testing through Swagger |

---

## Solution Structure

```text
CMS
├── CMS.API
│   ├── Controllers
│   ├── Models
│   ├── Program.cs
│   └── appsettings.json
│
├── CMS.BLL
│   ├── DTOs
│   └── Services
│
├── CMS.DAL
│   ├── Data
│   ├── Entities
│   └── Repositories
│
└── CMS.sln
```

### Layer Responsibilities

#### CMS.API

Responsible for:

- Receiving HTTP requests
- Model binding
- Returning HTTP responses
- Swagger configuration
- Dependency injection registration
- Controller-level error responses

#### CMS.BLL

Responsible for:

- Business rules
- Validation
- DTO mapping
- Duplicate checks
- Customer update behavior
- Service-level processing

#### CMS.DAL

Responsible for:

- Entity Framework Core DbContext
- Database entities
- Repository operations
- SQL Server data access

---

## Database Schema

The system uses four related tables.

### Customers

| Column | Type | Description |
|---|---|---|
| CustomerId | int | Primary key and identity |
| CustomerName | nvarchar(150) | Customer's full name |
| EmailAddress | nvarchar(255) | Unique email address |
| DateOfBirth | date | Optional date of birth |
| NationalIdNumber | nvarchar(50) | Unique National ID |
| Status | nvarchar(20) | Active or Inactive |
| CreatedDate | datetime2 | Record creation time in UTC |
| LastUpdatedDate | datetime2 | Last update time in UTC |

### CustomerMobiles

| Column | Type | Description |
|---|---|---|
| CustomerMobileId | int | Primary key and identity |
| CustomerId | int | Foreign key to Customers |
| MobileNumber | nvarchar(50) | Unique mobile number |
| MobileType | nvarchar(20) | Primary, Alternate, Office, Home, or Other |

### CustomerAddresses

| Column | Type | Description |
|---|---|---|
| CustomerAddressId | int | Primary key and identity |
| CustomerId | int | Foreign key to Customers |
| AddressType | nvarchar(20) | Present, Permanent, Mailing, Office, or Other |
| AddressText | nvarchar(500) | Full address |

### CustomerDocuments

| Column | Type | Description |
|---|---|---|
| CustomerDocumentId | int | Primary key and identity |
| CustomerId | int | Foreign key to Customers |
| DocumentType | nvarchar(30) | NID, TaxCertificate, Photo, Signature, or Other |
| OriginalFileName | nvarchar(255) | Original uploaded file name |
| StoredFileName | nvarchar(255) | Generated stored file name |
| FilePath | nvarchar(500) | Stored file path |
| ContentType | nvarchar(100) | File MIME type |
| FileSizeBytes | bigint | File size in bytes |
| UploadedDate | datetime2 | Upload time in UTC |

### Relationships

```text
Customers
├── One-to-many → CustomerMobiles
├── One-to-many → CustomerAddresses
└── One-to-many → CustomerDocuments
```

Related mobile, address, and document database records are deleted when their parent customer is deleted, based on the configured foreign-key behavior.

---

## API Endpoints

### Customer Endpoints

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/customer-management/create-customer` | Create a customer |
| GET | `/api/customer-management/get-customer/{id}` | Get a customer by ID |
| GET | `/api/customer-management/get-all-customers` | Get paginated customers, search customer, filter, sort |
| PUT | `/api/customer-management/update-customer/{id}` | Update a customer |
| DELETE | `/api/customer-management/delet-customer/{id}` | Delete a customer |

### Get All Customers Query Parameters

| Parameter | Default | Description |
|---|---:|---|
| pageNumber | 1 | Requested page |
| pageSize | 10 | Number of records per page |
| search | Empty | Search by customer name |
| status | Empty | Filter by Active or Inactive |
| sortBy | CustomerId | Field used for sorting |
| sortOrder | asc | asc or desc |

Example:

```http
GET /api/customer-management?pageNumber=1&pageSize=10&status=Active&sortBy=CustomerName&sortOrder=asc
```

### Customer Document Endpoint

Include this endpoint in the final README only when the document-upload feature exists in the final project.

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/customer-managemen/{customerId}/documents` | Upload a customer document |

---

## Example Create Customer Request

```json
{
  "customerName": "Md Arman Islam",
  "emailAddress": "arman@example.com",
  "dateOfBirth": "2000-05-15",
  "nationalIdNumber": "1223456789",
  "mobileNumbers": [
    {
      "mobileNumber": "01711111111",
      "mobileType": "Primary"
    },
    {
      "mobileNumber": "01822222222",
      "mobileType": "Alternate"
    }
  ],
  "addresses": [
    {
      "addressType": "Present",
      "addressText": "Dhaka, Bangladesh"
    },
    {
      "addressType": "Permanent",
      "addressText": "Narayanganj, Bangladesh"
    }
  ]
}
```

---

## Example Update Customer Request

Existing child IDs should be included to preserve them.

```json
{
  "customerName": "Md Arman Islam Updated",
  "emailAddress": "arman@example.com",
  "dateOfBirth": "2000-05-15",
  "nationalIdNumber": "1223456789",
  "status": "Active",
  "mobileNumbers": [
    {
      "customerMobileId": 1,
      "mobileNumber": "01711111111",
      "mobileType": "Primary"
    },
    {
      "customerMobileId":1,
      "mobileNumber": "01822222222",
      "mobileType": "Alternate"
    }
  ],
  "addresses": [
    {
      "customerAddressId": 1,
      "addressType": "Present",
      "addressText": "Uttara, Dhaka"
    }
  ]
}
```

Update behavior:

```text
Existing child ID supplied → Update the existing record
ID omitted or null         → Add a new record
Existing child omitted     → Delete that child record
```

---

## HTTP Status Codes

| Status | Meaning |
|---:|---|
| 200 | Request completed successfully |
| 201 | Customer or document created successfully |
| 400 | Invalid request or validation failure |
| 404 | Customer not found |
| 409 | Duplicate email, NID, or mobile number |
| 500 | Unexpected server error |

---

## Error Handling

The API handles:

- Invalid request payload
- Invalid customer ID
- Customer not found
- Duplicate email address
- Duplicate National ID number
- Duplicate mobile number
- Invalid customer status
- Invalid mobile type
- Invalid address type
- Invalid mobile or address child ID
- Unsupported document type or extension
- Unexpected server errors

Example error response:

```json
{
  "message": "Customer with ID 999 was not found."
}
```

---

## Swagger API Documentation

Swagger is enabled for interactive API documentation and manual endpoint testing.

After starting the API, open the URL shown in the application output. Swagger is configured at the application root because `RoutePrefix` is empty.

Example:

```text
https://localhost:{port}/
```

Swagger displays:

- Available endpoints
- HTTP methods
- Route and query parameters
- Request models
- Response models
- Validation rules
- Interactive “Try it out” support

---

## Docker Setup

This project is fully containerized. You can run it either by building the Docker image from source or by using the pre-built image.

No local .NET SDK, SQL Server, or SQL Server Management Studio installation is required when running with Docker.

### Prerequisites

Install and start:

- Docker Desktop

### Option A — Clone and Build from Source

This option builds the API image using the included `Dockerfile`.

```bash
git clone https://github.com/Arman20021/Customer-Management-System-IDLC.git
cd Customer-Management-System-IDLC
docker compose up --build
```

### Option B — Use the Pre-built Docker Image

This option uses the pre-built image published on Docker Hub:

```text
arman20022/cms-api:1.0.0
```

Run:

```bash
git clone https://github.com/Arman20021/Customer-Management-System-IDLC.git
cd Customer-Management-System-IDLC
docker compose up
```

Docker Compose downloads the required images when they are not already available locally.

### Open the Application

After both containers have started, open:

```text
http://localhost:8080/
```

Swagger UI is available at the root URL.

---

## Build and Run Locally

### Prerequisites

Install:

- .NET 9 SDK
- Visual Studio 2022 or another compatible IDE
- SQL Server or SQL Server Express
- SQL Server Management Studio, optional

### Configure the Database Connection

Update `CMS.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=CMSdb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Change the server name when your SQL Server instance is different.

### Restore Packages

From the solution directory:

```bash
dotnet restore
```

### Build the Solution

```bash
dotnet build
```

### Run the API

```bash
dotnet run --project CMS.API
```

Open the Swagger URL displayed in the terminal.

---


## Assumptions

- Email addresses are unique across all customers.
- National ID numbers are unique across all customers.
- Mobile numbers are unique across all customers.
- Every customer must have at least one mobile number.
- Every customer must have exactly one Primary mobile number.
- Customer status can only be Active or Inactive.
- Mobile types are Primary, Alternate, Office, Home, and Other.
- Address types are Present, Permanent, Mailing, Office, and Other.
- Document types are NID, TaxCertificate, Photo, Signature, and Other.
- Dates are stored using UTC for creation, update, and upload timestamps.
- Update Customer uses complete mobile and address lists.
- Existing mobile and address IDs are preserved when their IDs are supplied.
- Omitted existing mobile or address records are removed during a full update.
- Customer deletion is permanent.
- Deleted SQL identity values are not reused.
- The API has been manually tested using Swagger.

---

## AI Tools Used

OpenAI ChatGPT was used as a development-support tool for:

- Understanding the assignment requirements
- Planning the layered architecture
- Explaining ASP.NET Core and Entity Framework Core concepts
- Reviewing code structure
- Troubleshooting errors
- Suggesting validation and error-handling approaches
- Preparing documentation

---

## Sample Prompts

```text
-Suppose you are a senior .NET Core developer. Give me a proper three-layer architecture for a Customer Management System and identify I will use database first method. 
-I don’t like the architecture here mine , check it and if there is any improvement tell me that. 
-Which nuget package should install here like bll, api, dal. Show me the package name. 
-My database Connection is not working , here is the appsetting.json . Tell me what is the problem. 
-How can I set some records not null in ssms. 
-I created the Customers, CustomerMobiles, CustomerAddresses, and CustomerDocuments tables manually in SSMS. Review the table relationships and tell me whether any foreign key or unique constraint is missing.
-I am using Entity Framework Core database-first. Check my scaffold command and tell me whether the DbContext and entity folders are being generated in the correct projects.
-I have configured Swagger, but it does not open when I run the API. Here is my Program.cs. Find the missing or incorrect configuration and write the corrected Swagger setup.
-I was trying to create a customer with multiple mobile numbers and addresses, but only the customer information is being saved. Here is my repository and service code. Find the problem and write the corrected code.
-I have written this CreateCustomerService, but duplicate email and NID validation are not working correctly. Review my code and rewrite only the duplicate-checking part.
-I have implemented Create Customer and Get Customer by ID using the same service and repository. Review this design and suggest how I should organize the remaining features without making the project unnecessarily complex.
-I need exactly one Primary mobile number for every customer. Review my validation logic and tell me whether it handles zero Primary and multiple Primary numbers correctly.
-I am checking duplicate mobile numbers inside the request and also in the database. Review both checks and tell me whether any duplicate case is still missing.
-My API returns 500 instead of 409 when I use an email that already exists. Review the controller and service code and tell me where the exception should be handled.
-Get Customer by ID endpoint returns null even though the customer exists in the database. Here is my controller, service, and repository code. Find where the customer data is being lost.
-I have created the Get Customer by ID repository method, but mobile numbers, addresses, and documents are not included in the response. Review my query and write the correct Include and mapping code.
-From now on, I want to create a separate service and repository for each feature. Show me how to implement Get All Customers using this structure.
-Review my Get All Customers repository code. It should supports pagination, searching, filtering, and sorting. But it is not working while I was checking it with swagger, it shows 400 error, How can I fix this error?
-I was trying to add searching and status filtering to Get All Customers. Here is my current IQueryable code. Review it and write the correct query without executing the database query too early.
-I have written sorting using sortBy and sortOrder, but invalid sort fields cause an unexpected error. Review my code and write safe sorting validation.
-I implemented Get All Customers, but it is not showing mobile numbers, addresses, and documents. What is missing from my code?
-Get All Customers endpoint is working. How can I return TotalRecords, TotalPages, HasPreviousPage, and HasNextPage?
-I don’t understand how build the update customer service, give a proper roadmap to build the update customer service.
-I was trying to create Update Customer using a separate service and repository. Here is my request DTO and existing repository code. Review my design and write the missing update method.
-This is my Update Customer implementation. Mobile and address IDs change after every update. Explain why this happens and suggest a way to preserve existing child-record IDs.
-Update Customer endpoint updates the main customer information, but mobile numbers and addresses remain unchanged. Find the problem in my service and repository and write the corrected code.
-Rewrite this UpdateCustomerService code and identify any validation, null-handling, duplicate-checking, or data-consistency problems.
-A submitted mobile ID must belong to the customer being updated. How can I validate this safely before updating the database?
-Customer update currently removes every mobile and address and inserts them again. Compare this approach with updating existing child records, adding new records, and deleting omitted records.
-I wrote update logic that removes all mobile and address records and adds them again. It works, but the child IDs change every time. Rewrite this method so existing records are updated without changing their IDs.
-Review my duplicate-checking logic for email, National ID, and mobile numbers. Confirm that the current customer is excluded during an update.
-I had created the Delete Customer repository, but deleting a customer causes a foreign-key error. Review my relationships and delete code and explain whether I need Include, manual child deletion, or cascade delete.
-Delete Customer service returns success even when the customer does not exist. Here is my repository and service code. Write the correct not-found handling.
-Review the HTTP status codes used by my API for invalid requests, customer not found, duplicate data, and unexpected server errors. Show corrections where necessary.
-I was trying to return 400 for invalid IDs and 404 for missing customers. Review this controller action and write the correct response-handling code.
-I created a customer document upload feature, but Swagger shows a JSON request instead of a file-selection input. Here is my request model and controller action. Write the correct multipart/form-data implementation.
-My uploaded file is saved in the folder, but its metadata is not saved in CustomerDocuments. Review my service and repository flow and write the missing database-save code.
-My document upload accepts unsupported files such as .exe. Here is my validation method. Rewrite it to allow only PDF, JPG, JPEG, and PNG files with a maximum size of 5 MB.
-My PUT endpoint shows blank string and integer placeholder values in Swagger. Why does it not automatically show the selected customer's existing information?
-Review my Entity Framework Core DbContext and confirm whether it can receive different connection strings for local development and Docker.
-Dockerfile and Docker Compose setup for an ASP.NET Core API and SQL Server. Identify anything that would prevent an evaluator from running the project with minimal configuration.
-Dockerfile and Docker Compose setup for an ASP.NET Core API and SQL Server. Identify anything that would prevent an evaluator from running the project with minimal configuration.
-This Docker build is failing with the following error. Explain the cause and show only the required correction without redesigning the project.
```

### Verification of AI-Generated Code

AI-generated code was not used without verification. The code was checked by:

- Reviewing the generated logic manually 
- Building the application in Visual Studio 
- Testing API endpoints through Swagger 
- Checking database changes in SQL Server 
- Testing valid and invalid request payloads 
- Reviewing HTTP status codes and error responses
- Debugging and correcting compilation and runtime errors

### Modifications Made

The generated code was modified to match the project requirements, including:

- Adapting code to the existing three-layer architecture  
- Updating database entity and property names 
- Adding project-specific validation rules 
- Changing API route names 
- Updating dependency injection registrations 
- Modifying connection strings 
- Adjusting Swagger configuration 
- Customizing pagination and response formats

## Author

Name: Md Arman Islam  
Email: mdarmanislam20021@gmail.com  
GitHub: https://github.com/Arman20021