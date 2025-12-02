
using System.Data;

using Microsoft.Extensions.DependencyInjection;

namespace Ivony.Data;

public interface IDatabaseTransactionFactory
{
  IDatabaseTransaction CreateTransaction();
}