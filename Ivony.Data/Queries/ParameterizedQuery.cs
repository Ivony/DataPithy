using Ivony.Data.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ivony.Data.Queries
{

  /// <summary>
  /// 代表一个参数化查询
  /// </summary>
  public class ParameterizedQuery : DbQuery, IParameterizedQueryPartial
  {



    /// <summary>
    /// 定义匹配占位符的正则表达式
    /// </summary>
    public static readonly Regex ParameterPlaceholdRegex = new Regex( @"(&#(?<index>[0-9]+)#)|(@#(?<dbName>([^#]|##)+)#)", RegexOptions.Compiled );


    /// <summary>
    /// 定义系统参数占位符的正则表达式
    /// </summary>
    public static readonly Regex SystemParameterPlaceholdRegex = new Regex( @"{(?<index>[0-9]+)}", RegexOptions.Compiled );

    /// <summary>
    /// 获取查询文本模板
    /// </summary>
    public string TextTemplate
    {
      get;
      private set;
    }


    /// <summary>
    /// 获取参数值
    /// </summary>
    public object[] ParameterValues
    {
      get;
      private set;
    }


    /// <summary>
    /// 判断该参数化查询是否为一个空的查询
    /// </summary>
    internal bool IsEmpty() => TextTemplate == String.Empty;


    /// <summary>
    /// 构建参数化查询对象
    /// </summary>
    /// <param name="template">查询文本模板</param>
    /// <param name="values">参数值</param>
    /// <param name="configures">查询配置数据</param>
    internal ParameterizedQuery( string template, object[] values, DbQueryConfigures configures = null ) : base( configures )
    {
      TextTemplate = template ?? throw new ArgumentNullException( nameof( template ) );


      if ( values == null )
        throw new ArgumentNullException( nameof( values ) );

      ParameterValues = new object[values.Length];
      values.CopyTo( ParameterValues, 0 );
    }




    private string ConvertTextTemplate( string format )
    {
      return SystemParameterPlaceholdRegex.Replace( format, match => $"#{match.Groups["index"]}#" );
    }


    /// <summary>
    /// 将参数化查询解析为另一个参数化查询的一部分。
    /// </summary>
    /// <param name="builder">参数化查询构建器</param>
    public void AppendTo( IParameterizedQueryBuilder builder )
    {

      int index = 0;

      foreach ( Match match in ParameterPlaceholdRegex.Matches( TextTemplate ) )
      {

        var length = match.Index - index;
        if ( length > 0 )
          builder.Append( TextTemplate.Substring( index, length ) );


        var parameterIndex = int.Parse( match.Groups["index"].Value );
        builder.AppendParameter( ParameterValues[parameterIndex] );

        index = match.Index + match.Length;
      }

      builder.Append( TextTemplate.Substring( index, TextTemplate.Length - index ) );
    }



    /// <summary>
    /// 重写 ToString 方法，输出参数化查询的字符串表达形式
    /// </summary>
    /// <returns>字符串表达形式</returns>
    public override string ToString()
    {
      if ( stringExpression == null )
        stringExpression = GetString();

      return stringExpression;
    }

    private string stringExpression;

    private string GetString()
    {
      var writer = new StringWriter();

      writer.WriteLine( "\"" + TextTemplate.Replace( "\"", "\"\"" ) + "\"" );
      writer.WriteLine();

      for ( int i = 0; i < this.ParameterValues.Length; i++ )
      {
        writer.WriteLine( "#{0}#: {1}", i, ParameterValues[i] );
      }

      return writer.ToString();
    }

    /// <summary>
    /// 串联两个参数化查询对象
    /// </summary>
    /// <param name="query1">第一个参数化查询对象</param>
    /// <param name="query2">第二个参数化查询对象</param>
    /// <returns>串联后的参数化查询对象</returns>
    public static ParameterizedQuery operator +( ParameterizedQuery query1, ParameterizedQuery query2 )
    {
      if ( query2 == null || query2.IsEmpty() )
        return query1;

      return query1.Concat( query2 );
    }

    /// <summary>
    /// 串联两个参数化查询对象
    /// </summary>
    /// <param name="query1">第一个参数化查询对象</param>
    /// <param name="query2">第二个参数化查询对象</param>
    /// <returns>串联后的参数化查询对象</returns>
    public static ParameterizedQuery operator +( ParameterizedQuery query1, FormattableString query2 )
    {
      return query1.Concat( query2 );
    }

    /// <summary>
    /// 串联两个参数化查询对象
    /// </summary>
    /// <param name="query1">第一个参数化查询对象</param>
    /// <param name="query2">第二个参数化查询对象</param>
    /// <returns>串联后的参数化查询对象</returns>
    public static ParameterizedQuery operator +( FormattableString query1, ParameterizedQuery query2 )
    {
      return Db.Template( query1 ).Concat( query2 );
    }



    internal bool IsStartWithWhiteSpace()
    {
      return char.IsWhiteSpace( TextTemplate[0] );
    }


    /// <summary>
    /// 创建使用新的查询配置的副本
    /// </summary>
    /// <param name="configures">要使用的查询配置</param>
    /// <returns></returns>
    protected internal override DbQuery Clone( DbQueryConfigures configures )
    {
      return new ParameterizedQuery( TextTemplate, ParameterValues, configures );
    }

  }
}
