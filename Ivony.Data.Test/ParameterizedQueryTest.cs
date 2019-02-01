using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ivony.Data;
using Ivony.Data.Queries;

namespace Ivony.Data.Test
{
  [TestClass]
  public class ParameterizedQueryTest
  {


    [TestInitialize]
    public void Enter()
    {


    }

    [TestCleanup]
    public void Exit()
    {
    }

    [TestMethod]
    public void TemplateParserTest()
    {

      var query = Db.T( $"SELECT * FROM Users" );
      Assert.AreEqual( query.TextTemplate, "SELECT * FROM Users", "无参数模板解析测试失败" );

      query = Db.T( $"SELECT * FROM Users WHERE ID = '#123'" );
      Assert.AreEqual( query.TextTemplate, "SELECT * FROM Users WHERE ID = '##123'", "含有转义符的无参数模板解析测试失败" );

      query = Db.T( $"SELECT * FROM Users WHERE ID = {123}" );
      Assert.AreEqual( query.TextTemplate, "SELECT * FROM Users WHERE ID = &#0#", "单参数模板解析测试失败" );
      Assert.AreEqual( query.ParameterValues.Length, 1, "单参数模板解析测试失败" );
      Assert.AreEqual( query.ParameterValues[0], 123, "单参数模板解析测试失败" );

      query = Db.T( $"SELECT * FROM Users WHERE ID = '{{0}}'" );
      Assert.AreEqual( query.TextTemplate, "SELECT * FROM Users WHERE ID = '{0}'", "花括号转义测试失败" );

      query = Db.T( $"SELECT * FROM Users WHERE ID = {{{1}}}" );
      Assert.AreEqual( query.TextTemplate, "SELECT * FROM Users WHERE ID = {&#0#}", "花括号混合转义测试失败" );
      Assert.AreEqual( query.ParameterValues.Length, 1, "花括号混合转义测试失败" );
      Assert.AreEqual( query.ParameterValues[0], 1, "花括号混合转义测试失败" );


      var q = Db.T( $"SELECT ID FROM Users WHERE Username = {"Ivony"}" );
      query = Db.T( $"SELECT * FROM Users WHERE ID = ( {q} )" );
      Assert.AreEqual( query.TextTemplate, "SELECT * FROM Users WHERE ID = ( SELECT ID FROM Users WHERE Username = &#0# )", "一个参数化查询作为另一个参数化查询参数测试失败" );
      Assert.AreEqual( query.ParameterValues.Length, 1, "一个参数化查询作为另一个参数化查询参数测试失败" );
      Assert.AreEqual( query.ParameterValues[0], "Ivony", "一个参数化查询作为另一个参数化查询参数测试失败" );


      query = Db.T( $"SELECT * FROM Users WHERE ID = ( {Db.T( $"SELECT ID FROM Users WHERE Username = {"Ivony"}" ) } ) AND Status = {3}" );
      Assert.AreEqual( query.TextTemplate, $"SELECT * FROM Users WHERE ID = ( SELECT ID FROM Users WHERE Username = &#0# ) AND Status = &#1#", "一个参数化查询作为另一个参数化查询参数测试失败" );
      Assert.AreEqual( query.ParameterValues.Length, 2, "一个参数化查询作为另一个参数化查询参数测试失败" );
      Assert.AreEqual( query.ParameterValues[0], "Ivony", "一个参数化查询作为另一个参数化查询参数测试失败" );
      Assert.AreEqual( query.ParameterValues[1], 3, "一个参数化查询作为另一个参数化查询参数测试失败" );

    }


