CREATE TABLE {@schema_name}.{@table_name} (
	Document NVARCHAR(MAX),
	vId AS JSON_VALUE(Document,'$.Id')
)

CREATE INDEX idx_{@table_name}_json_Id
ON {@schema_name}.{@table_name}(vId)