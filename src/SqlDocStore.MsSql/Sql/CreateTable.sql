IF NOT EXISTS (SELECT * FROM sys.objects o INNER JOIN sys.schemas s ON o.schema_id = s.schema_id WHERE o.name='{@table_name}' AND s.name = '{@schema_name}' AND o.type='U')
BEGIN
	CREATE TABLE {@schema_name}.{@table_name} (
		Document NVARCHAR(MAX),
		ETag UNIQUEIDENTIFIER,
		CONSTRAINT [Document must be formatted as JSON]
		CHECK (ISJSON(Document)>0),
		vId AS JSON_VALUE(Document,'$.Id')
	)

	CREATE INDEX idx_{@table_name}_json_Id
	ON {@schema_name}.{@table_name}(vId)
END