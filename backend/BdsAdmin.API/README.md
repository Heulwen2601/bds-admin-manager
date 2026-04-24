# EF Core Setup — Real Estate Admin

## 1. Cài packages

```bash
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
```

## 2. Đăng ký DbContext trong Program.cs

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
```

## 3. Connection string trong appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=realestate_db;Username=postgres;Password=yourpassword"
  }
}
```

## 4. Tạo Migration và update DB

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## 5. Cấu trúc file

```
RealEstate.EFCore/
├── Entities/
│   ├── User.cs
│   ├── Category.cs
│   ├── Property.cs
│   └── Others.cs          ← PropertyImage, Message, Notification
└── Data/
    └── AppDbContext.cs
```

## Lưu ý quan trọng

- **Message** dùng `OnDelete(Restrict)` cho cả Sender và Receiver để tránh lỗi
  multiple cascade paths của PostgreSQL.
- **Category** tự tham chiếu qua `ParentId`, dùng `Restrict` để không xóa
  parent khi còn child.
- **gen_random_uuid()** là hàm native của PostgreSQL (>= 13), không cần extension.
- `UpdatedAt` nên update thủ công trong service hoặc override `SaveChangesAsync`.