    [TestMethod]
    public void TemplateParseListTest()
    {
      var query = Db.T( $"SELECT * FROM Users WHERE ID IN ( {(1, 2, 3)} )" );
      Assert.AreEqual( query.TextTemplate, $"SELECT * FROM Users WHERE ID IN ( &#0#, &#1#, &#2# )", "以元组作为参数测试失败" );
      Assert.AreEqual( query.ParameterValues.Length, 3, "以列表作为参数测试失败" );
      Assert.AreEqual( query.ParameterValues[0], 1, "以列表作为参数测试失败" );
      Assert.AreEqual( query.ParameterValues[1], 2, "以列表作为参数测试失败" );
      Assert.AreEqual( query.ParameterValues[2], 3, "以列表作为参数测试失败" );

      query = Db.T( $"SELECT * FROM Users WHERE ID IN ( {ValueTuple.Create()} )" );
      Assert.AreEqual( query.TextTemplate, "SELECT * FROM Users WHERE ID IN (  )", "以空列表作为参数测试失败" );
      Assert.AreEqual( query.ParameterValues.Length, 0, "以空列表作为参数测试失败" );


      query = Db.T( $"SELECT * FROM Users WHERE ID IN ( {("1", "2", "3")} )" );
      Assert.AreEqual( query.TextTemplate, "SELECT * FROM Users WHERE ID IN ( &#0#, &#1#, &#2# )", "以引用类型数组作为参数测试失败" );
      Assert.AreEqual( query.ParameterValues.Length, 3, "以引用类型数组作为参数测试失败" );
      Assert.AreEqual( query.ParameterValues[0], "1", "以引用类型数组作为参数测试失败" );
      Assert.AreEqual( query.ParameterValues[1], "2", "以引用类型数组作为参数测试失败" );
      Assert.AreEqual( query.ParameterValues[2], "3", "以引用类型数组作为参数测试失败" );



      query = Db.T( $"SELECT * FROM Users WHERE ID IN ( {(1, 2, 3)} )" );
      Assert.AreEqual( query.TextTemplate, "SELECT * FROM Users WHERE ID IN ( &#0#, &#1#, &#2# )", "以object数组作为参数测试失败" );
      Assert.AreEqual( query.ParameterValues.Length, 3, "以object数组作为参数测试失败" );
      Assert.AreEqual( query.ParameterValues[0], 1, "以object数组作为参数测试失败" );
      Assert.AreEqual( query.ParameterValues[1], 2, "以object数组作为参数测试失败" );
      Assert.AreEqual( query.ParameterValues[2], 3, "以object数组作为参数测试失败" );



      query = Db.T( $"SELECT * FROM Users WHERE ID IN ( {(1, 2, 3)} )" );
      Assert.AreEqual( query.TextTemplate, "SELECT * FROM Users WHERE ID IN ( &#0#, &#1#, &#2# )", "以值类型数组作为参数测试失败" );
      Assert.AreEqual( query.ParameterValues.Length, 3, "以值类型数组作为参数测试失败" );
      Assert.AreEqual( query.ParameterValues[0], 1, "以值类型数组作为参数测试失败" );
      Assert.AreEqual( query.ParameterValues[1], 2, "以值类型数组作为参数测试失败" );
      Assert.AreEqual( query.ParameterValues[2], 3, "以值类型数组作为参数测试失败" );


    }



    [TestMethod]
    public void TemplateParseNullTest()
    {
      var query = Db.T( $"SELECT * FROM Users WHERE ID = {null} AND Username = {null}" );
      Assert.AreEqual( query.TextTemplate, "SELECT * FROM Users WHERE ID = &#0# AND Username = &#1#", "以多个 null 值作为参数测试失败" );
      Assert.AreEqual( query.ParameterValues.Length, 2, "以多个 null 值作为参数测试失败" );
      Assert.IsNull( query.ParameterValues[0], "以多个 null 值作为参数测试失败" );
      Assert.IsNull( query.ParameterValues[1], "以多个 null 值作为参数测试失败" );


      query = Db.T( $"SELECT * FROM Users WHERE ID = {null}" );
      Assert.AreEqual( query.TextTemplate, "SELECT * FROM Users WHERE ID = &#0#", "以 null 作为参数测试失败" );
      Assert.AreEqual( query.ParameterValues.Length, 1, "以 null 作为参数测试失败" );
      Assert.IsNull( query.ParameterValues[0], "以 null 作为参数测试失败" );
    }


