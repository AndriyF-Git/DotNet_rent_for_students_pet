-- PROMPT v1.6: Seed data for RentForStudents database (10 records per table)
-- Run in SSMS: copy-paste and press F5

USE [RentForStudents];
GO

-- =============================================
-- 1. HousingListings (10 records)
-- RoomType: 0=Single, 1=Double, 2=Studio, 3=Apartment
-- =============================================
INSERT INTO [HousingListings] ([Id], [Title], [Description], [City], [District], [PricePerMonth], [RoomType], [AreaSqm], [IsActive], [CreatedAtUtc])
VALUES
    (NEWID(), N'Cozy studio near university',        N'Fully furnished studio, 5 min walk to campus. Utilities included.',                 N'Kyiv',      N'Shevchenkivskyi',  8500.00,  2, 28, 1, '2026-01-15 10:00:00'),
    (NEWID(), N'Spacious apartment for 2 students',  N'Two bedrooms, modern kitchen, great view. Near metro station.',                     N'Kyiv',      N'Obolonskyi',       12000.00, 3, 55, 1, '2026-01-20 14:30:00'),
    (NEWID(), N'Budget single room in dormitory',    N'Shared bathroom, Wi-Fi included. Perfect for first-year students.',                 N'Lviv',      N'Sykhivskyi',       3500.00,  0, 14, 1, '2026-02-01 09:00:00'),
    (NEWID(), N'Double room near Polytechnic',       N'Shared room for two, close to Lviv Polytechnic. Quiet neighbourhood.',              N'Lviv',      N'Frankivskyi',      4500.00,  1, 22, 1, '2026-02-05 11:15:00'),
    (NEWID(), N'Modern studio in city center',       N'Brand new renovation, smart home features. Walking distance to tram.',              N'Odesa',     N'Prymorskyi',       9000.00,  2, 32, 1, '2026-02-10 16:45:00'),
    (NEWID(), N'Affordable single near campus',      N'Small but comfortable. Shared kitchen with one other tenant.',                      N'Kharkiv',   N'Kyivskyi',         3000.00,  0, 12, 1, '2026-02-15 08:00:00'),
    (NEWID(), N'Premium apartment with balcony',     N'Three rooms, balcony, parking spot. Ideal for a group of students.',                N'Kyiv',      N'Pecherskyi',       18000.00, 3, 72, 1, '2026-02-20 13:00:00'),
    (NEWID(), N'Double room in shared flat',         N'Friendly flatmates, common area, laundry. Bills split equally.',                    N'Dnipro',    N'Tsentralnyi',      4000.00,  1, 20, 0, '2026-03-01 10:30:00'),
    (NEWID(), N'Studio with kitchenette',            N'Self-contained unit with small kitchen. Quiet street, good bus connections.',        N'Lviv',      N'Halytskyi',        7500.00,  2, 26, 1, '2026-03-05 15:00:00'),
    (NEWID(), N'Large apartment near medical uni',   N'Four rooms, two bathrooms. Perfect for medical students on long rotations.',        N'Kyiv',      N'Holosiivskyi',     22000.00, 3, 90, 1, '2026-03-10 12:00:00');
GO

-- =============================================
-- 2. RentalApplications (10 records)
-- Status: 0=Pending, 1=Approved, 2=Rejected
-- Uses random ListingIds from the records above
-- =============================================
DECLARE @L1 UNIQUEIDENTIFIER, @L2 UNIQUEIDENTIFIER, @L3 UNIQUEIDENTIFIER,
        @L4 UNIQUEIDENTIFIER, @L5 UNIQUEIDENTIFIER;

-- Pick 5 listings to reference
SELECT @L1 = [Id] FROM [HousingListings] ORDER BY [CreatedAtUtc] ASC OFFSET 0 ROWS FETCH NEXT 1 ROWS ONLY;
SELECT @L2 = [Id] FROM [HousingListings] ORDER BY [CreatedAtUtc] ASC OFFSET 1 ROWS FETCH NEXT 1 ROWS ONLY;
SELECT @L3 = [Id] FROM [HousingListings] ORDER BY [CreatedAtUtc] ASC OFFSET 2 ROWS FETCH NEXT 1 ROWS ONLY;
SELECT @L4 = [Id] FROM [HousingListings] ORDER BY [CreatedAtUtc] ASC OFFSET 4 ROWS FETCH NEXT 1 ROWS ONLY;
SELECT @L5 = [Id] FROM [HousingListings] ORDER BY [CreatedAtUtc] ASC OFFSET 6 ROWS FETCH NEXT 1 ROWS ONLY;

