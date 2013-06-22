using System;

namespace Eventos.Core.Interfaces
{
	public interface IEventoExecutionPlan<TParameters, TResult> where TResult : TParameters
	{
		string Name { get; set; }
		int Count { get; }
		
		TResult Start(TParameters Parameters = default(TParameters));
	}
}