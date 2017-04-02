CREATE TABLE {@schema_name}.{@table_name} (
	Document NVARCHAR(MAX)
	CONSTRAINT [Document must be formatted as JSON]
	CHECK (ISJSON(Document)>0),
	vId AS JSON_VALUE(Document,'$.Id')
)

CREATE INDEX idx_{@table_name}_json_Id
ON {@schema_name}.{@table_name}(vId)