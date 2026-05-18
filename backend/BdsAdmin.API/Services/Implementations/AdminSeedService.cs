using System.Collections.Generic;
using System.Linq;
using BdsAdmin.API.Constants;
using BdsAdmin.API.Entities;
using BdsAdmin.API.Options;
using BdsAdmin.API.Repositories.Interfaces;
using BdsAdmin.API.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace BdsAdmin.API.Services.Implementations;

public class AdminSeedService : IAdminSeedService
{
    private readonly AdminSeedOptions _options;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly ILogger<AdminSeedService> _logger;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly ISellerProfileRepository _sellerProfileRepository;

    public AdminSeedService(
        IOptions<AdminSeedOptions> options,
        IUserRepository userRepository,
        IPasswordService passwordService,
        ICategoryRepository categoryRepository,
        IPropertyRepository propertyRepository,
        ISellerProfileRepository sellerProfileRepository,
        ILogger<AdminSeedService> logger)
    {
        _options = options.Value;
        _userRepository = userRepository;
        _passwordService = passwordService;
        _categoryRepository = categoryRepository;
        _propertyRepository = propertyRepository;
        _sellerProfileRepository = sellerProfileRepository;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var email = _options.Email?.Trim();
        var password = _options.Password?.Trim();
        var fullName = _options.FullName?.Trim();
        var phone = _options.Phone?.Trim();

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(fullName))
        {
            _logger.LogInformation("Admin seed skipped because AdminSeed config is incomplete.");
            return;
        }

        var existingUser = await _userRepository.GetByEmailAsync(email);
        if (existingUser != null)
        {
            if (!string.Equals(existingUser.Role, AppRoles.Admin, StringComparison.OrdinalIgnoreCase))
            {
                existingUser.Role = AppRoles.Admin;
                existingUser.UpdatedAt = DateTime.UtcNow;
                await _userRepository.SaveChangesAsync();
                _logger.LogInformation("Promoted existing user {Email} to admin role.", email);
            }

            await SeedConsultantAsync();
            await SeedCategoriesAsync();
            await SeedExtraCategoriesAsync();
            await SeedSamplePropertiesAsync();
            return;
        }

        var user = new User
        {
            FullName = fullName,
            Email = email,
            Phone = phone,
            Role = AppRoles.Admin,
            IsPasswordMigrated = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        user.PasswordHash = _passwordService.HashPassword(user, password);

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        _logger.LogInformation("Seeded admin account {Email}.", email);
        await SeedConsultantAsync();
        await SeedCategoriesAsync();
        await SeedExtraCategoriesAsync();
        await SeedSamplePropertiesAsync();
    }

