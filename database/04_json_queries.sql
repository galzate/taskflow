-- ============================================================================
-- 10-11-2026 Construido por: Ing GAAE - Demonstrates native JSON queries over Tasks.Metadata.
-- Functions used: ISJSON, JSON_VALUE, OPENJSON and FOR JSON PATH.
-- Runs inside a transaction that rolls back, so it leaves no data.
-- ============================================================================

USE TaskFlowDb;
GO

SET XACT_ABORT ON;
BEGIN TRANSACTION;

DECLARE @demoEmail NVARCHAR(320) = CONCAT(N'json.demo.', NEWID(), N'@example.com');

INSERT INTO dbo.Users (Name, Email)
VALUES (N'JSON Demo User', @demoEmail);

DECLARE @demoUserId INT = SCOPE_IDENTITY();

INSERT INTO dbo.Tasks (Title, Description, Status, AssignedUserId, Metadata)
VALUES
(
    N'Prepare monthly board report',
    N'Consolidate the main KPIs for the board meeting',
    'Pending',
    @demoUserId,
    N'{"priority":"High","tags":["reporting","finance"],"dueDate":"2026-11-20","estimatedHours":8}'
);

-- ----------------------------------------------------------------------------
-- 10-11-2026 Construido por: Ing GAAE - 1) Reads scalar values with JSON_VALUE.
-- ----------------------------------------------------------------------------
SELECT
    Id,
    Title,
    JSON_VALUE(Metadata, '$.priority') AS Priority,
    JSON_VALUE(Metadata, '$.dueDate') AS DueDate,
    CAST(JSON_VALUE(Metadata, '$.estimatedHours') AS INT) AS EstimatedHours
FROM dbo.Tasks
WHERE Metadata IS NOT NULL;

-- ----------------------------------------------------------------------------
-- 10-11-2026 Construido por: Ing GAAE - 2) Filters rows by a JSON property.
-- ----------------------------------------------------------------------------
SELECT
    Id,
    Title,
    JSON_VALUE(Metadata, '$.priority') AS Priority
FROM dbo.Tasks
WHERE JSON_VALUE(Metadata, '$.priority') = 'High'
ORDER BY CreatedAt DESC;

-- ----------------------------------------------------------------------------
-- 10-11-2026 Construido por: Ing GAAE - 3) Shreds the JSON tags array with OPENJSON.
-- ----------------------------------------------------------------------------
SELECT
    t.Id,
    t.Title,
    tags.[value] AS Tag
FROM dbo.Tasks AS t
CROSS APPLY OPENJSON(t.Metadata, '$.tags') AS tags
WHERE t.Metadata IS NOT NULL
ORDER BY t.Id, tags.[value];

-- ----------------------------------------------------------------------------
-- 10-11-2026 Construido por: Ing GAAE - 4) Returns the task list as a JSON document with FOR JSON PATH.
-- ----------------------------------------------------------------------------
SELECT
    t.Id,
    t.Title,
    t.Status,
    t.AssignedUserId,
    t.CreatedAt
FROM dbo.Tasks AS t
ORDER BY t.CreatedAt DESC
FOR JSON PATH, ROOT('tasks');

-- ----------------------------------------------------------------------------
-- 10-11-2026 Construido por: Ing GAAE - 5) Validates every JSON payload with ISJSON.
-- ----------------------------------------------------------------------------
SELECT
    Id,
    Title,
    ISJSON(Metadata) AS IsValidJson
FROM dbo.Tasks;

ROLLBACK TRANSACTION;
GO
