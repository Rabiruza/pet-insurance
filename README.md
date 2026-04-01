# Pet Insurance Test Automation Framework

Цей проєкт демонструє ключові технології з вакансії GlobalLogic:
- C# + Playwright для UI automation
- RestSharp для API testing
- Database testing (SQL/NoSQL patterns)
- Azure cloud concepts (Functions, Service Bus)
- CI/CD pipeline integration

---

## 🎓 REST API Testing Course for QA Engineers

This project includes a **complete REST API testing course** using RESTful Booker API.

### 📖 Course Materials:

| Document | Description |
|----------|-------------|
| [`LEARNING_GUIDE.md`](./LEARNING_GUIDE.md) | Theory: HTTP, REST, JSON, testing concepts |
| [`HOMEWORK.md`](./HOMEWORK.md) | Practical assignments for each lesson |

### 🗂️ Course Structure:

```
PetInsurance.Tests/
├── ApiClients/RestfulBooker/
│   ├── IRestfulBookerClient.cs    # Interface definition
│   └── RestfulBookerClient.cs     # Implementation
├── Models/RestfulBooker/
│   └── Booking.cs                 # Data models
├── Tests/API/RestfulBooker/
│   ├── Lesson1_GetBookingTests.cs     # GET requests
│   ├── Lesson2_CreateBookingTests.cs  # POST requests
│   ├── Lesson3_UpdateBookingTests.cs  # PUT/PATCH + Auth
│   └── Lesson4_DeleteBookingTests.cs  # DELETE + CRUD
└── Postman/
    └── RESTful_Booker_Learning.postman_collection.json
```

### 🚀 Quick Start - Run All API Tests:

```bash
# Run all API tests
dotnet test --filter Category=API

# Run specific lesson
dotnet test --filter "Category=API&Category=Lesson1"
dotnet test --filter "Category=API&Category=Lesson2"
dotnet test --filter "Category=API&Category=Lesson3"
dotnet test --filter "Category=API&Category=Lesson4"

# Run with detailed output
dotnet test --logger "console;verbosity=detailed" --filter Category=API
```

### 📬 Postman Setup:

1. Open Postman
2. Click **Import**
3. Select `PetInsurance.Tests/Postman/RESTful_Booker_Learning.postman_collection.json`
4. Start exploring the API manually!

---

## Структура проєкту

```
PetInsuranceTestFramework/
├── PetInsurance.Tests/
│   ├── Config/              # Налаштування та константи
│   ├── Core/                # Базові класи фреймворку
│   ├── PageObjects/         # Page Object Model для UI
│   ├── ApiClients/          # API клієнти (RestSharp)
│   ├── DatabaseHelpers/     # SQL/NoSQL helpers
│   ├── Tests/
│   │   ├── UI/             # Playwright UI тести
│   │   ├── API/            # API тести
│   │   └── Integration/    # Інтеграційні тести
│   └── Utils/              # Допоміжні класи
├── azure-pipelines.yml     # CI/CD конфігурація
└── README.md
```

## Швидкий старт

### 1. Встановлення залежностей
```bash
dotnet restore
pwsh bin/Debug/net8.0/playwright.ps1 install
```

### 2. Запуск тестів
```bash
# Всі тести
dotnet test

# Тільки UI тести
dotnet test --filter Category=UI

# Тільки API тести
dotnet test --filter Category=API

# З детальним логуванням
dotnet test --logger "console;verbosity=detailed"
```

##  Що покриває проєкт

### День 1: UI + API Testing
- Page Object Model pattern
- Playwright best practices
- RestSharp API клієнти
- Data-driven тести

### День 2: Azure + Database
- Azure Functions simulation
- Service Bus patterns
- SQL query тести
- NoSQL (Cosmos DB patterns)

### День 3: CI/CD + Interview Prep
- Azure DevOps pipeline
- Kubernetes/AKS concepts
- Типові питання співбесіди

##  Тестові endpoints

**UI Testing:**
- https://petstore.swagger.io (Pet Store demo)
- https://demoqa.com (різні UI елементи)

**API Testing:**
- https://petstore.swagger.io/v2 (Pet Store API)
- https://restful-booker.herokuapp.com (Booking API)

##  Ключові паттерни

1. **Page Object Model** - чітка структура UI тестів
2. **Builder Pattern** - для створення тестових даних
3. **Factory Pattern** - для ініціалізації клієнтів
4. **Repository Pattern** - для database операцій
5. **Singleton** - для конфігурації
