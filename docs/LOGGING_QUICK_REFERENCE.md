# 🎯 BlogApp Logging Quick Reference

## 📊 3-Tier Logging Architecture

```
┌──────────────────────────────────────────────────────────────────┐
│                    LOGGING ARCHITECTURE                          │
└──────────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────────┐
│ 🔍 Tier 1: FILE LOGS (Development & Debug)                      │
├──────────────────────────────────────────────────────────────────┤
│ Location:    logs/blogapp-YYYY-MM-DD.txt                        │
│ Levels:      Debug, Info, Warning, Error, Critical             │
│ Retention:   31 days                                            │
│ Purpose:     Debugging, troubleshooting                         │
│ Volume:      HIGH (all requests, stack traces)                  │
└──────────────────────────────────────────────────────────────────┘
                              ↓
┌──────────────────────────────────────────────────────────────────┐
│ 📊 Tier 2: STRUCTURED LOGS (Production Monitoring)              │
├──────────────────────────────────────────────────────────────────┤
│ Location:    PostgreSQL "Logs" table                            │
│ Levels:      Information, Warning, Error, Critical              │
│ Retention:   90 days (auto-cleanup @ 3 AM daily)                │
│ Purpose:     Monitoring, alerting, analytics                    │
│ Volume:      MEDIUM (important events only)                     │
│ Query:       SELECT * FROM "Logs" WHERE level = 'Error'         │
└──────────────────────────────────────────────────────────────────┘
                              ↓
┌──────────────────────────────────────────────────────────────────┐
│ 📋 Tier 3: ACTIVITY LOGS (Compliance & Audit)                   │
├──────────────────────────────────────────────────────────────────┤
│ Location:    PostgreSQL "ActivityLogs" table                    │
│ Events:      User actions (create/update/delete)                │
│ Retention:   UNLIMITED (compliance requirement)                 │
│ Purpose:     Audit trail, security, legal                       │
│ Volume:      LOW (business events only)                         │
│ Query:       SELECT * FROM "ActivityLogs" WHERE "UserId" = 5    │
└──────────────────────────────────────────────────────────────────┘
```

---

## 🎨 Log Level Decision Tree

```
Is this a development detail? (variable values, flow control)
    ↓ YES → LogDebug() → File only
    ↓ NO
    
Is this an important business event? (user login, order created)
    ↓ YES → LogInformation() → File + DB
    ↓ NO
    
Is this a potential issue? (deprecated API, high latency)
    ↓ YES → LogWarning() → File + DB + Alert
    ↓ NO
    
Is this a handled error? (validation failure, external API error)
    ↓ YES → LogError() → File + DB + Alert + Investigate
    ↓ NO
    
Is this a system failure? (DB down, out of memory)
    ↓ YES → LogCritical() → File + DB + Alert + Page On-call Engineer
```

---

## 🚦 When to Use Which Log Type?

| Scenario | File Log | Structured Log | Activity Log |
|----------|----------|----------------|--------------|
| User logged in | ✅ Debug | ✅ Info | ✅ Activity |
| Post created | ✅ Debug | ✅ Info | ✅ Activity |
| Validation failed | ✅ Warning | ✅ Warning | ❌ |
| Exception thrown | ✅ Error | ✅ Error | ❌ |
| DB connection lost | ✅ Critical | ✅ Critical | ❌ |
| Request details | ✅ Debug | ❌ | ❌ |
| Stack trace | ✅ Error | ✅ Error | ❌ |
| User deleted post | ✅ Info | ✅ Info | ✅ Activity |

---

## 💻 Code Examples

### ✅ GOOD Examples

```csharp
// 1. Structured logging with parameters
_logger.LogInformation(
    "User {UserId} created post {PostId} in category {CategoryId}",
    userId, postId, categoryId
);

// 2. Exception logging with context
try {
    await ProcessPayment(orderId, amount);
} catch (Exception ex) {
    _logger.LogError(ex, 
        "Payment processing failed for order {OrderId}, amount {Amount}", 
        orderId, amount
    );
    throw;
}

// 3. Performance-sensitive logging
if (_logger.IsEnabled(LogLevel.Debug)) {
    _logger.LogDebug("Query result: {@Result}", expensiveQuery);
}

// 4. Activity logging (automatic via MediatR)
var command = new CreatePostCommand { Title = "New Post" };
await _mediator.Send(command);
// → Automatically logged to ActivityLogs table
```

### ❌ BAD Examples

