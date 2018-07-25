using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Ivony.Data.Queries;
using Ivony.Data.Common;

namespace Ivony.Data
{


  /// <summary>
  /// SQL模板解析器
  /// </summary>
  public class TemplateParser
  {

    private static Regex numberRegex = new Regex( @"\G\{(?<index>[0-9]+)\}", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture );



    public static ParameterizedQuery ParseTemplate( FormattableString template )
    {
      var builder = new ParameterizedQueryBuilder();
      return ParseTemplate( builder, template );
    }



    /// <summary>
    /// 解析查询模板
    /// </summary>
    /// <param name="builder">参数化查询构建器</param>
    /// <param name="templateText">模板文本</param>
    /// <param name="args">模板参数</param>
    /// <returns>解析结果</returns>
    public static ParameterizedQuery ParseTemplate( ParameterizedQueryBuilder builder, FormattableString template )
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


        return builder.CreateQuery();
      }
    }

    private static FormatException FormatError( string templateText, int i )
    {
      return new FormatException( string.Format( "解析字符串 \"{0}\" 时在字符 {1} 处出现问题。", templateText, i ) );
    }

    private static void AddParameter( ParameterizedQueryBuilder builder, object value )
    {

      var array = value as Array;
      if ( array != null && !(array is byte[]) )
        value = new ParameterArray( array );

      builder.AppendParameter( value );
    }
  }
}
