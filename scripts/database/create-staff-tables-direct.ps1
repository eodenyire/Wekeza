Write-Host "Creating Staff tables directly in PostgreSQL..." -ForegroundColor Cyan

# Create a simple .NET console app to execute SQL
$csharpCode = @"
using System;
using Npgsql;

class Program
{
    static void Main()
    {
        var connectionString = "Host=localhost;Database=wekeza_banking_comprehensive;Username=postgres;Password=the_beast_pass";
        
        var staffTableSql = @"
            CREATE TABLE IF NOT EXISTS ""Staff"" (
                ""Id"" uuid NOT NULL,
                ""EmployeeId"" character varying(50) NOT NULL,
                ""FirstName"" character varying(100) NOT NULL,
                ""LastName"" character varying(100) NOT NULL,
                ""Email"" character varying(200) NOT NULL,
                ""Phone"" character varying(20) NOT NULL,
                ""Role"" character varying(50) NOT NULL,
                ""BranchId"" integer NOT NULL,
                ""BranchName"" character varying(200) NOT NULL,
                ""DepartmentId"" integer NOT NULL,
                ""DepartmentName"" character varying(200) NOT NULL,
                ""Status"" character varying(20) NOT NULL,
                ""CreatedAt"" timestamp with time zone NOT NULL,
                ""CreatedBy"" character varying(100),
                ""UpdatedAt"" timestamp with time zone,
                ""UpdatedBy"" character varying(100),
                ""LastLogin"" timestamp with time zone,
                CONSTRAINT ""PK_Staff"" PRIMARY KEY (""Id"")
            );
            
            CREATE TABLE IF NOT EXISTS ""StaffLogins"" (
                ""Id"" uuid NOT NULL,
                ""StaffId"" uuid NOT NULL,
                ""LoginTime"" timestamp with time zone NOT NULL,
                ""IpAddress"" character varying(50),
                ""UserAgent"" text,
                CONSTRAINT ""PK_StaffLogins"" PRIMARY KEY (""Id""),
                CONSTRAINT ""FK_StaffLogins_Staff_StaffId"" FOREIGN KEY (""StaffId"") REFERENCES ""Staff"" (""Id"") ON DELETE CASCADE
            );
            
            CREATE UNIQUE INDEX IF NOT EXISTS ""IX_Staff_Email"" ON ""Staff"" (""Email"");
            CREATE UNIQUE INDEX IF NOT EXISTS ""IX_Staff_EmployeeId"" ON ""Staff"" (""EmployeeId"");
            CREATE INDEX IF NOT EXISTS ""IX_StaffLogins_StaffId"" ON ""StaffLogins"" (""StaffId"");
        ";
        
        try
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            
            using var command = new NpgsqlCommand(staffTableSql, connection);
            command.ExecuteNonQuery();
            
            Console.WriteLine(""SUCCESS: Staff tables created successfully!"");
        }
        catch (Exception ex)
        {
            Console.WriteLine(""ERROR: "" + ex.Message);
        }
    }
}
"@

# Save the C# code to a temporary file
$tempDir = [System.IO.Path]::GetTempPath()
$tempProject = Join-Path $tempDir "CreateStaffTables"
$tempCsFile = Join-Path $tempProject "Program.cs"
$tempProjFile = Join-Path $tempProject "CreateStaffTables.csproj"

# Create directory
New-Item -ItemType Directory -Path $tempProject -Force | Out-Null

# Create the C# file
$csharpCode | Out-File -FilePath $tempCsFile -Encoding UTF8

# Create a simple project file
$projContent = @"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Npgsql" Version="8.0.0" />
  </ItemGroup>
</Project>
"@

$projContent | Out-File -FilePath $tempProjFile -Encoding UTF8

try {
    Write-Host "Building and running Staff table creation tool..." -ForegroundColor Yellow
    
    # Build and run the project
    Push-Location $tempProject
    dotnet run
    Pop-Location
    
    Write-Host "Staff table creation completed!" -ForegroundColor Green
    
} catch {
    Write-Host "Error creating Staff tables: $($_.Exception.Message)" -ForegroundColor Red
    
    Write-Host "Trying alternative approach..." -ForegroundColor Yellow
    
    # Alternative: Use the application's EnsureCreated method
    Write-Host "The application will create missing tables when it starts." -ForegroundColor Cyan
    Write-Host "Please start the application and test the staff creation endpoint." -ForegroundColor Green
} finally {
    # Clean up
    if (Test-Path $tempProject) {
        Remove-Item -Path $tempProject -Recurse -Force -ErrorAction SilentlyContinue
    }
}

Write-Host "Process completed!" -ForegroundColor Magenta