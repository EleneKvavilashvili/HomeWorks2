A REST API for managing users and loan requests, built with ASP.NET Core 10 (.NET 10). Users can register, request loans, and manage their own profile; Accountants can review and manage all users and loans. It is equipped with JWT Token authorization, role-based authentication (User / Accountant), logging system and Swagger/OpenAPI documentation. 

Features
•	JWT authentication with two roles: User and Accountant
•	User management — register, login, view/update/delete profile, change password
•	Loan management — request, view, update, and delete loans while they're still InProcess
•	Accountant tools — view/filter all loans, view/manage any user, block/unblock users
•	Action trail — every significant action and error is written to ActionLogs / ErrorLogs tables
•	Validation with FluentValidation, password hashing with BCrypt
•	Swagger/OpenAPI UI for exploring and testing endpoints
•	Unit tests (xUnit + Moq) covering services and controllers

Technical specs
•	Framework: .NET 10.0 (ASP.NET Core Web API)
•	ORM: Entity Framework Core
•	Database: Microsoft SQL Server
•	Security & Auth: JWT Bearer Tokens, BCrypt.Net
•	Object Mapping: AutoMapper
•	Documentation: Swagger / OpenAPI (Swashbuckle)
•	Testing: xUnit, Moq, EF Core InMemory provider

Getting Started
•	Ensure that the correct connection string is specified in the “appsettings.json” file: 
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=LoanAPIDb;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "AppSettings": {
    "Secret": "a-long-random-secret-at-least-32-bytes"
  }
}
•	Run the API: using dotnet run or F5, By default (see launchSettings.json):
HTTP: http://localhost:5195
HTTPS: https://localhost:7152
Swagger UI opens automatically in development at /swagger.
•	Run tests using dotnet test


Roles
The system supports two distinct user roles:
•	User: Can manage their own profile, request loans, view, update, or delete their pending loan requests.
•	Accountant: Has full access to view all users and loans, filter loan applications, and block/unblock users. Accountants cannot apply for loans or be blocked.

Authenticating in Swagger:
•	Register a user via POST /api/users/register.
•	Authenticate via POST /api/users/login to receive a JWT token.
•	Click the Authorize button in the top right corner of Swagger UI.
•	Paste the token (Bearer <your_token>) and click Authorize.
Tokens carry the user's Name and Role (User or Accountant) claims and expire after 7 days.


API Endpoints Overview
1. User Controller (/api/users)
•	POST /api/users/register
o	Description: Registers a new user. All usernames are unique.
o	Access: Public.
o	Note: Setting "isAccountant": true creates an Accountant profile.
•	POST /api/users/login
o	Description: Authenticates credentials and returns a JWT token.
o	Access: Public.
•	GET /api/users/Accountant
o	Description: Retrieves a list of all registered users.
o	Access: Accountant only.
•	GET /api/users/User/{requestedUsername}
o	Description: Fetches user details by username.
o	Access: Authorized (Regular users can only view their own profile; Accountants can view any profile).
•	PUT /api/users/Update/{requestedUsername}
o	Description: Updates user profile information.
o	Access: Authorized (Self-update or Accountant).
•	PUT /api/users/ChangePassword
o	Description: Updates password for the authorized user (requires current password).
o	Access: Authorized.
•	PUT /api/users/block/{requestedUsername}
o	Description: Blocks or unblocks a specified user (isBlocked: true/false).
o	Access: Accountant only.
•	DELETE /api/users/Delete/{requestedUsername}
o	Description: Deletes a user profile.
o	Access: Authorized. Users with active (non-rejected) loans cannot be deleted.


2. Loan Controller (/api/loans)
•	POST /api/loans/Request
o	Description: Submits a new loan application.
o	Access: Regular Users only. (Blocked users and Accountants are restricted).
o	Note: UserId is automatically assigned based on the authenticated token.
•	GET /api/loans/{requestedUsername}
o	Description: Retrieves loans associated with a specific username (supports filtering by loanId, type, and status).
o	Access: Authorized (Users can view their own loans; Accountants can view any user's loans).
•	GET /api/loans/Accountant/FilterLoans
o	Description: Retrieves and filters all system loans by type and status.
o	Access: Accountant only.
•	PUT /api/loans/Update/{id}
o	Description: Updates loan details.
o	Access: Authorized. Updates are restricted to loans currently InProcess. Regular users can only update their loans.
•	DELETE /api/loans/Delete/{id}
o	Description: Cancels/Deletes a loan request.
o	Access: Authorized. Only loans with an InProcess status can be deleted.

Action & Error Logging
The API features built-in database logging services:
•	ActionLogs: Records system actions (UserRegistered, UserLoggedIn, LoanCreated, etc.) with user IDs, loan IDs, and UTC timestamps.
•	ErrorLogs: Captures failure reasons and validation issues categorized by ErrorType.

