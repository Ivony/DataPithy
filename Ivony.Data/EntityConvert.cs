using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data
{


  /// <summary>
  /// 提供实体转换的静态方法
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public static class EntityConvert<T>
  {



    private static object sync = new object();
    private static Dictionary<PropertyInfo, object[]> _propertyAttributesCache = new Dictionary<PropertyInfo, object[]>();


    /// <summary>
    /// 获取指定属性上的特性
    /// </summary>
    /// <param name="property">要获取特性的属性</param>
    /// <returns>属性上所设置的特性</returns>
    private static object[] GetAttributes( PropertyInfo property )
    {
      lock ( sync )
      {

        if ( _propertyAttributesCache.TryGetValue( property, out object[] attributes ) )
          return attributes;

        attributes = property.GetCustomAttributes( false );

        _propertyAttributesCache[property] = attributes;

        return attributes;
      }
    }


    /// <summary>
    /// 获取属性所对应的字段名
    /// </summary>
    /// <param name="property">要获取字段名的属性</param>
    /// <returns></returns>
    private static string GetFieldname( PropertyInfo property )
      => property.GetCustomAttribute<FieldNameAttribute>()?.FieldName ?? property.Name;




    private static Action<IDataRecord, T> fillMethod;

    /// <summary>
    /// 获取实体填充方法
    /// </summary>
    /// <returns>针对指定实体的转换方法</returns>
    internal static Action<IDataRecord, T> GetFillMethod()
    {

      lock ( sync )
      {
        return fillMethod ??= CreateFillMethod();
      }

    }

    private static Action<IDataRecord, T> CreateFillMethod()
    {
      var entityType = typeof( T );

      var dataRecord = Expression.Parameter( typeof( IDataRecord ), "dataRecord" );
      var entity = Expression.Parameter( entityType, "entity" );

      var valueMethod = typeof( DataRecordExtensions ).GetMethod( nameof( DataRecordExtensions.FieldValue ), 1, new[] { typeof( IDataRecord ), typeof( string ) } );

      var setters =
        from property in entityType.GetProperties()
        where property.GetCustomAttribute<NonFieldAttribute>() is null
        where property.CanWrite
        let name = GetFieldname( property )
        let set = property.GetSetMethod()
        let value = Expression.Call( valueMethod.MakeGenericMethod( property.PropertyType ), dataRecord, Expression.Constant( name ) )
        select Expression.Call( entity, set, value );

      return Expression.Lambda<Action<IDataRecord, T>>( Expression.Block( setters.ToArray() ), dataRecord, entity ).Compile();

    }


    /// <summary>
    /// 用指定的数据对象填充实体对象
    /// </summary>
    /// <param name="dataRecord">数据记录</param>
    /// <param name="entity">要填充的实体对象</param>
    /// <returns>填充好的实体对象</returns>
    public static T FillEntity( IDataRecord dataRecord, T entity )
    {
      GetFillMethod()( dataRecord, entity );
      return entity;
    }




    static EntityConvert()
    {
      thisFillMethod = typeof( EntityConvert<T> ).GetMethod( nameof( FillEntity ) );

      var type = typeof( T );
      var attribute = type.GetCustomAttributes( typeof( EntityConvertAttribute ), false ).OfType<EntityConvertAttribute>().FirstOrDefault();

      if ( attribute != null )
        converterActivator = attribute.CreateConverter<T>;   //缓存创建实例的方法
      else
        converterActivator = CreateConverter;
    }



    private static MethodInfo thisFillMethod;
    private static Func<IEntityConverter<T>> converterActivator;
    private static IEntityConverter<T> converter;




    /// <summary>
    /// 获取实体转换器
    /// </summary>
    /// <returns>实体转换器</returns>
    internal static IEntityConverter<T> GetConverter()
    {

      lock ( sync )
      {
        if ( converter != null )
          return converter;

      }


      var instance = converterActivator();

      lock ( sync )
      {
        if ( instance.IsReusable && converter == null )
          converter = instance;


        return converter ?? instance;
      }
    }


    private static bool CheckMethodSignature( MethodBase method, params Type[] types )
    {
      return new HashSet<Type>( method.GetParameters().Select( p => p.ParameterType ) ).SetEquals( new HashSet<Type>( types ) );
    }


    /// <summary>
    /// 创建实体转换器
    /// </summary>
    /// <returns>实体转换器</returns>
    private static IEntityConverter<T> CreateConverter()
    {

      var type = typeof( T );


      var parameter = Expression.Parameter( typeof( IDataRecord ), "dataItem" );

      var constructor = type.GetConstructor( new[] { typeof( IDataRecord ) } );
      if ( constructor != null )
        return CreateConverter( Expression.New( constructor, parameter ) );

      constructor = type.GetConstructor( Array.Empty<Type>() );
      if ( constructor != null )
        return CreateConverter( Expression.Call( thisFillMethod, parameter, Expression.New( constructor ) ) );

      throw new NotSupportedException( string.Format( "不支持 {0} 类型的实体转换，因为该类型没有公开的无参或 IDataRecord 类型的构造函数，也没有指定了自定义实体类型转换器。", typeof( T ).AssemblyQualifiedName ) );


      IEntityConverter<T> CreateConverter( Expression body )
      {
        var func = Expression.Lambda<Func<IDataRecord, T>>( body, parameter );
        return new EntityConverter( func.Compile() );
      }

    }

    private static IEntityConverter<T> CreateConverter( MethodInfo method )
    {
      return new EntityConverter( (Func<IDataRecord, T>) Delegate.CreateDelegate( typeof( Func<IDataRecord, T> ), method ) );

    }







    private class EntityConverter : IEntityConverter<T>
    {

      private Func<IDataRecord, T> _method;

      public EntityConverter( Func<IDataRecord, T> method )
      {
        _method = method;
      }


      public T Convert( IDataRecord dataItem )
      {
        return _method( dataItem );
      }

      public bool IsReusable
      {
        get { return true; }
      }
    }




    /// <summary>
    /// 提供默认的 EntityConverter 对象，这个对象什么都不做，并且被设置为可重用和需要预转换。
    /// </summary>
    private class DefaultEntityConverter : IEntityConverter<T>
    {
      public T Convert( IDataRecord dataItem )
      {
        throw new NotImplementedException();
      }

      public bool IsReusable { get { return true; } }
    }




  }
}
