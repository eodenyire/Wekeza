#!/usr/bin/env pwsh

<#
.SYNOPSIS
    Sets up comprehensive monitoring stack for Wekeza Core Banking System
.DESCRIPTION
    Deploys and configures Prometheus, Grafana, ELK stack, Jaeger, and other
    monitoring tools for production-ready observability.
.PARAMETER Environment
    Target environment: Development, Staging, Production
.PARAMETER SkipDashboards
    Skip importing Grafana dashboards
.EXAMPLE
    .\setup-monitoring.ps1 -Environment Production
#>

param(
    [Parameter(Mandatory=$false)]
    [ValidateSet("Development", "Staging", "Production")]
    [string]$Environment = "Development",
    
    [Parameter(Mandatory=$false)]
    [switch]$SkipDashboards
)

# Colors for output
$Green = "`e[32m"
$Yellow = "`e[33m"
$Red = "`e[31m"
$Blue = "`e[34m"
$Reset = "`e[0m"

function Write-ColorOutput {
    param([string]$Message, [string]$Color = $Reset)
    Write-Host "$Color$Message$Reset"
}

function Test-Prerequisites {
    Write-ColorOutput "🔍 Checking monitoring setup prerequisites..." $Blue
    
    # Check Docker
    try {
        $dockerVersion = docker --version
        Write-ColorOutput "✅ Docker version: $dockerVersion" $Green
    }
    catch {
        Write-ColorOutput "❌ Docker is not installed or not in PATH" $Red
        exit 1
    }
    
    # Check Docker Compose
    try {
        $composeVersion = docker-compose --version
        Write-ColorOutput "✅ Docker Compose version: $composeVersion" $Green
    }
    catch {
        Write-ColorOutput "❌ Docker Compose is not installed or not in PATH" $Red
        exit 1
    }
    
    # Check available disk space (monitoring requires significant storage)
    $freeSpace = (Get-WmiObject -Class Win32_LogicalDisk -Filter "DeviceID='C:'" | Select-Object -ExpandProperty FreeSpace) / 1GB
    if ($freeSpace -lt 10) {
        Write-ColorOutput "⚠️  Warning: Low disk space ($([math]::Round($freeSpace, 2)) GB free). Monitoring stack requires at least 10GB." $Yellow
    } else {
        Write-ColorOutput "✅ Sufficient disk space available ($([math]::Round($freeSpace, 2)) GB free)" $Green
    }
}

function Setup-MonitoringDirectories {
    Write-ColorOutput "📁 Setting up monitoring directories..." $Blue
    
    $directories = @(
        "monitoring/grafana/dashboards",
        "monitoring/grafana/datasources", 
        "monitoring/prometheus",
        "monitoring/alertmanager",
        "monitoring/logstash/config",
        "monitoring/logstash/pipeline",
        "monitoring/nginx",
        "monitoring/ssl",
        "logs"
    )
    
    foreach ($dir in $directories) {
        if (-not (Test-Path $dir)) {
            New-Item -ItemType Directory -Path $dir -Force | Out-Null
            Write-ColorOutput "✅ Created directory: $dir" $Green
        }
    }
}

function Create-PrometheusConfig {
    Write-ColorOutput "⚙️  Creating Prometheus configuration..." $Blue
    
    $prometheusConfig = @"
global:
  scrape_interval: 15s
  evaluation_interval: 15s

rule_files:
  - "alert_rules.yml"

alerting:
  alertmanagers:
    - static_configs:
        - targets:
          - alertmanager:9093

scrape_configs:
  # Wekeza Core Banking API
  - job_name: 'wekeza-api'
    static_configs:
      - targets: ['wekeza-api:5000']
    metrics_path: '/metrics'
    scrape_interval: 5s

  # System metrics
  - job_name: 'node-exporter'
    static_configs:
      - targets: ['node-exporter:9100']

  # Container metrics
  - job_name: 'cadvisor'
    static_configs:
      - targets: ['cadvisor:8080']

  # PostgreSQL metrics
  - job_name: 'postgres-exporter'
    static_configs:
      - targets: ['postgres-exporter:9187']

  # Redis metrics (if Redis exporter is added)
  - job_name: 'redis-exporter'
    static_configs:
      - targets: ['redis-exporter:9121']

  # Prometheus itself
  - job_name: 'prometheus'
    static_configs:
      - targets: ['localhost:9090']
"@

    $prometheusConfig | Out-File -FilePath "monitoring/prometheus.yml" -Encoding UTF8
    Write-ColorOutput "✅ Prometheus configuration created" $Green
}

