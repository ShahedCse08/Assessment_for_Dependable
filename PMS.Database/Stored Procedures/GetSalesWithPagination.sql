CREATE PROCEDURE GetSalesWithPagination
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchKeyword NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @TotalCount INT;
    SELECT @TotalCount = COUNT(*) 
    FROM SalesMaster
    WHERE (@SearchKeyword IS NULL 
           OR CustomerName LIKE '%' + @SearchKeyword + '%' 
           OR InvoiceNumber LIKE '%' + @SearchKeyword + '%' 
           OR Contact LIKE '%' + @SearchKeyword + '%');

    SELECT  
        sm.*,
        @TotalCount AS TotalCount
    FROM SalesMaster sm
    WHERE (@SearchKeyword IS NULL 
           OR CustomerName LIKE '%' + @SearchKeyword + '%' 
           OR InvoiceNumber LIKE '%' + @SearchKeyword + '%' 
           OR Contact LIKE '%' + @SearchKeyword + '%')
    ORDER BY sm.Id DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
GO
