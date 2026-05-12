-- =============================================================================
-- Import tin bất động sản mẫu vào PostgreSQL (BdsAdmin.API)
-- =============================================================================
-- Điều kiện:
--   1. Đã chạy migration EF (bảng "Properties", "PropertyImages", "Categories", "Users", "SellerProfiles").
--   2. Danh mục: apartment, house, office (seed API). Script tự thêm (nếu chưa có):
--      nha-cho-thue, can-ho-cho-thue, du-an-can-ho, du-an-dat-nen.
--   3. Có ít nhất một User Role = 'Seller' và (khuyến nghị) SellerProfile — ví dụ seller-demo@bds.local
--      sau khi khởi động API với AdminSeed đầy đủ.
--
-- Chạy (từ máy có psql):
--   psql -h HOST -U USER -d DATABASE -f backend/BdsAdmin.API/SeedData/import-properties.sql
--
-- Script idempotent: bỏ qua nếu "ListingCode" đã tồn tại.
-- =============================================================================

BEGIN;

INSERT INTO "Categories" ("Id", "Name", "GroupName", "Slug", "IsDeleted", "CreatedAt", "UpdatedAt", "ParentId", "DeletedAt")
VALUES
  ('d2222222-2222-4222-8222-222222222201'::uuid, 'Nhà cho thuê', 'Nhà đất cho thuê', 'nha-cho-thue', false, NOW(), NOW(), NULL, NULL),
  ('d2222222-2222-4222-8222-222222222202'::uuid, 'Căn hộ cho thuê', 'Nhà đất cho thuê', 'can-ho-cho-thue', false, NOW(), NOW(), NULL, NULL),
  ('d2222222-2222-4222-8222-222222222203'::uuid, 'Dự án căn hộ', 'Dự án', 'du-an-can-ho', false, NOW(), NOW(), NULL, NULL),
  ('d2222222-2222-4222-8222-22222222２04'::uuid, 'Dự án đất nền', 'Dự án', 'du-an-dat-nen', false, NOW(), NOW(), NULL, NULL)
ON CONFLICT ("Slug") DO NOTHING;

