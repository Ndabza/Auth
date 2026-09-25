  # Auth API (.NET 10 Microservice)

A production-ready authentication and media metadata backend built using **.NET 10**, **PostgreSQL**, and **Azurite Storage Emulator**. The entire ecosystem is containerised with **Docker** and **Docker Compose** for seamless local development and deployment orchestration.

## 🏗️ Architecture: Clean Architecture

The solution is decoupled into four distinct layers following the principles of Domain-Driven Design (DDD) and Clean Architecture:

[ Domain ] <---- [ Application ] <---- [ Infrastructure ]
^
|
[ Auth.Api ]

---

## 🚀 Quick Start (Local Run via Docker)

### Prerequisites
Make sure you have [Docker Desktop](https://docker.com) installed on your machine.

### Step 1: Clone the Repository
```bash
git clone https://github.com/Ndabza/Auth
cd Auth
```

### Step 2: Spin Up the Stack
Run the following Docker Compose command from the root directory to build the Multi-stage API image and fetch database infrastructure:
```bash
docker compose up -d --build
```

Docker will automatically orchestrate:
1.  **PostgreSQL** running on port `5432`
2.  **Azurite Storage Emulator** mapping ports `10000-10002`
3.  **Auth.Api** compiling via optimized .NET 10 multi-stage configuration on port `5000`

### Step 3: Access the API
Once the containers report a healthy state, navigate to the local interactive Swagger OpenAPI platform inside your browser:

👉 **`http://localhost:5000/swagger`**

---

## 🔐 Configuration & Environment Variables

When running via Docker Compose, secrets are managed securely using default fallback values that can be overridden using an environmental `.env` file at the project root.

```env
# Database Credentials
DB_USER=postgres
DB_PASSWORD=mysecretpassword!
DB_NAME=authdb

# API Run Mode
ASPNETCORE_ENVIRONMENT=Development
```

### Authentication Header Usage
To consume the secured `/api/auth/movies` endpoint, inject your JWT into the API call header exactly like this:
```http
Authorization: Bearer <your_jwt_access_token>
```

---

## 📂 Git & Local Volumes Persistence
Your local development metadata is safely mapped to persistent Docker volumes. Stopping your container stack using `docker compose down` will **not** wipe out your user database or emulator files. 

The tracking of temporary storage assets is omitted from version control globally via the active repository `.gitignore`.
