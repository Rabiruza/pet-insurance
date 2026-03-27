# Pet Insurance Test Automation Framework

Цей проєкт демонструє ключові технології з вакансії GlobalLogic:
- C# + Playwright для UI automation
- RestSharp для API testing
- Database testing (SQL/NoSQL patterns)
- Azure cloud concepts (Functions, Service Bus)
- CI/CD pipeline integration

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
