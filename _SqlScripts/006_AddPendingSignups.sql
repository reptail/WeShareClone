CREATE TABLE PendingSignups (
    Id           INT           NOT NULL IDENTITY(1, 1),
    Email        NVARCHAR(256) NOT NULL,
    Name         NVARCHAR(256) NOT NULL,
    CreatedAtUtc DATETIME2(3)  NOT NULL CONSTRAINT DF_PendingSignups_CreatedAtUtc DEFAULT SYSUTCDATETIME(),
    CONSTRAINT PK_PendingSignups       PRIMARY KEY (Id),
    CONSTRAINT UQ_PendingSignups_Email UNIQUE      (Email)
);