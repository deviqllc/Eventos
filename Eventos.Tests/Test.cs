using System;
using NUnit.Framework;
using Eventos.Core;
using Eventos.Core.Base;
using Eventos.Core.Exceptions;

namespace Eventos.Tests
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
		public Evento<TestCalculationEventoParameters, TestCalculationEventoResults> getValuesEvento;
		public Evento<TestCalculationEventoParameters, TestCalculationEventoResults> addValuesEvento;
		public Evento<TestCalculationEventoParameters, TestCalculationEventoResults> subtractValuesEvento;
		public Evento<TestCalculationEventoParameters, TestCalculationEventoResults> populateCValueEvento;

		public TestExecutionPlan1() : base("Execution Plan 1")
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

			base.setSequence (getValuesEvento, addValuesEvento, subtractValuesEvento, populateCValueEvento);
		}

		public TestCalculationEventoResults Start()
		{
			return base.start ();
		}
	}

	public class TestExecutionPlan2 : EventoExecutionPlanBase<TestCalculationEventoParameters, TestCalculationEventoResults>
	{
		public TestExecutionPlan2() : base("Execution Plan 2")
		{

		}

		public TestCalculationEventoResults Start()
		{
			return base.start ();
		}
	}


	[TestFixture()]
	public class Test
	{
		public Evento<TestCalculationEventoParameters, TestCalculationEventoResults> getValuesEvento;
		public Evento<TestCalculationEventoParameters, TestCalculationEventoResults> addValuesEvento;
		public Evento<TestCalculationEventoParameters, TestCalculationEventoResults> subtractValuesEvento;
		public Evento<TestCalculationEventoParameters, TestCalculationEventoResults> populateCValueEvento;

		public TestCalculationEventoParameters testParameterSet1 = new TestCalculationEventoParameters() {
			a = 10,
			b = 20
		};

		public TestCalculationEventoParameters testParameterSet2 = new TestCalculationEventoParameters() {
			a = 20,
			b = 30
		};

		[SetUp()]
		public void init()
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

		[Test()]
		public void Single_Evento_Execution_With_No_Initial_Parameters ()
		{
			TestCalculationEventoResults result = this.getValuesEvento.Start ();

			Assert.IsNotNull (result);
			Assert.AreEqual (result.a, this.testParameterSet1.a);
			Assert.AreEqual (result.b, this.testParameterSet1.b);
			Assert.AreEqual (result.c, 0);
		}


		[Test()]
		public void Manually_Planned_Execution_Plan_With_No_Initial_Parameters ()
		{
			this.getValuesEvento.Then (this.addValuesEvento);
			this.addValuesEvento.Then (this.subtractValuesEvento);
			this.subtractValuesEvento.Then (this.populateCValueEvento);
			TestCalculationEventoResults result = this.getValuesEvento.Start ();

			Assert.AreEqual (result.a, (this.testParameterSet1.a + 1 - 2) * 3);
			Assert.AreEqual (result.b, (this.testParameterSet1.b + 1 - 2) * 2);
			Assert.AreEqual (result.c, (((this.testParameterSet1.a + 1 - 2) * 3) + ((this.testParameterSet1.b + 1 - 2) * 2)) - 1);
		}

		[Test()]
		public void Manually_Planned_Execution_Plan_With_Initial_Parameters ()
		{
			this.getValuesEvento.Then (this.addValuesEvento);
			this.addValuesEvento.Then (this.subtractValuesEvento);
			this.subtractValuesEvento.Then (this.populateCValueEvento);
			TestCalculationEventoResults result = this.getValuesEvento.Start (this.testParameterSet2);

			Assert.AreEqual (result.a, (this.testParameterSet2.a + 1 - 2) * 3);
			Assert.AreEqual (result.b, (this.testParameterSet2.b + 1 - 2) * 2);
			Assert.AreEqual (result.c, (((this.testParameterSet2.a + 1 - 2) * 3) + ((this.testParameterSet2.b + 1 - 2) * 2)) - 1);
		}

		[Test()]
		public void Manually_Planned_Execution_Plan_With_No_Initial_Parameters_Started_From_SecondStep ()
		{
			//this.getValuesEvento.Then (this.addValuesEvento);
			this.addValuesEvento.Then (this.subtractValuesEvento);
			this.subtractValuesEvento.Then (this.populateCValueEvento);
			TestCalculationEventoResults result = this.addValuesEvento.Start (this.testParameterSet2);

			Assert.AreEqual (result.a, (this.testParameterSet2.a + 1 - 2) * 3);
			Assert.AreEqual (result.b, (this.testParameterSet2.b + 1 - 2) * 2);
			Assert.AreEqual (result.c, (((this.testParameterSet2.a + 1 - 2) * 3) + ((this.testParameterSet2.b + 1 - 2) * 2)) - 1);
		}

		[Test()]
		public void Execution_Plan_With_No_Initial_Parameters ()
		{
			TestExecutionPlan1 testEP = new TestExecutionPlan1 ();
			TestCalculationEventoResults result = testEP.Start ();

			Assert.AreEqual (result.a, (this.testParameterSet1.a + 1 - 2) * 3);
			Assert.AreEqual (result.b, (this.testParameterSet1.b + 1 - 2) * 2);
			Assert.AreEqual (result.c, (((this.testParameterSet1.a + 1 - 2) * 3) + ((this.testParameterSet1.b + 1 - 2) * 2)) - 1);
		}

		[Test()]
		[ExpectedException(typeof(EmptyExecutionPlanException))]
		public void Execution_Plan_With_No_Sequence_And_With_No_Initial_Parameters_Throws_EmptyExecutionPlanException ()
		{
			TestExecutionPlan2 testEP = new TestExecutionPlan2 ();
			TestCalculationEventoResults result = testEP.Start ();
			Assert.IsNotNull (result);
		}
	}
}

