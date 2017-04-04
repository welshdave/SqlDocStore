SELECT Document FROM {@schema_name}.{@table_name} where cast(JSON_VALUE(Document,'$.Id') as nvarchar(max)) = @id;
