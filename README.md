# Music-Booking-App
# 🎵 Music Booking API

## Overview
The **Music Booking API** is a web application that facilitates booking and event management for artistes and event organizers. It includes authentication with OTP verification, role-based access, and payment integration using **Paystack**. Users can sign up as **Artists, Event Organizers, Regular Users, or Admins** to perform different actions within the system.

## Features
### 🛡️ Authentication
- User registration with role selection (**Artist, Event Organizer, User, Admin**)
- OTP-based authentication (**Send OTP, Verify OTP**)
- JWT-based authorization for secured API access

### 🎭 User Roles & Profiles
- **Artist**: Can create and manage their artist profile
- **Event Organizer**: Can create events and book artists
- **Regular User**: Can purchase tickets for events
- **Admin**: Manages platform operations

### 🎟️ Booking & Ticketing
- **Event organizers** can **propose bookings** to artists
- **Artists** can **approve or reject** bookings
- **Users** can **purchase tickets** for events
- Integrated **Paystack** for **secure payments**

## 📡 API Endpoints

### Authentication
| Endpoint | Method | Description |
|----------|--------|-------------|
| `/api/auth/register` | `POST` | Register a user with role |
| `/api/auth/login` | `POST` | Login and receive JWT token |
| `/api/auth/send-otp` | `POST` | Send OTP for authentication |
| `/api/auth/verify-otp` | `POST` | Verify OTP for authentication |

### Artist & Event Management
| Endpoint | Method | Description |
|----------|--------|-------------|
| `/api/artists/profile` | `POST` | Create an artist profile |
| `/api/events/create` | `POST` | Create an event |
| `/api/events/{id}` | `GET` | Fetch event details |

### Booking
| Endpoint | Method | Description |
|----------|--------|-------------|
| `/api/bookings/propose` | `POST` | Propose a booking request |
| `/api/bookings/approve/{id}` | `POST` | Approve a booking |
| `/api/bookings/pay/{id}` | `POST` | Make payment for a booking |

### Ticketing
| Endpoint | Method | Description |
|----------|--------|-------------|
| `/api/tickets/purchase` | `POST` | Purchase event tickets |

## 💰 Payment Integration
The API integrates **Paystack** for secure online payments. Payments are required for:
- Booking approvals (organizer pays the artist)
- Ticket purchases (users buy tickets for events)

## 🏗️ Tech Stack
- **C# .NET** (ASP.NET Core)
- **Entity Framework Core** (Database ORM)
- **JWT Authentication** (Secure user access)
- **FluentValidation** (Input validation)
- **Paystack API** (Online payments)
- **Swagger** (API documentation)

## 🔧 Setup & Installation
### Prerequisites
- **.NET 8** installed
- **SQL Server** (or preferred DB)
- **Paystack Secret Key** for payments

### Configure Environment Variables
Create `appsettings.json` and update the Paystack key:
```json
{
  "Jwt": {
    "Key": "your_secret_key",
    "Issuer": "your_issuer"
  },
  "Paystack": {
    "SecretKey": "your_paystack_secret_key"
  }
}
```

### Run the API
```sh
dotnet build
dotnet run
```

## 📜 License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🤝 Contributing
Pull requests are welcome! Open an issue to discuss any improvements or feature requests.

## 🚀 API Documentation
Once the API is running, you can access the **Swagger UI**:
```
http://localhost:7082/swagger
```


