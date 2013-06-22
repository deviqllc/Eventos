using System;
using Eventos.Core.Interfaces;

namespace Eventos.Core.Exceptions
{
	public class EventoException : Exception
	{
		public EventoException () : base()
		{
		}

		public EventoException(string Message) : base(Message)
		{

		}
	}

	public class EventoExecutionPlanException : EventoException
	{
		public EventoExecutionPlanException() : base()
		{

		}

		public EventoExecutionPlanException(string Message) : base(Message)
		{

		}
	}

	public class EmptyExecutionPlanException : EventoExecutionPlanException
	{
		public EmptyExecutionPlanException() : base("Current Execution Plan has no registered eventos.")
		{
		}

		public EmptyExecutionPlanException(string ExecutionPlanName) : base(string.Format ("Execution Plan with name '{0}' has no registered eventos.", ExecutionPlanName))
		{ 
		}
	}
}

