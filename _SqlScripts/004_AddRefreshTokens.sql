CREATE TABLE RefreshTokens (
    Id           INT           NOT NULL IDENTITY(1,1),
    UserId       INT           NOT NULL,
    Token        NVARCHAR(512) NOT NULL,
    CreatedAtUtc DATETIME2(3)  NOT NULL CONSTRAINT DF_RefreshTokens_CreatedAtUtc DEFAULT SYSUTCDATETIME(),
    ExpiresAtUtc DATETIME2(3)  NOT NULL,

    CONSTRAINT PK_RefreshTokens       PRIMARY KEY (Id),
    CONSTRAINT FK_RefreshTokens_UserId FOREIGN KEY (UserId) REFERENCES Users(Id),
    CONSTRAINT UQ_RefreshTokens_Token  UNIQUE (Token)
);