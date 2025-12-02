# DbExecutor 选择机制详解

## 1. 核心概念

### 1.1 关键接口

| 接口名称 | 主要职责 |
|---------|---------|
| `IDbExecutable` | 定义可执行查询的基本契约，提供 `Execute()` 和 `ExecuteAsync()` 方法 |
| `IDbExecutor` | 同步执行数据库查询的核心接口 |
| `IAsyncDbExecutor` | 异步执行数据库查询的扩展接口 |
| `IDatabase` | 表示一个数据库实例，提供获取执行器的方法 |
| `IDatabaseProvider` | 数据库提供程序，负责创建和管理数据库实例 |
| `DbQueryConfigures` | 存储查询配置和相关服务的容器 |

### 1.2 核心类

| 类名称 | 主要职责 |
|-------|---------|
| `DbQuery` | 实现 `IDbExecutable` 接口的抽象基类，提供执行查询的核心逻辑 |
| `Db` | 静态工具类，管理当前数据库上下文和事务 |

## 2. ExecuteXXX 方法执行流程

### 2.1 整体流程概述

当调用 `ExecuteXXX` 方法时，其内部会调用 `IDbExecutable.Execute()` 或 `ExecuteAsync()` 方法，然后根据返回的 `IDbExecuteContext` 或 `IAsyncDbExecuteContext` 处理结果。

```
ExecuteXXX<T>() → IDbExecutable.Execute() → IDbExecutor.Execute() → IDbExecuteContext → 处理结果
```

### 2.2 同步执行器选择流程

以 `Execute()` 方法为例，其执行器选择流程如下：

```csharp
// DbQuery.Execute() 方法实现
public IDbExecuteContext Execute()
{
    var executor = Configures?.GetService<IDbExecutor>() ?? GetDatabase().GetDbExecutor();
    return executor?.Execute( this ) ?? throw NotSupported();
}
```

1. **第一步：从查询配置中获取执行器**
   - 首先检查查询配置（`Configures`）中是否直接注册了 `IDbExecutor` 服务
   - 如果有，则直接使用该执行器
   - 这允许在查询级别自定义执行器

2. **第二步：获取数据库实例**
   - 如果查询配置中没有注册执行器，则调用 `GetDatabase()` 方法获取数据库实例
   - `GetDatabase()` 方法的实现：
     ```csharp
     protected IDatabase GetDatabase()
     {
         return Configures?.GetService<IDatabase>() ?? Db.CurrentDatabase
             ?? throw NoSpecifiedDatabase();
     }
     ```
   - 首先检查查询配置中是否注册了 `IDatabase` 服务
   - 如果没有，则使用 `Db.CurrentDatabase` 获取当前上下文的数据库实例

3. **第三步：从数据库实例获取执行器**
   - 调用数据库实例的 `GetDbExecutor()` 方法获取执行器
   - 不同数据库类型（如 SQL Server、MySQL 等）会返回其特定的执行器实现

### 2.3 异步执行器选择流程

异步执行器的选择流程与同步类似，但使用 `IAsyncDbExecutor` 接口：

```csharp
// DbQuery.ExecuteAsync() 方法实现
public Task<IAsyncDbExecuteContext> ExecuteAsync( CancellationToken token = default )
{
    var executor = Configures.GetService<IAsyncDbExecutor>() ?? GetDatabase().GetAsyncDbExecutor();
    return executor?.ExecuteAsync( this, token ) ?? throw NotSupportedAsync();
}
```

1. **第一步：从查询配置中获取异步执行器**
   - 首先检查查询配置中是否直接注册了 `IAsyncDbExecutor` 服务
   - 如果有，则直接使用该异步执行器

2. **第二步：获取数据库实例**
   - 与同步流程相同，调用 `GetDatabase()` 方法获取数据库实例

3. **第三步：从数据库实例获取异步执行器**
   - 调用数据库实例的 `GetAsyncDbExecutor()` 方法获取异步执行器
   - 如果数据库不支持异步执行，可能会返回 `null` 或抛出异常

## 3. 数据库上下文管理

