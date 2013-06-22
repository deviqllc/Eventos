using System;
using Eventos.Core.Base;
using Eventos;
using Eventos.Core.Interfaces;

namespace Eventos.Core
{
	public class EventoExecutionPlan<TParameters, TResult> : EventoExecutionPlanBase<TParameters, TResult>, IEventoExecutionPlan<TParameters, TResult> where TResult : TParameters
	{
		public EventoExecutionPlan (string Name) : base(Name)
		{
		}
	}
}