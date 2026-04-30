-- PROMPT v1.6: Stored Procedures for HousingListings (DB Diagram 1 — Housing Catalog Layer)
-- CRUD operations for the parent table of the Housing Catalog business layer.

USE [RentForStudents];
GO

-- =============================================
-- 1. CREATE — Insert a new housing listing
-- =============================================
CREATE OR ALTER PROCEDURE sp_HousingListings_Create
    @Id              UNIQUEIDENTIFIER,
    @Title           NVARCHAR(120),
    @Description     NVARCHAR(2000) = NULL,
    @City            NVARCHAR(120),
    @District        NVARCHAR(120) = NULL,
    @PricePerMonth   DECIMAL(12,2),
    @RoomType        INT,
    @AreaSqm         INT,
    @IsActive        BIT = 1,
    @CreatedAtUtc    DATETIME2 = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @CreatedAtUtc IS NULL
        SET @CreatedAtUtc = GETUTCDATE();

    INSERT INTO [HousingListings] (
        [Id], [Title], [Description], [City], [District],
        [PricePerMonth], [RoomType], [AreaSqm], [IsActive], [CreatedAtUtc]
    )
    VALUES (
        @Id, @Title, @Description, @City, @District,
        @PricePerMonth, @RoomType, @AreaSqm, @IsActive, @CreatedAtUtc
    );

    SELECT * FROM [HousingListings] WHERE [Id] = @Id;
END;
GO

-- =============================================
-- 2. READ — Get a single listing by Id
-- =============================================
CREATE OR ALTER PROCEDURE sp_HousingListings_GetById
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT [Id], [Title], [Description], [City], [District],
           [PricePerMonth], [RoomType], [AreaSqm], [IsActive], [CreatedAtUtc]
    FROM [HousingListings]
    WHERE [Id] = @Id;
END;
GO

-- =============================================
-- 3. READ — Get all active listings with optional filters
-- =============================================
CREATE OR ALTER PROCEDURE sp_HousingListings_GetAll
    @City         NVARCHAR(120) = NULL,
    @MaxPrice     DECIMAL(12,2) = NULL,
    @RoomType     INT = NULL,
    @OnlyActive   BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    SELECT [Id], [Title], [Description], [City], [District],
           [PricePerMonth], [RoomType], [AreaSqm], [IsActive], [CreatedAtUtc]
    FROM [HousingListings]
    WHERE (@OnlyActive = 0 OR [IsActive] = 1)
      AND (@City IS NULL OR [City] = @City)
      AND (@MaxPrice IS NULL OR [PricePerMonth] <= @MaxPrice)
      AND (@RoomType IS NULL OR [RoomType] = @RoomType)
    ORDER BY [CreatedAtUtc] DESC;
END;
GO

-- =============================================
-- 4. UPDATE — Update listing details
-- =============================================
CREATE OR ALTER PROCEDURE sp_HousingListings_Update
    @Id              UNIQUEIDENTIFIER,
    @Title           NVARCHAR(120),
    @Description     NVARCHAR(2000) = NULL,
    @City            NVARCHAR(120),
    @District        NVARCHAR(120) = NULL,
    @PricePerMonth   DECIMAL(12,2),
    @RoomType        INT,
    @AreaSqm         INT,
    @IsActive        BIT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [HousingListings]
    SET [Title]         = @Title,
        [Description]   = @Description,
        [City]          = @City,
        [District]      = @District,
        [PricePerMonth] = @PricePerMonth,
        [RoomType]      = @RoomType,
        [AreaSqm]       = @AreaSqm,
        [IsActive]      = @IsActive
    WHERE [Id] = @Id;

    SELECT * FROM [HousingListings] WHERE [Id] = @Id;
END;
GO

-- =============================================
-- 5. DELETE — Delete a listing (cascades to RentalApplications via FK)
-- =============================================
CREATE OR ALTER PROCEDURE sp_HousingListings_Delete
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM [HousingListings]
    WHERE [Id] = @Id;

    SELECT @@ROWCOUNT AS [RowsDeleted];
END;
GO
