DECLARE @id nvarchar(max) = cast(JSON_VALUE(@document,'$.Id') as nvarchar(max))
IF EXISTS (SELECT 1 FROM {@schema_name}.{@table_name} WHERE vId = @id)
	THROW 50002, 'Document with this Id already exists', 1

DECLARE @newETag UNIQUEIDENTIFIER = newid()

INSERT INTO {@schema_name}.{@table_name} (Document, ETag) VALUES (@document, @etag)

