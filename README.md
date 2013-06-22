#Eventos
######Eventos = events in spanish / Evento = event in spanish.
Eventos was born out of a weekend experiment. Its an small, simple and extensible pseudo-pipeline framework for C# MONO and .NET 4+ that allows execution of interchangeable, re-arrangable, reusable and replaceable atomic tasks called Eventos with the minimum amount of code. Its made with reusability and modularity in mind. 

#### Possible Usages:
+ ETLs, ETLs, ETLs, …
+ Business Logic reusability in general…
+ Take out the garbage, cook and bring back the newspaper for you. In the future it should be able to bath your dog at the same time if you ask it for it. But for now is just one thing at a time in the laundry list.


I am working on adding more functionality to it. For now only the common pipe pattern is available allowing the creation of a forward-only sequences. I am working on execution plan branching, multi-threading, and a lot more… It is fun… Hopefully its useful… ;)


## References and Dependencies
Please, reference Evento.Core.DLL. That's everything you need. The project is compiled in 4.5 but it should work fine if you downgrade it to 4.0.

```
using Eventos.Core;
// If you want to catch specific Framework Exceptions
using Eventos.Exceptions;
// If you want to create a custom ExecutionPlan
using Eventos.Base;
```

## The Evento Object
The Evento class is a generics class it must be defined with the Parameters Type and the Result Type. The Result Type must inherit from the Parameters type in order to allow piping of the results to the next Evento in the Execution Plan sequence.

```
Evento<TParametersClass, TResultClass>	
```

```
…
private class TestParameters
{
	public int a { get; set; }
	public int b { get; set; }
}

private class TestResults : TestParameters
{
	// Optional. I just added it to demonstrate the point.
	public int c { get; set; }
}

private Evento<TestParameters, TestResult> newEvento;

…
```

### Create a new Instance
Evento objects are pretty straightforward. You will only care about its constructor 9 out of 10, until new features are added. The constructor requires an anonymous function that accepts one parameter that I prefer to call ctx since it will be populated with the EventoExecutionContext object generated when the Evento is Executed.

```
…
private Evento<TestParameters, TestResult> newEvento = new Evento<TestParameters, TestResult>((ctx) => {
	int ax = ctx.HasExecutionParameters ? ctx.ExecutionParameters.a + 10 : 2;
	int bx = ctx.HasExecutionParameters ? ctx.ExecutionParameters.b + 10 : 2;
	return new TestResult() {
		a = ax,
		b = bx,
		c = 2 // Optional
	};
});
…
```

Another way of doing it …

```
…
private Function<EventoExecutionContext, TestResult> fn = (ctx) => {
	int ax = ctx.HasExecutionParameters ? ctx.ExecutionParameters.a + 10 : 2;
	int bx = ctx.HasExecutionParameters ? ctx.ExecutionParameters.b + 10 : 2;
	return new TestResult() {
		a = ax,
		b = bx,
		c = 2 // Optional
	};
};
private Evento<TestParameters, TestResult> newEvento = new Evento<TestParameters, TestResult>(fn);
…
```

## The Evento Execution Context Class
The Evento Execution context class instance gets created during the exception of the Evento instance, not on its creation. This class holds all relevant information required by the Evento instance. 

* **ExecutionParameters**: Parameters passed to the Evento by the client or by a previous Evento piped in an Execution Plan.
* **ExecutionResults**: Values resulted from the execution of the Evento. This value is the direct result from the function wrapped inside the Evento Class.
* **Parent**: Is a reference to the Evento that created the Context. The Evento creates the Context and passes it self into it. (Not necessarily a great idea but I will revisit it in the future.)
* **HasExecutionParameters**: Determines if ExecutionParameters were passed into the Evento at the time of the execution. (basically X != null).
* **HasExecutionResults**: Determines if Execution Results were generated during the execution of the Evento. (same X != null).

A look at its scope…

```
…
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
…
```

As you can see the context lives in the Start method and it is passed to the wrapped Function.

## Execution Plans
### Manual
An Execution Plan is just a set of Eventos executed in sequence by you or by a custom execution plan class. 
In order for the Evento class to be able to be sequenced into an execution plan I exposed a method called **Then()**.

```
…
newEvento.Then(anotherEvento);
anotherEvento.Then(yetAnotherEvento);

TestResult result = newEvento.Start();
…
```

You can continue to pipe Eventos eternally. They will not execute until you call the ***Start*** method in any of them. Yes, you can start the execution of the execution plan anywhere down the pipe. But you will need to provide parameters if your Eventos require data from the ones you have skipped.

```
…
//newEvento.Then(anotherEvento);
anotherEvento.Then(yetAnotherEvento);
yetAnotherEvento.Then(oneMoreEvento);

TestResult result = anotherEvento.Start(new TestParameters() {
	a = 10,
	b = 20
});
…
```

The result will be returned from the last Evento in the pipe. I guess now it starts to make sense. You have 1000 Eventos, each of them adds 1 to the result of the previous one. When you start your first Evento in the Execution plan you pass or initialize a value to 0 and pass it to the next Evento until the last one returns 1000. Thanks it.

