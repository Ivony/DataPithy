namespace Ivony.Data.SqlDom
{
  public sealed class OrderByClause
  {
    internal OrderByClause( params OrderByItem[] items )
    {
      OrderByItems = items;
    }

    public OrderByItem[] OrderByItems { get; }


    public override string ToString()
    {
      return string.Join<OrderByItem>( ", ", OrderByItems );

    }
  }
}