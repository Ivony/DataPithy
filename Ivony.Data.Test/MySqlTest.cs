﻿using System;
using System.Data;
using System.Linq;
using Ivony.Data.MySqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ivony.Data.Queries;

namespace Ivony.Data.Test
{
  [TestClass]
  public class MySqlTest
  {

    private IDisposable scope;


    [TestInitialize]
    public void Enter()
    {

      scope = Db.UseDatabase( MySqlDb.Connect( "10.168.95.112", "test_wangling", "wangling", "a135246A" ) );

      Db.T( $"DROP TABLE IF EXISTS testTable" ).ExecuteNonQuery();
      Db.T( $@"
CREATE TABLE testTable(
Id int(11) NOT NULL AUTO_INCREMENT,
Name varchar(50) DEFAULT NULL,
Content text,
PRIMARY KEY (Id)
)" ).ExecuteNonQuery();

    }

    [TestCleanup]
    public void Exit()
    {
      scope.Dispose();
    }



    [TestMethod]
    public void StandardTest()
    {
      Assert.IsNull( Db.T( $"SELECT * FROM testTable" ).ExecuteScalar(), "空数据表查询" );
      Assert.IsNull( Db.T( $"SELECT * FROM testTable" ).ExecuteFirstRow(), "空数据表查询" );

      Assert.AreEqual( Db.T( $"SELECT COUNT(*) FROM testTable" ).ExecuteScalar<int>(), 0, "空数据表查询" );
      Assert.AreEqual( Db.T( $"INSERT INTO testTable ( Name, Content) VALUES ( {"Ivony"}, {"Test"} )" ).ExecuteNonQuery(), 1, "插入数据测试失败" );
      Assert.AreEqual( Db.T( $"SELECT * FROM testTable" ).ExecuteDynamics().Length, 1, "插入数据后查询" );
      Assert.IsNotNull( Db.T( $"SELECT ID FROM testTable" ).ExecuteFirstRow(), "插入数据后查询" );

      var dataItem = Db.T( $"SELECT * FROM testTable" ).ExecuteDynamicObject();
      Assert.AreEqual( dataItem.Name, "Ivony", "插入数据后查询" );
      Assert.AreEqual( dataItem["Content"], "Test", "插入数据后查询" );
    }

    [TestMethod]
    public void TransactionTest()
    {
      using ( var transaction = Db.EnterTransaction() )
      {
        Assert.AreEqual( Db.T( $"INSERT INTO testTable ( Name, Content ) VALUES ( {"Ivony"}, {"Test"} )" ).ExecuteNonQuery(), 1, "插入数据测试失败" );
        Assert.AreEqual( Db.T( $"SELECT * FROM testTable" ).ExecuteDynamics().Length, 1, "插入数据后查询" );
      }

      Assert.AreEqual( Db.T( $"SELECT * FROM testTable" ).ExecuteDynamics().Length, 0, "自动回滚事务" );

      using ( var transaction = Db.EnterTransaction() )
      {
        Assert.AreEqual( Db.T( $"INSERT INTO testTable ( Name, Content ) VALUES ( {"Ivony"}, {"Test"} )" ).ExecuteNonQuery(), 1, "插入数据测试失败" );
        Assert.AreEqual( Db.T( $"SELECT * FROM testTable" ).ExecuteDynamics().Length, 1, "插入数据后查询" );

        transaction.Rollback();
      }

      Assert.AreEqual( Db.T( $"SELECT * FROM testTable" ).ExecuteDynamics().Length, 0, "手动回滚事务" );



      using ( var transaction = Db.EnterTransaction() )
      {
        Assert.AreEqual( Db.T( $"INSERT INTO testTable ( Name, Content ) VALUES ( {"Ivony"}, {"Test"} )" ).ExecuteNonQuery(), 1, "插入数据测试失败" );
        Assert.AreEqual( Db.T( $"SELECT * FROM testTable" ).ExecuteDynamics().Length, 1, "插入数据后查询" );

        transaction.Commit();
      }

      Assert.AreEqual( Db.T( $"SELECT * FROM testTable" ).ExecuteDynamics().Length, 1, "手动提交事务" );

      Db.T( $"TRUNCATE TABLE testTable" ).ExecuteNonQuery();



      using ( var transaction = Db.EnterTransaction() )
      {
        Exception exception = null;
        try
        {
          Db.T( $"INSERT INTO testTable ( Name, Content ) VALUES ( {"Ivony"}, {"Test"} )" ).ExecuteNonQuery();
          transaction.Commit();
          Db.T( $"INSERT INTO testTable ( Name, Content ) VALUES ( {"Ivony"}, {"Test"} )" ).ExecuteNonQuery();

        }
        catch ( Exception e )
        {
          exception = e;
        }

        Assert.IsNotNull( exception, "提交事务后再执行操作引发异常" );
      }



      using ( var transaction = Db.EnterTransaction() )
      {
        Exception exception = null;
        try
        {
          Db.T( $"INSERT INTO testTable ( Name, Content ) VALUES ( {"Ivony"}, {"Test"} )" ).ExecuteNonQuery();
          transaction.Commit();
          Db.T( $"INSERT INTO testTable ( Name, Content ) VALUES ( {"Ivony"}, {"Test"} )" ).ExecuteNonQuery();

        }
        catch ( Exception e )
        {
          exception = e;
        }

        Assert.IsNotNull( exception, "提交事务后再执行操作引发异常" );
      }



      Db.T( $"TRUNCATE TABLE testTable" ).ExecuteNonQuery();

      Db.Transaction( () =>
      {
        Db.T( $"INSERT INTO testTable ( Name, Content ) VALUES ( {"Ivony"}, {"Test"} )" ).ExecuteNonQuery();
        Assert.AreEqual( Db.T( $"SELECT * FROM testTable" ).ExecuteDynamics().Length, 1, "直接运行事务" );

        Db.Rollback();
      } );

      Assert.AreEqual( Db.T( $"SELECT * FROM testTable" ).ExecuteDynamics().Length, 0, "直接运行事务回滚" );


      Db.Transaction( () =>
      {
        Db.T( $"INSERT INTO testTable ( Name, Content ) VALUES ( {"Ivony"}, {"Test"} )" ).ExecuteNonQuery();
      } );


      var content = Db.Transaction( () =>
      {
        return Db.T( $"SELECT Content FROM testTable WHERE Name = {"Ivony"}" ).ExecuteScalar<string>();
      } );

      Assert.AreEqual( content, "Test", "直接运行事务并返回值" );




      {
        Exception exception = null;
        var transaction = (MySqlDbTransaction) Db.EnterTransaction();

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

        Assert.IsNotNull( exception, "事务中出现异常测试" );
        Assert.AreEqual( transaction.Connection.State, ConnectionState.Closed, "退出事务自动关闭连接" );
      }
    }

    [TestMethod]
    public void TraceTest()
    {
      var traceService = new TestTraceService();
      using ( Db.EnterTransaction() )
      {
        Db.T( $"SELECT * FROM testTable" ).WithTraceService( traceService ).ExecuteDataTable();

        var tracing = traceService.Last();

        var logs = tracing.TraceEvents;
        Assert.AreEqual( logs.Count, 3 );

        Assert.AreEqual( logs[0].EventName, "OnExecuting" );
        Assert.AreEqual( logs[1].EventName, "OnLoadingData" );
        Assert.AreEqual( logs[2].EventName, "OnComplete" );


        try
        {
          Db.T( $"SELECT * FROM Nothing" ).WithTraceService( traceService ).ExecuteDynamics();
        }
        catch
        {

        }

        tracing = traceService.Last();

        logs = tracing.TraceEvents;
        Assert.AreEqual( logs.Count, 2 );

        Assert.AreEqual( logs[0].EventName, "OnExecuting" );
        Assert.AreEqual( logs[1].EventName, "OnException" );

      }
    }


    [TestMethod]
    public void DynamicObjectTest()
    {
      Db.T( $"INSERT INTO testTable ( Content ) VALUES ( {"Test"} )" ).ExecuteNonQuery();

      var data = Db.T( $"SELECT * FROM testTable" ).ExecuteDynamicObject();

      Assert.IsNull( (string) data.Name );
      Assert.IsNull( (int?) data.Name );
    }

  }
}
