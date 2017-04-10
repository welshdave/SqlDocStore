MERGE {@schema_name}.{@table_name} 
USING OPENJSON(@document) WITH (Id nvarchar(max)) As doc
ON doc.Id = vId
WHEN MATCHED THEN 
	UPDATE SET Document = @document, ETag = newid()
WHEN NOT MATCHED THEN 
	INSERT (Document, ETag) VALUES (@document, newid())
;