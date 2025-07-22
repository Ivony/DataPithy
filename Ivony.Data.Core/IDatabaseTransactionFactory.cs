
using System.Data;

using Microsoft.Extensions.DependencyInjection;

namespace Ivony.Data.Core;

public interface IDatabaseTransactionFactory
{
  IDatabaseTransaction CreateTransaction();
}