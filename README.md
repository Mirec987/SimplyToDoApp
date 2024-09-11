# ToDoApp

A simple ToDo application built with ASP.NET Core that uses Redis for caching and SignalR for real-time notifications. The app also integrates with a SQL database and can be extended to include other technologies.

## Features

- **ToDo Management**: Allows you to create, update, and delete ToDo items.
- **Redis Caching**: Caches the list of ToDo items to reduce database load and improve performance.
- **SignalR**: Sends real-time notifications to clients when ToDo items are added, updated, or deleted.
- **SQL Database**: Stores ToDo items in a SQL database (using Entity Framework Core).

## Technologies

- **.NET 8**: Core framework for building the web API.
- **Entity Framework Core**: For database access and migrations.
- **Redis**: Distributed caching for better performance.
- **SignalR**: Real-time communication for client notifications.
- **MsSQL Server**: SQL database to store ToDo items.
- **Docker**: Optionally used to run Redis and other dependencies in containers.

## Prerequisites

- [.NET SDK 8.0+](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (or LocalDB)
- [Redis](https://redis.io/download) (local or cloud)

## Configure appsettings

Make sure you have the correct configurations in appsettings.json for your SQL Server and Redis connections.
