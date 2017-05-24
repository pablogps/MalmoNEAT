using System;
using System.Collections.Generic;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;

namespace SharpNeat
{
    public static class PopulationReadWrite
    {
        static NeatEvolutionAlgorithm<NeatGenome> evolutionAlgorithm;
        static string populationFilePath;
        static string championFilePath;

        static PopulationReadWrite()
        {
            string folderPath = AppDomain.CurrentDomain.BaseDirectory;
            populationFilePath = folderPath + "_Population.xml";
            championFilePath = folderPath + "_Champion.xml";
        }

        static public void GetEvolutionAlgorithm(NeatEvolutionAlgorithm<NeatGenome> inEvolutionAlgorithm)
        {
            evolutionAlgorithm = inEvolutionAlgorithm;
        }

        static public void SaveChampion()
        {
            List<NeatGenome> onlyTheChamp = new List<NeatGenome>() { evolutionAlgorithm.CurrentChampGenome }; 
            var doc = NeatGenomeXmlIO.SaveComplete(onlyTheChamp, false);
            doc.Save(championFilePath);
        }

        static public void SavePopulation()
        {
            var doc = NeatGenomeXmlIO.SaveComplete(evolutionAlgorithm.GenomeList, false);
            doc.Save(populationFilePath);   
            SaveChampion();        
        }





        // from factory

        /*

/// <summary>
/// Saves the population of genomes in an specific folder.
/// </summary>
void SavePopulation(IList<NeatGenome> genomeList, string genericFilePath,
					string experimentName)
{
	// We need to provide a new directory path, adding /Module* where
	// * is the youngest module number.
	int module = genomeList[0].FindYoungestModule();
	string newFolder = genericFilePath + "/Module" + module.ToString();

	// We then need to provide paths for the population and champion.
	string champFileSavePath = newFolder + string.Format("/{0}.champ.xml",
														 experimentName);
	string popFileSavePath = newFolder + string.Format("/{0}.pop.xml",
														 experimentName);
	// TODO: IEC-ESP
	//_optimizer.SavePopulation(newFolder, popFileSavePath, champFath);
        }

        */


    }
}
