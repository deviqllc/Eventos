using System;
using Eventos.Core.Base;
using Eventos.Core.Interfaces;

namespace Eventos.Core
{
	public class Evento<TParameters, TResult> : EventoBase<TParameters, TResult>, IEvento<TParameters, TResult> where TResult : TParameters
	{
		public Evento(Func<EventoExecutionContext<TParameters, TResult>, TResult> Function, string Name = null) : base(Function, Name)
		{
		}
	}
}