    [TestMethod]
    public void ConcatTest()
    {



      var query = Db.T( $"SELECT * FROM Users;" );

      Assert.AreEqual( (query + query).TextTemplate, "SELECT * FROM Users; SELECT * FROM Users;", "两个纯文本模板连接测试失败" );
      Assert.AreEqual( query.Concat( query ).TextTemplate, "SELECT * FROM Users; SELECT * FROM Users;", "两个纯文本模板连接测试失败" );


      Assert.AreEqual( (query + query + query).TextTemplate, "SELECT * FROM Users; SELECT * FROM Users; SELECT * FROM Users;", "多个纯文本模板连接测试失败" );
      Assert.AreEqual( query.ConcatQueries( query, query ).TextTemplate, "SELECT * FROM Users; SELECT * FROM Users; SELECT * FROM Users;", "多个纯文本模板连接测试失败" );


      query = Db.T( $"SELECT * FROM Users WHERE UserID = {1};" );

      Assert.AreEqual( (query + query + query).TextTemplate, "SELECT * FROM Users WHERE UserID = &#0#; SELECT * FROM Users WHERE UserID = &#1#; SELECT * FROM Users WHERE UserID = &#2#;", "多个带参数模板连接测试失败" );
      Assert.AreEqual( query.ConcatQueries( query, query ).TextTemplate, "SELECT * FROM Users WHERE UserID = &#0#; SELECT * FROM Users WHERE UserID = &#1#; SELECT * FROM Users WHERE UserID = &#2#;", "多个带参数模板连接测试失败" );


      ParameterizedQuery query1 = null;
      query += query1;
      Assert.AreEqual( query.TextTemplate, "SELECT * FROM Users WHERE UserID = &#0#;", "参数化查询对象连接一个 null 值失败" );


      query += $"";
      Assert.AreEqual( query.TextTemplate, "SELECT * FROM Users WHERE UserID = &#0#;", "参数化查询对象连接一个空字符串失败" );

      query += "DELETE Users;".AsTextQuery();
      Assert.AreEqual( query.TextTemplate, "SELECT * FROM Users WHERE UserID = &#0#; DELETE Users;", "连接纯文本查询失败" );

      query += " DELETE Users;".AsTextQuery();
      Assert.AreEqual( query.TextTemplate, "SELECT * FROM Users WHERE UserID = &#0#; DELETE Users; DELETE Users;", "连接空白字符开头查询失败" );


    }



    /*
    [TestMethod]
    public void JoinTest()
    {
      var query = Db.T( $"SELECT * FROM Users" );
      query += "WHERE";
      query += Db.Join( "AND", new[] { Db.T( "UserID <> {0}", 0 ), Db.T( "Username = {0}", "Ivony" ) } );


      Assert.AreEqual( query.TextTemplate, "SELECT * FROM Users WHERE UserID <> #0# AND Username = #1#" );
      Assert.AreEqual( query.ParameterValues.Length, 2 );
      Assert.AreEqual( query.ParameterValues[0], 0 );
      Assert.AreEqual( query.ParameterValues[1], "Ivony" );



      query = Db.T( "SELECT * FROM Users" );
      query += "WHERE";
      query += Db.Join( "AND", new ParameterizedQuery[0] );

      Assert.AreEqual( query.TextTemplate, "SELECT * FROM Users WHERE" );


      query = Db.T( "SELECT * FROM Users" );
      query += "WHERE";
      query += Db.Join( "AND", new ParameterizedQuery[] { null } );

      Assert.AreEqual( query.TextTemplate, "SELECT * FROM Users WHERE" );


      query = Db.T( "SELECT * FROM Users" );
      query += "WHERE";
      query += Db.Join( "AND", new ParameterizedQuery[] { Db.T( "UserID <> {0}", 0 ), null, Db.T( "Username = {0}", "Ivony" ) } );

      Assert.AreEqual( query.TextTemplate, "SELECT * FROM Users WHERE UserID <> #0# AND Username = #1#" );
      Assert.AreEqual( query.ParameterValues.Length, 2 );
      Assert.AreEqual( query.ParameterValues[0], 0 );
      Assert.AreEqual( query.ParameterValues[1], "Ivony" );


    }
    */
  }
}
