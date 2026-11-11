/* ============================================================================
   10-11-2026 Construido por: Ing GAAE
   Required business query: tasks by user, optional status filter and ordered
   by creation date.
   ============================================================================ */

USE TaskFlowDb;
GO

/* ----------------------------------------------------------------------------
   10-11-2026 Construido por: Ing GAAE - Parameterized query used by the API.
   @AssignedUserId : user to filter by (required).
   @Status         : optional status filter (NULL returns every status).
   @SortDirection  : ASC or DESC ordering by CreatedAt.
   ---------------------------------------------------------------------------- */
DECLARE @AssignedUserId INT = 1;
DECLARE @Status VARCHAR(20) = NULL;          -- NULL = all statuses
DECLARE @SortDirection VARCHAR(4) = 'DESC';  -- ASC or DESC

SELECT
    t.Id,
    t.Title,
    t.Description,
    t.Status,
    t.AssignedUserId,
    u.Name AS AssignedUserName,
    t.Metadata,
    t.CreatedAt,
    t.UpdatedAt
FROM dbo.Tasks AS t
INNER JOIN dbo.Users AS u
    ON u.Id = t.AssignedUserId
WHERE t.AssignedUserId = @AssignedUserId
  AND (@Status IS NULL OR t.Status = @Status)
ORDER BY
    CASE WHEN @SortDirection = 'ASC' THEN t.CreatedAt END ASC,
    CASE WHEN @SortDirection = 'DESC' THEN t.CreatedAt END DESC;
GO

/* ----------------------------------------------------------------------------
   10-11-2026 Construido por: Ing GAAE - Equivalent query without a user
   filter, useful for the global task list.
   ---------------------------------------------------------------------------- */
DECLARE @GlobalStatus VARCHAR(20) = 'Pending';

SELECT
    t.Id,
    t.Title,
    t.Status,
    t.AssignedUserId,
    u.Name AS AssignedUserName,
    t.CreatedAt
FROM dbo.Tasks AS t
INNER JOIN dbo.Users AS u
    ON u.Id = t.AssignedUserId
WHERE (@GlobalStatus IS NULL OR t.Status = @GlobalStatus)
ORDER BY t.CreatedAt DESC;
GO

/* ----------------------------------------------------------------------------
   10-11-2026 Construido por: Ing GAAE - Task count grouped by user and status.
   ---------------------------------------------------------------------------- */
SELECT
    u.Id AS AssignedUserId,
    u.Name AS AssignedUserName,
    t.Status,
    COUNT(*) AS TaskCount
FROM dbo.Tasks AS t
INNER JOIN dbo.Users AS u
    ON u.Id = t.AssignedUserId
GROUP BY u.Id, u.Name, t.Status
ORDER BY u.Name, t.Status;
GO
