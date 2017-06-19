using System;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;
using SharpNeat.IecEsp;

namespace SharpNeat
{
	public class ProgramCoordination
	{
        static bool programRunning;

        //Debug.WriteLine is not working :(
        //ConsoleTraceListener c = new ConsoleTraceListener(true);
        //Trace.Listeners.Add(c);
        static NeatEvolutionAlgorithm<NeatGenome> evolutionAlgorithm;

		public static void Main()
		{
            programRunning = true;
        	Console.WriteLine("Start program: " + DateTime.Now.ToString() + "\n");
            Initialize();
            PopulationReadWrite.SavePopulation();
            // At this stage we want to mimic classic interactive evolution.
            // For this we create only one module connecting all inputs and
            // outputs. Modular functionality will be added later.
            ModuleOperationManager.AddCompleteModule();
            StartEvolution();
            while (programRunning == true) {}
            // Maybe create "while(running==true)" to avoid ReadLine (if we remove
            // ReadLine now the program will simply end without completing other threads)
        	Console.WriteLine("End program: " + DateTime.Now.ToString());
		}

        static void Initialize()
        {
            // My experiment controls what NEAT will do (how genomes will be 
            // evaluated, their inputs and outputs, etc.)
            MyExperiment currentExperiment = new MyExperiment("Malmo", "Malmo.config.xml");
            InitializeEvolutionAlgorithm(currentExperiment);
            PopulationReadWrite.GetEvolutionAlgorithm(evolutionAlgorithm);
            ModuleOperationManager.GetEvolutionAlgorithm(evolutionAlgorithm);         
        }

        static void InitializeEvolutionAlgorithm(MyExperiment currentExperiment)
        {
            evolutionAlgorithm = currentExperiment.CreateEvolutionAlgorithm();
            evolutionAlgorithm.UpdateEvent += new EventHandler(whenUpdateEvent);
            evolutionAlgorithm.PausedEvent += new EventHandler(whenPauseEvent);
        }

        static void whenUpdateEvent(object sender, EventArgs e)
        {
            Console.WriteLine(string.Format("gen={0:N0} bestFitness={1:N6}",
							  evolutionAlgorithm.CurrentGeneration,
							  evolutionAlgorithm.Statistics._maxFitness));
            PopulationReadWrite.SavePopulation();
        }

        static void whenPauseEvent(object sender, EventArgs e)
        {    
            PopulationReadWrite.SavePopulation();
            // In this version we decide to stop the program at a pause event.
            programRunning = false;
        }

        static void StartEvolution()
        {
            evolutionAlgorithm.MakeEvolutionReady();
            Console.WriteLine("\nEvolution ready, starting iterations");
            PopulationReadWrite.SavePopulation();
            evolutionAlgorithm.StartContinue(); 
        }
	}
}
