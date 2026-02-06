CREATE TABLE SalesMaster
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    InvoiceNumber NVARCHAR(20) NOT NULL,
    Date DATETIME NOT NULL,
    CustomerName NVARCHAR(100) NOT NULL,
    Contact NVARCHAR(50),
    SubTotal DECIMAL(10,2),
    Discount DECIMAL(10,2),
    GrandTotal DECIMAL(10,2),
    CreatedBy INT NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedBy INT NULL,
    UpdatedAt DATETIME NULL  
);