INSERT INTO Users (Email, Name)
OUTPUT INSERTED.*
VALUES (@Email, @Name);