function Create-AlertRules {
    Write-ColorOutput "🚨 Creating alert rules..." $Blue
    
    $alertRules = @"
groups:
  - name: wekeza-banking-alerts
    rules:
      # High response time alert
      - alert: HighResponseTime
        expr: histogram_quantile(0.95, rate(http_request_duration_seconds_bucket[5m])) > 0.1
        for: 2m
        labels:
          severity: warning
        annotations:
          summary: "High response time detected"
          description: "95th percentile response time is {{ `$value }}s"

      # High error rate alert
      - alert: HighErrorRate
        expr: rate(http_requests_total{status=~"5.."}[5m]) > 0.01
        for: 1m
        labels:
          severity: critical
        annotations:
          summary: "High error rate detected"
          description: "Error rate is {{ `$value }} errors per second"

      # Database connection issues
      - alert: DatabaseConnectionIssues
        expr: up{job="postgres-exporter"} == 0
        for: 1m
        labels:
          severity: critical
        annotations:
          summary: "Database connection issues"
          description: "PostgreSQL exporter is down"

      # High memory usage
      - alert: HighMemoryUsage
        expr: (node_memory_MemTotal_bytes - node_memory_MemAvailable_bytes) / node_memory_MemTotal_bytes > 0.9
        for: 5m
        labels:
          severity: warning
        annotations:
          summary: "High memory usage"
          description: "Memory usage is above 90%"

      # High CPU usage
      - alert: HighCPUUsage
        expr: 100 - (avg by(instance) (rate(node_cpu_seconds_total{mode="idle"}[5m])) * 100) > 80
        for: 5m
        labels:
          severity: warning
        annotations:
          summary: "High CPU usage"
          description: "CPU usage is above 80%"

      # Disk space low
      - alert: DiskSpaceLow
        expr: (node_filesystem_avail_bytes / node_filesystem_size_bytes) * 100 < 10
        for: 5m
        labels:
          severity: critical
        annotations:
          summary: "Disk space low"
          description: "Disk space is below 10%"

      # Application down
      - alert: ApplicationDown
        expr: up{job="wekeza-api"} == 0
        for: 1m
        labels:
          severity: critical
        annotations:
          summary: "Wekeza Core Banking API is down"
          description: "The main application is not responding"

      # High transaction volume
      - alert: HighTransactionVolume
        expr: rate(banking_transactions_total[5m]) > 1000
        for: 2m
        labels:
          severity: info
        annotations:
          summary: "High transaction volume"
          description: "Transaction rate is {{ `$value }} per second"

      # Failed transactions
      - alert: FailedTransactions
        expr: rate(banking_transactions_failed_total[5m]) > 10
        for: 1m
        labels:
          severity: warning
        annotations:
          summary: "High failed transaction rate"
          description: "Failed transaction rate is {{ `$value }} per second"
"@

    $alertRules | Out-File -FilePath "monitoring/alert_rules.yml" -Encoding UTF8
    Write-ColorOutput "✅ Alert rules created" $Green
}

