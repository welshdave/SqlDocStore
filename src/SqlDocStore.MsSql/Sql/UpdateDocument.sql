DECLARE @id nvarchar(max) = cast(JSON_VALUE(@document,'$.Id') as nvarchar(max))
DECLARE @existingETag UNIQUEIDENTIFIER
SELECT @existingETag = ETag FROM {@schema_name}.{@table_name} where vId = @id;
IF(@existingETag <> @etag)
	THROW 50001, 'ETag has changed', 1

DECLARE @newETag UNIQUEIDENTIFIER = newid()
UPDATE {@schema_name}.{@table_name} SET Document = @document, ETag = @newETag
SELECT @newETag
