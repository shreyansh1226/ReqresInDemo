# Reqres.in API Client

.NET solution for interacting with reqres.in API

## Solution Structure
- `src/ReqresInApiClient`: Class library with core logic
- `src/ReqresInDemo.Console`: Console demo application
- `tests/ReqresInApiClient.Tests`: Unit tests

## Features
- HTTP client with Polly retry policies
- In-memory caching
- Configurable settings (base URL, cache duration, retries)
- Error handling with custom exceptions
- Pagination handling
- Unit tests

## Getting Started

1. Clone repository

2. Configure Startup Project
   - Set `ReqresInDemo.Console` as the startup project in your IDE
