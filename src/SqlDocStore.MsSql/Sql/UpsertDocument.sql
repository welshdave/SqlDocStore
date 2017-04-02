MERGE {@schema_name}.{@table_name} 
USING OPENJSON(@document) WITH (Id nvarchar(max)) As doc
ON doc.Id = cast(JSON_VALUE(Document,'$.Id') as nvarchar(max))
WHEN MATCHED THEN 
	UPDATE SET Document = @document
WHEN NOT MATCHED THEN 
	INSERT (Document) VALUES (@document)
;