CREATE TABLE SalesDetails
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    SalesMasterId INT NOT NULL,
    MedicineId INT NOT NULL,
    MedicineName NVARCHAR(100),
    [BatchNumber] NVARCHAR(50),
    ExpiryDate DATE,
    Quantity INT,
    UnitPrice DECIMAL(10,2),
    LineTotal DECIMAL(10,2),
    FOREIGN KEY (SalesMasterId) REFERENCES SalesMaster(Id),
    FOREIGN KEY (MedicineId) REFERENCES Medicines(Id)
);
