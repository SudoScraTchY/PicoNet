# User and Role Management API Documentation

This document describes the User and Role management endpoints that were added to PicoNet for admin user management.

## Authorization

All endpoints in `UsersController` and `RolesController` require the `RequireAdmin` policy, which means the user must have the `Admin` role.

Authorization policy is set in `PicoNet.Api/Program.cs`:
```csharp
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdmin", policy => policy.RequireRole("Admin"));
});
```

Default roles "User" and "Admin" are created on application startup after migrations run.

---

## Users Controller

**Base Route:** `api/users`

All endpoints require `[Authorize(Policy = "RequireAdmin")]`

### List Users
**Endpoint:** `GET /api/users`

**Query Parameters:**
- `pageNumber` (int, optional, default: 1)
- `pageSize` (int, optional, default: 10)

**Response:**
```json
{
  "users": [
    {
      "id": "00000000-0000-0000-0000-000000000000",
      "username": "john_doe",
      "email": "john@example.com",
      "emailConfirmed": false,
      "twoFactorEnabled": false,
      "lockoutEnabled": true,
      "accessFailedCount": 0,
      "createdAt": "2026-06-27T00:00:00Z",
      "roles": ["User"]
    }
  ],
  "totalCount": 50,
  "pageNumber": 1,
  "pageSize": 10
}
```

### Get User by ID
**Endpoint:** `GET /api/users/{userId}`

**Parameters:**
- `userId` (guid, required) — User ID

**Response:**
```json
{
  "id": "00000000-0000-0000-0000-000000000000",
  "username": "john_doe",
  "email": "john@example.com",
  "emailConfirmed": false,
  "twoFactorEnabled": false,
  "lockoutEnabled": true,
  "accessFailedCount": 0,
  "createdAt": "2026-06-27T00:00:00Z",
  "roles": ["User"]
}
```

**Status Codes:**
- `200 OK` — User found
- `404 Not Found` — User not found

### Create User
**Endpoint:** `POST /api/users`

**Request Body:**
```json
{
  "username": "jane_doe",
  "email": "jane@example.com",
  "password": "SecurePassword123",
  "roles": ["User", "Admin"]
}
```

**Response:** Same as `UserResponse` (201 Created)

**Status Codes:**
- `201 Created` — User created successfully
- `400 Bad Request` — Invalid request data
- `409 Conflict` — User already exists (email or username conflict)

**Notes:**
- If no roles are provided, the user is assigned the "User" role by default.
- Password must be at least 8 characters.
- Email must be unique.
- Username must be unique.

### Update User
**Endpoint:** `PATCH /api/users/{userId}`

**Parameters:**
- `userId` (guid, required)

**Request Body (all optional):**
```json
{
  "username": "jane_doe_updated",
  "email": "jane_new@example.com",
  "isActive": true
}
```

**Response:** Updated `UserResponse`

**Status Codes:**
- `200 OK` — User updated
- `404 Not Found` — User not found
- `409 Conflict` — Email/Username already in use

**Notes:**
- `isActive` when set to `false` locks the user (sets lockout indefinitely).
- `isActive` when set to `true` resets lockout and access failed count.

### Delete User
**Endpoint:** `DELETE /api/users/{userId}`

**Parameters:**
- `userId` (guid, required)

**Response:** 204 No Content

**Status Codes:**
- `204 No Content` — User deleted
- `404 Not Found` — User not found

### Manual Verify User
**Endpoint:** `POST /api/users/{userId}/verify`

**Parameters:**
- `userId` (guid, required)

**Request Body:**
```json
{
  "setEmailConfirmed": true,
  "setPhoneConfirmed": false
}
```

**Response:** Updated `UserResponse`

**Status Codes:**
- `200 OK` — User verified
- `404 Not Found` — User not found

**Notes:**
- Use this endpoint to manually mark a user's email/phone as confirmed.
- Useful for admin-verified registrations.

### Force Password Change
**Endpoint:** `POST /api/users/{userId}/force-password-change`

**Parameters:**
- `userId` (guid, required)

**Request Body:**
```json
{
  "newPassword": "NewSecurePassword123",
  "sendEmailNotification": true
}
```

**Response:** Updated `UserResponse`

**Status Codes:**
- `200 OK` — Password changed
- `404 Not Found` — User not found
- `400 Bad Request` — Invalid password

**Notes:**
- Password must meet complexity requirements (min 8 chars).
- `sendEmailNotification` is optional (default: true).

### Add Role to User
**Endpoint:** `POST /api/users/{userId}/roles`

**Parameters:**
- `userId` (guid, required)

