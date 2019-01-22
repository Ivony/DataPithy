using System;
using System.Runtime.Serialization;

namespace Ivony.Data.Queries
{
  [Serializable]
  internal class NotInitializedException : Exception
  {
    public NotInitializedException()
    {
    }

    public NotInitializedException( string message ) : base( message )
    {
    }

    public NotInitializedException( string message, Exception innerException ) : base( message, innerException )
    {
    }

    protected NotInitializedException( SerializationInfo info, StreamingContext context ) : base( info, context )
    {
    }
  }
}