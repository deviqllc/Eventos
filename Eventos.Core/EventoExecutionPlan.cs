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
		
		/// <summary>
		/// Sets the flow of eventos.
		/// </summary>
		/// <param name='eventos'>
		/// Eventos.
		/// </param>
		public void SetSequence(params IEvento<TParameters, TResult>[] Eventos)
		{
			base.setSequence(Eventos);
		}
		
		/// <summary>
		/// Executes current flow.
		/// </summary>
		/// <param name='parameters'>
		/// Parameters.
		/// </param>
		public TResult Start(TParameters Parameters = default(TParameters))
		{
			return base.start (Parameters);
		}
	}
}

