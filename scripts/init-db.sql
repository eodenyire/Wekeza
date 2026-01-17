-- Wekeza Bank - Database Initialization Script
-- This script runs when the PostgreSQL container first starts

-- Create extensions
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pg_trgm";

-- Create schemas
CREATE SCHEMA IF NOT EXISTS audit;
CREATE SCHEMA IF NOT EXISTS reporting;

-- Grant permissions
GRANT ALL ON SCHEMA public TO wekeza_app;
GRANT ALL ON SCHEMA audit TO wekeza_app;
GRANT ALL ON SCHEMA reporting TO wekeza_app;

-- Create audit table for tracking changes
CREATE TABLE IF NOT EXISTS audit.audit_log (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    table_name VARCHAR(100) NOT NULL,
    operation VARCHAR(10) NOT NULL,
    old_data JSONB,
    new_data JSONB,
    changed_by VARCHAR(100),
    changed_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Create index for faster queries
CREATE INDEX IF NOT EXISTS idx_audit_log_table_name ON audit.audit_log(table_name);
CREATE INDEX IF NOT EXISTS idx_audit_log_changed_at ON audit.audit_log(changed_at);

-- Log successful initialization
INSERT INTO audit.audit_log (table_name, operation, new_data, changed_by)
VALUES ('system', 'INIT', '{"message": "Database initialized successfully"}', 'system');
