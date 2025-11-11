# DaisyForum

Hệ thống diễn đàn trực tuyến được xây dựng với ASP.NET Core 7.0, tích hợp IdentityServer4, SignalR và các tính năng hiện đại.

## Tổng quan

DaisyForum là một nền tảng diễn đàn đầy đủ tính năng với:
- Xác thực và phân quyền người dùng
- Chat thời gian thực
- Tích hợp đăng nhập mạng xã hội (Google, Facebook, Microsoft)
- API RESTful với Swagger documentation
- Hệ thống phân tích nội dung với Google Natural Language API
- Gợi ý nội dung dựa trên content-based filtering

## Cấu trúc dự án

```
DaisyForum/
├── DaisyForum.BackendServer/        # API Backend và IdentityServer
├── DaisyForum.WebPortal/            # Giao diện người dùng
├── DaisyForum.Frontend.AdminApp/    # Trang quản trị
├── DaisyForum.ViewModels/           # Shared ViewModels và DTOs
├── DaisyForum.BackendServer.UnitTest/   # Unit tests cho Backend
└── DaisyForum.ViewModels.UnitTest/      # Unit tests cho ViewModels
```

## Công nghệ sử dụng

### Backend
- **Framework**: ASP.NET Core 7.0
- **Database**: SQL Server với Entity Framework Core 7.0
- **Authentication**: IdentityServer4 + ASP.NET Core Identity
- **Real-time**: SignalR
- **Validation**: FluentValidation
- **Logging**: Serilog
- **API Documentation**: Swagger/OpenAPI
- **Caching**: SQL Server Distributed Cache
- **Mapping**: AutoMapper

### External Services
- Google Natural Language API (phân tích nội dung)
- Google reCAPTCHA (bảo mật)
- OneSignal (push notifications)
- Email service (MailKit)

## Yêu cầu hệ thống

- .NET 7.0 SDK trở lên
- SQL Server 2016 trở lên
- Visual Studio 2022 hoặc VS Code
- Node.js (cho frontend apps)

## Cài đặt

### 1. Clone repository

```bash
git clone <repository-url>
cd DaisyForum
```

### 2. Cấu hình Database

Cập nhật connection string trong `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=DaisyForumDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

### 3. Chạy Migrations

```bash
cd DaisyForum.BackendServer
dotnet ef database update
```

### 4. Cấu hình External Services

Cập nhật các thông tin sau trong `appsettings.json`:

```json
{
  "Authentication": {
    "Google": {
      "ClientId": "YOUR_GOOGLE_CLIENT_ID",
      "ClientSecret": "YOUR_GOOGLE_CLIENT_SECRET"
    },
    "Facebook": {
      "AppId": "YOUR_FACEBOOK_APP_ID",
      "AppSecret": "YOUR_FACEBOOK_APP_SECRET"
    },
    "Microsoft": {
      "ClientId": "YOUR_MICROSOFT_CLIENT_ID",
      "ClientSecret": "YOUR_MICROSOFT_CLIENT_SECRET"
    }
  },
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "Port": 587,
    "SenderName": "DaisyForum",
    "SenderEmail": "your-email@gmail.com",
    "Username": "your-email@gmail.com",
    "Password": "your-app-password"
  },
  "GoogleRecaptcha": {
    "Sitekey": "YOUR_RECAPTCHA_SITE_KEY",
    "Secretkey": "YOUR_RECAPTCHA_SECRET_KEY"
  }
}
```

### 5. Cấu hình CORS

Thêm allowed origins trong `appsettings.json`:

```json
{
  "AllowedOrigins": [
    "http://localhost:3000",
    "http://localhost:4200",
    "https://your-domain.com"
  ]
}
```

### 6. Cấu hình IdentityServer Clients

Thêm clients trong `appsettings.json`:

```json
{
  "IdentityServer": {
    "Clients": [
      {
        "ClientId": "webapp",
        "ClientName": "Web Application",
        "AllowedGrantTypes": ["implicit"],
        "RedirectUris": ["http://localhost:3000/signin-oidc"],
        "PostLogoutRedirectUris": ["http://localhost:3000/signout-callback-oidc"],
        "AllowedScopes": ["openid", "profile", "api.daisyforum"],
        "AllowAccessTokensViaBrowser": true
      }
    ]
  }
}
```

## Chạy ứng dụng

### Backend Server

```bash
cd DaisyForum.BackendServer
dotnet restore
dotnet run
```

Backend sẽ chạy tại:
- HTTPS: `https://localhost:5001`
- HTTP: `http://localhost:5000`
- Swagger UI: `https://localhost:5001/swagger`

