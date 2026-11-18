-- ============================================================================
-- 10-11-2026 Construido por: Ing GAAE - Creates the TaskFlow database schema (Users, Tasks).
-- Engine: Microsoft SQL Server 2022
-- ============================================================================

IF DB_ID(N'TaskFlowDb') IS NULL
BEGIN
    CREATE DATABASE TaskFlowDb;
END;
GO

USE TaskFlowDb;
GO

-- ----------------------------------------------------------------------------
-- 10-11-2026 Construido por: Ing GAAE - Creates the Users table.
-- - Id is the primary key.
-- - Email must be unique.
-- ----------------------------------------------------------------------------
IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users
    (
        Id          INT             IDENTITY(1,1) NOT NULL,
        Name        NVARCHAR(100)   NOT NULL,
        Email       NVARCHAR(150)   NOT NULL,
        CreatedAt   DATETIME2(0)    NOT NULL
            CONSTRAINT DF_Users_CreatedAt DEFAULT SYSUTCDATETIME(),

        CONSTRAINT PK_Users PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT UQ_Users_Email UNIQUE (Email)
    );
END;
GO

-- ----------------------------------------------------------------------------
-- 10-11-2026 Construido por: Ing GAAE - Creates the Tasks table.
-- - Id is the primary key.
-- - Title and Description are required.
-- - AssignedUserId is a foreign key to Users (a task must have an owner).
-- - Status is restricted to Pending, InProgress and Done.
-- - Metadata stores a JSON document validated with ISJSON.
-- ----------------------------------------------------------------------------
IF OBJECT_ID(N'dbo.Tasks', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Tasks
    (
        Id              INT             IDENTITY(1,1) NOT NULL,
        Title           NVARCHAR(200)   NOT NULL,
        Description     NVARCHAR(1000)  NOT NULL,
        Status          VARCHAR(20)     NOT NULL
            CONSTRAINT DF_Tasks_Status DEFAULT 'Pending',
        AssignedUserId  INT             NOT NULL,
        Metadata        NVARCHAR(MAX)   NULL,
        CreatedAt       DATETIME2(0)    NOT NULL
            CONSTRAINT DF_Tasks_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt       DATETIME2(0)    NULL,

        CONSTRAINT PK_Tasks PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT FK_Tasks_Users FOREIGN KEY (AssignedUserId)
            REFERENCES dbo.Users (Id),
        CONSTRAINT CK_Tasks_Status CHECK (Status IN ('Pending', 'InProgress', 'Done')),
        CONSTRAINT CK_Tasks_Metadata_IsJson CHECK (Metadata IS NULL OR ISJSON(Metadata) = 1)
    );
END;
GO

-- ============================================================================
-- 10-11-2026 Construido por: Ing GAAE - Creates the TaskStatusHistory table (bitacora).
-- ============================================================================
IF OBJECT_ID(N'dbo.TaskStatusHistory', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TaskStatusHistory
    (
        Id          INT             IDENTITY(1,1) NOT NULL,
        TaskId      INT             NOT NULL,
        FromStatus  VARCHAR(20)     NOT NULL,
        ToStatus    VARCHAR(20)     NOT NULL,
        Observation NVARCHAR(1000)  NULL,
        ChangedAt   DATETIME2(0)    NOT NULL
            CONSTRAINT DF_TaskStatusHistory_ChangedAt DEFAULT SYSUTCDATETIME(),

        CONSTRAINT PK_TaskStatusHistory PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT FK_TaskStatusHistory_Tasks FOREIGN KEY (TaskId)
            REFERENCES dbo.Tasks (Id)
            ON DELETE CASCADE,
        CONSTRAINT CK_TaskStatusHistory_FromStatus CHECK (FromStatus IN ('Pending', 'InProgress', 'Done')),
        CONSTRAINT CK_TaskStatusHistory_ToStatus CHECK (ToStatus IN ('Pending', 'InProgress', 'Done'))
    );
END;
GO

-- ============================================================================
-- 10-11-2026 Construido por: Ing GAAE - Creates the TaskActivity table (activity bitacora).
-- ============================================================================
IF OBJECT_ID(N'dbo.TaskActivities', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TaskActivities
    (
        Id          INT             IDENTITY(1,1) NOT NULL,
        TaskId      INT             NOT NULL,
        Description NVARCHAR(1000)  NOT NULL,
        LoggedAt    DATETIME2(0)    NOT NULL
            CONSTRAINT DF_TaskActivities_LoggedAt DEFAULT SYSUTCDATETIME(),

        CONSTRAINT PK_TaskActivities PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT FK_TaskActivities_Tasks FOREIGN KEY (TaskId)
            REFERENCES dbo.Tasks (Id)
            ON DELETE CASCADE
    );
END;
GO