WITH seller_src AS (
  SELECT u."Id" AS user_id, s."Id" AS profile_id
  FROM "Users" u
  LEFT JOIN "SellerProfiles" s ON s."UserId" = u."Id"
  WHERE u."Email" = 'seller-demo@bds.local'
  LIMIT 1
),
seller_fb AS (
  SELECT u."Id" AS user_id, s."Id" AS profile_id
  FROM "Users" u
  JOIN "SellerProfiles" s ON s."UserId" = u."Id"
  WHERE u."Role" = 'Seller'
  ORDER BY u."CreatedAt"
  LIMIT 1
),
seller AS (
  SELECT COALESCE((SELECT user_id FROM seller_src), (SELECT user_id FROM seller_fb)) AS user_id,
         COALESCE((SELECT profile_id FROM seller_src), (SELECT profile_id FROM seller_fb)) AS profile_id
),
ins1 AS (
  INSERT INTO "Properties" (
    "Id", "UserId", "SellerProfileId", "CategoryId", "LocationId",
    "Title", "Description", "Price", "PricePerM2", "Area",
    "Address", "Ward", "District", "City", "ProjectName",
    "Status", "RejectedReason", "ExpiredAt", "ListingCode", "ListingType",
    "IsDeleted", "CreatedAt", "UpdatedAt", "DeletedAt"
  )
  SELECT
    'b1111111-1111-4111-8111-111111111101'::uuid,
    s.user_id,
    s.profile_id,
    c."Id",
    NULL::uuid,
    'Căn hộ 3PN view công viên, The Sun Avenue',
    'Nội thất cơ bản, ban công rộng, khu compound an ninh 24/7.',
    7200000000::numeric(18,2),
    98000000::numeric(18,2),
    73.50::numeric(10,2),
    '28 Mai Chí Thọ, phường An Phú',
    'An Phú',
    'Thủ Đức',
    'TP. Hồ Chí Minh',
    'The Sun Avenue',
    'Published',
    NULL,
    NULL,
    'IMPORT-HCM-001',
    'VIP',
    false,
    NOW(), NOW(),
    NULL
  FROM seller s
  CROSS JOIN "Categories" c
  WHERE c."Slug" = 'apartment' AND c."IsDeleted" = false
    AND EXISTS (SELECT 1 FROM seller WHERE user_id IS NOT NULL)
    AND NOT EXISTS (SELECT 1 FROM "Properties" p WHERE p."ListingCode" = 'IMPORT-HCM-001')
  RETURNING "Id"
),
img1 AS (
  INSERT INTO "PropertyImages" (
    "Id", "PropertyId", "Url", "IsPrimary", "SortOrder", "ObjectName", "ContentType",
    "IsDeleted", "UploadedAt", "DeletedAt"
  )
  SELECT
    'c1111111-1111-4111-8111-111111111101'::uuid,
    'b1111111-1111-4111-8111-111111111101'::uuid,
    'https://images.unsplash.com/photo-1502672260266-1c1ef2d93688?w=1200&q=80',
    true,
    0,
    NULL,
    'image/jpeg',
    false,
    NOW(),
    NULL
  WHERE EXISTS (SELECT 1 FROM ins1)
    AND NOT EXISTS (SELECT 1 FROM "PropertyImages" WHERE "Id" = 'c1111111-1111-4111-8111-111111111101'::uuid)
  RETURNING "Id"
),
ins2 AS (
  INSERT INTO "Properties" (
    "Id", "UserId", "SellerProfileId", "CategoryId", "LocationId",
    "Title", "Description", "Price", "PricePerM2", "Area",
    "Address", "Ward", "District", "City", "ProjectName",
    "Status", "RejectedReason", "ExpiredAt", "ListingCode", "ListingType",
    "IsDeleted", "CreatedAt", "UpdatedAt", "DeletedAt"
  )
  SELECT
    'b1111111-1111-4111-8111-111111111102'::uuid,
    s.user_id,
    s.profile_id,
    c."Id",
    NULL::uuid,
    'Nhà liền kề Gamuda Gardens, 75m² x 4 tầng',
    'Gara ô tô, sân sau, nội thất gỗ tự nhiên tầng trệt.',
    16500000000::numeric(18,2),
    220000000::numeric(18,2),
    75::numeric(10,2),
    'KĐT Gamuda Gardens, Trần Phú',
    'Trần Phú',
    'Hoàng Mai',
    'Hà Nội',
    'Gamuda Gardens',
    'Published',
    NULL,
    NULL,
    'IMPORT-HN-001',
    'Diamond',
    false,
    NOW(), NOW(),
    NULL
  FROM seller s
  CROSS JOIN "Categories" c
  WHERE c."Slug" = 'house' AND c."IsDeleted" = false
    AND EXISTS (SELECT 1 FROM seller WHERE user_id IS NOT NULL)
    AND NOT EXISTS (SELECT 1 FROM "Properties" p WHERE p."ListingCode" = 'IMPORT-HN-001')
  RETURNING "Id"
),
img2 AS (
  INSERT INTO "PropertyImages" (
    "Id", "PropertyId", "Url", "IsPrimary", "SortOrder", "ObjectName", "ContentType",
    "IsDeleted", "UploadedAt", "DeletedAt"
  )
  SELECT
    'c1111111-1111-4111-8111-111111111102'::uuid,
    'b1111111-1111-4111-8111-111111111102'::uuid,
    'https://images.unsplash.com/photo-1600585154526-990dced4db0d?w=1200&q=80',
    true,
    0,
    NULL,
    'image/jpeg',
    false,
    NOW(),
    NULL
  WHERE EXISTS (SELECT 1 FROM ins2)
    AND NOT EXISTS (SELECT 1 FROM "PropertyImages" WHERE "Id" = 'c1111111-1111-4111-8111-111111111102'::uuid)
  RETURNING "Id"
),
ins3 AS (
  INSERT INTO "Properties" (
    "Id", "UserId", "SellerProfileId", "CategoryId", "LocationId",
    "Title", "Description", "Price", "PricePerM2", "Area",
    "Address", "Ward", "District", "City", "ProjectName",
    "Status", "RejectedReason", "ExpiredAt", "ListingCode", "ListingType",
    "IsDeleted", "CreatedAt", "UpdatedAt", "DeletedAt"
  )
  SELECT
    'b1111111-1111-4111-8111-111111111103'::uuid,
    s.user_id,
    s.profile_id,
    c."Id",
    NULL::uuid,
    'Văn phòng cho thuê APEC Mandala Wyndham Phú Yên — 85m²',
    'View biển, sảnh đón 5 sao, phù hợp văn phòng đại diện.',
    42000000::numeric(18,2),
    494118::numeric(18,2),
    85::numeric(10,2),
    'Đường Nguyễn Tất Thành',
    'Phú Đông',
    'Tuy Hòa',
    'Phú Yên',
    'APEC Mandala Wyndham',
    'Pending',
    NULL,
    NULL,
    'IMPORT-DN-001',
    'Standard',
    false,
    NOW(), NOW(),
    NULL
  FROM seller s
  CROSS JOIN "Categories" c
  WHERE c."Slug" = 'office' AND c."IsDeleted" = false
    AND EXISTS (SELECT 1 FROM seller WHERE user_id IS NOT NULL)
    AND NOT EXISTS (SELECT 1 FROM "Properties" p WHERE p."ListingCode" = 'IMPORT-DN-001')
  RETURNING "Id"
),
img3 AS (
  INSERT INTO "PropertyImages" (
    "Id", "PropertyId", "Url", "IsPrimary", "SortOrder", "ObjectName", "ContentType",
    "IsDeleted", "UploadedAt", "DeletedAt"
  )
  SELECT
    'c1111111-1111-4111-8111-111111111103'::uuid,
    'b1111111-1111-4111-8111-111111111103'::uuid,
    'https://images.unsplash.com/photo-1497366811353-6870744d04b2?w=1200&q=80',
    true,
    0,
    NULL,
    'image/jpeg',
    false,
    NOW(),
    NULL
  WHERE EXISTS (SELECT 1 FROM ins3)
    AND NOT EXISTS (SELECT 1 FROM "PropertyImages" WHERE "Id" = 'c1111111-1111-4111-8111-111111111103'::uuid)
  RETURNING "Id"
),
ins4 AS (
  INSERT INTO "Properties" (
    "Id", "UserId", "SellerProfileId", "CategoryId", "LocationId",
    "Title", "Description", "Price", "PricePerM2", "Area",
    "Address", "Ward", "District", "City", "ProjectName",
    "Status", "RejectedReason", "ExpiredAt", "ListingCode", "ListingType",
    "IsDeleted", "CreatedAt", "UpdatedAt", "DeletedAt"
  )
  SELECT
    'b1111111-1111-4111-8111-111111111201'::uuid,
    s.user_id,
    s.profile_id,
    c."Id",
    NULL::uuid,
    'Nhà nguyên căn 3 tầng cho thuê Quận 3 — full nội thất',
    'Hẻm xe hơi, hợp đồng tối thiểu 12 tháng.',
    22000000::numeric(18,2),
    366667::numeric(18,2),
    60::numeric(10,2),
    'Đường Võ Văn Tần, phường 5',
    'Phường 5',
    'Quận 3',
    'TP. Hồ Chí Minh',
    NULL,
    'Published',
    NULL,
    NULL,
    'IMPORT-THUE-001',
    'Standard',
    false,
    NOW(), NOW(),
    NULL
  FROM seller s
  CROSS JOIN "Categories" c
  WHERE c."Slug" = 'nha-cho-thue' AND c."IsDeleted" = false
    AND EXISTS (SELECT 1 FROM seller WHERE user_id IS NOT NULL)
    AND NOT EXISTS (SELECT 1 FROM "Properties" p WHERE p."ListingCode" = 'IMPORT-THUE-001')
  RETURNING "Id"
),
img4 AS (
  INSERT INTO "PropertyImages" (
    "Id", "PropertyId", "Url", "IsPrimary", "SortOrder", "ObjectName", "ContentType",
    "IsDeleted", "UploadedAt", "DeletedAt"
  )
  SELECT
    'c1111111-1111-4111-8111-111111111201'::uuid,
    'b1111111-1111-4111-8111-111111111201'::uuid,
    'https://images.unsplash.com/photo-1605276374104-dee2a0ed3cd6?w=1200&q=80',
    true, 0, NULL, 'image/jpeg', false, NOW(), NULL
  WHERE EXISTS (SELECT 1 FROM ins4)
    AND NOT EXISTS (SELECT 1 FROM "PropertyImages" WHERE "Id" = 'c1111111-1111-4111-8111-111111111201'::uuid)
  RETURNING "Id"
),
ins5 AS (
  INSERT INTO "Properties" (
    "Id", "UserId", "SellerProfileId", "CategoryId", "LocationId",
    "Title", "Description", "Price", "PricePerM2", "Area",
    "Address", "Ward", "District", "City", "ProjectName",
    "Status", "RejectedReason", "ExpiredAt", "ListingCode", "ListingType",
    "IsDeleted", "CreatedAt", "UpdatedAt", "DeletedAt"
  )
  SELECT
    'b1111111-1111-4111-8111-111111111202'::uuid,
    s.user_id,
    s.profile_id,
    c."Id",
    NULL::uuid,
    'Căn hộ 2PN cho thuê Masteri An Phú — nội thất đầy đủ',
    'View Landmark 81, giá thuê đã gồm phí quản lý.',
    18000000::numeric(18,2),
    295082::numeric(18,2),
    61::numeric(10,2),
    '179 Hanoi Highway, An Phú',
    'An Phú',
    'Thủ Đức',
    'TP. Hồ Chí Minh',
    'Masteri An Phú',
    'Published',
    NULL,
    NULL,
    'IMPORT-THUE-002',
    'VIP',
    false,
    NOW(), NOW(),
    NULL
  FROM seller s
  CROSS JOIN "Categories" c
  WHERE c."Slug" = 'can-ho-cho-thue' AND c."IsDeleted" = false
    AND EXISTS (SELECT 1 FROM seller WHERE user_id IS NOT NULL)
    AND NOT EXISTS (SELECT 1 FROM "Properties" p WHERE p."ListingCode" = 'IMPORT-THUE-002')
  RETURNING "Id"
),
img5 AS (
  INSERT INTO "PropertyImages" (
    "Id", "PropertyId", "Url", "IsPrimary", "SortOrder", "ObjectName", "ContentType",
    "IsDeleted", "UploadedAt", "DeletedAt"
  )
  SELECT
    'c1111111-1111-4111-8111-111111111202'::uuid,
    'b1111111-1111-4111-8111-111111111202'::uuid,
    'https://images.unsplash.com/photo-1522708323590-d24dbb6b0267?w=1200&q=80',
    true, 0, NULL, 'image/jpeg', false, NOW(), NULL
  WHERE EXISTS (SELECT 1 FROM ins5)
    AND NOT EXISTS (SELECT 1 FROM "PropertyImages" WHERE "Id" = 'c1111111-1111-4111-8111-111111111202'::uuid)
  RETURNING "Id"
),
ins6 AS (
  INSERT INTO "Properties" (
    "Id", "UserId", "SellerProfileId", "CategoryId", "LocationId",
    "Title", "Description", "Price", "PricePerM2", "Area",
    "Address", "Ward", "District", "City", "ProjectName",
    "Status", "RejectedReason", "ExpiredAt", "ListingCode", "ListingType",
    "IsDeleted", "CreatedAt", "UpdatedAt", "DeletedAt"
  )
  SELECT
    'b1111111-1111-4111-8111-111111111301'::uuid,
    s.user_id,
    s.profile_id,
    c."Id",
    NULL::uuid,
    'Căn hộ dự án The 9 Stellars — 2PN + 1 đa năng, bàn giao 2026',
    'Mặt tiền Xa lộ Hà Nội, kết nối Metro số 1.',
    4500000000::numeric(18,2),
    81818182::numeric(18,2),
    55::numeric(10,2),
    'Xa lộ Hà Nội, Long Bình',
    'Long Bình',
    'Thủ Đức',
    'TP. Hồ Chí Minh',
    'The 9 Stellars',
    'Published',
    NULL,
    NULL,
    'IMPORT-DA-001',
    'Diamond',
    false,
    NOW(), NOW(),
    NULL
  FROM seller s
  CROSS JOIN "Categories" c
  WHERE c."Slug" = 'du-an-can-ho' AND c."IsDeleted" = false
    AND EXISTS (SELECT 1 FROM seller WHERE user_id IS NOT NULL)
    AND NOT EXISTS (SELECT 1 FROM "Properties" p WHERE p."ListingCode" = 'IMPORT-DA-001')
  RETURNING "Id"
),
img6 AS (
  INSERT INTO "PropertyImages" (
    "Id", "PropertyId", "Url", "IsPrimary", "SortOrder", "ObjectName", "ContentType",
    "IsDeleted", "UploadedAt", "DeletedAt"
  )
  SELECT
    'c1111111-1111-4111-8111-111111111301'::uuid,
    'b1111111-1111-4111-8111-111111111301'::uuid,
    'https://images.unsplash.com/photo-1545324418-cc1a3fa10c00?w=1200&q=80',
    true, 0, NULL, 'image/jpeg', false, NOW(), NULL
  WHERE EXISTS (SELECT 1 FROM ins6)
    AND NOT EXISTS (SELECT 1 FROM "PropertyImages" WHERE "Id" = 'c1111111-1111-4111-8111-111111111301'::uuid)
  RETURNING "Id"
),
ins7 AS (
  INSERT INTO "Properties" (
    "Id", "UserId", "SellerProfileId", "CategoryId", "LocationId",
    "Title", "Description", "Price", "PricePerM2", "Area",
    "Address", "Ward", "District", "City", "ProjectName",
    "Status", "RejectedReason", "ExpiredAt", "ListingCode", "ListingType",
    "IsDeleted", "CreatedAt", "UpdatedAt", "DeletedAt"
  )
  SELECT
    'b1111111-1111-4111-8111-111111111302'::uuid,
    s.user_id,
    s.profile_id,
    c."Id",
    NULL::uuid,
    'Đất nền dự án Long Hậu — lô góc B2-15, sổ riêng',
    'KCN Long Hậu 5 phút, hạ tầng điện âm nước máy.',
    2800000000::numeric(18,2),
    56000000::numeric(18,2),
    50::numeric(10,2),
    'Đường N7, KDC Long Hậu',
    'Long Hậu',
    'Cần Giuộc',
    'Long An',
    'KDC Long Hậu',
    'Published',
    NULL,
    NULL,
    'IMPORT-DA-002',
    'VIP',
    false,
    NOW(), NOW(),
    NULL
  FROM seller s
  CROSS JOIN "Categories" c
  WHERE c."Slug" = 'du-an-dat-nen' AND c."IsDeleted" = false
    AND EXISTS (SELECT 1 FROM seller WHERE user_id IS NOT NULL)
    AND NOT EXISTS (SELECT 1 FROM "Properties" p WHERE p."ListingCode" = 'IMPORT-DA-002')
  RETURNING "Id"
),
img7 AS (
  INSERT INTO "PropertyImages" (
    "Id", "PropertyId", "Url", "IsPrimary", "SortOrder", "ObjectName", "ContentType",
    "IsDeleted", "UploadedAt", "DeletedAt"
  )
  SELECT
    'c1111111-1111-4111-8111-111111111302'::uuid,
    'b1111111-1111-4111-8111-111111111302'::uuid,
    'https://images.unsplash.com/photo-1500382017468-9049fed747ef?w=1200&q=80',
    true, 0, NULL, 'image/jpeg', false, NOW(), NULL
  WHERE EXISTS (SELECT 1 FROM ins7)
    AND NOT EXISTS (SELECT 1 FROM "PropertyImages" WHERE "Id" = 'c1111111-1111-4111-8111-111111111302'::uuid)
  RETURNING "Id"
)
SELECT 1;

COMMIT;