### Web Portal

```bash
cd DaisyForum.WebPortal
dotnet restore
dotnet run
```

### Admin App

```bash
cd DaisyForum.Frontend.AdminApp
npm install
npm start
```

## Chạy Tests

```bash
# Test Backend
cd DaisyForum.BackendServer.UnitTest
dotnet test

# Test ViewModels
cd DaisyForum.ViewModels.UnitTest
dotnet test
```

## API Documentation

Sau khi chạy backend, truy cập Swagger UI tại:
```
https://localhost:5001/swagger
```

### Authentication với Swagger

1. Click nút "Authorize"
2. Chọn scopes: `api.daisyforum`
3. Login với tài khoản của bạn
4. Access token sẽ tự động được thêm vào requests

## Tính năng chính

### 1. Quản lý người dùng
- Đăng ký/Đăng nhập
- Đăng nhập qua Google, Facebook, Microsoft
- Quản lý profile
- Phân quyền theo roles

### 2. Diễn đàn
- Tạo và quản lý categories
- Đăng bài viết (posts)
- Bình luận (comments)
- Vote/Like
- Tìm kiếm và lọc

### 3. Chat thời gian thực
- SignalR Hub tại `/chatHub`
- Private messaging
- Group chat

### 4. Gợi ý nội dung
- Content-based filtering
- Phân tích ngôn ngữ tự nhiên với Google NLP
- Gợi ý bài viết liên quan

### 5. Bảo mật
- reCAPTCHA protection
- HTTPS enforcement
- Security headers (XSS, Content-Type, Referrer Policy)
- Rate limiting
- Account lockout

### 6. Notifications
- OneSignal push notifications
- Email notifications
- Real-time notifications qua SignalR

## Cấu trúc Database

Database sẽ được tự động seed với dữ liệu mẫu khi chạy lần đầu, bao gồm:
- Default roles (Admin, Moderator, User)
- Sample categories
- Test users

## Troubleshooting

### Lỗi kết nối Database
```bash
# Kiểm tra connection string
# Đảm bảo SQL Server đang chạy
# Chạy lại migrations
dotnet ef database update
```

### Lỗi IdentityServer
```bash
# Xóa tempkey.jwk và restart
rm DaisyForum.BackendServer/tempkey.jwk
dotnet run
```

### Lỗi CORS
- Kiểm tra AllowedOrigins trong appsettings.json
- Đảm bảo frontend URL được thêm vào danh sách

## Development

### Thêm Migration mới

```bash
cd DaisyForum.BackendServer
dotnet ef migrations add MigrationName
dotnet ef database update
```

### Code Style
- Sử dụng C# naming conventions
- Async/await cho tất cả I/O operations
- Dependency Injection cho services
- Repository pattern cho data access

## Production Deployment

### 1. Build

```bash
dotnet publish -c Release -o ./publish
```

### 2. Cấu hình Production

Tạo `appsettings.Production.json` với:
- Production connection string
- Production URLs
- Secure secrets (sử dụng Azure Key Vault hoặc environment variables)
- Disable developer exception page

### 3. Security Checklist

- [ ] Thay đổi tất cả default secrets
- [ ] Enable HTTPS
- [ ] Cấu hình firewall
- [ ] Enable logging và monitoring
- [ ] Backup database định kỳ
- [ ] Cấu hình rate limiting
- [ ] Review security headers

## License


## Liên hệ


## Đóng góp

Contributions are welcome! Please feel free to submit a Pull Request.