**Request Body:**
```json
{
  "roleName": "Admin"
}
```

**Response:**
```json
{
  "userId": "00000000-0000-0000-0000-000000000000",
  "username": "john_doe",
  "roles": ["User", "Admin"]
}
```

**Status Codes:**
- `200 OK` — Role added
- `404 Not Found` — User or role not found
- `409 Conflict` — User already has this role

### Remove Role from User
**Endpoint:** `DELETE /api/users/{userId}/roles/{roleName}`

**Parameters:**
- `userId` (guid, required)
- `roleName` (string, required) — Name of the role to remove

**Response:** Updated `UserRolesResponse`

**Status Codes:**
- `200 OK` — Role removed
- `404 Not Found` — User or role not found

---

## Roles Controller

**Base Route:** `api/roles`

All endpoints require `[Authorize(Policy = "RequireAdmin")]`

### List Roles
**Endpoint:** `GET /api/roles`

**Response:**
```json
[
  {
    "id": "00000000-0000-0000-0000-000000000000",
    "roleName": "Admin",
    "userCount": 2
  },
  {
    "id": "00000000-0000-0000-0000-000000000001",
    "roleName": "User",
    "userCount": 48
  }
]
```

### Create Role
**Endpoint:** `POST /api/roles`

**Request Body:**
```json
{
  "roleName": "Moderator",
  "description": "Moderator role for content moderation"
}
```

**Response:** `RoleResponse` (201 Created)

**Status Codes:**
- `201 Created` — Role created
- `400 Bad Request` — Invalid request
- `409 Conflict` — Role already exists

### Delete Role
**Endpoint:** `DELETE /api/roles/{roleName}`

**Parameters:**
- `roleName` (string, required) — Name of the role to delete

**Response:** 204 No Content

**Status Codes:**
- `204 No Content` — Role deleted
- `404 Not Found` — Role not found
- `409 Conflict` — Cannot delete built-in roles (User, Admin)

**Notes:**
- Built-in roles `User` and `Admin` cannot be deleted.

---

## Implementation Details

### Project Structure

**Request DTOs:**
- `PicoNet.Contracts/DTOs/Requests/Users/` — User management requests
- `PicoNet.Contracts/DTOs/Requests/Roles/` — Role management requests

**Response DTOs:**
- `PicoNet.Contracts/DTOs/Responses/Users/` — User management responses
- `PicoNet.Contracts/DTOs/Responses/Roles/` — Role management responses

**Commands:**
- `PicoNet.Application/Features/Users/Commands/` — User management commands
- `PicoNet.Application/Features/Roles/Commands/` — Role management commands

**Handlers:**
- `PicoNet.Application/Features/Users/Handler/` — User management handlers
- `PicoNet.Application/Features/Roles/Handler/` — Role management handlers

**Controllers:**
- `PicoNet.Api/Controllers/UsersController.cs`
- `PicoNet.Api/Controllers/RolesController.cs`

### Message Bus Integration

All endpoints use Wolverine's `IMessageBus` to dispatch commands and handlers. Example from `UsersController`:

```csharp
var result = await _bus.InvokeAsync<ErrorOr<UserResponse>>(
    new CreateUserCommand(request.Username, request.Email, request.Password, request.Roles), ct);
```

### Error Handling

All responses follow the `ErrorOr<T>` pattern and convert errors to problem details:

```csharp
return result.Match(Results.Ok, errors => errors.ToProblemResult());
```

Common error codes:
- `User.NotFound` — 404
- `User.EmailExists` — 409 Conflict
- `User.UsernameExists` — 409 Conflict
- `User.CreationFailed` — 409 Conflict
- `Role.AlreadyExists` — 409 Conflict
- `Role.NotFound` — 404

---

## Usage Examples

### Create a new admin user via cURL

```bash
curl -X POST https://localhost:5001/api/users \
  -H "Authorization: Bearer YOUR_ADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin2",
    "email": "admin2@example.com",
    "password": "SecurePassword123",
    "roles": ["Admin"]
  }'
```

### Add Admin role to existing user

```bash
curl -X POST https://localhost:5001/api/users/{userId}/roles \
  -H "Authorization: Bearer YOUR_ADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"roleName": "Admin"}'
```

### Force password change for a user

```bash
curl -X POST https://localhost:5001/api/users/{userId}/force-password-change \
  -H "Authorization: Bearer YOUR_ADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"newPassword": "NewPassword123", "sendEmailNotification": true}'
```

### List all available roles

```bash
curl -X GET https://localhost:5001/api/roles \
  -H "Authorization: Bearer YOUR_ADMIN_TOKEN"
```

