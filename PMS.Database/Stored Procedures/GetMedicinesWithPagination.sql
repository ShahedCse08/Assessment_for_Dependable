CREATE PROCEDURE GetMedicinesWithPagination
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchKeyword NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @TotalCount INT;

    SELECT 
        @TotalCount = COUNT(*)
    FROM Medicines
    WHERE
        (@SearchKeyword IS NULL
         OR Name LIKE '%' + @SearchKeyword + '%'
         OR BatchNumber LIKE '%' + @SearchKeyword + '%');

    SELECT
        m.*,
        @TotalCount AS TotalCount
    FROM Medicines m
    WHERE
        (@SearchKeyword IS NULL
         OR Name LIKE '%' + @SearchKeyword + '%'
         OR BatchNumber LIKE '%' + @SearchKeyword + '%')
         
    ORDER BY m.Id desc
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
GO
