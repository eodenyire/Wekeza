-- Insert missing migration history records
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion") 
VALUES 
    ('20260122171306_InitialCreate', '8.0.23'),
    ('20260122205721_AddBranchManagerTables', '8.0.23'),
    ('20260123205321_AddCustomerCareEntities', '8.0.23')
ON CONFLICT ("MigrationId") DO NOTHING;