    private async Task SeedConsultantAsync()
    {
        const string email = "consultant@bds.local";
        if (await _userRepository.GetByEmailAsync(email) != null) return;
        var consultant = new User
        {
            FullName = "Default Consultant",
            Email = email,
            Phone = "0900000000",
            Role = AppRoles.Consultant,
            IsPasswordMigrated = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        consultant.PasswordHash = _passwordService.HashPassword(consultant, "Consultant@123");
        await _userRepository.AddAsync(consultant);
        await _userRepository.SaveChangesAsync();
    }

    private async Task SeedCategoriesAsync()
    {
        var existing = await _categoryRepository.GetAllAsync();
        if (existing.Any()) return;
        var categories = new[]
        {
            new Category 
                { 
                    Name = "Bán căn hộ chung cư", 
                    GroupName = "Nhà đất bán", 
                    Slug = "ban-can-ho-chung-cu", 
                    CreatedAt = DateTime.UtcNow, 
                    UpdatedAt = DateTime.UtcNow 
                },

                new Category 
                { 
                    Name = "Bán nhà riêng", 
                    GroupName = "Nhà đất bán", 
                    Slug = "ban-nha-rieng", 
                    CreatedAt = DateTime.UtcNow, 
                    UpdatedAt = DateTime.UtcNow 
                },

                new Category 
                { 
                    Name = "Cho thuê văn phòng", 
                    GroupName = "Nhà đất cho thuê", 
                    Slug = "cho-thue-van-phong", 
                    CreatedAt = DateTime.UtcNow, 
                    UpdatedAt = DateTime.UtcNow 
                }
        };
        foreach (var category in categories) await _categoryRepository.AddAsync(category);
        await _categoryRepository.SaveChangesAsync();
    }

    /// <summary>Bổ sung danh mục nhà thuê & dự án nếu chưa có (slug cố định).</summary>
    private async Task SeedExtraCategoriesAsync()
    {
        var existing = await _categoryRepository.GetAllAsync();
        var slugs = new HashSet<string>(existing.Select(c => c.Slug), StringComparer.OrdinalIgnoreCase);
        var toAdd = new List<Category>();
        var now = DateTime.UtcNow;

        void TryAdd(string slug, string name, string groupName)
        {
            if (slugs.Contains(slug)) return;
            toAdd.Add(new Category
            {
                Name = name,
                GroupName = groupName,
                Slug = slug,
                CreatedAt = now,
                UpdatedAt = now
            });
            slugs.Add(slug);
        }

        TryAdd("nha-cho-thue", "Nhà cho thuê", "Nhà đất cho thuê");
        TryAdd("can-ho-cho-thue", "Căn hộ cho thuê", "Nhà đất cho thuê");
        TryAdd("du-an-can-ho", "Dự án căn hộ", "Dự án");
        TryAdd("du-an-dat-nen", "Dự án đất nền", "Dự án");

        foreach (var c in toAdd) await _categoryRepository.AddAsync(c);
        if (toAdd.Count > 0)
        {
            await _categoryRepository.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} extra categories (rent & projects).", toAdd.Count);
        }
    }

