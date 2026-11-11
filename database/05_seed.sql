-- ============================================================================
-- 10-11-2026 Construido por: Ing GAAE - Seeds demo users and tasks for local testing.
-- ============================================================================

USE TaskFlowDb;
GO

-- ----------------------------------------------------------------------------
-- 10-11-2026 Construido por: Ing GAAE - Demo users.
-- ----------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM dbo.Users)
BEGIN
    INSERT INTO dbo.Users (Name, Email)
    VALUES
        (N'Ana Torres', N'ana.torres@taskflow.dev'),
        (N'Carlos Mejia', N'carlos.mejia@taskflow.dev'),
        (N'Laura Gomez', N'laura.gomez@taskflow.dev');
END;
GO

-- ----------------------------------------------------------------------------
-- 10-11-2026 Construido por: Ing GAAE - Demo tasks with JSON metadata.
-- ----------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM dbo.Tasks)
BEGIN
    INSERT INTO dbo.Tasks (Title, Description, Status, AssignedUserId, Metadata)
    VALUES
        (N'Design database schema',
         N'Define the Users and Tasks tables with keys and indexes',
         'Done', 1, N'{"priority":"High","tags":["database","design"]}'),

        (N'Build REST API',
         N'Implement the users and tasks endpoints with validations',
         'InProgress', 2, N'{"priority":"High","tags":["backend","api"]}'),

        (N'Create Angular UI',
         N'Task list with status filter and reactive creation form',
         'Pending', 3, N'{"priority":"Medium","tags":["frontend"]}'),

        (N'Write README',
         N'Document setup steps, technical decisions and pending items',
         'Pending', 1, N'{"priority":"Low","tags":["docs"]}');
END;
GO
