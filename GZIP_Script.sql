CREATE ASSEMBLY NexCLR_GZIP FROM 'D:\GIT\SQL_Server_CLR\NexCLR_GZIP\bin\Debug\NexCLR_GZIP.dll';  
GO
CREATE FUNCTION [dbo].CompressString (@Text NVARCHAR(MAX))
RETURNS NVARCHAR(MAX)
AS EXTERNAL NAME NexCLR_GZIP.Gzip.CompressString
GO
CREATE FUNCTION [dbo].DecompressString (@Text NVARCHAR(MAX))
RETURNS NVARCHAR(MAX)
AS EXTERNAL NAME NexCLR_GZIP.Gzip.DecompressString

------------------------------------

DROP FUNCTION [dbo].CompressString
DROP FUNCTION [dbo].DecompressString
GO
DROP ASSEMBLY NexCLR_GZIP