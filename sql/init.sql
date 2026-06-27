-- RequestPlatform Database Initialization Script
-- For SQL Server running in Docker

USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'RequestPlatformDb')
BEGIN
    CREATE DATABASE RequestPlatformDb;
END
GO

USE RequestPlatformDb;
GO

-- Create Requests table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Requests')
BEGIN
    CREATE TABLE Requests (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        [Type] NVARCHAR(50) NOT NULL,
        [Status] NVARCHAR(50) NOT NULL,
        DynamicData NVARCHAR(MAX) NOT NULL,
        CreatedAt DATETIME2 NOT NULL,
        CONSTRAINT CK_Requests_DynamicData_IsJson CHECK (ISJSON(DynamicData) = 1)
    );
END
GO

-- Create indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Requests_Type' AND object_id = OBJECT_ID('Requests'))
    CREATE INDEX IX_Requests_Type ON Requests ([Type]);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Requests_Status' AND object_id = OBJECT_ID('Requests'))
    CREATE INDEX IX_Requests_Status ON Requests ([Status]);
GO

-- Sample data: Vacation requests
INSERT INTO Requests (Id, [Type], [Status], DynamicData, CreatedAt)
VALUES
    (NEWID(), 'Vacation', 'Pending',
     '{"employeeName": "Maria Garcia", "startDate": "2024-03-15", "endDate": "2024-03-22", "totalDays": 5, "reason": "Family vacation to Cancun", "contactDuringAbsence": "maria.personal@email.com"}',
     GETUTCDATE()),
    (NEWID(), 'Vacation', 'Approved',
     '{"employeeName": "Carlos Rodriguez", "startDate": "2024-04-01", "endDate": "2024-04-05", "totalDays": 3, "reason": "Personal days", "contactDuringAbsence": "+1-555-0102"}',
     DATEADD(DAY, -10, GETUTCDATE())),
    (NEWID(), 'Vacation', 'Rejected',
     '{"employeeName": "Ana Lopez", "startDate": "2024-12-23", "endDate": "2024-12-31", "totalDays": 7, "reason": "Holiday season break", "contactDuringAbsence": "ana.lopez@personal.com"}',
     DATEADD(DAY, -30, GETUTCDATE()));

-- Sample data: Loan requests
INSERT INTO Requests (Id, [Type], [Status], DynamicData, CreatedAt)
VALUES
    (NEWID(), 'Loan', 'Pending',
     '{"employeeName": "Juan Martinez", "amount": 5000.00, "currency": "USD", "installments": 12, "reason": "Home renovation", "monthlyPayment": 441.67}',
     GETUTCDATE()),
    (NEWID(), 'Loan', 'Approved',
     '{"employeeName": "Laura Fernandez", "amount": 2000.00, "currency": "USD", "installments": 6, "reason": "Emergency medical expenses", "monthlyPayment": 350.00}',
     DATEADD(DAY, -15, GETUTCDATE())),
    (NEWID(), 'Loan', 'Pending',
     '{"employeeName": "Roberto Sanchez", "amount": 10000.00, "currency": "USD", "installments": 24, "reason": "Vehicle purchase down payment", "monthlyPayment": 437.50}',
     DATEADD(DAY, -3, GETUTCDATE()));

-- Sample data: Permission requests
INSERT INTO Requests (Id, [Type], [Status], DynamicData, CreatedAt)
VALUES
    (NEWID(), 'Permission', 'Approved',
     '{"employeeName": "Diana Torres", "date": "2024-02-14", "hours": 4, "type": "Medical Appointment", "description": "Follow-up appointment with cardiologist"}',
     DATEADD(DAY, -5, GETUTCDATE())),
    (NEWID(), 'Permission', 'Pending',
     '{"employeeName": "Miguel Herrera", "date": "2024-03-01", "hours": 2, "type": "Personal Errand", "description": "Bank appointment for mortgage signing"}',
     GETUTCDATE()),
    (NEWID(), 'Permission', 'Rejected',
     '{"employeeName": "Patricia Morales", "date": "2024-02-20", "hours": 8, "type": "Personal Day", "description": "Attending a family event out of town"}',
     DATEADD(DAY, -20, GETUTCDATE()));
GO

SELECT 'Database initialized successfully. Total requests inserted: ' + CAST(COUNT(*) AS VARCHAR(10)) FROM Requests;
GO
