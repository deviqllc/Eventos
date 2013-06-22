using System;
using System.Collections.Generic;
using System.Dynamic;
using Eventos.Core.Interfaces;

namespace Eventos.Core
{
	/// <summary>
	/// Evento execution context.
	/// </summary>
	public class EventoExecutionContext<TParameters, TResult> where TResult : TParameters
	{
		public EventoExecutionContext(IEvento<TParameters, TResult> ParentEvento, TParameters Parameters = default(TParameters))
		{
			this.Parent = ParentEvento;
			this.ExecutionParameters = Parameters;
			this.ExecutionResults = default(TResult);
		}
		
		/// <summary>
		/// Parent Evento
		/// </summary>
		/// <value>
		/// The parent evento.
		/// </value>
		public IEvento<TParameters, TResult> Parent { get; set; }
		
		/// <summary>
		/// Gets or sets the excecution parameters.
		/// </summary>
		/// <value>
		/// The excecution parameters.
		/// </value>
		public TParameters ExecutionParameters { get; private set; }
		
		/// <summary>
		/// Gets or sets the execution results.
		/// </summary>
		/// <value>
		/// The execution results.
		/// </value>
		public TResult ExecutionResults { get; set; }
		
		/// <summary>
		/// Gets a value indicating whether this instance has execution parameters.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance has execution parameters; otherwise, <c>false</c>.
		/// </value>
		public bool HasExecutionParameters 
		{
			get
			{
				return this.ExecutionParameters != null;
			}
		}
		
		/// <summary>
		/// Gets a value indicating whether this instance has execution results.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance has execution results; otherwise, <c>false</c>.
		/// </value>
		public bool HasExecutionResults 
		{
			get
			{
				return this.ExecutionResults != null;
			}
		}
		
	}
}

