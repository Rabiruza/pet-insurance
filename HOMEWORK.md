# REST API Testing - Homework Assignments

## 📚 Course Structure

This course has **4 lessons** with hands-on practice using RESTful Booker API.

---

## 🎯 Lesson 1: GET Requests & HTTP Basics

### What You Learn:
- Making GET requests
- Checking HTTP status codes (200, 404)
- Validating response data
- Using Assert vs FluentAssertions

### Files to Study:
- `PetInsurance.Tests/Tests/API/RestfulBooker/Lesson1_GetBookingTests.cs`
- `PetInsurance.Tests/ApiClients/RestfulBooker/RestfulBookerClient.cs`

### Postman Practice:
1. Import `RESTful_Booker_Learning.postman_collection.json` into Postman
2. Run all requests in "01. Health Check" and "03. GET Bookings" folders
3. Observe the response bodies and status codes

### Homework Tasks:

#### Task 1.1: Run Existing Tests
```bash
dotnet test --filter "Category=API&Category=Lesson1"
```
- ✅ All tests should pass
- 📝 Note which tests use `Assert` vs `FluentAssertions`

#### Task 1.2: Add a New Test
Add a test that verifies:
- Get booking ID 2 exists
- Check that `totalprice` is greater than 0
- Check that `bookingdates` is not null

#### Task 1.3: Postman Exploration
- In Postman, create a NEW request (not from collection)
- GET `https://restful-booker.herokuapp.com/booking`
- Save this request to a new collection called "My Practice"

---

## 🎯 Lesson 2: POST - Creating Resources

### What You Learn:
- Creating resources with POST
- Sending JSON body
- Validating 201 Created
- Extracting data from responses

### Files to Study:
- `PetInsurance.Tests/Tests/API/RestfulBooker/Lesson2_CreateBookingTests.cs`

### Postman Practice:
1. Run requests in "04. POST Create Booking" folder
2. Notice the response contains `bookingid` - save this!

### Homework Tasks:

#### Task 2.1: Run Tests
```bash
dotnet test --filter "Category=API&Category=Lesson2"
```

#### Task 2.2: Create Booking with Different Data
Add a test that creates a booking with:
- Your name as firstname/lastname
- `totalprice` = 500
- `depositpaid` = false
- `additionalneeds` = "Extra pillows"

Verify the response matches your input.

#### Task 2.3: Edge Case Testing
Create tests for:
- Booking with very long firstname (50+ characters)
- Booking with `totalprice` = 0
- Booking with only `firstname` (no other fields)

**Question:** Which of these succeed? Which fail? Why?

#### Task 2.4: Postman Variables
1. In Postman, create a POST request to create a booking
2. Use the "Tests" tab to extract `bookingid` from response:
```javascript
const jsonData = pm.response.json();
pm.collectionVariables.set("myBookingId", jsonData.bookingid);
```
3. Use `{{myBookingId}}` in a subsequent GET request

---

## 🎯 Lesson 3: Authentication & Updates (PUT/PATCH)

### What You Learn:
- Authentication flow
- Using tokens (cookies)
- PUT vs PATCH difference
- Testing 401/403 errors

### Files to Study:
- `PetInsurance.Tests/Tests/API/RestfulBooker/Lesson3_UpdateBookingTests.cs`

### Key Concept:
| PUT | PATCH |
|-----|-------|
| Replaces ENTIRE resource | Updates ONLY provided fields |
| All fields required | Only changed fields needed |
| Like rewriting a document | Like editing specific words |

### Postman Practice:
1. Run "02. Authentication" - get a token
2. Copy the token value
3. Run "05. PUT Update Booking" with your token
4. Run "06. PATCH Partial Update" with your token
5. Notice the difference in request body size

### Homework Tasks:

#### Task 3.1: Run Tests
```bash
dotnet test --filter "Category=API&Category=Lesson3"
```

#### Task 3.2: Understand Token Authentication
Write a test that:
1. Gets a token
2. Creates a booking
3. Updates it with PATCH (change only `firstname`)
4. Verifies ONLY `firstname` changed (other fields same)

#### Task 3.3: Negative Testing
Create tests for:
- PUT without token → expect failure
- PATCH with wrong token → expect failure
- PUT to non-existent booking → expect 404

