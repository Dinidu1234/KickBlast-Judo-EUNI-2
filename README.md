# KickBlast Lux UI

Premium WPF training fee calculation system for KickBlast Judo, built with .NET 8, WPF + MVVM, EF Core, and SQLite.

## Requirements
- Windows 10/11
- Visual Studio 2022 (17.8+) with **.NET desktop development** workload
- .NET 8 SDK

## Solution Structure
```
KickBlastLuxUI.sln
/KickBlastLuxUI           (WPF UI)
/KickBlastLuxUI.Services  (domain + app services)
/KickBlastLuxUI.Data      (EF Core + SQLite)
```

## Setup & Run
1. Open `KickBlastLuxUI.sln` in Visual Studio.
2. Restore NuGet packages.
3. Set **KickBlastLuxUI** as the startup project.
4. Press **F5** to run.

The SQLite database will be created on first run at:
```
KickBlastLuxUI/bin/Debug/net8.0-windows/Data/kickblastlux.db
```

## Pricing Settings
Pricing values are stored in `KickBlastLuxUI/appsettings.json` under `Pricing`.
Use the **Settings** page to update pricing at runtime; the file is saved and applied immediately.

## Reset Database
Use **Settings → Database → Reset Database** to drop and recreate the local database with seed data.

## Architecture Overview
- **UI (Views + ViewModels)**: Strict MVVM; commands and bindings only.
- **Services**: Pricing configuration, calculation rules, theme management.
- **Data**: EF Core DbContext with seeded training plans and athletes.

## Key Features
- Training plan management and athlete CRUD.
- Fee calculation with validation (coaching hours 0–5, competitions >= 0, beginner restriction).
- Dashboard KPIs and recent calculations.
- History explorer with filter and export.
- Light/Dark theme toggle and accent color selection.

