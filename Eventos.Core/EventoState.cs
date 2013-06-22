using System;

namespace Eventos.Core
{
	public enum EventoState
	{
		Queued,
		PreparingForExecution,
		Running,
		Completed,
		Failed
	}
}