#### Task 3.4: Postman Challenge
1. Create a token in Postman
2. Automatically save it to a variable:
```javascript
// In Tests tab of auth request
const jsonData = pm.response.json();
pm.collectionVariables.set("token", jsonData.token);
```
3. Use `{{token}}` in Cookie header: `token={{token}}`

---

## 🎯 Lesson 4: DELETE & Complete CRUD Workflows

### What You Learn:
- DELETE operations
- 204 No Content responses
- Complete CRUD workflows
- Test cleanup patterns
- Idempotent operations

### Files to Study:
- `PetInsurance.Tests/Tests/API/RestfulBooker/Lesson4_DeleteBookingTests.cs`

### Postman Practice:
1. Run "07. DELETE Booking"
2. Notice: No response body, just status 204

### Homework Tasks:

#### Task 4.1: Run Tests
```bash
dotnet test --filter "Category=API&Category=Lesson4"
```

#### Task 4.2: Create Your Own CRUD Test
Write a complete test that:
1. CREATE: Make a new booking with your name
2. READ: Verify it was created
3. UPDATE: Change the price
4. READ: Verify the update
5. DELETE: Remove the booking
6. READ: Verify it's deleted

Use `try/finally` for cleanup!

#### Task 4.3: Test Idempotency
Research: What does "idempotent" mean?
- Which HTTP methods are idempotent? (GET, PUT, PATCH, DELETE)
- Why is POST NOT idempotent?

Write a test that demonstrates DELETE idempotency.

---

## 🏆 Final Project: QA Test Suite

### Assignment: Create a Complete Test Suite

Create a new test file: `RestfulBookerQaSuite.cs`

**Requirements:**

1. **Happy Path Test** (positive scenario)
   - Create booking → Update → Verify → Delete

2. **Error Handling Tests** (negative scenarios)
   - Invalid auth credentials
   - Update non-existent booking
   - Delete without auth

3. **Data Validation Tests**
   - Special characters in names
   - Boundary values (price = 0, price = 9999)
   - Required vs optional fields

4. **Integration Test**
   - Full workflow with proper cleanup
   - Log all steps for debugging

### Example Structure:
```csharp
[TestFixture]
[Category("API")]
[Category("FinalProject")]
public class RestfulBookerQaSuite : BaseApiTest
{
    [Test]
    public void CompleteBookingWorkflow_HappyPath_Success()
    {
        // Your implementation here
    }

    [Test]
    public void CreateBooking_WithInvalidData_ReturnsError()
    {
        // Your implementation here
    }
}
```

---

## 📝 Knowledge Check Questions

Answer these questions (write answers in comments or discuss with mentor):

### HTTP Basics:
1. What's the difference between POST and PUT?
2. What does HTTP 204 mean?
3. What does "idempotent" mean? Give examples.

### RESTful Booker:
4. How do you authenticate with the API?
5. What's the difference between PUT and PATCH?
6. What happens when you DELETE a non-existent booking?

### Testing:
7. Why use `try/finally` in tests?
8. What's the difference between `Assert.That()` and FluentAssertions?
9. When should you clean up test data?

### QA Mindset:
10. What's a "positive test" vs "negative test"?
11. Why test error cases (404, 403)?
12. What makes a good API test?

---

## 🎓 Next Steps

After completing all lessons:

1. **Review** all test files
2. **Run** all tests: `dotnet test --filter Category=API`
3. **Experiment** with modifying tests
4. **Explore** other APIs (PetStore, ReqRes, etc.)
5. **Learn** about:
   - API mocking
   - Contract testing
   - Performance testing
   - CI/CD integration

---

## 💡 Tips for Success

1. **Run tests frequently** - Don't wait until everything is perfect
2. **Read error messages** - They tell you what went wrong
3. **Use Postman first** - Manual testing before automation
4. **Commit often** - Save your progress with git
5. **Ask questions** - When stuck, ask for help!

---

## 📞 Need Help?

Common issues and solutions:

| Problem | Solution |
|---------|----------|
| Tests fail with timeout | Check internet connection |
| Token is null | Verify credentials are correct |
| Booking not found | It may have been deleted |
| JSON parsing error | Check model properties match API |

Good luck! 🚀
