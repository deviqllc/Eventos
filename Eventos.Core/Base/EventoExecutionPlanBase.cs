using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eventos.Core.Exceptions;
using Eventos.Core.Interfaces;

namespace Eventos.Core.Base
{
	public abstract class EventoExecutionPlanBase<TParameters, TResult> where TResult : TParameters
	{
		protected internal List<IEvento<TParameters, TResult>> flow = new List<IEvento<TParameters, TResult>>();
		
		public EventoExecutionPlanBase(string Name)
		{
			this.Name = Name;
		}
		
		/// <summary>
		/// Sets the flow.
		/// </summary>Starts this evento instance using the specified Parameters.
		/// <param name='eventos'>
		/// Eventos.
		/// </param>
		protected internal void setSequence(params IEvento<TParameters, TResult>[] Eventos)
		{
			int i = 0;
			this.flow = Eventos.ToList();
			while (i < (this.Count-1))
			{
				this.flow[i].Then (this.flow[i+1]);
				i++;
			}
		}
		
		/// <summary>
		/// Start the specified parameters.
		/// </summary>
		/// <param name='parameters'>
		/// Parameters.
		/// </param>
		/// <exception cref='Exception'>
		/// Represents errors that occur during application execution.
		/// </exception>
		protected internal TResult start(TParameters Parameters = default(TParameters))
		{
			if (this.Count > 0)
			{
				return this.flow[0].Start (Parameters);
			}
			else
			{
				throw new EmptyExecutionPlanException(this.Name);
			}
		}
		
		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		public string Name { get; set; }
		
		/// <summary>
		/// Gets the count.
		/// </summary>
		/// <value>
		/// The count.
		/// </value>
		public int Count 
		{
			get
			{
				return this.flow.Count();
			}
		}
	}
}

