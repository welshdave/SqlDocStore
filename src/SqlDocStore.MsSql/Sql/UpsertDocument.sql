MERGE {@schema_name}.{@table_name} 
USING OPENJSON(@document) WITH (Id nvarchar(max)) As doc
ON doc.Id = vId
WHEN MATCHED THEN 
	UPDATE SET Document = @document, ETag = newid(), [Type] = @type
WHEN NOT MATCHED THEN 
	INSERT (Document, ETag, [Type]) VALUES (@document, newid(), @type)
;