using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Ivony.Data
{

  public interface IRelationalData : IReadOnlyList<IRelationalDataRecord>
  {

  }

  public interface IRelationalDataRecord
  {

    T FieldValue<T>( string name );

  }

  public interface IRelationalDataSchema : IReadOnlyList<IRelationalDataFieldMapping>
  {



  }

  public interface IRelationalDataFieldMapping
  {

    string Name { get; }

    Type DataType { get; }

    System.Data.DbType DbType { get; }

  }
}
