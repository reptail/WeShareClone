-- Add optional user-defined name to PasskeyCredentials
ALTER TABLE PasskeyCredentials
    ADD Name NVARCHAR(100) NULL;
