-- PROMPT v1.7: View + Cursor-based Stored Procedure (DB Layer)
-- View: vw_HousingListingApplicationSummary
-- SP:   sp_GenerateListingDemandReport (cursor -> temp table)

USE [RentForStudents];
GO

-- =============================================
-- 1. VIEW — Listing + Application summary
--    Joins HousingListings with RentalApplications,
--    aggregates application counts per listing.
-- =============================================
CREATE OR ALTER VIEW [dbo].[vw_HousingListingApplicationSummary]
AS
SELECT
    hl.[Id]                                         AS ListingId,
    hl.[Title],
    hl.[City],
    hl.[District],
    hl.[PricePerMonth],
    CASE hl.[RoomType]
        WHEN 0 THEN N'Studio'
        WHEN 1 THEN N'One Bedroom'
        WHEN 2 THEN N'Two Bedroom'
        WHEN 3 THEN N'Three Bedroom'
        ELSE        N'Unknown'
    END                                             AS RoomTypeName,
    hl.[AreaSqm],
    hl.[IsActive],
    hl.[CreatedAtUtc]                               AS ListingCreatedAt,
    COUNT(ra.[Id])                                  AS TotalApplications,
    SUM(CASE WHEN ra.[Status] = 0 THEN 1 ELSE 0 END) AS PendingCount,
    SUM(CASE WHEN ra.[Status] = 1 THEN 1 ELSE 0 END) AS ApprovedCount,
    SUM(CASE WHEN ra.[Status] = 2 THEN 1 ELSE 0 END) AS RejectedCount,
    MAX(ra.[CreatedAtUtc])                          AS LastApplicationDate
FROM [dbo].[HousingListings] hl
LEFT JOIN [dbo].[RentalApplications] ra
    ON ra.[ListingId] = hl.[Id]
GROUP BY
    hl.[Id],
    hl.[Title],
    hl.[City],
    hl.[District],
    hl.[PricePerMonth],
    hl.[RoomType],
    hl.[AreaSqm],
    hl.[IsActive],
    hl.[CreatedAtUtc];
GO

-- =============================================
-- 2. STORED PROCEDURE — Demand report via cursor
--    Reads vw_HousingListingApplicationSummary row by row,
--    classifies each listing by demand level,
--    and writes results into a temporary table.
--
--    Demand classification:
--      'Hot'    -> TotalApplications >= 5
--      'Active' -> TotalApplications 1..4
--      'None'   -> TotalApplications = 0
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[sp_GenerateListingDemandReport]
AS
BEGIN
    SET NOCOUNT ON;

    -- Temp table to accumulate cursor results
    CREATE TABLE #ListingDemandReport
    (
        ListingId           UNIQUEIDENTIFIER    NOT NULL,
        Title               NVARCHAR(120)       NOT NULL,
        City                NVARCHAR(120)       NOT NULL,
        RoomTypeName        NVARCHAR(40)        NOT NULL,
        PricePerMonth       DECIMAL(12, 2)      NOT NULL,
        TotalApplications   INT                 NOT NULL,
        PendingCount        INT                 NOT NULL,
        ApprovedCount       INT                 NOT NULL,
        RejectedCount       INT                 NOT NULL,
        LastApplicationDate DATETIME2           NULL,
        DemandLevel         NVARCHAR(10)        NOT NULL
    );

    -- Cursor variables
    DECLARE @ListingId           UNIQUEIDENTIFIER;
    DECLARE @Title               NVARCHAR(120);
    DECLARE @City                NVARCHAR(120);
    DECLARE @RoomTypeName        NVARCHAR(40);
    DECLARE @PricePerMonth       DECIMAL(12, 2);
    DECLARE @TotalApplications   INT;
    DECLARE @PendingCount        INT;
    DECLARE @ApprovedCount       INT;
    DECLARE @RejectedCount       INT;
    DECLARE @LastApplicationDate DATETIME2;
    DECLARE @DemandLevel         NVARCHAR(10);

    DECLARE listing_cursor CURSOR LOCAL FAST_FORWARD FOR
        SELECT
            ListingId,
            Title,
            City,
            RoomTypeName,
            PricePerMonth,
            TotalApplications,
            PendingCount,
            ApprovedCount,
            RejectedCount,
            LastApplicationDate
        FROM [dbo].[vw_HousingListingApplicationSummary]
        WHERE IsActive = 1
        ORDER BY TotalApplications DESC, Title;

    OPEN listing_cursor;

    FETCH NEXT FROM listing_cursor INTO
        @ListingId,
        @Title,
        @City,
        @RoomTypeName,
        @PricePerMonth,
        @TotalApplications,
        @PendingCount,
        @ApprovedCount,
        @RejectedCount,
        @LastApplicationDate;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        -- Classify demand level
        SET @DemandLevel =
            CASE
                WHEN @TotalApplications >= 5 THEN N'Hot'
                WHEN @TotalApplications >= 1 THEN N'Active'
                ELSE                               N'None'
            END;

        INSERT INTO #ListingDemandReport
        (
            ListingId, Title, City, RoomTypeName, PricePerMonth,
            TotalApplications, PendingCount, ApprovedCount, RejectedCount,
            LastApplicationDate, DemandLevel
        )
        VALUES
        (
            @ListingId, @Title, @City, @RoomTypeName, @PricePerMonth,
            @TotalApplications, @PendingCount, @ApprovedCount, @RejectedCount,
            @LastApplicationDate, @DemandLevel
        );

        FETCH NEXT FROM listing_cursor INTO
            @ListingId,
            @Title,
            @City,
            @RoomTypeName,
            @PricePerMonth,
            @TotalApplications,
            @PendingCount,
            @ApprovedCount,
            @RejectedCount,
            @LastApplicationDate;
    END;

    CLOSE listing_cursor;
    DEALLOCATE listing_cursor;

    -- Return final report ordered by demand level, then by application count
    SELECT
        ListingId,
        Title,
        City,
        RoomTypeName,
        PricePerMonth,
        TotalApplications,
        PendingCount,
        ApprovedCount,
        RejectedCount,
        LastApplicationDate,
        DemandLevel
    FROM #ListingDemandReport
    ORDER BY
        CASE DemandLevel
            WHEN N'Hot'    THEN 1
            WHEN N'Active' THEN 2
            ELSE                3
        END,
        TotalApplications DESC;

    DROP TABLE #ListingDemandReport;
END;
GO
