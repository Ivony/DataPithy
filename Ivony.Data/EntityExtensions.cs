using Ivony.Data.Common;

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
    public static T[] ExecuteEntities<T>( this IDbExecutable query ) => ExecuteEntities( query, EntityConvert<T>.GetConverter() );


    /// <summary>
    /// 查询数据库并将第一个结果集填充实体类型
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">要执行的查询</param>
    /// <param name="token">取消指示</param>
    /// <returns>实体集</returns>
    public static Task<T[]> ExecuteEntitiesAsync<T>( this IDbExecutable query, CancellationToken token = default ) => ExecuteEntitiesAsync( query, EntityConvert<T>.GetConverter() );

    /// <summary>
    /// 查询数据库并将第一个结果集填充实体类型
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">要执行的查询</param>
    /// <param name="converter">实体转换器</param>
    /// <returns>实体集</returns>
    public static T[] ExecuteEntities<T>( this IDbExecutable query, IEntityConverter<T> converter )
    {
      var data = DataRecordExtensions.EnumerateDataRecords( query );
      return data.Select( dataItem => dataItem.ToEntity( converter ) ).ToArray();
    }


    /// <summary>
    /// 查询数据库并将第一个结果集填充实体类型
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">要执行的查询</param>
    /// <param name="token">取消指示</param>
    /// <param name="converter">实体转换器</param>
    /// <returns>实体集</returns>
    public async static Task<T[]> ExecuteEntitiesAsync<T>( this IDbExecutable query, IEntityConverter<T> converter, CancellationToken token = default )
    {
      var result = new List<T>();

      await foreach ( var item in DataRecordExtensions.EnumerateDataRecordsAsync( query, token ) )
        result.Add( item.ToEntity( converter ) );

      return result.ToArray();
    }


    /// <summary>
    /// 查询数据库并将结果首行填充实体
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">要执行的查询</param>
    /// <returns>实体</returns>
    public static T ExecuteEntity<T>( this IDbExecutable query )
      => ExecuteEntity( query, EntityConvert<T>.GetConverter() );

    /// <summary>
    /// 查询数据库并将结果首行填充实体
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">要执行的查询</param>
    /// <param name="token">取消指示</param>
    /// <returns>实体</returns>
    public static Task<T> ExecuteEntityAsync<T>( this IDbExecutable query, CancellationToken token = default )
      => ExecuteEntityAsync( query, EntityConvert<T>.GetConverter(), token );


    /// <summary>
    /// 查询数据库并将结果首行填充实体
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">要执行的查询</param>
    /// <param name="converter">实体转换方法</param>
    /// <returns>实体</returns>
    public static T ExecuteEntity<T>( this IDbExecutable query, IEntityConverter<T> converter )
      => DataRecordExtensions.EnumerateDataRecords( query ).Select( item => item.ToEntity<T>() ).FirstOrDefault();


    /// <summary>
    /// 查询数据库并将结果首行填充实体
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="query">要执行的查询</param>
    /// <param name="token">取消指示</param>
    /// <param name="converter">实体转换方法</param>
    /// <returns>实体</returns>
    public async static Task<T> ExecuteEntityAsync<T>( this IDbExecutable query, IEntityConverter<T> converter, CancellationToken token = default )
    {
      await foreach ( var item in DataRecordExtensions.EnumerateDataRecordsAsync( query ) )
      {
        return item.ToEntity( converter );
      }

      return default;
    }




    /// <summary>
    /// 将 IDataRecord 转换为实体
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
    /// <param name="record">包含数据的 IDataRecord</param>
    /// <param name="converter">实体转换器</param>
    /// <returns>实体</returns>
    public static T ToEntity<T>( this IDataRecord record, IEntityConverter<T> converter )
    {
      if ( record is null )
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






  }
}
