# REST API Testing Learning Path for QA Engineers

## 📚 Course Overview

This course teaches REST API testing using:
- **C# with RestSharp** - for automated API tests
- **Postman** - for manual API exploration and testing
- **RESTful Booker** - our practice API (https://restful-booker.herokuapp.com)

---

## 🎯 What is REST API?

**REST** (Representational State Transfer) is an architectural style for web services.

### Key Concepts:

1. **Resources** - Everything is a resource (users, bookings, products)
2. **HTTP Methods** - Actions you perform on resources
3. **Endpoints** - URLs where resources are accessed
4. **Request/Response** - Client sends request, server returns response

---

## 🔧 HTTP Methods (CRUD Operations)

| Method | Operation | Example | Idempotent |
|--------|-----------|---------|------------|
| **GET** | Read | Get booking by ID | ✅ Yes |
| **POST** | Create | Create new booking | ❌ No |
| **PUT** | Update/Replace | Update entire booking | ✅ Yes |
| **PATCH** | Partial Update | Update just price | ✅ Yes |
| **DELETE** | Delete | Remove booking | ✅ Yes |

**Idempotent** = Multiple identical requests have the same effect as one request

---

## 📦 RESTful Booker API Endpoints

```
Base URL: https://restful-booker.herokuapp.com

Authentication:
POST   /auth                    - Create token

Bookings:
GET    /booking                 - Get all bookings
GET    /booking/{id}            - Get booking by ID
POST   /booking                 - Create new booking
PUT    /booking/{id}            - Update booking
PATCH  /booking/{id}            - Partial update
DELETE /booking/{id}            - Delete booking

Health Check:
GET    /ping                    - Check API is alive
```

---

## 🚀 Module 1: HTTP Basics

### HTTP Request Structure:
```
POST /booking HTTP/1.1
Host: restful-booker.herokuapp.com
Content-Type: application/json
Accept: application/json

{
    "firstname": "John",
    "lastname": "Doe"
}
```

### HTTP Response Structure:
```
HTTP/1.1 201 Created
Content-Type: application/json

{
    "bookingid": 123,
    "booking": { ... }
}
```

### Common HTTP Status Codes:

| Code | Meaning | Description |
|------|---------|-------------|
| 200 | OK | Request succeeded |
| 201 | Created | Resource created |
| 204 | No Content | Delete succeeded |
| 400 | Bad Request | Invalid input |
| 401 | Unauthorized | Authentication required |
| 403 | Forbidden | Access denied |
| 404 | Not Found | Resource doesn't exist |
| 500 | Internal Server Error | Server error |

---

## 📝 Module 2: JSON Format

**JSON** (JavaScript Object Notation) is the data format for API requests/responses.

### JSON Example:
```json
{
    "firstname": "John",
    "lastname": "Doe",
    "totalprice": 100,
    "depositpaid": true,
    "bookingdates": {
        "checkin": "2024-01-01",
        "checkout": "2024-01-10"
    },
    "additionalneeds": ["breakfast", "wifi"]
}
```

### JSON Data Types:
- **String**: `"text"`
- **Number**: `100`, `3.14`
- **Boolean**: `true`, `false`
- **Object**: `{ "key": "value" }`
- **Array**: `[1, 2, 3]`
- **Null**: `null`

---

## 🧪 Module 3: Testing Concepts

### What QA Engineers Test:

1. **Status Code** - Did the operation succeed?
2. **Response Body** - Is the data correct?
3. **Response Time** - Is it fast enough?
4. **Headers** - Are content types correct?
5. **Schema Validation** - Does JSON structure match?
6. **Error Handling** - Are errors handled properly?

### Test Case Example:
```
Test: Create a new booking
Given: Valid booking data
When: POST /booking with valid data
Then: 
  - Status code = 201
  - Response contains bookingid
  - Booking data matches request
```

---

## 📖 Next Steps

1. **Module 4**: Postman Basics (Manual Testing)
2. **Module 5**: C# with RestSharp (Automated Testing)
3. **Module 6**: Advanced Testing Patterns
4. **Module 7**: CI/CD Integration

---

## 🏠 Homework

1. Read this guide completely
2. Open Postman and create a GET request to `https://restful-booker.herokuapp.com/ping`
3. Try to identify: Request method, URL, Headers, Response status, Response body

---

## 📞 Support

Ask questions when you're stuck! Common topics:
- Understanding HTTP methods
- JSON structure
- Writing C# tests
- Postman usage
