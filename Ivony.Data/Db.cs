using Ivony.Data.Queries;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ivony.Data
{
  public static class Db
  {


    private static DbContext _default;
    private static readonly object _sync = new object();

    public static DbContext Context { get; private set; } = _default = CreateDefaultContext();



    /// <summary>
    /// 使用指定的服务提供程序初始化数据访问上下文
    /// </summary>
    /// <param name="serviceProvider">服务提供程序</param>
    public static void InitializeDb( this IServiceProvider serviceProvider )
    {
      lock ( _sync )
      {
        if ( Context != _default )
          throw new InvalidOperationException();

        Context = new DbContext( serviceProvider );
      }
    }

    /// <summary>
    /// 解析模板表达式，创建参数化查询对象
    /// </summary>
    /// <param name="template">参数化模板</param>
    /// <returns>参数化查询</returns>
    public static ParameterizedQuery T( FormattableString template )
    {
      return Template( template );
    }

    /// <summary>
    /// 解析模板表达式，创建参数化查询对象
    /// </summary>
    /// <param name="template">参数化模板</param>
    /// <returns>参数化查询</returns>
    public static ParameterizedQuery Template( FormattableString template )
    {
      return Context.GetTemplateParser().ParseTemplate( template );
    }



    /// <summary>
    /// 配置使用 DataPithy
    /// </summary>
    /// <param name="services">服务配置</param>
    public static IServiceCollection AddDataPithy( this IServiceCollection services )
    {

      services.AddSingleton( typeof( ITemplateParser ), typeof( TemplateParser ) );
      services.AddTransient( typeof( IParameterizedQueryBuilder ), typeof( ParameterizedQueryBuilder ) );

      return services;
    }




    private static DbContext CreateDefaultContext()
    {

      var services = new ServiceCollection();
      services.AddDataPithy();


      return new DbContext( services.BuildServiceProvider() );
    }

    public static IDbTransactionContext BeginTransaction()
    {
      throw new NotImplementedException();
    }

  }
}

