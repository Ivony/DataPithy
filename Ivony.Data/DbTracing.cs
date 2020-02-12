﻿using Ivony.Data.Queries;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data
{


  /// <summary>
  /// IDbTracing 的一个标准实现
  /// </summary>
  public sealed class DbTracing : IDbTracing
  {


    /// <summary>
    /// 创建一个 DbTracing 实例
    /// </summary>
    /// <param name="query">即将执行的查询对象</param>
    public DbTracing( DbQuery query ) : this( query, null ) { }



    /// <summary>
    /// 创建一个 DbTracing 实例
    /// </summary>
    /// <param name="query">即将执行的查询对象</param>
    /// <param name="complateCallback">当查询执行完成之后需要回调的方法</param>
    public DbTracing( DbQuery query, Action<DbTracing> complateCallback )
    {

      QueryObject = query;

      ExecutionTime = TimeSpan.Zero;
      QueryTime = TimeSpan.Zero;


      callback = complateCallback;
    }




    private readonly Stopwatch executionStopwatch = new Stopwatch();
    private readonly Stopwatch queryStopwatch = new Stopwatch();



    /// <summary>
    /// 获取此次查询执行的查询对象
    /// </summary>
    public DbQuery QueryObject
    {
      get;
      private set;
    }

    /// <summary>
    /// 获取此次查询执行的命令对象
    /// </summary>
    public object CommandObject
    {
      get;
      private set;
    }



    /// <summary>
    /// 数据库查询执行时间
    /// </summary>
    public TimeSpan ExecutionTime
    {
      get;
      private set;
    }


    /// <summary>
    /// 总计查询时间（即查询执行时间+数据读取时间）
    /// </summary>
    public TimeSpan QueryTime
    {
      get;
      private set;
    }


    /// <summary>
    /// 获取查询中出现的异常（如果有的话）
    /// </summary>
    public Exception Exception
    {
      get;
      private set;
    }



    private readonly List<TraceEventDescriptor> events = new List<TraceEventDescriptor>();


    /// <summary>
    /// 获取查询过程中所出现的事件列表
    /// </summary>
    public IReadOnlyList<TraceEventDescriptor> TraceEvents => events;


    private readonly Action<DbTracing> callback;


    void IDbTracing.OnExecuting( object commandObject )
    {

      events.Add( new TraceEventDescriptor( "OnExecuting", DateTime.UtcNow ) );
      CommandObject = commandObject;
      executionStopwatch.Restart();
      queryStopwatch.Restart();
    }

    void IDbTracing.OnLoadingData( IDbExecuteContext context )
    {
      events.Add( new TraceEventDescriptor( "OnLoadingData", DateTime.UtcNow ) );
      executionStopwatch.Stop();
      ExecutionTime = executionStopwatch.Elapsed;
    }

    void IDbTracing.OnComplete()
    {
      if ( Exception != null )//如果已经因为异常而退出，则不再执行 Complete 逻辑
        return;


      events.Add( new TraceEventDescriptor( "OnComplete", DateTime.UtcNow ) );
      queryStopwatch.Stop();
      QueryTime = queryStopwatch.Elapsed;

      OnCompleted();
    }


    void IDbTracing.OnException( Exception exception )
    {
      events.Add( new TraceEventDescriptor( "OnException", DateTime.UtcNow ) );
      executionStopwatch.Stop();
      queryStopwatch.Stop();

      QueryTime = queryStopwatch.Elapsed;

      Exception = exception;

      OnCompleted();
    }


    private void OnCompleted()
    {
      callback?.Invoke( this );
    }


  }


  /// <summary>
  /// 定义 DbTracing 在追踪过程中发生的事件的描述
  /// </summary>
  public struct TraceEventDescriptor
  {

    internal TraceEventDescriptor( string name, DateTime time )
    {
      UtcTime = time;
      EventName = name;
    }
    /// <summary>
    /// 事件名称
    /// </summary>
    public string EventName { get; }

    /// <summary>
    /// 事件发生时间
    /// </summary>
    public DateTime UtcTime { get; private set; }

  }

}
