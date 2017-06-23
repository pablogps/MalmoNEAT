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
            Console.WriteLine("CONSIDER INCLUDING THE SIZE OF THE NETWORK IN THE FITNESS!\n");


            programRunning = true;
        	Console.WriteLine("Start program: " + DateTime.Now.ToString() + "\n");
            Initialize();
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
            PopulationReadWrite.SavePopulation();
            AddFirstModuleIfNeeded();  
        }

        static void InitializeEvolutionAlgorithm(MyExperiment currentExperiment)
        {
            // Passing a file path the new evolution algorithm will try 
            // to read an existing population.
            evolutionAlgorithm = currentExperiment.CreateEvolutionAlgorithm(
                    PopulationReadWrite.PopulationFilePath);
            evolutionAlgorithm.UpdateEvent += new EventHandler(whenUpdateEvent);
            evolutionAlgorithm.PausedEvent += new EventHandler(whenPauseEvent);
        }

        /// <summary>
        /// Newly created genomes will only have an empty "carcass" with some 
        /// basic elements (input and output connections and a regulatoty neuron).
        /// We need to create a module with all the internal connections. The most
        /// basic module will include "local input" and "local output" neurons as
        /// interface between all inputs and outputs (in the future it will be
        /// possible to connect only to a subset of these).
        /// 
        /// This step is not necessary if an old (complete) population has been
        /// loaded.
        /// </summary>
        static void AddFirstModuleIfNeeded()
        {
            if (CheckNewModuleNeeded())
            {
                ModuleOperationManager.AddCompleteModule();                
            }
        }

        /// <summary>
        /// Checks the module ID of the last neuron in one (the first) genome of
        /// the population. If (and only if) this is "0" this means the genome
        /// is not complete, and so a new (first) module is needed.
        /// </summary>
        static bool CheckNewModuleNeeded()
        {
            int latestModuleID;
            int lastNeuronIndex = evolutionAlgorithm.GenomeList[0].NeuronGeneList.Count - 1;
            latestModuleID = evolutionAlgorithm.GenomeList[0].NeuronGeneList[lastNeuronIndex].ModuleId;
            if (latestModuleID == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
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
