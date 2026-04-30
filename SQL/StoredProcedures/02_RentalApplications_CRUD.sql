-- PROMPT v1.6: Stored Procedures for RentalApplications (DB Diagram 1 — Housing Catalog Layer)
-- CRUD operations for the child table. FK dependency: HousingListings.Id -> RentalApplications.ListingId

USE [RentForStudents];
GO

-- =============================================
-- 1. CREATE — Insert a new rental application (validates ListingId exists)
-- =============================================
CREATE OR ALTER PROCEDURE sp_RentalApplications_Create
    @Id              UNIQUEIDENTIFIER,
    @ListingId       UNIQUEIDENTIFIER,
    @ApplicantName   NVARCHAR(120),
    @Phone           NVARCHAR(40),
    @Email           NVARCHAR(254),
    @Message         NVARCHAR(2000) = NULL,
    @Status          INT = 0,
    @CreatedAtUtc    DATETIME2 = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Validate that the parent listing exists
    IF NOT EXISTS (SELECT 1 FROM [HousingListings] WHERE [Id] = @ListingId)
    BEGIN
        DECLARE @msg NVARCHAR(200) = CONCAT('Listing with Id ''', CAST(@ListingId AS NVARCHAR(36)), ''' does not exist.');
        RAISERROR(@msg, 16, 1);
        RETURN;
    END;

    IF @CreatedAtUtc IS NULL
        SET @CreatedAtUtc = GETUTCDATE();

    INSERT INTO [RentalApplications] (
        [Id], [ListingId], [ApplicantName], [Phone], [Email],
        [Message], [Status], [CreatedAtUtc]
    )
    VALUES (
        @Id, @ListingId, @ApplicantName, @Phone, @Email,
        @Message, @Status, @CreatedAtUtc
    );

    SELECT * FROM [RentalApplications] WHERE [Id] = @Id;
END;
GO

-- =============================================
-- 2. READ — Get a single application by Id
-- =============================================
CREATE OR ALTER PROCEDURE sp_RentalApplications_GetById
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT [Id], [ListingId], [ApplicantName], [Phone], [Email],
           [Message], [Status], [CreatedAtUtc]
    FROM [RentalApplications]
    WHERE [Id] = @Id;
END;
GO

-- =============================================
-- 3. READ — Get all applications for a specific listing
-- =============================================
CREATE OR ALTER PROCEDURE sp_RentalApplications_GetByListingId
    @ListingId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT [Id], [ListingId], [ApplicantName], [Phone], [Email],
           [Message], [Status], [CreatedAtUtc]
    FROM [RentalApplications]
    WHERE [ListingId] = @ListingId
    ORDER BY [CreatedAtUtc] DESC;
END;
GO

-- =============================================
-- 4. UPDATE — Update application data (applicant details)
-- =============================================
CREATE OR ALTER PROCEDURE sp_RentalApplications_Update
    @Id              UNIQUEIDENTIFIER,
    @ApplicantName   NVARCHAR(120),
    @Phone           NVARCHAR(40),
    @Email           NVARCHAR(254),
    @Message         NVARCHAR(2000) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [RentalApplications]
    SET [ApplicantName] = @ApplicantName,
        [Phone]         = @Phone,
        [Email]         = @Email,
        [Message]       = @Message
    WHERE [Id] = @Id;

    SELECT * FROM [RentalApplications] WHERE [Id] = @Id;
END;
GO

-- =============================================
-- 5. UPDATE STATUS — Approve/Reject application (separate business action)
-- =============================================
CREATE OR ALTER PROCEDURE sp_RentalApplications_UpdateStatus
    @Id     UNIQUEIDENTIFIER,
    @Status INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Validate status value (0=Pending, 1=Approved, 2=Rejected)
    IF @Status NOT IN (0, 1, 2)
    BEGIN
        RAISERROR('Invalid status value. Expected 0 (Pending), 1 (Approved), or 2 (Rejected).', 16, 1);
        RETURN;
    END;

    UPDATE [RentalApplications]
    SET [Status] = @Status
    WHERE [Id] = @Id;

    SELECT * FROM [RentalApplications] WHERE [Id] = @Id;
END;
GO

-- =============================================
-- 6. DELETE — Delete an application
-- =============================================
CREATE OR ALTER PROCEDURE sp_RentalApplications_Delete
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM [RentalApplications]
    WHERE [Id] = @Id;

    SELECT @@ROWCOUNT AS [RowsDeleted];
END;
GO
