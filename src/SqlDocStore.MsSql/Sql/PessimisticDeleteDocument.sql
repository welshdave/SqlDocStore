DECLARE @existingETag UNIQUEIDENTIFIER
SELECT @existingETag = ETag FROM {@schema_name}.{@table_name} where vId = @id;
IF(@existingETag <> @etag)
	THROW 50001, 'ETag has changed', 1

DELETE FROM {@schema_name}.{@table_name} where vId = @id AND ETag = @etag;
    
