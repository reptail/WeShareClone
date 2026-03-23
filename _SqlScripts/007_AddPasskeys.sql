CREATE TABLE PasskeyCredentials (
    Id           INT              NOT NULL IDENTITY(1, 1),
    UserId       INT              NOT NULL,
    CredentialId VARBINARY(1024)  NOT NULL,
    PublicKey    VARBINARY(MAX)   NOT NULL,
    SignCount    BIGINT           NOT NULL CONSTRAINT DF_PasskeyCredentials_SignCount    DEFAULT 0,
    AaGuid       UNIQUEIDENTIFIER NOT NULL,
    CreatedAtUtc DATETIME2(3)     NOT NULL CONSTRAINT DF_PasskeyCredentials_CreatedAtUtc DEFAULT SYSUTCDATETIME(),
    CONSTRAINT PK_PasskeyCredentials                  PRIMARY KEY (Id),
    CONSTRAINT FK_PasskeyCredentials_UserId           FOREIGN KEY (UserId) REFERENCES Users(Id),
    CONSTRAINT UQ_PasskeyCredentials_CredentialId     UNIQUE      (CredentialId)
);

CREATE TABLE PasskeyChallenges (
    Id            INT           NOT NULL IDENTITY(1, 1),
    Email         NVARCHAR(256) NOT NULL,
    ChallengeType TINYINT       NOT NULL,
    OptionsJson   NVARCHAR(MAX) NOT NULL,
    ExpiresAtUtc  DATETIME2(3)  NOT NULL,
    CONSTRAINT PK_PasskeyChallenges                        PRIMARY KEY (Id),
    CONSTRAINT UQ_PasskeyChallenges_Email_ChallengeType    UNIQUE      (Email, ChallengeType)
);