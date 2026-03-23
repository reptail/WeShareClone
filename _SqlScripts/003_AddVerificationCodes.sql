CREATE TABLE VerificationCodes (
    Id           INT           NOT NULL IDENTITY(1,1),
    Email        NVARCHAR(256) NOT NULL,
    CodeHash     NVARCHAR(256) NOT NULL,
    CreatedAtUtc DATETIME2(3)  NOT NULL CONSTRAINT DF_VerificationCodes_CreatedAtUtc DEFAULT SYSUTCDATETIME(),
    ExpiresAtUtc DATETIME2(3)  NOT NULL,

    CONSTRAINT PK_VerificationCodes       PRIMARY KEY (Id),
    CONSTRAINT UQ_VerificationCodes_Email UNIQUE (Email)
);