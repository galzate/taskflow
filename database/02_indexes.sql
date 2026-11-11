-- ============================================================================
-- 10-11-2026 Construido por: Ing GAAE - Creates the indexes for task filtering queries.
-- ============================================================================

USE TaskFlowDb;
GO

-- ----------------------------------------------------------------------------
-- 10-11-2026 Construido por: Ing GAAE - Composite index for filtering by
-- user + status and sorting by creation date (the main query of the system).
-- ----------------------------------------------------------------------------
IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_Tasks_AssignedUserId_Status_CreatedAt'
      AND object_id = OBJECT_ID(N'dbo.Tasks')
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_Tasks_AssignedUserId_Status_CreatedAt
        ON dbo.Tasks (AssignedUserId, Status, CreatedAt DESC)
        INCLUDE (Title, Description, UpdatedAt);
END;
GO

-- ----------------------------------------------------------------------------
-- 10-11-2026 Construido por: Ing GAAE - Index for global filtering by status
-- ordered by creation date (used when no user filter is provided).
-- ----------------------------------------------------------------------------
IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_Tasks_Status_CreatedAt'
      AND object_id = OBJECT_ID(N'dbo.Tasks')
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_Tasks_Status_CreatedAt
        ON dbo.Tasks (Status, CreatedAt DESC)
        INCLUDE (Title, AssignedUserId);
END;
GO

-- ----------------------------------------------------------------------------
-- 10-11-2026 Construido por: Ing GAAE - The unique constraint UQ_Users_Email
-- already creates a unique index on Users(Email); it is documented here for
-- completeness of the indexing strategy.
-- ----------------------------------------------------------------------------
