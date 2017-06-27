using System;
using System.Collections.Generic;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;

namespace SharpNeat.IecEsp
{
    public static class ModuleOperationManager
    {
        static NeatEvolutionAlgorithm<NeatGenome> evolutionAlgorithm;
        static NeatGenomeFactory factory;
        static NeatGenome genome;
        static UIvariables uiVar;
        static readonly Random randomGenerator;

        static ModuleOperationManager()
        {
            randomGenerator = new Random();
        }

        public static NeatGenome Genome
        {
            get { return genome; }
        }

        static public void GetEvolutionAlgorithm(NeatEvolutionAlgorithm<NeatGenome> inEvolutionAlgorithm)
        {
	        evolutionAlgorithm = inEvolutionAlgorithm;
            UpdateGenome();
            factory = genome.GenomeFactory;
        }

        static void UpdateGenome()
        {
            genome = evolutionAlgorithm.GenomeList[0];
        }

        static public void AddCompleteModule()
        {
            // Reset evolution process if it is running! (Or ensure it is stopped
            // before calling this method.)
            Console.WriteLine("\nCreating a module for the genomes. This module will " +
                              "create connections for all inputs and outputs. " +
                              "(This may be used to simulate traditional interactive " +
                              "evolution with no modular functionality.)\n");
            PrepareUIvar();
            // WARNING: 3 parameters is too many!
            factory.AddNewModule(evolutionAlgorithm.GenomeList, 
                                 evolutionAlgorithm.CurrentChampGenome, uiVar);
            UpdateGenomeStatistics();
            RandomChampion();
            PopulationReadWrite.SavePopulation();
            // TODO: IMPORTANT NOTICE FROM UNITY IMPORT. See comment at the end of the script.*
            // Easy patch: update the ID generator here.
            factory.InitializeGeneratorAfterLoad(evolutionAlgorithm.GenomeList);
        }

        static void PrepareUIvar()
        {
            // We use evolutionAlgorithm.GenomeList[0] instead of genome so that
            // we don't have to make sure to have an updated genome here!
            int newId = evolutionAlgorithm.GenomeList[0].FindYoungestModule() + 1;
            uiVar = new UIvariables();
			CreateCompleteInputOutputList(newId);
            // Pandeomonium initially set to 0
            uiVar.pandemonium.Add(newId, 0);            
        }

        /// <summary>
        /// Modifies uiVar to include all possible inputs and outputs in the new
        /// module. Input list and output list will get information to create
        /// a link to (from) all input (output) neurons in the genome. 
        /// A provisional connection is also made ready for the regulatory node.
        /// </summary>
        static void CreateCompleteInputOutputList(int newId)
        {
            List<newLink> inputList = CreateListWithinNodeIndices(0, genome.Input + 1);
            List<newLink> outputList = CreateListWithinNodeIndices(genome.Input + 1, genome.Input + genome.Output + 1);
        	uiVar.localInputList.Add(newId, inputList);
            uiVar.localOutputList.Add(newId, outputList);
            // For the regulatory neuron we create a provisional connection to bias (index = 0)
            List<newLink> regulationList = new List<newLink>();
            regulationList.Add(CreateDefaultWithTarget(0));
            uiVar.regulatoryInputList.Add(newId, regulationList);
        }

        /// <summary>
        /// Returns a default link information (weight 1, ID 0) to connect to
        /// (or from) all neurons in the genome between index1 (included) and
        /// index2 (excluded). This may be used both for input-to-localInput and
        /// localOutput-to-output connections.
        /// </summary>
        static List<newLink> CreateListWithinNodeIndices(int index1, int index2)
        {
        	List<newLink> returnList = new List<newLink>();
        	for (int i = index1; i < index2; ++i)
        	{
        		returnList.Add(CreateDefaultWithTarget((uint)i));
        	}
        	return returnList;
        }

        /// <summary>
        /// Creates a new newLink element with default weight.
        /// </summary>
        static newLink CreateDefaultWithTarget(uint target)
        {
        	newLink newElement = new newLink();
        	newElement.otherNeuron = target;
        	newElement.weight = 1.0;
        	newElement.id = 0;
        	return newElement;
        }

        /// <summary>
        /// Updates the current champion genome (in case we operate on the genomes
        /// before an evolution event). They are all clones at this point!
        /// Note that even if all genomes in the list have been updated, the
        /// champion may not! (It is not merely a link to a genome id, but a copy).
        /// 
        /// Although sometimes all genomes are equal, here we take a random
        /// member from the genome list.
        /// </summary>
        static void RandomChampion()
        {
            int myRandom = randomGenerator.Next(evolutionAlgorithm.GenomeList.Count);
            evolutionAlgorithm.CurrentChampGenome = evolutionAlgorithm.GenomeList[myRandom];
        }

        /// <summary>
        /// This method may be used to update the champion after changes that don't
        /// clone the active module (so we need to update the champion genome because
        /// something has changed, but we do not want a random genome).
        /// </summary>
        static void UpdateOldChampion()
        {
            uint champId = evolutionAlgorithm.CurrentChampGenome.Id;
            foreach (NeatGenome localGenome in evolutionAlgorithm.GenomeList)
            {
                if (localGenome.Id == champId)
                {
                    evolutionAlgorithm.CurrentChampGenome = localGenome;
                    return;
                }
            }            
        }

        static void UpdateGenomeStatistics()
        {
            UpdateGenome();
            factory.UpdateStatistics(genome);
            foreach (NeatGenome localGenome in evolutionAlgorithm.GenomeList)
            {
                localGenome.RecalculateConnectivityDataFromNodes();
            }
        }
    }
}

//*
// There seems to be a bug here: when a population is loaded the ID
// generator is reset to the last value used + 1. Then, only the FIRST
// time AddCompleteModule is called, even if at the very end of AddCompleteModule
// the ID generator is more advanced (because elements have been created)
// the ID at the exit of AddNewModule is again the original value used
// after loading the population. Why? Problems with references?
// This is not usually relevant (apparently AddNewModule will find the
// correct value again if called a second time, and for some reason in new
// calls the problem does not happen again). But AskMutateOnce will
// not correct the problem, and it may try to add elements with repeated
// IDs, which creates genomes that fail the integrity check.
