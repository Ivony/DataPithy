namespace Ivony.Data.SqlDom.Builders.DynamicHosts
{
  internal class FieldDynamicHost : SqlDynamicHost
  {

    public FieldDynamicHost( string tableName, string fieldName ) : base( new FieldReference( tableName, fieldName ) )
    {
      TableName = tableName ?? throw new System.ArgumentNullException( nameof( tableName ) );
      FieldName = fieldName ?? throw new System.ArgumentNullException( nameof( fieldName ) );
    }

    public string TableName { get; }
    public string FieldName { get; }
  }
}