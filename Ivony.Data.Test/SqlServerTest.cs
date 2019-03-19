using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ivony.Data.Queries;
using System.Threading.Tasks;
using System.Data;
using Ivony.Data.SqlClient;
using System.Xml.Linq;
using Ivony.Data.Common;
using System.Data.SqlClient;

namespace Ivony.Data.Test
{
  [TestClass]
  public class SqlServerTest
  {


    private TestTraceService traceService;
    private SqlDbExecutor db;


    private IDisposable scope;



    [TestInitialize]
    public void Enter()
    {

      Db.UseDatabase( SqlServerDb.Connect( "Data Source=(localdb)\\ProjectsV13;Initial Catalog=Test;" ) );


      Db.T( $"IF OBJECT_ID(N'[dbo].[Test1]') IS NOT NULL DROP TABLE [dbo].[Test1]" ).ExecuteNonQuery();
      Db.T( $@"
CREATE TABLE [dbo].[Test1]
(
    [ID] INT NOT NULL IDENTITY,
    [Name] NVARCHAR(50) NOT NULL , 
    [Content] NTEXT NULL, 
    [XmlContent] XML NULL,
    [Index] INT NOT NULL, 
    CONSTRAINT [PK_Test1] PRIMARY KEY ([ID]) 
)" ).ExecuteNonQuery();

    }


    public SqlServerTest()
    {


    }



    [TestMethod]
    public void StandardTest1()
    {
      Assert.IsNull( Db.T( $"SELECT ID FROM Test1" ).ExecuteScalar(), "空数据表查询测试失败" );
      Assert.IsNull( Db.T( $"SELECT ID FROM Test1" ).ExecuteFirstRow(), "空数据表查询测试失败" );
      Assert.AreEqual( Db.T( $"SELECT COUNT(*) FROM Test1" ).ExecuteScalar<int>(), 0, "空数据表查询测试失败" );
      Assert.AreEqual( Db.T( $"INSERT INTO Test1 ( Name, Content, [Index] ) VALUES ( {"Ivony"}, {"Test"}, {1} )" ).ExecuteNonQuery(), 1, "插入数据测试失败" );
      Assert.AreEqual( Db.T( $"SELECT * FROM Test1" ).ExecuteDynamics().Length, 1, "插入数据后查询测试失败" );
      Assert.IsNotNull( Db.T( $"SELECT ID FROM Test1" ).ExecuteFirstRow(), "插入数据后查询测试失败" );

      var dataItem = Db.T( $"SELECT * FROM Test1" ).ExecuteDynamicObject();
      Assert.AreEqual( dataItem.Name, "Ivony", "插入数据后查询测试失败" );
      Assert.AreEqual( dataItem["Content"], "Test", "插入数据后查询测试失败" );
    }


    [TestMethod]
    public void AsyncTest1()
    {
      var task = _AsyncTest1();
      task.Wait();
    }


    public async Task _AsyncTest1()
    {
      Assert.IsNull( await Db.T( $"SELECT ID FROM Test1" ).ExecuteScalarAsync(), "空数据表查询测试失败" );
      Assert.IsNull( await Db.T( $"SELECT ID FROM Test1" ).ExecuteFirstRowAsync(), "空数据表查询测试失败" );
      Assert.AreEqual( await Db.T( $"SELECT COUNT(*) FROM Test1" ).ExecuteScalarAsync<int>(), 0, "空数据表查询测试失败" );
      Assert.AreEqual( await Db.T( $"INSERT INTO Test1 ( Name, Content, [Index] ) VALUES ( {"Ivony"}, {"Test"}, {1} )" ).ExecuteNonQueryAsync(), 1, "插入数据测试失败" );
      Assert.AreEqual( (await Db.T( $"SELECT * FROM Test1" ).ExecuteDynamicsAsync()).Length, 1, "插入数据后查询测试失败" );
      Assert.IsNotNull( await Db.T( $"SELECT ID FROM Test1" ).ExecuteFirstRowAsync(), "插入数据后查询测试失败" );

    }




    [TestMethod]
    public void TransactionTest()
    {
      using ( var transaction = Db.EnterTransaction() )
      {
        Assert.AreEqual( Db.T( $"INSERT INTO Test1 ( Name, Content, [Index] ) VALUES ( {"Ivony"}, {"Test"}, {1})" ).ExecuteNonQuery(), 1, "插入数据测试失败" );
        Assert.AreEqual( Db.T( $"SELECT * FROM Test1" ).ExecuteDynamics().Length, 1, "插入数据后查询测试失败" );
      }

      Assert.AreEqual( Db.T( $"SELECT * FROM Test1" ).ExecuteDynamics().Length, 0, "自动回滚事务测试失败" );

      using ( var transaction = Db.EnterTransaction() )
      {
        Assert.AreEqual( Db.T( $"INSERT INTO Test1 ( Name, Content, [Index] ) VALUES ( {"Ivony"}, {"Test"}, {1} )" ).ExecuteNonQuery(), 1, "插入数据测试失败" );
        Assert.AreEqual( Db.T( $"SELECT * FROM Test1" ).ExecuteDynamics().Length, 1, "插入数据后查询测试失败" );

        transaction.Rollback();
      }

      Assert.AreEqual( Db.T( $"SELECT * FROM Test1" ).ExecuteDynamics().Length, 0, "手动回滚事务测试失败" );



      using ( var transaction = Db.EnterTransaction() )
      {
        Assert.AreEqual( Db.T( $"INSERT INTO Test1 ( Name, Content, [Index] ) VALUES ( {"Ivony"}, {"Test"}, {1} )" ).ExecuteNonQuery(), 1, "插入数据测试失败" );
        Assert.AreEqual( Db.T( $"SELECT * FROM Test1" ).ExecuteDynamics().Length, 1, "插入数据后查询测试失败" );

        transaction.Commit();
      }

      Assert.AreEqual( Db.T( $"SELECT * FROM Test1" ).ExecuteDynamics().Length, 1, "手动提交事务测试失败" );



      {
        Exception exception = null;
        var transaction = (SqlServerTransactionContext) Db.EnterTransaction();

        try
        {
          using ( transaction )
          {
            Db.T( $"SELECT * FROM Nothing" ).ExecuteNonQuery();
            transaction.Commit();
          }
        }
        catch ( Exception e )
        {
          exception = e;
        }

        Assert.IsNotNull( exception, "事务中出现异常测试失败" );
        Assert.AreEqual( transaction.Connection.State, ConnectionState.Closed );
      }
    }


    [TestMethod]
    public void ParameterSpecificationTest()
    {

      SqlParameterizedQueryParser.RegisterParameterSpecification( typeof( int ), SqlDbType.Decimal );

      Db.T( $"SELECT * FROM Test1 WHERE [Index] = {0}" ).ExecuteNonQuery();
      var command = (SqlCommand) traceService.Last().CommandObject;
      Assert.AreEqual( command.Parameters[0].SqlDbType, SqlDbType.Decimal, "注册参数规范测试失败" );

      SqlParameterizedQueryParser.UnregisterParameterSpecification( typeof( int ) );

      Db.T( $"SELECT * FROM Test1 WHERE [Index] = {0}" ).ExecuteNonQuery();
      command = (SqlCommand) traceService.Last().CommandObject;
      Assert.AreNotEqual( command.Parameters[0].SqlDbType, SqlDbType.Decimal, "解除注册参数规范测试失败" );

    }


    [TestMethod]
    public void TraceTest()
    {

      Db.T( $"SELECT * FROM Test1" ).ExecuteDataTable();

      var tracing = traceService.Last();

      var events = tracing.TraceEvents;
      Assert.AreEqual( events.Length, 3 );

      Assert.IsTrue( tracing.QueryTime >= tracing.ExecutionTime );

      Assert.AreEqual( events[0].EventName, "OnExecuting" );
      Assert.AreEqual( events[1].EventName, "OnLoadingData" );
      Assert.AreEqual( events[2].EventName, "OnComplete" );


      try
      {
        Db.T( $"SELECT * FROM Nothing" ).ExecuteDynamics();
      }
      catch
      {

      }

      tracing = traceService.Last();

      events = tracing.TraceEvents;
      Assert.AreEqual( events.Length, 2 );

      Assert.AreEqual( events[0].EventName, "OnExecuting" );
      Assert.AreEqual( events[1].EventName, "OnException" );
    }


    [TestMethod]
    public void XmlFieldTest()
    {

      DbValueConverter.Register( new XDocumentValueConverter() );

      var document = new XDocument( new XDeclaration( "1.0", "utf-8", "yes" ),
        new XElement( "Root",
          new XAttribute( "test", "test-value" ),
          new XElement( "Item" ),
          new XElement( "Item" ),
          new XElement( "Item" )
        ) );

      Db.T( $"INSERT INTO Test1 ( Name, XmlContent, [Index] ) VALUES ( {"XML content"}, {document}, {1} )" ).ExecuteNonQuery();

      var document1 = Db.T( $"SELECT TOP 1 XmlContent FROM Test1" ).ExecuteScalar<XDocument>();

      Assert.AreEqual( document.ToString( SaveOptions.OmitDuplicateNamespaces ), document1.ToString( SaveOptions.OmitDuplicateNamespaces ) );

      Db.T( $"UPDATE Test1 SET XmlContent = {null} " ).ExecuteNonQuery();
      Assert.IsNull( Db.T( $"SELECT XmlContent FROM Test1 " ).ExecuteScalar<XDocument>() );


      DbValueConverter.Unregister<XDocument>();

    }


    [TestMethod]
    public void NullableConvertTest()
    {
      Db.T( $"SELECT [Index] FROM Test1" ).ExecuteScalar<int?>();
    }

    [TestMethod]
    public void ConvertExceptionTest()
    {
      Db.T( $"INSERT INTO Test1 ( Name, Content, [Index] ) VALUES( {"Test"}, {"TestContent"}, {1} )" ).ExecuteNonQuery();

      Exception exception = null;
      try
      {
        Db.T( $"SELECT [Index] FROM Test1" ).ExecuteEntity<WrongEntity>();
      }
      catch ( InvalidCastException e )
      {
        exception = e;
      }

      Assert.IsNotNull( exception, "转换异常测试失败" );
      Assert.IsNotNull( exception.Data["DataColumnName"], "转换异常测试失败" );


    }


    [TestMethod]
    public void EntityTest()
    {
      Db.T( $"INSERT INTO Test1 ( Name, Content, [Index] ) VALUES( {"Test"}, {"TestContent"}, {1} )" ).ExecuteNonQuery();
      Db.T( $"INSERT INTO Test1 ( Name, Content, [Index] ) VALUES(  {"Test"}, {"TestContent"}, {2} )" ).ExecuteNonQuery();

      Db.T( $"SELECT Name, Content, [Index] FROM Test1" ).ExecuteEntity<CorrectEntity>();
      Db.T( $"SELECT Name, Content, [Index] FROM Test1" ).ExecuteEntities<CorrectEntity>();

      var entity = (CorrectEntity) Db.T( $"SELECT Name, Content, [Index] FROM Test1" ).ExecuteDynamicObject();
      var entities = Db.T( $"SELECT Name, Content, [Index] FROM Test1" ).ExecuteDynamics().Select( item => (CorrectEntity) item ).ToArray();

    }


    [TestMethod]
    public void ConvertibleTest()
    {

      Db.T( $"INSERT INTO Test1 ( Name, Content, [Index] ) VALUES ( {"5"}, {"0.9"}, {100} )" ).ExecuteNonQuery();

      Assert.AreEqual( Db.T( $"SELECT TOP 1 [Index] FROM Test1" ).ExecuteScalar<long>(), 100L );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 [Index] FROM Test1" ).ExecuteScalar<int>(), 100 );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 [Index] FROM Test1" ).ExecuteScalar<short>(), (short) 100 );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 [Index] FROM Test1" ).ExecuteScalar<byte>(), (byte) 100 );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 [Index] FROM Test1" ).ExecuteScalar<ulong>(), (ulong) 100 );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 [Index] FROM Test1" ).ExecuteScalar<uint>(), 100u );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 [Index] FROM Test1" ).ExecuteScalar<ushort>(), (ushort) 100 );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 [Index] FROM Test1" ).ExecuteScalar<sbyte>(), (sbyte) 100 );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 [Index] FROM Test1" ).ExecuteScalar<char>(), (char) 100 );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 [Index] FROM Test1" ).ExecuteScalar<decimal>(), (decimal) 100 );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 [Index] FROM Test1" ).ExecuteScalar<double>(), (double) 100 );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 [Index] FROM Test1" ).ExecuteScalar<float>(), (float) 100 );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 [Index] FROM Test1" ).ExecuteScalar<string>(), "100" );

      Assert.AreEqual( Db.T( $"SELECT TOP 1 [Index] FROM Test1" ).ExecuteScalar<long?>(), 100L );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 [Index] FROM Test1" ).ExecuteScalar<int?>(), 100 );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 [Index] FROM Test1" ).ExecuteScalar<short?>(), (short) 100 );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 [Index] FROM Test1" ).ExecuteScalar<byte?>(), (byte) 100 );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 [Index] FROM Test1" ).ExecuteScalar<ulong?>(), (ulong) 100 );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 [Index] FROM Test1" ).ExecuteScalar<uint?>(), 100u );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 [Index] FROM Test1" ).ExecuteScalar<ushort?>(), (ushort) 100 );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 [Index] FROM Test1" ).ExecuteScalar<sbyte?>(), (sbyte) 100 );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 [Index] FROM Test1" ).ExecuteScalar<char?>(), (char) 100 );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 [Index] FROM Test1" ).ExecuteScalar<decimal?>(), (decimal) 100 );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 [Index] FROM Test1" ).ExecuteScalar<double?>(), (double) 100 );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 [Index] FROM Test1" ).ExecuteScalar<float?>(), (float) 100 );

      Assert.AreEqual( Db.T( $"SELECT TOP 1 Name FROM Test1" ).ExecuteScalar<long>(), 5 );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 Name FROM Test1" ).ExecuteScalar<int>(), 5 );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 Name FROM Test1" ).ExecuteScalar<short>(), (short) 5 );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 Name FROM Test1" ).ExecuteScalar<byte>(), (byte) 5 );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 Name FROM Test1" ).ExecuteScalar<ulong>(), (ulong) 5 );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 Name FROM Test1" ).ExecuteScalar<uint>(), 5u );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 Name FROM Test1" ).ExecuteScalar<ushort>(), (ushort) 5 );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 Name FROM Test1" ).ExecuteScalar<sbyte>(), (sbyte) 5 );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 Name FROM Test1" ).ExecuteScalar<decimal>(), (decimal) 5 );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 Name FROM Test1" ).ExecuteScalar<double>(), (double) 5 );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 Name FROM Test1" ).ExecuteScalar<float>(), (float) 5 );


      Assert.AreEqual( Db.T( $"SELECT TOP 1 Content FROM Test1" ).ExecuteScalar<decimal>(), (decimal) 0.9m );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 Content FROM Test1" ).ExecuteScalar<double>(), (double) 0.9 );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 Content FROM Test1" ).ExecuteScalar<float>(), (float) 0.9 );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 Content FROM Test1" ).ExecuteScalar<decimal?>(), (decimal) 0.9m );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 Content FROM Test1" ).ExecuteScalar<double?>(), (double) 0.9 );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 Content FROM Test1" ).ExecuteScalar<float?>(), (float) 0.9 );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 Content FROM Test1" ).ExecuteScalar<string>(), "0.9" );


      Assert.AreEqual( Db.T( $"SELECT TOP 1 XmlContent FROM Test1" ).ExecuteScalar<long?>(), null );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 XmlContent FROM Test1" ).ExecuteScalar<int?>(), null );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 XmlContent FROM Test1" ).ExecuteScalar<short?>(), null );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 XmlContent FROM Test1" ).ExecuteScalar<byte?>(), null );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 XmlContent FROM Test1" ).ExecuteScalar<ulong?>(), null );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 XmlContent FROM Test1" ).ExecuteScalar<uint?>(), null );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 XmlContent FROM Test1" ).ExecuteScalar<ushort?>(), null );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 XmlContent FROM Test1" ).ExecuteScalar<sbyte?>(), null );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 XmlContent FROM Test1" ).ExecuteScalar<char?>(), null );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 XmlContent FROM Test1" ).ExecuteScalar<decimal?>(), null );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 XmlContent FROM Test1" ).ExecuteScalar<double?>(), null );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 XmlContent FROM Test1" ).ExecuteScalar<float?>(), null );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 XmlContent FROM Test1" ).ExecuteScalar<string>(), null );


      Db.T( $"DELETE Test1" ).ExecuteNonQuery();

      Assert.AreEqual( Db.T( $"SELECT TOP 1 XmlContent FROM Test1" ).ExecuteScalar<long?>(), null );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 XmlContent FROM Test1" ).ExecuteScalar<int?>(), null );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 XmlContent FROM Test1" ).ExecuteScalar<short?>(), null );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 XmlContent FROM Test1" ).ExecuteScalar<byte?>(), null );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 XmlContent FROM Test1" ).ExecuteScalar<ulong?>(), null );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 XmlContent FROM Test1" ).ExecuteScalar<uint?>(), null );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 XmlContent FROM Test1" ).ExecuteScalar<ushort?>(), null );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 XmlContent FROM Test1" ).ExecuteScalar<sbyte?>(), null );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 XmlContent FROM Test1" ).ExecuteScalar<char?>(), null );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 XmlContent FROM Test1" ).ExecuteScalar<decimal?>(), null );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 XmlContent FROM Test1" ).ExecuteScalar<double?>(), null );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 XmlContent FROM Test1" ).ExecuteScalar<float?>(), null );
      Assert.AreEqual( Db.T( $"SELECT TOP 1 XmlContent FROM Test1" ).ExecuteScalar<string>(), null );

      Db.T( $"INSERT INTO Test1 ( Name, Content, [Index] ) VALUES ( {("1900/1/2 13:00:05.276", "ABC", 0)} )" ).ExecuteNonQuery();
      Assert.AreEqual( Db.T( $"SELECT TOP 1 Name FROM Test1" ).ExecuteScalar<DateTime>(), new DateTime( 1900, 1, 2, 13, 0, 5, 276 ) );


    }



    [TestMethod]
    public void EnumTest()
    {

      Db.T( $"INSERT INTO Test1 ( Name, Content, [Index] ) VALUES ( {(TestEnum.One.ToString(), TestEnum.Two.ToString(), TestEnum.Three)} )"
         ).ExecuteNonQuery();


      Assert.AreEqual( Db.T( $"SELECT Name FROM Test1" ).ExecuteScalar<string>(), "One" );
      Assert.AreEqual( Db.T( $"SELECT [Index] FROM Test1" ).ExecuteScalar<int>(), 3 );
      Assert.AreEqual( Db.T( $"SELECT Name FROM Test1" ).ExecuteScalar<TestEnum>(), TestEnum.One );
      Assert.AreEqual( Db.T( $"SELECT [Index] FROM Test1" ).ExecuteScalar<TestEnum>(), TestEnum.Three );


    }


    enum TestEnum : long
    {
      One = 1,
      Two,
      Three,
    }






    public class WrongEntity
    {
      public string Name { get; set; }
      public string Content { get; set; }
      public TimeSpan Index { get; set; }
    }


    public class CorrectEntity
    {
      public string Name { get; set; }
      public string Content { get; set; }

      [FieldName( "Index" )]
      public long OrderIndex { get; set; }

      [NonField]
      public object NonDataField { get; set; }
    }

  }
}
