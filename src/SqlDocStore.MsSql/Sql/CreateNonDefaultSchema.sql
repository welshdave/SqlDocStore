IF NOT EXISTS (
		SELECT schema_name
		FROM information_schema.schemata
		WHERE schema_name = '{@schema_name}'
		)
BEGIN
	EXEC sp_executesql N'CREATE SCHEMA {@schema_name}'
END