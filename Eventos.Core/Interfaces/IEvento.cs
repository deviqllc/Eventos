using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Eventos.Core.Interfaces
{
	public interface IEvento<TParameters, TResult> where TResult : TParameters
	{
		string UID { get; }
		string Name { get; set; }
		IEvento<TParameters, TResult> Callback { get; }
		EventoState State { get; }
		
		TResult Start(TParameters Parameters = default(TParameters));
		IEvento<TParameters, TResult> Then(IEvento<TParameters, TResult> Callback);
	}
}