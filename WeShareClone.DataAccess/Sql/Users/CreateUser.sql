INSERT INTO Users (Email, Name, Phone)
OUTPUT INSERTED.*
VALUES (@Email, @Name, @Phone);