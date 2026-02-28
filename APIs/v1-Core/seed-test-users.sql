-- Seed Test Users for Wekeza Banking System
-- This script adds test users with standard roles for portal testing

-- Insert Teller User
INSERT INTO "Users" (
    "Id", "Username", "Email", "FirstName", "LastName", "EmployeeId", 
    "Status", "PasswordHash", "MustChangePassword", "FailedLoginAttempts", 
    "MfaEnabled", "MfaMethod", "PhoneNumber", "Department", "JobTitle",
    "SecurityClearance", "TimeZone", "Language", 
    "CreatedAt", "CreatedBy", "LastModifiedAt", "LastModifiedBy"
) VALUES (
    '550e8400-e29b-41d4-a716-446655440001'::uuid, 
    'teller', 
    'teller@wekeza.com', 
    'John', 
    'Teller', 
    'TEL001001',
    0, -- PendingActivation
    '$2a$11$5ydoLmeCWWWLXZOvBzBmHeChE8x4rDiJ1c5ZQCjCMkK4G3B4Wig3W', -- bcrypt hash of 'password'
    false,
    0,
    false,
    0, -- NoMfa
    '+254700123456',
    'Frontline Services',
    'Teller',
    1, -- Basic clearance
    'Africa/Nairobi',
    'en-US',
    NOW(), 
    'system',
    NOW(),
    'system'
) ON CONFLICT DO NOTHING;

-- Insert Admin User
INSERT INTO "Users" (
    "Id", "Username", "Email", "FirstName", "LastName", "EmployeeId", 
    "Status", "PasswordHash", "MustChangePassword", "FailedLoginAttempts", 
    "MfaEnabled", "MfaMethod", "PhoneNumber", "Department", "JobTitle",
    "SecurityClearance", "TimeZone", "Language", 
    "CreatedAt", "CreatedBy", "LastModifiedAt", "LastModifiedBy"
) VALUES (
    '550e8400-e29b-41d4-a716-446655440002'::uuid,
    'admin',
    'admin@wekeza.com',
    'System',
    'Admin',
    'ADM001001',
    0, -- PendingActivation
    '$2a$11$5ydoLmeCWWWLXZOvBzBmHeChE8x4rDiJ1c5ZQCjCMkK4G3B4Wig3W', -- bcrypt hash of 'password'
    false,
    0,
    false,
    0, -- NoMfa
    '+254700000000',
    'IT Administration',
    'System Administrator',
    3, -- High clearance
    'Africa/Nairobi',
    'en-US',
    NOW(),
    'system',
    NOW(),
    'system'
) ON CONFLICT DO NOTHING;

-- Insert Loan Officer User
INSERT INTO "Users" (
    "Id", "Username", "Email", "FirstName", "LastName", "EmployeeId", 
    "Status", "PasswordHash", "MustChangePassword", "FailedLoginAttempts", 
    "MfaEnabled", "MfaMethod", "PhoneNumber", "Department", "JobTitle",
    "SecurityClearance", "TimeZone", "Language", 
    "CreatedAt", "CreatedBy", "LastModifiedAt", "LastModifiedBy"
) VALUES (
    '550e8400-e29b-41d4-a716-446655440003'::uuid,
    'loanofficer',
    'loanofficer@wekeza.com',
    'Patricia',
    'Loan Officer',
    'LOAN001001',
    0, -- PendingActivation
    '$2a$11$5ydoLmeCWWWLXZOvBzBmHeChE8x4rDiJ1c5ZQCjCMkK4G3B4Wig3W', -- bcrypt hash of 'password'
    false,
    0,
    false,
    0, -- NoMfa
    '+254700111111',
    'Credit Department',
    'Loan Officer',
    2, -- Medium clearance
    'Africa/Nairobi',
    'en-US',
    NOW(),
    'system',
    NOW(),
    'system'
) ON CONFLICT DO NOTHING;

-- Insert Risk Officer User
INSERT INTO "Users" (
    "Id", "Username", "Email", "FirstName", "LastName", "EmployeeId", 
    "Status", "PasswordHash", "MustChangePassword", "FailedLoginAttempts", 
    "MfaEnabled", "MfaMethod", "PhoneNumber", "Department", "JobTitle",
    "SecurityClearance", "TimeZone", "Language", 
    "CreatedAt", "CreatedBy", "LastModifiedAt", "LastModifiedBy"
) VALUES (
    '550e8400-e29b-41d4-a716-446655440004'::uuid,
    'riskofficer',
    'riskofficer@wekeza.com',
    'James',
    'Risk Officer',
    'RISK001001',
    0, -- PendingActivation
    '$2a$11$5ydoLmeCWWWLXZOvBzBmHeChE8x4rDiJ1c5ZQCjCMkK4G3B4Wig3W', -- bcrypt hash of 'password'
    false,
    0,
    false,
    0, -- NoMfa
    '+254700222222',
    'Risk Management',
    'Risk Officer',
    2, -- Medium clearance
    'Africa/Nairobi',
    'en-US',
    NOW(),
    'system',
    NOW(),
    'system'
) ON CONFLICT DO NOTHING;

-- Insert Supervisor User
INSERT INTO "Users" (
    "Id", "Username", "Email", "FirstName", "LastName", "EmployeeId", 
    "Status", "PasswordHash", "MustChangePassword", "FailedLoginAttempts", 
    "MfaEnabled", "MfaMethod", "PhoneNumber", "Department", "JobTitle",
    "SecurityClearance", "TimeZone", "Language", 
    "CreatedAt", "CreatedBy", "LastModifiedAt", "LastModifiedBy"
) VALUES (
    '550e8400-e29b-41d4-a716-446655440005'::uuid,
    'supervisor',
    'supervisor@wekeza.com',
    'Jane',
    'Supervisor',
    'SUP001001',
    0, -- PendingActivation
    '$2a$11$5ydoLmeCWWWLXZOvBzBmHeChE8x4rDiJ1c5ZQCjCMkK4G3B4Wig3W', -- bcrypt hash of 'password'
    false,
    0,
    false,
    0, -- NoMfa
    '+254700333333',
    'Frontline Services',
    'Teller Supervisor',
    1, -- Basic clearance
    'Africa/Nairobi',
    'en-US',
    NOW(),
    'system',
    NOW(),
    'system'
) ON CONFLICT DO NOTHING;

-- Assign roles to users (using INSERT INTO "UserRoles" or similar pattern)
-- This depends on how your User roles are structured. Adjust based on your schema.

SELECT 'Seed data inserted successfully' as message;
