# FinTracker — Personal Finance Manager

Desktop application for tracking income and expenses built with WPF and .NET 8.

## Features
- Add/edit/delete transactions (income & expenses)
- Categorize transactions
- Monthly budget tracking with progress indicators
- Visual charts: expense breakdown by category, monthly trends
- Export transactions to CSV
- Local SQLite database

## Tech Stack
- .NET 8 (WPF)
- Entity Framework Core + SQLite
- LiveCharts2 for data visualization
- CommunityToolkit.Mvvm (MVVM pattern)

## Build & Run
```bash
cd FinTracker
dotnet restore
dotnet build
dotnet run
```

## Project Structure
- `Models/` — Data models (Transaction, Category, Budget)
- `ViewModels/` — MVVM view models with commands and properties
- `Views/` — WPF XAML views
- `Services/` — Business logic and data access
- `Data/` — EF Core database context
