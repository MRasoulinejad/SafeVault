# 🔐 SafeVault — Secure Minimal Web API (Test Project)

![.NET](https://img.shields.io/badge/.NET-9.0-blueviolet)
![Status](https://img.shields.io/badge/status-Completed-brightgreen)
![License](https://img.shields.io/badge/license-MIT-lightgrey)

**SafeVault** is a **test project** developed to explore and implement secure coding practices using **ASP.NET Core Minimal APIs**.

> 🧪 This project was built for learning purposes as part of a multi-step assignment focused on preventing common web vulnerabilities, including SQL Injection and Cross-Site Scripting (XSS), and implementing modern authentication and authorization.

---

## ✨ Features

- ✅ Input validation & sanitization
- ✅ Protection against SQL Injection
- ✅ XSS prevention through regex-based cleaning
- ✅ Password hashing with **BCrypt**
- ✅ JWT-based authentication
- ✅ Role-based authorization (RBAC)
- ✅ Automated security tests with **NUnit**

---

## 🛠️ Tech Stack

- **ASP.NET Core 9.0**
- **SQLite**
- **JWT Bearer Authentication**
- **BCrypt.Net**
- **NUnit (unit testing)**

---

## 📁 Project Structure

SafeVault/ ├── SafeVault/ # Main API project │ ├── Program.cs │ ├── Models/ │ ├── Services/ │ └── SafeVault.csproj ├── SafeVault.Tests/ # NUnit Test Project │ └── SecurityTests.cs ├── SafeVault.sln # Solution File



🔐 Sample Endpoints
Endpoint	Method	Auth Required	Description
/register	POST	❌	Register a new user with role
/login	POST	❌	Login and receive a JWT token
/profile	GET	✅ Any user	View user info from JWT claims
/admin	GET	✅ Admin only	Protected admin dashboard endpoint


🧪 Security Tests Included
✅ SQL Injection
✅ XSS via <script>
✅ Role restriction enforcement
✅ Password hash validation

⚠️ Disclaimer
This is a test/learning project and not intended for production use.



