using Ivony.Data.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.Common
{

  /// <summary>
  /// 辅助实现 IParameterizedQueryParser 的基类
  /// </summary>
  /// <typeparam name="TCommand">解析完成的命令对象的类型</typeparam>
  public abstract class ParameterizedQueryLiteralValueParser<TCommand> : IParameterizedQueryParser<TCommand>
  {


    /// <summary>
    /// 创建查询命令
    /// </summary>
    /// <param name="query">参数化查询</param>
    /// <returns>查询命令</returns>
    public TCommand Parse( ParameterizedQuery query )
    {

      var regex = ParameterizedQuery.DbPartialPlaceholdRegex;




      var text = regex.Replace( query.TextTemplate, ( match ) =>
      {

        if ( match.Groups["index"].Success )
        {
          var index = int.Parse( match.Groups["index"].Value );
          return GetLiteralValue( DbValueConverter.ConvertTo( query.ParameterValues[index], null ) );
        }
        else if ( match.Groups["name"].Success )
        {
          var name = match.Groups["name"].Value.Replace( "##", "#" );
          return GetDbName( name );
        }
        else
          throw new InvalidOperationException();

      } );


      return CreateCommand( text.Replace( "##", "#" ) );
    }

    /// <summary>
    /// 派生类实现此方法以创建命令对象
    /// </summary>
    /// <param name="commandText">命令文本</param>
    /// <returns></returns>
    protected abstract TCommand CreateCommand( string commandText );


    /// <summary>
    /// 获取指定参数值在查询命令中的表达式
    /// </summary>
    /// <param name="value">参数值</param>
    /// <returns>该参数值在查询命令中的字面表达方式</returns>
    protected abstract string GetLiteralValue( object value );

    /// <summary>
    /// 获取指定对象名称在查询命令中的表达式
    /// </summary>
    /// <param name="name">数据对象名称</param>
    /// <returns></returns>
    protected abstract string GetDbName( string name );

  }
}
