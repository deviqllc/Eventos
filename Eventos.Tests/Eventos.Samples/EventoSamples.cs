using System;
using Eventos.Core;
using Eventos.Core.Base;

namespace Eventos.Samples
{
	public class TestCalculationEventoParameters
	{
		public int a { get; set; }
		public int b { get; set; }
	}

	public class TestCalculationEventoResults : TestCalculationEventoParameters
	{
		public int c { get; set; }
	}

	public class TestExecutionPlan1 : EventoExecutionPlanBase<TestCalculationEventoParameters, TestCalculationEventoResults>
	{
		public TestExecutionPlan1() : base("Execution Plan 1")
		{
		}
	}

	public class TestExecutionPlan2 : EventoExecutionPlanBase<TestCalculationEventoParameters, TestCalculationEventoResults>
	{
		public TestExecutionPlan2() : base("Execution Plan 2")
		{

		}
	}


	public class EventoSamples
	{
		private Evento<TestCalculationEventoParameters, TestCalculationEventoResults> getValuesEvento;
		private Evento<TestCalculationEventoParameters, TestCalculationEventoResults> addValuesEvento;
		private Evento<TestCalculationEventoParameters, TestCalculationEventoResults> subtractValuesEvento;
		private Evento<TestCalculationEventoParameters, TestCalculationEventoResults> populateCValueEvento;

		public EventoSamples ()
		{
			this.getValuesEvento = new Evento<TestCalculationEventoParameters, TestCalculationEventoResults> ((ctx) => {
				return new TestCalculationEventoResults {
					a = ctx.HasExecutionParameters ? ctx.ExecutionParameters.a : 10,
					b = ctx.HasExecutionParameters ? ctx.ExecutionParameters.b : 20
				};
			}, "Get Values");
			this.addValuesEvento = new Evento<TestCalculationEventoParameters, TestCalculationEventoResults> ((ctx) => {
				return new TestCalculationEventoResults {
					a = ctx.ExecutionParameters.a + 1, // 11
					b = ctx.ExecutionParameters.b + 1  // 21
				};
			}, "Add Values");
			this.subtractValuesEvento = new Evento<TestCalculationEventoParameters, TestCalculationEventoResults> ((ctx) => {
				return new TestCalculationEventoResults {
					a = ctx.ExecutionParameters.a - 2, // 9
					b = ctx.ExecutionParameters.b - 2 //19
				};
			}, "Subtract Values");
			this.populateCValueEvento = new Evento<TestCalculationEventoParameters, TestCalculationEventoResults> ((ctx) => {
				return new TestCalculationEventoResults {
					a = ctx.ExecutionParameters.a * 3, // 27
					b = ctx.ExecutionParameters.b * 2, // 38
					c = ((ctx.ExecutionParameters.a) * 3 + (ctx.ExecutionParameters.b * 2)) - 1 // 64
				};
			}, "Populate C Value");
		}

		public void Run()
		{
			this.FirstSample ();
			this.SecondSample ();
			this.ThirdSample ();
		}

		private void FirstSample()
		{
			Console.WriteLine("Simple Evento Execution");
			TestCalculationEventoResults results = getValuesEvento.Start ();
			Console.WriteLine("a={0}, b={1}, c={2}", results.a, results.b, results.c);
		}

		private void SecondSample()
		{
			Console.WriteLine("Two Chained Eventos Execution");
			getValuesEvento.Then (addValuesEvento);
			TestCalculationEventoResults results = getValuesEvento.Start ();
			Console.WriteLine("a={0}, b={1}, c={2}", results.a, results.b, results.c);
		}

		private void ThirdSample()
		{
			Console.WriteLine("All Eventos Execution");
			getValuesEvento.Then (addValuesEvento);
			addValuesEvento.Then (subtractValuesEvento);
			subtractValuesEvento.Then (populateCValueEvento);
			TestCalculationEventoResults results = getValuesEvento.Start ();
			Console.WriteLine("a={0}, b={1}, c={2}", results.a, results.b, results.c);
		}

	}
}