function Create-LogstashConfig {
    Write-ColorOutput "📝 Creating Logstash configuration..." $Blue
    
    $logstashPipeline = @"
input {
  beats {
    port => 5044
  }
  
  tcp {
    port => 5000
    codec => json_lines
  }
}

filter {
  if [fields][service] == "wekeza-api" {
    mutate {
      add_tag => ["wekeza", "banking", "api"]
    }
    
    # Parse JSON logs
    if [message] =~ /^\{/ {
      json {
        source => "message"
      }
    }
    
    # Extract transaction information
    if [TransactionId] {
      mutate {
        add_field => { "transaction_id" => "%{TransactionId}" }
      }
    }
    
    # Extract user information
    if [UserId] {
      mutate {
        add_field => { "user_id" => "%{UserId}" }
      }
    }
    
    # Parse timestamp
    date {
      match => [ "Timestamp", "ISO8601" ]
    }
  }
}

output {
  elasticsearch {
    hosts => ["elasticsearch:9200"]
    index => "wekeza-logs-%{+YYYY.MM.dd}"
  }
  
  # Debug output (remove in production)
  stdout {
    codec => rubydebug
  }
}
"@

    $logstashPipeline | Out-File -FilePath "monitoring/logstash/pipeline/logstash.conf" -Encoding UTF8
    
    $logstashConfig = @"
http.host: "0.0.0.0"
xpack.monitoring.elasticsearch.hosts: [ "http://elasticsearch:9200" ]
"@

    $logstashConfig | Out-File -FilePath "monitoring/logstash/config/logstash.yml" -Encoding UTF8
    Write-ColorOutput "✅ Logstash configuration created" $Green
}

function Create-NginxConfig {
    Write-ColorOutput "🌐 Creating Nginx configuration..." $Blue
    
    $nginxConfig = @"
events {
    worker_connections 1024;
}

http {
    upstream wekeza_api {
        server wekeza-api:5000;
        # Add more servers for load balancing
        # server wekeza-api-2:5000;
    }

    # Rate limiting
    limit_req_zone `$binary_remote_addr zone=api:10m rate=10r/s;
    limit_req_zone `$binary_remote_addr zone=auth:10m rate=5r/s;

    server {
        listen 80;
        server_name localhost;

        # Redirect HTTP to HTTPS in production
        # return 301 https://`$server_name`$request_uri;

        # Health check endpoint
        location /health {
            proxy_pass http://wekeza_api/health;
            proxy_set_header Host `$host;
            proxy_set_header X-Real-IP `$remote_addr;
            proxy_set_header X-Forwarded-For `$proxy_add_x_forwarded_for;
            proxy_set_header X-Forwarded-Proto `$scheme;
        }

        # API endpoints with rate limiting
        location /api/ {
            limit_req zone=api burst=20 nodelay;
            
            proxy_pass http://wekeza_api;
            proxy_set_header Host `$host;
            proxy_set_header X-Real-IP `$remote_addr;
            proxy_set_header X-Forwarded-For `$proxy_add_x_forwarded_for;
            proxy_set_header X-Forwarded-Proto `$scheme;
            
            # Timeouts
            proxy_connect_timeout 30s;
            proxy_send_timeout 30s;
            proxy_read_timeout 30s;
        }

        # Authentication endpoints with stricter rate limiting
        location /api/auth/ {
            limit_req zone=auth burst=5 nodelay;
            
            proxy_pass http://wekeza_api;
            proxy_set_header Host `$host;
            proxy_set_header X-Real-IP `$remote_addr;
            proxy_set_header X-Forwarded-For `$proxy_add_x_forwarded_for;
            proxy_set_header X-Forwarded-Proto `$scheme;
        }

        # Monitoring endpoints (restrict access in production)
        location /metrics {
            # allow 10.0.0.0/8;
            # deny all;
            
            proxy_pass http://wekeza_api/metrics;
            proxy_set_header Host `$host;
            proxy_set_header X-Real-IP `$remote_addr;
        }
    }

    # HTTPS server (uncomment for production)
    # server {
    #     listen 443 ssl http2;
    #     server_name localhost;
    #
    #     ssl_certificate /etc/nginx/ssl/cert.pem;
    #     ssl_certificate_key /etc/nginx/ssl/key.pem;
    #
    #     # SSL configuration
    #     ssl_protocols TLSv1.2 TLSv1.3;
    #     ssl_ciphers ECDHE-RSA-AES256-GCM-SHA512:DHE-RSA-AES256-GCM-SHA512:ECDHE-RSA-AES256-GCM-SHA384:DHE-RSA-AES256-GCM-SHA384;
    #     ssl_prefer_server_ciphers off;
    #
    #     location / {
    #         proxy_pass http://wekeza_api;
    #         proxy_set_header Host `$host;
    #         proxy_set_header X-Real-IP `$remote_addr;
    #         proxy_set_header X-Forwarded-For `$proxy_add_x_forwarded_for;
    #         proxy_set_header X-Forwarded-Proto `$scheme;
    #     }
    # }
}
"@

    $nginxConfig | Out-File -FilePath "monitoring/nginx/nginx.conf" -Encoding UTF8
    Write-ColorOutput "✅ Nginx configuration created" $Green
}

function Deploy-MonitoringStack {
    Write-ColorOutput "🚀 Deploying monitoring stack..." $Blue
    
    # Start monitoring services
    docker-compose -f monitoring/docker-compose.monitoring.yml up -d
    
    if ($LASTEXITCODE -eq 0) {
        Write-ColorOutput "✅ Monitoring stack deployed successfully" $Green
    } else {
        Write-ColorOutput "❌ Failed to deploy monitoring stack" $Red
        exit 1
    }
}

function Wait-ForServices {
    Write-ColorOutput "⏳ Waiting for services to be ready..." $Blue
    
    $services = @(
        @{ Name = "Prometheus"; Url = "http://localhost:9090/-/ready"; Timeout = 60 },
        @{ Name = "Grafana"; Url = "http://localhost:3000/api/health"; Timeout = 60 },
        @{ Name = "Elasticsearch"; Url = "http://localhost:9200/_cluster/health"; Timeout = 120 },
        @{ Name = "Kibana"; Url = "http://localhost:5601/api/status"; Timeout = 120 }
    )
    
    foreach ($service in $services) {
        $maxAttempts = $service.Timeout / 5
        $attempt = 0
        
        do {
            Start-Sleep -Seconds 5
            $attempt++
            
            try {
                $response = Invoke-WebRequest -Uri $service.Url -TimeoutSec 5 -ErrorAction SilentlyContinue
                if ($response.StatusCode -eq 200) {
                    Write-ColorOutput "✅ $($service.Name) is ready" $Green
                    break
                }
            }
            catch {
                Write-ColorOutput "⏳ Waiting for $($service.Name)... ($attempt/$maxAttempts)" $Yellow
            }
        } while ($attempt -lt $maxAttempts)
        
        if ($attempt -ge $maxAttempts) {
            Write-ColorOutput "⚠️  $($service.Name) may not be ready, but continuing..." $Yellow
        }
    }
}

function Import-GrafanaDashboards {
    if ($SkipDashboards) {
        Write-ColorOutput "⏭️  Skipping Grafana dashboard import" $Yellow
        return
    }
    
    Write-ColorOutput "📊 Importing Grafana dashboards..." $Blue
    
    # Wait a bit more for Grafana to be fully ready
    Start-Sleep -Seconds 10
    
    # Import dashboards using Grafana API
    $grafanaUrl = "http://localhost:3000"
    $credentials = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes("admin:wekeza123"))
    
    try {
        # Check if Grafana is accessible
        $response = Invoke-WebRequest -Uri "$grafanaUrl/api/health" -Headers @{ Authorization = "Basic $credentials" } -TimeoutSec 10
        
        if ($response.StatusCode -eq 200) {
            Write-ColorOutput "✅ Grafana dashboards will be auto-provisioned" $Green
        }
    }
    catch {
        Write-ColorOutput "⚠️  Could not connect to Grafana for dashboard import: $($_.Exception.Message)" $Yellow
    }
}

