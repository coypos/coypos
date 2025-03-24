# COYPOS Backend and Admin Panel
COYPOS Backend and Admin Panel are core components of the COYPOS project, a modern point-of-sale (POS) system designed for efficiency and ease of use.

## Project Description
The COYPOS Backend provides the server-side logic and data management for the entire COYPOS system, while the Admin Panel offers a comprehensive interface for system management and configuration.

## Key Features
- RESTful API for POS operations
- Real-time transaction processing
- Inventory and product management
- Reporting and analytics
- Payment system integration
- Comprehensive admin dashboard
- User and employee management
- Multi-language support
- Customizable settings

## Technologies
- ASP.NET Core
- Entity Framework Core
- MS SQL Server
- Vue.js
- TypeScript
- Docker

## System Requirements
- .NET SDK (version 6.0 or newer)
- Node.js (version 14 or newer)
- Docker
- MS SQL Server or PostgreSQL (depending on configuration)

## Installation
Clone the repository:

```bash
git clone https://github.com/coypos/coypos.git
cd coypos
```
Set up the backend:

```bash
cd CoyposServer
dotnet restore
dotnet build
```
Set up the admin panel:

```bash
cd ../coyposadmin
npm install
```
## Running the Application
To run the backend:

```bash
cd CoyposServer
dotnet run
```
To run the admin panel in development mode:

```bash
cd CoyposAdminPanel
npm run serve
```
### Docker Support
The project includes Docker support. Detailed instructions for building and running Docker containers will be added in the future.

### Configuration
Detailed information about project configuration, including database settings and environment variables, can be found in the appsettings.json file for the backend and in the .env file for the admin panel.

### API Documentation
API documentation is available after running the server at /swagger.

### Contributing
We welcome contributions! If you'd like to contribute to COYPOS Backend or Admin Panel, please review our contribution guidelines.

### License
This project is licensed under the MIT License.
