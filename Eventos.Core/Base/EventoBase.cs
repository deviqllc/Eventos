using System;
using System.Collections.Generic;
using System.Dynamic;
using Eventos.Core.Interfaces;

namespace Eventos.Core.Base
{
	public class EventoBase<TParameters, TResult> where TResult : TParameters
	{
		public EventoBase(Func<EventoExecutionContext<TParameters, TResult>, TResult> Function, string Name = null)
		{
			this.UID = Guid.NewGuid().ToString("N");
			this.Fn = Function;
			this.Name = Name;
		}

		#region " Public Methods and Functions "

		/// <summary>
		/// Create Evento Flow.
		/// </summary>
		/// <param name='childEvento'>
		/// Child evento.
		/// </param>
		public IEvento<TParameters, TResult> Then(IEvento<TParameters, TResult> Callback)
		{
			this.Callback = Callback;
			return (IEvento<TParameters, TResult>)this;
		}

		/// <summary>
		/// Starts this evento instance using the specified Parameters.
		/// </summary>
		/// <param name="parameters">Parameters.</param>
		/// <param name="executionplanParameters">Executionplan parameters.</param>
		public TResult Start(TParameters parameters = default(TParameters))
		{			
			EventoExecutionContext<TParameters, TResult> context = new EventoExecutionContext<TParameters, TResult>((IEvento<TParameters, TResult>)this, parameters);

			try
			{
				this.State = EventoState.Running;
				context.ExecutionResults = this.Fn(context);
				this.State = EventoState.Completed;

				if (this.Callback != null)
				{
					return this.Callback.Start(context.ExecutionResults);
				}
				else
				{
					return context.ExecutionResults;
				}
			}
			catch(Exception ex)
			{
				this.State = EventoState.Failed;
				throw ex;
			}
		}

		#endregion

		#region " Private Methods and Functions "

		#endregion

		#region " Public Properties "

		/// <summary>
		/// Gets or sets the UID
		/// </summary>
		/// <value>
		/// The user interface.
		/// </value>
		public string UID { get; private set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the state.
		/// </summary>
		/// <value>
		/// The state.
		/// </value>
		public EventoState State { get; private set; }

		/// <summary>
		/// Gets or sets the evento function.
		/// </summary>
		/// <value>
		/// The fn.
		/// </value>
		public Func<EventoExecutionContext<TParameters, TResult>, TResult> Fn { get; private set; }

		/// <summary>
		/// Get and sets the standard Callback
		/// </summary>
		/// <value>
		/// The callback.
		/// </value>
		public IEvento<TParameters, TResult> Callback { get; private set; }

		#endregion
	}
}