    /// <summary>
    /// Thêm bất động sản mẫu (tiếng Việt) khi chưa có tin đăng nào — chạy cùng lúc khởi động API.
    /// </summary>
    private async Task SeedSamplePropertiesAsync()
    {
        var anyProperties = await _propertyRepository.GetAllForAdminAsync();
        if (anyProperties.Count > 0) return;

        var categories = await _categoryRepository.GetAllAsync();
        if (categories.Count == 0)
        {
            _logger.LogInformation("Sample properties skipped: no categories in database.");
            return;
        }

        static Category? Pick(IReadOnlyList<Category> list, string slug) =>
            list.FirstOrDefault(c => string.Equals(c.Slug, slug, StringComparison.OrdinalIgnoreCase));

        var catApartment = Pick(categories, "apartment") ?? categories[0];
        var catHouse = Pick(categories, "house") ?? categories[0];
        var catOffice = Pick(categories, "office") ?? categories[0];
        var catRentHouse = Pick(categories, "nha-cho-thue") ?? catHouse;
        var catRentApt = Pick(categories, "can-ho-cho-thue") ?? catApartment;
        var catProjectApt = Pick(categories, "du-an-can-ho") ?? catApartment;
        var catProjectLand = Pick(categories, "du-an-dat-nen") ?? catHouse;

        const string sellerEmail = "seller-demo@bds.local";
        var seller = await _userRepository.GetByEmailAsync(sellerEmail);
        if (seller == null)
        {
            var nowUser = DateTime.UtcNow;
            seller = new User
            {
                FullName = "Công ty TNHH BĐS Demo",
                Email = sellerEmail,
                Phone = "0909123456",
                Role = AppRoles.Seller,
                IsPasswordMigrated = true,
                CreatedAt = nowUser,
                UpdatedAt = nowUser
            };
            seller.PasswordHash = _passwordService.HashPassword(seller, "SellerDemo@123");
            await _userRepository.AddAsync(seller);
            await _userRepository.SaveChangesAsync();
        }

        var profile = await _sellerProfileRepository.GetByUserIdAsync(seller.Id);
        if (profile == null)
        {
            var nowProfile = DateTime.UtcNow;
            profile = new SellerProfile
            {
                UserId = seller.Id,
                SellerType = SellerTypes.Broker,
                CompanyName = "BĐS Demo Việt Nam",
                ContactName = "Nguyễn Văn Demo",
                Phone = "0909123456",
                Address = "159 Xa lộ Hà Nội, P. Thảo Điền, TP. Thủ Đức, TP.HCM",
                TaxCode = "0312345678",
                CreatedAt = nowProfile,
                UpdatedAt = nowProfile
            };
            await _sellerProfileRepository.AddAsync(profile);
            await _sellerProfileRepository.SaveChangesAsync();
        }

        var now = DateTime.UtcNow;
        var samples = new[]
        {
            new Property
            {
                UserId = seller.Id,
                SellerProfileId = profile.Id,
                CategoryId = catApartment.Id,
                Title = "Căn hộ 2PN Masteri Thảo Điền, view sông, nội thất cao cấp",
                Description =
                    "Căn góc sáng, ban công Đông Nam. Tiện ích: hồ bơi, gym, BBQ. Phù hợp gia đình trẻ hoặc cho thuê.",
                Price = 5_800_000_000m,
                PricePerM2 = 95_000_000m,
                Area = 61m,
                Address = "159 Xa lộ Hà Nội, P. Thảo Điền",
                Ward = "Thảo Điền",
                District = "Thủ Đức",
                City = "TP. Hồ Chí Minh",
                ProjectName = "Masteri Thảo Điền",
                Status = PropertyStatuses.Published,
                ListingCode = "DEMO-HCM-001",
                ListingType = "VIP",
                CreatedAt = now,
                UpdatedAt = now,
                Images =
                [
                    new PropertyImage
                    {
                        Url = "https://images.unsplash.com/photo-1613490493576-7fde63acd811?w=1200&q=80",
                        IsPrimary = true,
                        SortOrder = 0,
                        ContentType = "image/jpeg",
                        UploadedAt = now
                    },
                    new PropertyImage
                    {
                        Url = "https://images.unsplash.com/photo-1616594039964-ae9021a400a0?w=1200&q=80",
                        IsPrimary = false,
                        SortOrder = 1,
                        ContentType = "image/jpeg",
                        UploadedAt = now
                    }
                ]
            },
            new Property
            {
                UserId = seller.Id,
                SellerProfileId = profile.Id,
                CategoryId = catHouse.Id,
                Title = "Nhà phố 4 tầng Quận 7, hẻm xe hơi 6m, sổ hồng riêng",
                Description = "Kết cấu: trệt + 3 lầu, 4PN 5WC. Khu dân cư an ninh, gần Lotte Mart Phú Mỹ Hưng.",
                Price = 12_500_000_000m,
                PricePerM2 = 125_000_000m,
                Area = 100m,
                Address = "Đường số 12, KDC Tân Mỹ",
                Ward = "Tân Mỹ",
                District = "Quận 7",
                City = "TP. Hồ Chí Minh",
                Status = PropertyStatuses.Published,
                ListingCode = "DEMO-HCM-002",
                ListingType = "Diamond",
                CreatedAt = now,
                UpdatedAt = now,
                Images =
                [
                    new PropertyImage
                    {
                        Url = "https://images.unsplash.com/photo-1564013799919-ab600027ffc6?w=1200&q=80",
                        IsPrimary = true,
                        SortOrder = 0,
                        ContentType = "image/jpeg",
                        UploadedAt = now
                    }
                ]
            },
            new Property
            {
                UserId = seller.Id,
                SellerProfileId = profile.Id,
                CategoryId = catOffice.Id,
                Title = "Văn phòng cho thuê mặt tiền Lê Lợi, Quận 1 — 120m²",
                Description = "View trung tâm, thang máy riêng tầng, phù hợp công ty, chi nhánh ngân hàng.",
                Price = 85_000_000m,
                PricePerM2 = 708_333m,
                Area = 120m,
                Address = "120-122 Lê Lợi",
                Ward = "Bến Thành",
                District = "Quận 1",
                City = "TP. Hồ Chí Minh",
                Status = PropertyStatuses.Published,
                ListingCode = "DEMO-HCM-003",
                ListingType = "Standard",
                CreatedAt = now,
                UpdatedAt = now,
                Images =
                [
                    new PropertyImage
                    {
                        Url = "https://images.unsplash.com/photo-1497366216548-37526070297c?w=1200&q=80",
                        IsPrimary = true,
                        SortOrder = 0,
                        ContentType = "image/jpeg",
                        UploadedAt = now
                    }
                ]
            },
            new Property
            {
                UserId = seller.Id,
                SellerProfileId = profile.Id,
                CategoryId = catApartment.Id,
                Title = "Căn hộ Vinhomes Grand Park 2PN+1, tầng trung, view nội khu",
                Description = "Chờ duyệt admin — ví dụ tin ở trạng thái Pending.",
                Price = 3_200_000_000m,
                PricePerM2 = 62_745_000m,
                Area = 51m,
                Address = "Đường Nguyễn Xiển, Long Bình",
                Ward = "Long Bình",
                District = "Thủ Đức",
                City = "TP. Hồ Chí Minh",
                ProjectName = "Vinhomes Grand Park",
                Status = PropertyStatuses.Pending,
                ListingCode = "DEMO-HCM-004",
                ListingType = "Standard",
                CreatedAt = now,
                UpdatedAt = now,
                Images =
                [
                    new PropertyImage
                    {
                        Url = "https://images.unsplash.com/photo-1600596542815-ffad4c1539a9?w=1200&q=80",
                        IsPrimary = true,
                        SortOrder = 0,
                        ContentType = "image/jpeg",
                        UploadedAt = now
                    }
                ]
            },
            new Property
            {
                UserId = seller.Id,
                SellerProfileId = profile.Id,
                CategoryId = catHouse.Id,
                Title = "Biệt thự song lập Lakeview City, Nhơn Trạch — 250m² đất",
                Description = "Sân vườn rộng, gara 2 xe. Khu compound khép kín, sân golf Long Thành 15 phút.",
                Price = 18_900_000_000m,
                PricePerM2 = 75_600_000m,
                Area = 250m,
                Address = "Đường số 3, Lakeview City",
                Ward = "Long Thọ",
                District = "Nhơn Trạch",
                City = "Đồng Nai",
                ProjectName = "Lakeview City",
                Status = PropertyStatuses.Published,
                ListingCode = "DEMO-DN-001",
                ListingType = "VIP",
                CreatedAt = now,
                UpdatedAt = now,
                Images =
                [
                    new PropertyImage
                    {
                        Url = "https://images.unsplash.com/photo-1600585154340-be6161a56a0c?w=1200&q=80",
                        IsPrimary = true,
                        SortOrder = 0,
                        ContentType = "image/jpeg",
                        UploadedAt = now
                    }
                ]
            },
            new Property
            {
                UserId = seller.Id,
                SellerProfileId = profile.Id,
                CategoryId = catRentHouse.Id,
                Title = "Nhà nguyên căn 3 tầng cho thuê Quận 3 — full nội thất",
                Description = "Hẻm xe hơi, phù hợp hộ gia đình hoặc văn phòng công ty nhỏ. Hợp đồng tối thiểu 12 tháng.",
                Price = 22_000_000m,
                PricePerM2 = 366_667m,
                Area = 60m,
                Address = "Đường Võ Văn Tần, phường 5",
                Ward = "Phường 5",
                District = "Quận 3",
                City = "TP. Hồ Chí Minh",
                Status = PropertyStatuses.Published,
                ListingCode = "DEMO-THUE-001",
                ListingType = "Standard",
                CreatedAt = now,
                UpdatedAt = now,
                Images =
                [
                    new PropertyImage
                    {
                        Url = "https://images.unsplash.com/photo-1605276374104-dee2a0ed3cd6?w=1200&q=80",
                        IsPrimary = true,
                        SortOrder = 0,
                        ContentType = "image/jpeg",
                        UploadedAt = now
                    }
                ]
            },
            new Property
            {
                UserId = seller.Id,
                SellerProfileId = profile.Id,
                CategoryId = catRentApt.Id,
                Title = "Căn hộ 2PN cho thuê Masteri An Phú — nội thất đầy đủ",
                Description = "View Landmark 81, tầng cao thoáng. Giá thuê đã gồm phí quản lý.",
                Price = 18_000_000m,
                PricePerM2 = 295_082m,
                Area = 61m,
                Address = "179 Hanoi Highway, An Phú",
                Ward = "An Phú",
                District = "Thủ Đức",
                City = "TP. Hồ Chí Minh",
                ProjectName = "Masteri An Phú",
                Status = PropertyStatuses.Published,
                ListingCode = "DEMO-THUE-002",
                ListingType = "VIP",
                CreatedAt = now,
                UpdatedAt = now,
                Images =
                [
                    new PropertyImage
                    {
                        Url = "https://images.unsplash.com/photo-1522708323590-d24dbb6b0267?w=1200&q=80",
                        IsPrimary = true,
                        SortOrder = 0,
                        ContentType = "image/jpeg",
                        UploadedAt = now
                    }
                ]
            },
            new Property
            {
                UserId = seller.Id,
                SellerProfileId = profile.Id,
                CategoryId = catProjectApt.Id,
                Title = "Căn hộ dự án The 9 Stellars — bàn giao 2026, 2PN + 1 đa năng",
                Description = "Mặt tiền Xa lộ Hà Nội, kết nối Metro số 1. Booking ưu đãi giai đoạn mở bán.",
                Price = 4_500_000_000m,
                PricePerM2 = 81_818_182m,
                Area = 55m,
                Address = "Xa lộ Hà Nội, Long Bình",
                Ward = "Long Bình",
                District = "Thủ Đức",
                City = "TP. Hồ Chí Minh",
                ProjectName = "The 9 Stellars",
                Status = PropertyStatuses.Published,
                ListingCode = "DEMO-DA-001",
                ListingType = "Diamond",
                CreatedAt = now,
                UpdatedAt = now,
                Images =
                [
                    new PropertyImage
                    {
                        Url = "https://images.unsplash.com/photo-1545324418-cc1a3fa10c00?w=1200&q=80",
                        IsPrimary = true,
                        SortOrder = 0,
                        ContentType = "image/jpeg",
                        UploadedAt = now
                    }
                ]
            },
            new Property
            {
                UserId = seller.Id,
                SellerProfileId = profile.Id,
                CategoryId = catProjectLand.Id,
                Title = "Đất nền dự án Long Hậu — lô góc B2-15, sổ riêng từng nền",
                Description = "Khu công nghiệp Long Hậu 5 phút, pháp lý minh bạch, hạ tầng điện âm nước máy.",
                Price = 2_800_000_000m,
                PricePerM2 = 56_000_000m,
                Area = 50m,
                Address = "Đường N7, KDC Long Hậu",
                Ward = "Long Hậu",
                District = "Cần Giuộc",
                City = "Long An",
                ProjectName = "KDC Long Hậu",
                Status = PropertyStatuses.Published,
                ListingCode = "DEMO-DA-002",
                ListingType = "VIP",
                CreatedAt = now,
                UpdatedAt = now,
                Images =
                [
                    new PropertyImage
                    {
                        Url = "https://images.unsplash.com/photo-1500382017468-9049fed747ef?w=1200&q=80",
                        IsPrimary = true,
                        SortOrder = 0,
                        ContentType = "image/jpeg",
                        UploadedAt = now
                    }
                ]
            }
        };

        foreach (var p in samples) await _propertyRepository.AddAsync(p);
        await _propertyRepository.SaveChangesAsync();
        _logger.LogInformation("Seeded {Count} sample real-estate listings (seller {Email}).", samples.Length, sellerEmail);
    }
}
