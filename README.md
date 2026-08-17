# 🏠 Real Estate

Nền tảng bất động sản trực tuyến được xây dựng với **Angular** (Frontend) và **ASP.NET 9** (Backend), hỗ trợ đăng tin, tìm kiếm, lọc và quản lý bất động sản toàn diện.

---

## 📋 Mục lục

- [Tổng quan](#-tổng-quan)
- [Tính năng](#-tính-năng)
- [Kiến trúc hệ thống](#-kiến-trúc-hệ-thống)
- [Công nghệ sử dụng](#-công-nghệ-sử-dụng)
- [Yêu cầu hệ thống](#-yêu-cầu-hệ-thống)
- [Cài đặt & Chạy dự án](#-cài-đặt--chạy-dự-án)
- [Cấu trúc thư mục](#-cấu-trúc-thư-mục)
- [API Documentation](#-api-documentation)
- [Triển khai (Deploy)](#-triển-khai-deploy)
- [Biến môi trường](#-biến-môi-trường)
- [Đóng góp](#-đóng-góp)

---

## 🌟 Tổng quan

**Real Estate** là ứng dụng web full-stack cho phép người dùng tìm kiếm, đăng tin và quản lý bất động sản (nhà ở, căn hộ, đất nền, thương mại). Hệ thống phân quyền rõ ràng giữa người mua, người bán và quản trị viên.

> **Trạng thái dự án:** 🚧 Đang phát triển

---

## ✨ Tính năng

### Người dùng (User)
- 🔐 Đăng ký / Đăng nhập (JWT + Refresh Token)
- 🔍 Tìm kiếm & lọc bất động sản theo khu vực, giá, loại hình, diện tích
- 📍 Xem bản đồ tích hợp (Google Maps / Leaflet)
- ❤️ Lưu tin yêu thích
- 📞 Liên hệ chủ tin / môi giới
- 🔔 Nhận thông báo khi có tin mới phù hợp

### Người đăng tin (Seller / Agent)
- 📝 Đăng, chỉnh sửa, xóa tin bất động sản
- 🖼️ Upload nhiều ảnh cho mỗi bất động sản
- 📊 Xem thống kê lượt xem tin đăng
- 💼 Quản lý danh sách tin đã đăng

### Quản trị viên (Admin)
- 👥 Quản lý người dùng
- ✅ Duyệt / từ chối tin đăng
- 🏷️ Quản lý danh mục, khu vực
- 📈 Dashboard thống kê tổng quan
- 🗑️ Xóa nội dung vi phạm

---

## 🏗️ Kiến trúc hệ thống

```
┌─────────────────────────────────────────────────────────┐
│                      CLIENT SIDE                        │
│                  Angular (SPA)                          │
│   ┌──────────┐  ┌──────────┐  ┌───────────────────┐    │
│   │  Modules │  │ Services │  │ State (NgRx/Signal)│    │
│   └──────────┘  └──────────┘  └───────────────────┘    │
└─────────────────────┬───────────────────────────────────┘
                      │ HTTPS / REST API
┌─────────────────────▼───────────────────────────────────┐
│                     SERVER SIDE                         │
│               ASP.NET 9 Web API                         │
│   ┌──────────┐  ┌──────────┐  ┌──────────────────┐     │
│   │Controllers│ │ Services │  │  Repositories    │     │
│   └──────────┘  └──────────┘  └────────┬─────────┘     │
│                                        │ EF Core        │
└────────────────────────────────────────┼────────────────┘
                                         │
                          ┌──────────────▼──────────┐
                          │        SQL Server        │
                          └─────────────────────────┘
```

### Các lớp Backend (Clean Architecture)

| Lớp | Mô tả |
|-----|-------|
| `API` | Controllers, Middleware, Swagger |
| `Application` | Use Cases, DTOs, Interfaces, Validators |
| `Domain` | Entities, Business Rules, Domain Events |
| `Infrastructure` | EF Core, Repositories, External Services |

---

## 🛠️ Công nghệ sử dụng

### Frontend
| Công nghệ | Phiên bản | Mục đích |
|-----------|-----------|----------|
| Angular | 17+ | Framework chính |
| TypeScript | 5.x | Ngôn ngữ lập trình |
| Angular Material / TailwindCSS | — | UI Components |
| RxJS | 7.x | Reactive programming |
| NgRx / Angular Signals | — | State management |
| Leaflet / Google Maps API | — | Bản đồ |

### Backend
| Công nghệ | Phiên bản | Mục đích |
|-----------|-----------|----------|
| ASP.NET Core | 9.0 | Web API Framework |
| Entity Framework Core | 9.x | ORM |
| SQL Server | 2022 | Cơ sở dữ liệu chính |
| Redis | 7.x | Caching |
| JWT Bearer | — | Xác thực |
| AutoMapper | — | Object mapping |
| FluentValidation | — | Validation |
| Swagger / Scalar | — | API Documentation |

---

## ⚙️ Yêu cầu hệ thống

| Công cụ | Phiên bản tối thiểu |
|---------|-------------------|
| Node.js | 20.x LTS |
| npm | 10.x |
| Angular CLI | 17.x |
| .NET SDK | 9.0 |
| SQL Server | 2019+ (hoặc Docker) |
| Redis | 7.x (hoặc Docker) |

---

## 🚀 Cài đặt & Chạy dự án

### 1. Clone repository

```bash
git clone https://github.com/your-username/real-estate.git
cd real-estate
```

### 2. Cấu hình Backend

```bash
cd backend

# Khôi phục packages
dotnet restore

# Sao chép file cấu hình
cp appsettings.example.json appsettings.Development.json
```

Chỉnh sửa `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=RealEstateDb;Trusted_Connection=True;TrustServerCertificate=True",
    "Redis": "localhost:6379"
  },
  "JwtSettings": {
    "SecretKey": "your-super-secret-key-min-32-chars",
    "Issuer": "RealEstateApi",
    "Audience": "RealEstateClient",
    "ExpiryMinutes": 60
  },
  "CloudinarySettings": {
    "CloudName": "your-cloud-name",
    "ApiKey": "your-api-key",
    "ApiSecret": "your-api-secret"
  }
}
```

```bash
# Tạo database & chạy migration
dotnet ef database update

# Seed dữ liệu mẫu (tuỳ chọn)
dotnet run --seed

# Chạy server
dotnet run
```

API chạy tại: `https://localhost:7001` | Swagger: `https://localhost:7001/swagger`

---

### 3. Cấu hình Frontend

```bash
cd frontend

# Cài đặt dependencies
npm install

# Sao chép file môi trường
cp src/environments/environment.example.ts src/environments/environment.ts
```

Chỉnh sửa `src/environments/environment.ts`:

```typescript
export const environment = {
  production: false,
  apiUrl: 'https://localhost:7001/api',
  googleMapsApiKey: 'your-google-maps-api-key'
};
```

```bash
# Chạy development server
ng serve

# Mở trình duyệt
# http://localhost:4200
```

---

### 4. Chạy bằng Docker (Khuyến nghị)

```bash
# Khởi động toàn bộ stack
docker-compose up -d

# Xem logs
docker-compose logs -f
```

Dịch vụ sau khi khởi động:

| Dịch vụ | URL |
|---------|-----|
| Frontend | http://localhost:4200 |
| Backend API | http://localhost:7001 |
| Swagger | http://localhost:7001/swagger |
| SQL Server | localhost:1433 |
| Redis | localhost:6379 |

---

## 📁 Cấu trúc thư mục

```
real-estate/
├── frontend/                        # Angular App
│   ├── src/
│   │   ├── app/
│   │   │   ├── core/               # Guards, Interceptors, Services cốt lõi
│   │   │   ├── shared/             # Components, Pipes, Directives dùng chung
│   │   │   ├── features/
│   │   │   │   ├── auth/           # Đăng nhập, đăng ký
│   │   │   │   ├── listings/       # Danh sách & chi tiết BĐS
│   │   │   │   ├── dashboard/      # Quản lý tin đăng
│   │   │   │   ├── admin/          # Trang quản trị
│   │   │   │   └── profile/        # Hồ sơ người dùng
│   │   │   └── app.routes.ts
│   │   ├── environments/
│   │   └── assets/
│   └── package.json
│
├── backend/                         # ASP.NET 9 Solution
│   ├── RealEstate.API/             # Web API project
│   │   ├── Controllers/
│   │   ├── Middleware/
│   │   └── Program.cs
│   ├── RealEstate.Application/     # Business Logic
│   │   ├── Features/               # CQRS - Commands & Queries
│   │   ├── DTOs/
│   │   └── Interfaces/
│   ├── RealEstate.Domain/          # Core Entities
│   │   ├── Entities/
│   │   └── Enums/
│   └── RealEstate.Infrastructure/  # Data Access
│       ├── Persistence/
│       ├── Repositories/
│       └── Migrations/
│
├── docker-compose.yml
└── README.md
```

---

## 📖 API Documentation

### Base URL
```
https://localhost:7001/api
```

### Xác thực
Tất cả các endpoint được bảo vệ yêu cầu header:
```
Authorization: Bearer <access_token>
```

---

### 🔐 Auth

| Method | Endpoint | Mô tả |
|--------|----------|-------|
| `POST` | `/auth/register` | Đăng ký tài khoản mới |
| `POST` | `/auth/login` | Đăng nhập, trả về JWT |
| `POST` | `/auth/refresh` | Làm mới access token |
| `POST` | `/auth/logout` | Đăng xuất |

**POST** `/auth/login`
```json
// Request
{
  "email": "user@example.com",
  "password": "Password@123"
}

// Response 200
{
  "accessToken": "eyJhbGciOiJIUzI1NiIs...",
  "refreshToken": "dGhpcyBpcyBh...",
  "expiresIn": 3600,
  "user": {
    "id": "uuid",
    "fullName": "Nguyễn Văn A",
    "email": "user@example.com",
    "role": "Seller"
  }
}
```

---

### 🏠 Listings (Bất động sản)

| Method | Endpoint | Auth | Mô tả |
|--------|----------|------|-------|
| `GET` | `/listings` | ❌ | Lấy danh sách BĐS (có phân trang & lọc) |
| `GET` | `/listings/{id}` | ❌ | Chi tiết một BĐS |
| `POST` | `/listings` | ✅ Seller | Tạo tin đăng mới |
| `PUT` | `/listings/{id}` | ✅ Chủ tin | Cập nhật tin đăng |
| `DELETE` | `/listings/{id}` | ✅ Chủ tin/Admin | Xóa tin đăng |
| `GET` | `/listings/my` | ✅ | Tin đăng của tôi |

**GET** `/listings` — Query Parameters:

| Tham số | Kiểu | Mô tả |
|---------|------|-------|
| `page` | int | Trang hiện tại (mặc định: 1) |
| `pageSize` | int | Số item mỗi trang (mặc định: 12) |
| `type` | string | `sale` \| `rent` |
| `category` | string | `apartment`, `house`, `land`, `commercial` |
| `provinceId` | int | ID tỉnh/thành phố |
| `districtId` | int | ID quận/huyện |
| `minPrice` | decimal | Giá tối thiểu |
| `maxPrice` | decimal | Giá tối đa |
| `minArea` | decimal | Diện tích tối thiểu (m²) |
| `maxArea` | decimal | Diện tích tối đa (m²) |
| `keyword` | string | Tìm kiếm theo từ khoá |
| `sortBy` | string | `price_asc`, `price_desc`, `newest` |

---

### 📸 Upload ảnh

| Method | Endpoint | Auth | Mô tả |
|--------|----------|------|-------|
| `POST` | `/listings/{id}/images` | ✅ | Upload ảnh cho tin đăng |
| `DELETE` | `/listings/{id}/images/{imageId}` | ✅ | Xóa ảnh |

---

### 👤 Users

| Method | Endpoint | Auth | Mô tả |
|--------|----------|------|-------|
| `GET` | `/users/me` | ✅ | Thông tin cá nhân |
| `PUT` | `/users/me` | ✅ | Cập nhật hồ sơ |
| `GET` | `/users/me/favorites` | ✅ | Danh sách BĐS yêu thích |
| `POST` | `/users/me/favorites/{listingId}` | ✅ | Thêm vào yêu thích |
| `DELETE` | `/users/me/favorites/{listingId}` | ✅ | Bỏ yêu thích |

---

### 🛡️ Admin

| Method | Endpoint | Auth | Mô tả |
|--------|----------|------|-------|
| `GET` | `/admin/listings/pending` | ✅ Admin | Tin chờ duyệt |
| `PUT` | `/admin/listings/{id}/approve` | ✅ Admin | Duyệt tin |
| `PUT` | `/admin/listings/{id}/reject` | ✅ Admin | Từ chối tin |
| `GET` | `/admin/users` | ✅ Admin | Danh sách người dùng |
| `PUT` | `/admin/users/{id}/ban` | ✅ Admin | Khóa tài khoản |
| `GET` | `/admin/stats` | ✅ Admin | Thống kê tổng quan |

---

## 🌐 Triển khai (Deploy)

### Deploy Backend lên Linux Server

```bash
# Build & publish
dotnet publish -c Release -o ./publish

# Chạy với systemd hoặc Docker
docker build -t real-estate-api .
docker run -d -p 7001:8080 --env-file .env real-estate-api
```

### Deploy Frontend

```bash
# Build production
ng build --configuration production

# Output tại dist/real-estate/
# Deploy lên Nginx, Vercel, Firebase Hosting, v.v.
```

### Nginx config mẫu (Frontend SPA)

```nginx
server {
    listen 80;
    server_name your-domain.com;
    root /var/www/real-estate;
    index index.html;

    location / {
        try_files $uri $uri/ /index.html;
    }

    location /api/ {
        proxy_pass http://localhost:7001/;
        proxy_http_version 1.1;
        proxy_set_header Host $host;
    }
}
```

---

## 🔑 Biến môi trường

### Backend (`appsettings.json`)

| Biến | Mô tả | Bắt buộc |
|------|-------|----------|
| `ConnectionStrings__DefaultConnection` | Chuỗi kết nối SQL Server | ✅ |
| `ConnectionStrings__Redis` | Chuỗi kết nối Redis | ✅ |
| `JwtSettings__SecretKey` | Khóa bí mật JWT (≥32 ký tự) | ✅ |
| `CloudinarySettings__CloudName` | Cloudinary cloud name | ✅ |
| `CloudinarySettings__ApiKey` | Cloudinary API key | ✅ |
| `CloudinarySettings__ApiSecret` | Cloudinary API secret | ✅ |

### Frontend (`environment.ts`)

| Biến | Mô tả | Bắt buộc |
|------|-------|----------|
| `apiUrl` | URL của Backend API | ✅ |
| `googleMapsApiKey` | Google Maps API Key | ⚠️ Tuỳ chọn |

---

## 🤝 Đóng góp

1. Fork repository này
2. Tạo branch mới: `git checkout -b feature/ten-tinh-nang`
3. Commit thay đổi: `git commit -m "feat: mô tả tính năng"`
4. Push lên branch: `git push origin feature/ten-tinh-nang`
5. Tạo Pull Request

### Quy ước commit (Conventional Commits)

| Prefix | Ý nghĩa |
|--------|---------|
| `feat:` | Tính năng mới |
| `fix:` | Sửa lỗi |
| `docs:` | Cập nhật tài liệu |
| `refactor:` | Tái cấu trúc code |
| `test:` | Thêm/sửa test |

---

## 📄 Giấy phép

Dự án được cấp phép theo [MIT License](LICENSE).

---

<p align="center">Made with ❤️ in Vietnam</p>