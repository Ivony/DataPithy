﻿using Ivony.Data.Common;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ivony.Data
{
  /// <summary>
  /// 提供面向 Entity 的扩展方法
  /// </summary>
  public static partial class EntityExtensions
  {



    /// <summary>
    /// 查询数据库并将第一个结果集填充实体类型
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">要执行的查询</param>
    /// <returns>实体集</returns>
    public static T[] ExecuteEntities<T>( this IDbExecutable query )
    {
      var data = Data.DataTableExecuteExtensions.ExecuteDataTable( query );
      return data.GetRows().Select( dataItem => dataItem.ToEntity<T>() ).ToArray();
    }


    /// <summary>
    /// 查询数据库并将第一个结果集填充实体类型
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">要执行的查询</param>
    /// <param name="token">取消指示</param>
    /// <returns>实体集</returns>
    public async static Task<T[]> ExecuteEntitiesAsync<T>( this IDbExecutable query, CancellationToken token = default( CancellationToken ) )
    {
      var data = await Data.DataTableExecuteExtensions.ExecuteDataTableAsync( query, token );
      return data.GetRows().Select( dataItem => dataItem.ToEntity<T>() ).ToArray();
    }


    /// <summary>
    /// 查询数据库并将第一个结果集填充实体类型
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">要执行的查询</param>
    /// <param name="converter">实体转换器</param>
    /// <returns>实体集</returns>
    public static T[] ExecuteEntities<T>( this IDbExecutable query, IEntityConverter<T> converter )
    {
      var data = Data.DataTableExecuteExtensions.ExecuteDataTable( query );
      return data.GetRows().Select( dataItem => dataItem.ToEntity( converter ) ).ToArray();
    }


    /// <summary>
    /// 查询数据库并将第一个结果集填充实体类型
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">要执行的查询</param>
    /// <param name="token">取消指示</param>
    /// <param name="converter">实体转换器</param>
    /// <returns>实体集</returns>
    public async static Task<T[]> ExecuteEntitiesAsync<T>( this IDbExecutable query, IEntityConverter<T> converter, CancellationToken token = default( CancellationToken ) )
    {
      var data = await Data.DataTableExecuteExtensions.ExecuteDataTableAsync( query, token );
      return data.GetRows().Select( dataItem => dataItem.ToEntity<T>( converter ) ).ToArray();
    }


    /// <summary>
    /// 查询数据库并将第一个结果集填充实体类型
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">要执行的查询</param>
    /// <param name="converter">实体转换器</param>
    /// <returns>实体集</returns>
    public static T[] ExecuteEntities<T>( this IDbExecutable query, Func<DataRow, T> converter )
    {
      var data = Data.DataTableExecuteExtensions.ExecuteDataTable( query );
      return data.GetRows().Select( dataItem => converter( dataItem ) ).ToArray();
    }


    /// <summary>
    /// 查询数据库并将第一个结果集填充实体类型
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">要执行的查询</param>
    /// <param name="token">取消指示</param>
    /// <param name="converter">实体转换器</param>
    /// <returns>实体集</returns>
    public async static Task<T[]> ExecuteEntitiesAsync<T>( this IDbExecutable query, Func<DataRow, T> converter, CancellationToken token = default( CancellationToken ) )
    {
      var data = await Data.DataTableExecuteExtensions.ExecuteDataTableAsync( query, token );
      return data.GetRows().Select( dataItem => converter( dataItem ) ).ToArray();
    }


    /// <summary>
    /// 查询数据库并将第一个结果集填充实体类型
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">要执行的查询</param>
    /// <param name="token">取消指示</param>
    /// <param name="converter">实体转换器</param>
    /// <returns>实体集</returns>
    public async static Task<T[]> ExecuteEntitiesAsync<T>( this IDbExecutable query, Func<DataRow, CancellationToken, Task<T>> converter, CancellationToken token = default( CancellationToken ) )
    {
      var data = await Data.DataTableExecuteExtensions.ExecuteDataTableAsync( query, token );
      List<T> result = new List<T>();

      foreach ( var dataItem in data.GetRows() )
      {
        var entity = await converter( dataItem, token );
        result.Add( entity );
      }

      return result.ToArray();

    }


    /// <summary>
    /// 查询数据库并将结果首行填充实体
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">要执行的查询</param>
    /// <returns>实体</returns>
    public static T ExecuteEntity<T>( this IDbExecutable query )
    {
      var dataItem = Data.DataTableExecuteExtensions.ExecuteFirstRow( query );
      return dataItem.ToEntity<T>();

    }

    /// <summary>
    /// 查询数据库并将结果首行填充实体
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">要执行的查询</param>
    /// <param name="token">取消指示</param>
    /// <returns>实体</returns>
    public async static Task<T> ExecuteEntityAsync<T>( this IDbExecutable query, CancellationToken token = default( CancellationToken ) )
    {
      var dataItem = await Data.DataTableExecuteExtensions.ExecuteFirstRowAsync( query, token );
      return dataItem.ToEntity<T>();

    }


    /// <summary>
    /// 查询数据库并将结果首行填充实体
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">要执行的查询</param>
    /// <param name="converter">实体转换方法</param>
    /// <returns>实体</returns>
    public static T ExecuteEntity<T>( this IDbExecutable query, IEntityConverter<T> converter )
    {
      var dataItem = Data.DataTableExecuteExtensions.ExecuteFirstRow( query );
      return dataItem.ToEntity<T>( converter );

    }


    /// <summary>
    /// 查询数据库并将结果首行填充实体
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">要执行的查询</param>
    /// <param name="token">取消指示</param>
    /// <param name="converter">实体转换方法</param>
    /// <returns>实体</returns>
    public async static Task<T> ExecuteEntityAsync<T>( this IDbExecutable query, IEntityConverter<T> converter, CancellationToken token = default( CancellationToken ) )
    {
      var dataItem = await Data.DataTableExecuteExtensions.ExecuteFirstRowAsync( query, token );
      return dataItem.ToEntity<T>( converter );

    }

    /// <summary>
    /// 查询数据库并将结果首行填充实体
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">要执行的查询</param>
    /// <param name="converter">实体转换方法</param>
    /// <returns>实体</returns>
    public static T ExecuteEntity<T>( this IDbExecutable query, Func<DataRow, T> converter )
    {
      var dataItem = Data.DataTableExecuteExtensions.ExecuteFirstRow( query );
      return converter( dataItem );
    }

    /// <summary>
    /// 异步查询数据库并将结果首行填充实体
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">要执行的查询</param>
    /// <param name="token">取消指示</param>
    /// <param name="converter">实体转换方法</param>
    /// <returns>实体</returns>
    public async static Task<T> ExecuteEntityAsync<T>( this IDbExecutable query, Func<DataRow, T> converter, CancellationToken token = default( CancellationToken ) )
    {
      var dataItem = await Data.DataTableExecuteExtensions.ExecuteFirstRowAsync( query, token );
      return converter( dataItem );
    }


    /// <summary>
    /// 异步查询数据库并将结果首行填充实体
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">要执行的查询</param>
    /// <param name="token">取消指示</param>
    /// <param name="converter">异步实体转换方法</param>
    /// <returns>实体</returns>
    public async static Task<T> ExecuteEntityAsync<T>( this IDbExecutable query, Func<DataRow, CancellationToken, Task<T>> converter, CancellationToken token = default( CancellationToken ) )
    {
      var dataItem = await Data.DataTableExecuteExtensions.ExecuteFirstRowAsync( query, token );
      return await converter( dataItem, token );
    }




    /// <summary>
    /// 将 DataRow 转换为实体
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="dataItem">包含数据的 DataRow</param>
    /// <returns>实体</returns>
    public static T ToEntity<T>( this DataRow dataItem )
    {
      return ToEntity<T>( dataItem.AsDataRecord(), null );
    }

    /// <summary>
    /// 将 DataRow 转换为实体
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="dataItem">包含数据的 DataRow</param>
    /// <param name="converter">实体转换器</param>
    /// <returns>实体</returns>
    public static T ToEntity<T>( this DataRow dataItem, IEntityConverter<T> converter )
    {
      var entityConverter = converter ?? EntityConvert<T>.GetConverter();
      return entityConverter.Convert( dataItem.AsDataRecord() );
    }


    /// <summary>
    /// 将 DataRow 转换为实体
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="dataItem">包含数据的 DataRow</param>
    /// <returns>实体</returns>
    public static T ToEntity<T>( this IDataRecord dataItem )
    {
      return ToEntity<T>( dataItem, null );
    }

    /// <summary>
    /// 将 DataRow 转换为实体
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="dataItem">包含数据的 DataRow</param>
    /// <param name="converter">实体转换器</param>
    /// <returns>实体</returns>
    public static T ToEntity<T>( this IDataRecord record, IEntityConverter<T> converter )
    {
      if ( record == null )
      {

        if ( typeof( T ).IsValueType )
          throw new ArgumentNullException( "dataItem" );

        else
          return default( T );//等同于return null
      }

      if ( record.FieldCount == 1 )
      {
        var value = record[0];

        if ( value is T )
          return (T) value;
      }


      var entityConverter = converter ?? EntityConvert<T>.GetConverter();
      return entityConverter.Convert( record );
    }





    private static object sync = new object();
    private static Dictionary<Type, Func<IDataRecord, object>> entityConverterDictionary = new Dictionary<Type, Func<IDataRecord, object>>();


    internal static object ToEntity( this IDataRecord dataItem, Type entityType )
    {
      return GetToEntityMethod( entityType )( dataItem );
    }


    private static Func<IDataRecord, object> GetToEntityMethod( Type entityType )
    {
      lock ( sync )
      {
        if ( entityConverterDictionary.ContainsKey( entityType ) )
          return entityConverterDictionary[entityType];


        var method = typeof( EntityExtensions )
          .GetMethod( "ToEntity", new[] { typeof( IDataRecord ) } )
          .MakeGenericMethod( entityType );

        return entityConverterDictionary[entityType] = (Func<IDataRecord, object>) Delegate.CreateDelegate( typeof( Func<IDataRecord, object> ), method );
      }
    }






    private static Dictionary<Type, Func<IDataRecord, DataColumn, object>> dbValueConverterDictionary = new Dictionary<Type, Func<IDataRecord, DataColumn, object>>();


    internal static object FieldValue( this IDataRecord dataItem, DataColumn column, Type valueType )
    {
      return GetFieldValueMethod( valueType )( dataItem, column );
    }


    private static Func<IDataRecord, DataColumn, object> GetFieldValueMethod( Type valueType )
    {
      lock ( sync )
      {
        if ( dbValueConverterDictionary.ContainsKey( valueType ) )
          return dbValueConverterDictionary[valueType];


        var method = fieldValueMethod.MakeGenericMethod( valueType );

        return dbValueConverterDictionary[valueType] = (Func<IDataRecord, DataColumn, object>) Delegate.CreateDelegate( typeof( Func<IDataRecord, DataColumn, object> ), method );
      }
    }




    /// <summary>
    /// 获取指定字段的值
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    /// <param name="dataRow">数据行</param>
    /// <param name="columnName">要返回其值的列名称</param>
    /// <returns>强类型的值</returns>
    public static T FieldValue<T>( this DataRow dataRow, string columnName )
    {
      return FieldValue<T>( dataRow, dataRow.Table.Columns[columnName] );
    }



    private static readonly MethodInfo fieldValueMethod = typeof( EntityExtensions )
      .GetMethods( BindingFlags.NonPublic | BindingFlags.Static )
      .First( method => method.Name == "FieldValue" && method.IsGenericMethod );


    private static T FieldValue<T>( this DataRow dataRow, DataColumn column )
    {
      if ( dataRow == null )
        throw new ArgumentNullException( "dataRow" );

      if ( column == null )
        throw new ArgumentNullException( "column" );


      try
      {
        return DbValueConverter.ConvertFrom<T>( dataRow[column] );
      }
      catch ( Exception e )
      {
        e.Data.Add( "DataColumnName", column.ColumnName );
        throw;
      }
    }
  }
}
