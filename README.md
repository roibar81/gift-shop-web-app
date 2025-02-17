# WebApp Solution

This is a .NET solution containing a web application with separate backend and frontend projects.

## Project Structure

- **WebApp.Api**: Backend Web API project
- **WebApp.Web**: Frontend MVC project with Bootstrap UI

## Prerequisites

- .NET 8.0 SDK
- Visual Studio 2022 or Visual Studio Code

## Getting Started

1. Clone the repository
2. Open the solution in Visual Studio or Visual Studio Code
3. Run both projects:
   - For Visual Studio: Set both projects as startup projects and press F5
   - For command line:
     ```
     # Run the API (from the WebApp.Api directory)
     dotnet run

     # Run the Web frontend (from the WebApp.Web directory)
     dotnet run
     ```

## Development

- The API project runs on `https://localhost:7001` by default
- The Web frontend runs on `https://localhost:7000` by default

## Features

- Modern Bootstrap UI
- RESTful API backend
- Swagger documentation for API endpoints 