INSERT INTO [RentalApplications] ([Id], [ListingId], [ApplicantName], [Phone], [Email], [Message], [Status], [CreatedAtUtc])
VALUES
    (NEWID(), @L1, N'Olena Kovalenko',    N'+380501234567', N'olena.koval@gmail.com',     N'I am a second-year student looking for a quiet place to study.',          0, '2026-01-18 09:00:00'),
    (NEWID(), @L1, N'Andrii Shevchenko',   N'+380671234568', N'andrii.shev@ukr.net',       N'Very interested! Can I move in next month?',                             1, '2026-01-19 14:00:00'),
    (NEWID(), @L2, N'Maria Bondarenko',    N'+380931234569', N'maria.bond@gmail.com',      N'My friend and I are looking for a place together. Is it still available?', 0, '2026-01-25 11:30:00'),
    (NEWID(), @L2, N'Dmytro Lysenko',      N'+380661234570', N'dmytro.lys@outlook.com',    N'I start my masters program in September. Perfect location for me.',       2, '2026-01-26 16:00:00'),
    (NEWID(), @L3, N'Yulia Marchenko',     N'+380731234571', N'yulia.march@gmail.com',     N'First year student at Lviv Polytechnic. Budget-friendly option!',         0, '2026-02-03 08:45:00'),
    (NEWID(), @L3, N'Ivan Tkachenko',      N'+380501234572', N'ivan.tkach@student.lp.edu', N'Can I visit this weekend? I am relocating from Kharkiv.',                 1, '2026-02-04 10:00:00'),
    (NEWID(), @L4, N'Kateryna Melnyk',     N'+380671234573', N'katya.mel@gmail.com',       N'Love the location! Is the deposit refundable?',                          0, '2026-02-12 13:15:00'),
    (NEWID(), @L4, N'Oleksandr Ponomarenko', N'+380931234574', N'olex.pon@ukr.net',        N'Erasmus exchange student from Poland. Need housing for 6 months.',        0, '2026-02-13 09:30:00'),
    (NEWID(), @L5, N'Viktoriia Sydorenko', N'+380661234575', N'vika.syd@gmail.com',        N'Group of 3 students. We are very tidy and quiet.',                       1, '2026-02-22 15:00:00'),
    (NEWID(), @L5, N'Bohdan Kravchuk',     N'+380731234576', N'bohdan.krav@outlook.com',   N'PhD student, need a place close to campus for the next 2 years.',        2, '2026-02-23 11:00:00');
GO

-- =============================================
-- 3. RentalApplicationProfiles (10 records)
-- Reusable student profiles (Prototype pattern source)
-- =============================================
INSERT INTO [RentalApplicationProfiles] ([Id], [ProfileName], [ApplicantName], [Phone], [Email], [Message], [CreatedAtUtc], [UpdatedAtUtc])
VALUES
    (NEWID(), N'Olena - main profile',         N'Olena Kovalenko',      N'+380501234567', N'olena.koval@gmail.com',     N'Second-year CS student, non-smoker, quiet lifestyle.',          '2026-01-10 08:00:00', '2026-01-10 08:00:00'),
    (NEWID(), N'Andrii - default',             N'Andrii Shevchenko',    N'+380671234568', N'andrii.shev@ukr.net',       N'Third-year engineering student. Early riser.',                  '2026-01-12 09:00:00', '2026-01-15 10:00:00'),
    (NEWID(), N'Maria - with friend',          N'Maria Bondarenko',     N'+380931234569', N'maria.bond@gmail.com',      N'Looking for a place with my study partner. We are both quiet.', '2026-01-14 10:00:00', '2026-01-14 10:00:00'),
    (NEWID(), N'Dmytro - masters',             N'Dmytro Lysenko',       N'+380661234570', N'dmytro.lys@outlook.com',    N'Starting masters in Data Science. Need fast internet.',         '2026-01-18 11:00:00', '2026-02-01 14:00:00'),
    (NEWID(), N'Yulia - budget',               N'Yulia Marchenko',      N'+380731234571', N'yulia.march@gmail.com',     N'First-year student on scholarship. Looking for affordable.',    '2026-01-20 12:00:00', '2026-01-20 12:00:00'),
    (NEWID(), N'Ivan - relocating',            N'Ivan Tkachenko',       N'+380501234572', N'ivan.tkach@student.lp.edu', N'Relocating from Kharkiv. Flexible on move-in date.',            '2026-01-25 13:00:00', '2026-02-10 09:00:00'),
    (NEWID(), N'Kateryna - short bio',         N'Kateryna Melnyk',      N'+380671234573', N'katya.mel@gmail.com',       N'Medical student, 4th year. Responsible tenant.',                '2026-02-01 14:00:00', '2026-02-01 14:00:00'),
    (NEWID(), N'Oleksandr - Erasmus',          N'Oleksandr Ponomarenko', N'+380931234574', N'olex.pon@ukr.net',         N'Exchange student from Warsaw. Speaks Ukrainian and English.',   '2026-02-05 15:00:00', '2026-02-05 15:00:00'),
    (NEWID(), N'Viktoriia - group',            N'Viktoriia Sydorenko',  N'+380661234575', N'vika.syd@gmail.com',        N'We are a group of 3 friends from the same faculty.',            '2026-02-10 16:00:00', '2026-03-01 11:00:00'),
    (NEWID(), N'Bohdan - PhD',                 N'Bohdan Kravchuk',      N'+380731234576', N'bohdan.krav@outlook.com',   N'PhD researcher, need long-term housing near the lab.',          '2026-02-15 17:00:00', '2026-02-15 17:00:00');
GO

-- =============================================
-- Verify: count records in each table
-- =============================================
SELECT 'HousingListings' AS [Table], COUNT(*) AS [Count] FROM [HousingListings]
UNION ALL
SELECT 'RentalApplications', COUNT(*) FROM [RentalApplications]
UNION ALL
SELECT 'RentalApplicationProfiles', COUNT(*) FROM [RentalApplicationProfiles];
GO