```csharp
// ❌ String concatenation
_logger.LogInformation("User " + userId + " created post " + postId);

// ❌ Logging sensitive data
_logger.LogInformation("User password: {Password}", password);

// ❌ Swallowing exceptions
try {
    await DeletePost(postId);
} catch { } // ❌ No logging!

// ❌ Using wrong log level
_logger.LogCritical("User clicked button"); // ❌ Not critical!
```

---

## 📈 Monitoring Queries

### File Logs (CLI)
```bash
# Tail live logs
tail -f logs/blogapp-2025-10-25.txt

# Search for errors
grep "ERROR" logs/blogapp-*.txt

# Count errors by day
grep -c "ERROR" logs/blogapp-*.txt
```

### Database Logs (SQL)
```sql
-- Errors in last 24 hours
SELECT message, raise_date, exception
FROM "Logs"
WHERE level = 'Error' 
  AND raise_date > NOW() - INTERVAL '24 hours';

-- Top error messages
SELECT message, COUNT(*) as count
FROM "Logs"
WHERE level = 'Error'
GROUP BY message
ORDER BY count DESC
LIMIT 10;

-- Slow requests
SELECT 
    properties->>'RequestPath' as path,
    AVG((properties->>'ElapsedMilliseconds')::numeric) as avg_ms
FROM "Logs"
WHERE properties ? 'ElapsedMilliseconds'
GROUP BY path
HAVING AVG((properties->>'ElapsedMilliseconds')::numeric) > 1000;
```

### Activity Logs (SQL)
```sql
-- User's recent activities
SELECT 
    a."ActivityType",
    a."Title",
    a."Timestamp",
    u."UserName"
FROM "ActivityLogs" a
LEFT JOIN "AppUsers" u ON a."UserId" = u."Id"
WHERE a."UserId" = 5
ORDER BY a."Timestamp" DESC;

-- Deleted posts audit
SELECT 
    a."Title",
    a."Timestamp",
    u."UserName"
FROM "ActivityLogs" a
LEFT JOIN "AppUsers" u ON a."UserId" = u."Id"
WHERE a."ActivityType" = 'post_deleted'
ORDER BY a."Timestamp" DESC;
```

---

## ⚙️ Configuration

### appsettings.json
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",      // Production
      "Microsoft.AspNetCore": "Warning"
    },
    "File": {
      "RetentionDays": 31
    },
    "Database": {
      "RetentionDays": 90,
      "EnableAutoCleanup": true      // Runs daily @ 3 AM
    },
    "ActivityLogs": {
      "RetentionDays": 0               // Unlimited
    }
  }
}
```

### appsettings.Development.json
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",            // Development
      "Microsoft": "Information"
    }
  }
}
```

---

## 🔔 Alerting (Seq)

### Seq Queries for Alerts

```
# Error spike (>10 errors in 5 minutes)
@Level = 'Error' 
| group by time(5m) 
| where count() > 10

# Slow requests (>5 seconds)
ElapsedMilliseconds > 5000

# 500 errors
StatusCode >= 500

# Failed logins (potential attack)
@MessageTemplate = 'Login failed for user {UserId}'
| group by time(1m)
| where count() > 5
```

---

## 🎯 Summary: Why 3 Tiers?

| Aspect | File Logs | Structured Logs | Activity Logs |
|--------|-----------|-----------------|---------------|
| **Purpose** | Debug | Monitor | Audit |
| **Audience** | Developers | DevOps/SRE | Business/Legal |
| **Volume** | High | Medium | Low |
| **Retention** | Short (31d) | Medium (90d) | Unlimited |
| **Query** | grep/tail | SQL/Seq | SQL |
| **Cost** | Low | Medium | Low |
| **Performance** | Fast write | Slower write | Slowest write |

**Result:** Each tier serves a specific purpose. Combining them would:
- ❌ Hurt performance (too many DB writes)
- ❌ Waste storage (debug logs in DB)
- ❌ Violate compliance (mixing audit with debug)
- ❌ Increase costs (unnecessary DB space)

✅ **Current architecture is OPTIMAL!**

---

## 📚 Related Files

- `LOGGING_ARCHITECTURE.md` - Detailed documentation
- `ACTIVITY_LOGGING_README.md` - Activity logging specifics
- `SerilogConfiguration.cs` - Log configuration
- `LogCleanupService.cs` - Automatic cleanup
- `ActivityLoggingBehavior.cs` - Activity logging behavior