### Execution Plan Class
The Execution Plan class is a simple mechanism for creating Evento Sequences in a modularized and reusable way. You can review the Execution Plan Base class. You can use the standard Execution Plan Class or you can just create your very own sequence that you can reuse in the future.

Simple usage:

```
EventoExecutionPlan simpleExecutionPlan = new EventExecutionPlan("The Name");
simpleExecutionPlan.setSequence(firstEvento, secondEvento, thirdEvento, forthEvento);
simpleExecutionPlan.Start()
```

Now, you can create your custom execution plans too:

```
…
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
...
```

Reusable eventos:


```
…

public class TestEvento1 : EventoBase<TestCalculationParameters, TestCalculationResult>
{
	public TestEvento1() : base((ctx) => {
				return new TestCalculationEventoResults {
					a = ctx.HasExecutionParameters ? ctx.ExecutionParameters.a : 10,
					b = ctx.HasExecutionParameters ? ctx.ExecutionParameters.b : 20
				};
			}, "Get Values")
			{
			}
}

public class TestEvento2 : EventoBase<TestCalculationParameters, TestCalculationResult>
{
	public TestEvento1() : base((ctx) => {
				return new TestCalculationEventoResults {
					a = ctx.HasExecutionParameters ? ctx.ExecutionParameters.a + 12 : 10,
					b = ctx.HasExecutionParameters ? ctx.ExecutionParameters.b + 12 : 20
				};
			}, "Add Values")
			{
			}
}


public class TestExecutionPlan1 : EventoExecutionPlanBase<TestCalculationEventoParameters, TestCalculationEventoResults>
	{
		public Evento<TestCalculationEventoParameters, TestCalculationEventoResults> getValuesEvento;
		public Evento<TestCalculationEventoParameters, TestCalculationEventoResults> addValuesEvento;

		public TestExecutionPlan1() : base("Execution Plan 1")
		{				
			this.getValuesEvento = new TestEvento1();
			this.addValuesEvento = new TestEvento2();

			base.setSequence (getValuesEvento, addValuesEvento);
		}

		public TestCalculationEventoResults Start()
		{
			return base.start ();
		}
	}
...
```

Multiple Sequences:

```
…

public class TestEvento1 : EventoBase<TestCalculationParameters, TestCalculationResult>
{
	public TestEvento1() : base((ctx) => {
				return new TestCalculationEventoResults {
					a = ctx.HasExecutionParameters ? ctx.ExecutionParameters.a : 10,
					b = ctx.HasExecutionParameters ? ctx.ExecutionParameters.b : 20
				};
			}, "Get Values")
			{
			}
}

public class TestEvento2 : EventoBase<TestCalculationParameters, TestCalculationResult>
{
	public TestEvento2() : base((ctx) => {
				return new TestCalculationEventoResults {
					a = ctx.HasExecutionParameters ? ctx.ExecutionParameters.a + 12 : 10,
					b = ctx.HasExecutionParameters ? ctx.ExecutionParameters.b + 12 : 20
				};
			}, "Add Values")
			{
			}
}

public class TestEvento3 : EventoBase<TestCalculationParameters, TestCalculationResult>
{
	public TestEvento3() : base((ctx) => {
				return new TestCalculationEventoResults {
					a = ctx.HasExecutionParameters ? ctx.ExecutionParameters.a - 10 : 0,
					b = ctx.HasExecutionParameters ? ctx.ExecutionParameters.b - 10 : 0
				};
			}, "Subract Values")
			{
			}
}


public class TestExecutionPlan1 : EventoExecutionPlanBase<TestCalculationEventoParameters, TestCalculationEventoResults>
	{
		public Evento<TestCalculationEventoParameters, TestCalculationEventoResults> getValuesEvento;
		public Evento<TestCalculationEventoParameters, TestCalculationEventoResults> addValuesEvento;
		public Evento<TestCalculationEventoParameters, TestCalculationEventoResults> subtractValuesEvento;


		public TestExecutionPlan1() : base("Execution Plan 1")
		{				
			this.getValuesEvento = new TestEvento1();
			this.addValuesEvento = new TestEvento2();
			this.subtractValuesEvento = new TestEvento3();

		}

		public TestCalculationEventoResults StartSequence1()
		{
			base.setSequence (getValuesEvento, addValuesEvento, subtractValuesEvento);
			return base.start ();
		}
		
		public TestCalculationEventoResults StartSequence2()
		{
			base.setSequence (getValuesEvento, subtractValuesEvento, addValuesEvento);
			return base.start ();
		}
	}
...
```

And more… 

Again, the Eventos sequence will not be executed until the Start method is called in the Execution plan abstract class instance. 

## What's next

- Other flow control options: Loops, Conditions, etc...
- Performance
- Concurrency (Multi-threading, async?) pending analysis…(for now you can wrap your threading logic around it)
- Execution Plan Branching (Non-Linear pipeline and parallel pipeline)
- Execution Plan Monitoring
- Execution Plan Control
- More performance
- ...