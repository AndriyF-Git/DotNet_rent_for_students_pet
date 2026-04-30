-- PROMPT v1.6: Stored Procedures for RentalApplicationProfiles (DB Diagram 2 — Student Profile Layer)
-- CRUD operations for student reusable profiles (Prototype source, no FK dependencies).

USE [RentForStudents];
GO

-- =============================================
-- 1. CREATE — Insert a new application profile
-- =============================================
CREATE OR ALTER PROCEDURE sp_RentalApplicationProfiles_Create
    @Id              UNIQUEIDENTIFIER,
    @ProfileName     NVARCHAR(80),
    @ApplicantName   NVARCHAR(120),
    @Phone           NVARCHAR(40),
    @Email           NVARCHAR(254),
    @Message         NVARCHAR(2000) = NULL,
    @CreatedAtUtc    DATETIME2 = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @CreatedAtUtc IS NULL
        SET @CreatedAtUtc = GETUTCDATE();

    INSERT INTO [RentalApplicationProfiles] (
        [Id], [ProfileName], [ApplicantName], [Phone], [Email],
        [Message], [CreatedAtUtc], [UpdatedAtUtc]
    )
    VALUES (
        @Id, @ProfileName, @ApplicantName, @Phone, @Email,
        @Message, @CreatedAtUtc, @CreatedAtUtc
    );

    SELECT * FROM [RentalApplicationProfiles] WHERE [Id] = @Id;
END;
GO

-- =============================================
-- 2. READ — Get a single profile by Id
-- =============================================
CREATE OR ALTER PROCEDURE sp_RentalApplicationProfiles_GetById
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT [Id], [ProfileName], [ApplicantName], [Phone], [Email],
           [Message], [CreatedAtUtc], [UpdatedAtUtc]
    FROM [RentalApplicationProfiles]
    WHERE [Id] = @Id;
END;
GO

-- =============================================
-- 3. READ — Get all profiles (sorted by last updated)
-- =============================================
CREATE OR ALTER PROCEDURE sp_RentalApplicationProfiles_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT [Id], [ProfileName], [ApplicantName], [Phone], [Email],
           [Message], [CreatedAtUtc], [UpdatedAtUtc]
    FROM [RentalApplicationProfiles]
    ORDER BY [UpdatedAtUtc] DESC;
END;
GO

-- =============================================
-- 4. UPDATE — Update profile data (auto-sets UpdatedAtUtc)
-- =============================================
CREATE OR ALTER PROCEDURE sp_RentalApplicationProfiles_Update
    @Id              UNIQUEIDENTIFIER,
    @ProfileName     NVARCHAR(80),
    @ApplicantName   NVARCHAR(120),
    @Phone           NVARCHAR(40),
    @Email           NVARCHAR(254),
    @Message         NVARCHAR(2000) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [RentalApplicationProfiles]
    SET [ProfileName]   = @ProfileName,
        [ApplicantName] = @ApplicantName,
        [Phone]         = @Phone,
        [Email]         = @Email,
        [Message]       = @Message,
        [UpdatedAtUtc]  = GETUTCDATE()
    WHERE [Id] = @Id;

    SELECT * FROM [RentalApplicationProfiles] WHERE [Id] = @Id;
END;
GO

-- =============================================
-- 5. DELETE — Delete a profile
-- =============================================
CREATE OR ALTER PROCEDURE sp_RentalApplicationProfiles_Delete
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM [RentalApplicationProfiles]
    WHERE [Id] = @Id;

    SELECT @@ROWCOUNT AS [RowsDeleted];
END;
GO
