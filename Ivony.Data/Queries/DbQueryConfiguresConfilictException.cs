using System;
using System.Runtime.Serialization;

namespace Ivony.Data.Queries
{
  [Serializable]
  internal class DbQueryConfiguresConfilictException : Exception
  {
    public DbQueryConfiguresConfilictException() : base( "merge DbQuery configures makes one or more conflicts" )
    {
    }


    protected DbQueryConfiguresConfilictException( SerializationInfo info, StreamingContext context ) : base( info, context )
    {
    }
  }
}