### 3.1 当前数据库获取机制

`Db.CurrentDatabase` 是获取当前上下文数据库的核心属性：

```csharp
// Db.CurrentDatabase 属性实现
public static IDatabase? CurrentDatabase => databaseHost.Value ?? Database( null );
```

1. **第一步：从异步本地存储获取**
   - 使用 `AsyncLocal<IDatabase>` 存储当前上下文的数据库实例
   - 支持异步上下文，确保在异步操作中上下文正确传递

2. **第二步：从数据库提供程序获取**
   - 如果异步本地存储中没有数据库实例，则调用 `Database(null)` 方法
   - `Database(null)` 会遍历所有注册的 `IDatabaseProvider`，调用其 `GetDatabase(null)` 方法
   - 返回第一个匹配的数据库实例，通常是默认数据库

### 3.2 数据库上下文切换

通过 `Db.UseDatabase()` 方法可以临时切换当前数据库上下文：

```csharp
// 使用指定名称的数据库
using (Db.UseDatabase("MyDatabase"))
{
    // 在此上下文中，CurrentDatabase 指向 "MyDatabase"
    var result = Db.T("SELECT * FROM Users").ExecuteEntities<User>();
}

// 使用指定的数据库实例
using (Db.UseDatabase(databaseInstance))
{
    // 在此上下文中，CurrentDatabase 指向 databaseInstance
    var result = Db.T("SELECT * FROM Users").ExecuteEntities<User>();
}
```

上下文切换的实现机制：

```csharp
// Db.UseDatabase() 方法实现
public static IDisposable UseDatabase( IDatabase database )
{
    if ( database == null )
        throw new ArgumentNullException( nameof( database ) );

    return DbContext.EnterContext( database );
}

// DbContext.EnterContext() 方法实现
public static IDisposable EnterContext( IDatabase current )
{
    var context = new DbContext( databaseHost.Value, current );
    databaseHost.Value = context.Current;
    return context;
}
```

- 使用 `DbContext` 类保存父级数据库上下文
- 切换时，将当前数据库实例保存到 `AsyncLocal<IDatabase>` 中
- 当 `using` 块结束时，释放 `DbContext` 对象，恢复父级数据库上下文

## 4. 查询配置管理

### 4.1 查询配置的作用

`DbQueryConfigures` 是一个字典结构，用于存储查询相关的配置和服务：

- 可以直接注册 `IDbExecutor` 或 `IAsyncDbExecutor` 服务
- 可以注册 `IDatabase` 服务，指定查询使用的数据库
- 支持服务的链式配置和合并

### 4.2 服务获取机制

```csharp
// DbQueryConfigures.GetService() 方法实现
public TService GetService<TService>() where TService : class
{
    if ( _settings.TryGetValue( servicePrefix + typeof( TService ).FullName, out var value ) == false )
        return default( TService );

    var factory = value as Func<TService>;
    if ( factory != null )
        return factory();

    return value as TService;
}
```

- 支持直接注册服务实例
- 支持注册服务工厂（`Func<TService>`），延迟创建服务实例
- 使用类型的完整名称作为服务键

### 4.3 查询配置合并

查询配置支持合并操作，允许在不同层级配置查询：

```csharp
// DbQueryConfigures.MergeWith() 方法实现
public DbQueryConfigures MergeWith( DbQueryConfigures configures )
{
    // 合并逻辑，处理冲突
}
```

- 当合并时，相同键的配置会通过冲突解析器处理
- 冲突解析器可以自定义，默认实现保留后合并的配置

## 5. 事务上下文管理

### 5.1 事务执行器获取

在事务上下文中，数据库实例本身就是一个事务对象：

```csharp
// 事务执行流程
using (var transaction = Db.EnterTransaction())
{
    // 在此上下文中，Db.CurrentDatabase 指向事务对象
    // 事务对象的 GetDbExecutor() 方法返回事务执行器
    var result = Db.T("INSERT INTO Users(Name) VALUES(@Name)", new { Name = "Test" }).ExecuteNonQuery();
    transaction.Commit();
}
```

### 5.2 事务上下文切换

