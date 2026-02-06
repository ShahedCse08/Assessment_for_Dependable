CREATE PROCEDURE GetInvoiceById
   @InvoiceId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        InvoiceNumber,
        [Date],
        CustomerName,
        Contact,
        SubTotal,
        Discount,
        GrandTotal
    FROM SalesMaster
    WHERE Id = @InvoiceId;

    SELECT 
        d.Id,
        d.SalesMasterId,
        d.MedicineId,
        d.MedicineName,
        d.BatchNumber,
        d.ExpiryDate,
        d.Quantity,
        d.UnitPrice,
        d.LineTotal
    FROM SalesDetails d
    WHERE d.SalesMasterId = @InvoiceId
    ORDER BY d.Id;
END
GO
