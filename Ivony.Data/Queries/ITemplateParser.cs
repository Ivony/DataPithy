using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Ivony.Data.Queries
{


  /// <summary>
  /// 定义字符串模板查询解析器
  /// </summary>
  public interface ITemplateParser
  {
    /// <summary>
    /// 解析字符串模板
    /// </summary>
    /// <param name="template">字符串模板</param>
    /// <returns>解析出来的参数化查询对象</returns>
    ParameterizedQuery ParseTemplate( FormattableString template );
  }


  /// <summary>
  /// SQL模板解析器
  /// </summary>
  public class TemplateParser : ITemplateParser
  {


    private IParameterizedQueryService _service;

    /// <summary>
    /// 创建 TemplateParser 对象
    /// </summary>
    /// <param name="service">参数化查询服务</param>
    public TemplateParser( IParameterizedQueryService service )
    {
      _service = service;
    }

    private static Regex numberRegex = new Regex( @"\G\{(?<index>[0-9]+)\}", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture );



    /// <summary>
    /// 解析查询模板
    /// </summary>
    /// <param name="template">查询模板</param>
    /// <returns>参数化查询对象</returns>
    public ParameterizedQuery ParseTemplate( FormattableString template )
    {
      return ParseTemplate( _service.CreateQueryBuild(), template );
    }



    /// <summary>
    /// 解析查询模板
    /// </summary>
    /// <param name="builder">参数化查询构建器</param>
    /// <param name="template">查询模板</param>
    /// <returns>解析结果</returns>
    private ParameterizedQuery ParseTemplate( IParameterizedQueryBuilder builder, FormattableString template )
    {
      if ( builder == null )
        throw new ArgumentNullException( nameof( builder ) );

      if ( template == null )
        throw new ArgumentNullException( nameof( template ) );



      lock ( builder.SyncRoot )
      {
        var templateText = template.Format;

        for ( var i = 0; i < templateText.Length; i++ )
        {

          var ch = templateText[i];

          if ( ch == '{' )
          {

            if ( i == templateText.Length - 1 )
              throw FormatError( templateText, i );

            if ( templateText[i + 1] == '{' )
            {
              i++;
              builder.Append( '{' );
              continue;
            }



            Match match = null;

            do
            {
              match = numberRegex.Match( templateText, i );

              if ( match.Success )
              {

                int parameterIndex;
                if ( !int.TryParse( match.Groups["index"].Value, out parameterIndex ) )
                  throw FormatError( templateText, i );

                AddParameter( builder, template.GetArgument( parameterIndex ) );
                break;
              }
            } while ( false );


            if ( match == null || !match.Success )
              throw FormatError( templateText, i );
            i += match.Length - 1;


          }
          else if ( ch == '}' )
          {
            if ( i == templateText.Length - 1 )
              throw FormatError( templateText, i );

            if ( templateText[i + 1] == '}' )
            {
              i++;
              builder.Append( '}' );
              continue;
            }
          }

          else
            builder.Append( ch );

        }


        return builder.BuildQuery( null );
      }
    }

    private static FormatException FormatError( string templateText, int i )
    {
      return new FormatException( string.Format( "解析字符串 \"{0}\" 时在字符 {1} 处出现问题。", templateText, i ) );
    }

    private void AddParameter( IParameterizedQueryBuilder builder, object value )
    {

      var array = value as Array;
      if ( array != null && !(array is byte[]) )
        value = new ParameterList( array );

      var tuple = value as ITuple;
      if ( tuple != null )
        value = ParameterList.Create( tuple );

      var template = value as FormattableString;
      if ( template != null )
        value = ParseTemplate( template );

      builder.AppendParameter( value );
    }
  }


}