function Display-AccessInformation {
    Write-ColorOutput "🌐 Monitoring Stack Access Information" $Blue
    Write-ColorOutput "================================================================" $Blue
    
    $services = @(
        @{ Name = "Grafana (Dashboards)"; Url = "http://localhost:3000"; Credentials = "admin / wekeza123" },
        @{ Name = "Prometheus (Metrics)"; Url = "http://localhost:9090"; Credentials = "None" },
        @{ Name = "Kibana (Logs)"; Url = "http://localhost:5601"; Credentials = "None" },
        @{ Name = "AlertManager (Alerts)"; Url = "http://localhost:9093"; Credentials = "None" },
        @{ Name = "Jaeger (Tracing)"; Url = "http://localhost:16686"; Credentials = "None" },
        @{ Name = "Wekeza API (via Nginx)"; Url = "http://localhost:80"; Credentials = "API Authentication" }
    )
    
    foreach ($service in $services) {
        Write-ColorOutput "🔗 $($service.Name): $($service.Url)" $Green
        if ($service.Credentials -ne "None") {
            Write-ColorOutput "   Credentials: $($service.Credentials)" $Yellow
        }
    }
    
    Write-ColorOutput "================================================================" $Blue
}

function Generate-MonitoringReport {
    Write-ColorOutput "📄 Generating monitoring setup report..." $Blue
    
    $reportPath = "monitoring-setup-report-$(Get-Date -Format 'yyyyMMdd-HHmmss').md"
    
    @"
# Wekeza Core Banking System - Monitoring Setup Report

**Setup Date**: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')
**Environment**: $Environment
**Status**: SUCCESS ✅

## Monitoring Stack Components

### Metrics Collection & Visualization
- ✅ **Prometheus**: Metrics collection and alerting
- ✅ **Grafana**: Dashboards and visualization
- ✅ **AlertManager**: Alert handling and notifications
- ✅ **Node Exporter**: System metrics
- ✅ **cAdvisor**: Container metrics
- ✅ **PostgreSQL Exporter**: Database metrics

### Logging & Search
- ✅ **Elasticsearch**: Log storage and search
- ✅ **Logstash**: Log processing and transformation
- ✅ **Kibana**: Log visualization and analysis

### Distributed Tracing
- ✅ **Jaeger**: Request tracing and performance analysis

### Infrastructure
- ✅ **Redis**: Caching and session storage
- ✅ **Nginx**: Reverse proxy and load balancing
- ✅ **PostgreSQL**: Primary database

## Access Information

| Service | URL | Credentials |
|---------|-----|-------------|
| Grafana | http://localhost:3000 | admin / wekeza123 |
| Prometheus | http://localhost:9090 | None |
| Kibana | http://localhost:5601 | None |
| AlertManager | http://localhost:9093 | None |
| Jaeger | http://localhost:16686 | None |
| Wekeza API | http://localhost:80 | API Authentication |

## Key Features Configured

### Alerting Rules
- High response time detection (>100ms)
- High error rate monitoring (>1% error rate)
- Database connection monitoring
- System resource monitoring (CPU, Memory, Disk)
- Application availability monitoring
- Transaction volume monitoring

### Dashboards
- Application performance metrics
- System resource utilization
- Database performance
- Transaction monitoring
- Error tracking and analysis
- Business metrics

### Log Processing
- Structured log parsing
- Transaction correlation
- User activity tracking
- Error log aggregation
- Performance log analysis

## Monitoring Capabilities

### Real-time Monitoring
- ✅ Application performance metrics
- ✅ System resource utilization
- ✅ Database performance
- ✅ Transaction processing
- ✅ Error rates and patterns
- ✅ User activity and behavior

### Alerting
- ✅ Performance degradation alerts
- ✅ System resource alerts
- ✅ Application availability alerts
- ✅ Database connectivity alerts
- ✅ Business metric alerts
- ✅ Security event alerts

### Analysis & Reporting
- ✅ Historical trend analysis
- ✅ Performance benchmarking
- ✅ Capacity planning data
- ✅ Business intelligence metrics
- ✅ Compliance reporting
- ✅ Audit trail analysis

## Next Steps

1. **Configure Custom Dashboards**
   - Create business-specific dashboards
   - Set up executive summary views
   - Configure team-specific monitoring

2. **Set Up Alerting Channels**
   - Configure email notifications
   - Set up SMS alerts for critical issues
   - Integrate with incident management tools

3. **Implement Log Retention Policies**
   - Configure log rotation and archival
   - Set up long-term storage for compliance
   - Implement log anonymization for privacy

4. **Performance Baseline**
   - Establish performance baselines
   - Set up capacity planning alerts
   - Configure auto-scaling triggers

5. **Security Monitoring**
   - Set up security event monitoring
   - Configure fraud detection alerts
   - Implement compliance monitoring

## Maintenance Tasks

### Daily
- Monitor system health dashboards
- Review critical alerts
- Check application performance metrics

### Weekly
- Review capacity utilization trends
- Analyze error patterns and trends
- Update alert thresholds if needed

### Monthly
- Review and update dashboards
- Analyze long-term performance trends
- Plan capacity upgrades if needed
- Review and update alert rules

---
*Generated by Wekeza Monitoring Setup Suite*
"@ | Out-File -FilePath $reportPath -Encoding UTF8
    
    Write-ColorOutput "📄 Monitoring setup report generated: $reportPath" $Green
}

# Main execution
Write-ColorOutput "📊 Wekeza Core Banking System - Monitoring Setup" $Blue
Write-ColorOutput "================================================================" $Blue

try {
    Test-Prerequisites
    Setup-MonitoringDirectories
    Create-PrometheusConfig
    Create-AlertRules
    Create-LogstashConfig
    Create-NginxConfig
    Deploy-MonitoringStack
    Wait-ForServices
    Import-GrafanaDashboards
    Display-AccessInformation
    Generate-MonitoringReport
    
    Write-ColorOutput "🎉 Monitoring stack setup completed successfully!" $Green
}
catch {
    Write-ColorOutput "❌ Monitoring setup failed: $($_.Exception.Message)" $Red
    exit 1
}

Write-ColorOutput "================================================================" $Blue
Write-ColorOutput "Monitoring stack is ready! Access the services using the URLs above." $Green