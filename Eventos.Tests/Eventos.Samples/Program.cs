using System;


namespace Eventos.Samples
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			EventoSamples samples = new EventoSamples ();

			Console.WriteLine ("Starting...\n");
			samples.Run ();
			Console.WriteLine ("\n");
		}
	}
}
