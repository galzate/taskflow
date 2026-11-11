/* ============================================================================
   10-11-2026 Construido por: Ing GAAE
   Native SQL Server JSON capabilities applied to the Tasks.Metadata column.
   Functions used: ISJSON, JSON_VALUE, OPENJSON and FOR JSON PATH.
   ============================================================================ */

USE TaskFlowDb;
GO

/* ----------------------------------------------------------------------------
   10-11-2026 Construido por: Ing GAAE - 1) Inserts a task with a JSON document.
   ---------------------------------------------------------------------------- */
INSERT INTO dbo.Tasks (Title, Description, Status, AssignedUserId, Metadata)
VALUES
(
    N'Prepare monthly board report',
    N'Consolidate the main KPIs for the board meeting',
    'Pending',
    1,
    N'{"priority":"High","tags":["reporting","finance"],"dueDate":"2026-11-20","estimatedHours":8}'
);
GO

/* ----------------------------------------------------------------------------
   10-11-2026 Construido por: Ing GAAE - 2) Reads scalar values from JSON
   with JSON_VALUE.
   ---------------------------------------------------------------------------- */
SELECT
    Id,
    Title,
    JSON_VALUE(Metadata, '$.priority') AS Priority,
    JSON_VALUE(Metadata, '$.dueDate') AS DueDate,
    CAST(JSON_VALUE(Metadata, '$.estimatedHours') AS INT) AS EstimatedHours
FROM dbo.Tasks
WHERE Metadata IS NOT NULL;
GO

/* ----------------------------------------------------------------------------
   10-11-2026 Construido por: Ing GAAE - 3) Filters rows by a JSON property.
   ---------------------------------------------------------------------------- */
SELECT
    Id,
    Title,
    JSON_VALUE(Metadata, '$.priority') AS Priority
FROM dbo.Tasks
WHERE JSON_VALUE(Metadata, '$.priority') = 'High'
ORDER BY CreatedAt DESC;
GO

/* ----------------------------------------------------------------------------
   10-11-2026 Construido por: Ing GAAE - 4) Shreds the JSON tags array with
   OPENJSON, returning one row per tag.
   ---------------------------------------------------------------------------- */
SELECT
    t.Id,
    t.Title,
    tags.[value] AS Tag
FROM dbo.Tasks AS t
CROSS APPLY OPENJSON(t.Metadata, '$.tags') AS tags
WHERE t.Metadata IS NOT NULL
ORDER BY t.Id, tags.[value];
GO

/* ----------------------------------------------------------------------------
   10-11-2026 Construido por: Ing GAAE - 5) Returns the task list as a JSON
   document with FOR JSON PATH.
   ---------------------------------------------------------------------------- */
SELECT
    t.Id,
    t.Title,
    t.Status,
    t.AssignedUserId,
    t.CreatedAt
FROM dbo.Tasks AS t
ORDER BY t.CreatedAt DESC
FOR JSON PATH, ROOT('tasks');
GO

/* ----------------------------------------------------------------------------
   10-11-2026 Construido por: Ing GAAE - 6) Validates every JSON payload with
   ISJSON (the table also enforces it through CK_Tasks_Metadata_IsJson).
   ---------------------------------------------------------------------------- */
SELECT
    Id,
    Title,
    ISJSON(Metadata) AS IsValidJson
FROM dbo.Tasks;
GO