```csharp
// Db.EnterTransaction() 方法实现
public static IDatabaseTransaction EnterTransaction( IDatabaseTransaction transaction )
{
    var context = DbContext.EnterContext( transaction );
    transaction.RegisterDispose( context );
    transaction.BeginTransaction();
    return transaction;
}
```

- 将事务对象作为数据库实例保存到上下文
- 事务对象的 `GetDbExecutor()` 方法返回的执行器会在事务中执行查询
- 事务提交或回滚时，所有关联的执行器操作都会受到影响

## 6. 执行器选择优先级

总结一下，DbExecutor 的选择优先级如下：

1. **查询配置中的执行器**：直接在查询配置中注册的执行器具有最高优先级
2. **查询配置中的数据库**：如果查询配置中注册了数据库，则使用该数据库的执行器
3. **当前上下文的数据库**：使用 `Db.CurrentDatabase` 获取的数据库实例的执行器
4. **默认数据库**：通过 `Database(null)` 获取的默认数据库实例的执行器

## 7. 扩展和自定义

### 7.1 自定义执行器

可以通过以下方式自定义执行器：

1. **实现 IDbExecutor 或 IAsyncDbExecutor 接口**：
   ```csharp
   public class CustomDbExecutor : IDbExecutor
   {
       public IDbExecuteContext Execute( DbQuery query )
       {
           // 自定义执行逻辑
       }
   }
   ```

2. **在查询配置中注册**：
   ```csharp
   var query = Db.T("SELECT * FROM Users");
   var configures = new DbQueryConfigures();
   configures.SetService<IDbExecutor>(new CustomDbExecutor());
   var result = query.ExecuteEntities<User>();
   ```

### 7.2 自定义数据库提供程序

```csharp
public class CustomDatabaseProvider : IDatabaseProvider
{
    public IDatabase GetDatabase( string? name )
    {
        if ( name == "Custom" )
            return new CustomDatabase();
        return null;
    }
}

// 注册提供程序
Db.AddDatabaseProvider(new CustomDatabaseProvider());
```

### 7.3 自定义异常处理

可以通过实现 `IDbExceptionFilter` 接口自定义异常处理逻辑，然后注册到数据库或执行器中。

## 8. 最佳实践

1. **优先使用查询配置**：在需要自定义执行器时，优先使用查询配置注册，避免影响全局上下文
2. **合理使用上下文切换**：使用 `using` 块明确管理数据库上下文，避免上下文泄漏
3. **事务中使用相同上下文**：在事务中确保所有查询使用相同的上下文，避免事务失效
4. **注册合适的数据库提供程序**：根据应用程序需求，注册相应的数据库提供程序
5. **考虑异步执行**：对于 I/O 密集型操作，优先使用异步执行器，提高应用程序性能

## 9. 常见问题排查

1. **"no specified database in current context" 异常**
   - 原因：当前上下文没有找到数据库实例
   - 解决方案：确保已注册数据库提供程序，或通过 `UseDatabase()` 方法指定数据库

2. **"there has no executor support query type" 异常**
   - 原因：没有找到支持该查询类型的执行器
   - 解决方案：确保已注册正确的数据库提供程序，或检查查询类型是否支持

3. **事务不生效**
   - 原因：查询使用了不同的数据库上下文
   - 解决方案：确保在事务上下文中执行所有相关查询，或显式传递事务执行器

4. **异步执行抛出同步执行异常**
   - 原因：数据库提供程序不支持异步执行
   - 解决方案：使用同步执行，或更换支持异步执行的数据库提供程序

## 10. 总结

Ivony.Data 框架的 DbExecutor 选择机制设计灵活，支持多种场景：

- 支持查询级别、数据库级别和全局级别的执行器配置
- 完善的异步上下文支持，确保在异步操作中上下文正确传递
- 强大的事务管理，支持事务内的执行器选择
- 良好的扩展性，允许自定义执行器、数据库提供程序等

通过理解 DbExecutor 的选择机制，开发者可以更好地使用和扩展 Ivony.Data 框架，满足不同场景的需求。