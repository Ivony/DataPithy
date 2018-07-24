using Ivony.Data.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ivony.Data
{


  /// <summary>
  /// 用于创建 IDbProvider 的服务对象
  /// </summary>
  interface IDbProviderFactory
  {
    IDbProvider GetDbProvider( string connectionName );
  }
}
