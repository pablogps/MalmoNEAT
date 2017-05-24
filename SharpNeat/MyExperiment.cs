using SharpNeat.Core;
using SharpNeat.Phenomes;
using System.Xml;

namespace SharpNeat
{
    public class MyExperiment : SimpleNeatExperiment
    {
        public override IPhenomeEvaluator<IBlackBox> PhenomeEvaluator
        {
            get { return new MalmoEvaluator(); }
        }

        /// <summary>
        /// Defines the number of input nodes in the neural network.
        /// The network has one input for each square on the board,
        /// so it has 9 inputs total.
        /// </summary>
        public override int InputCount
        {
            get { return 2; }
        }

        /// <summary>
        /// Defines the number of output nodes in the neural network.
        /// The network has one output for each square on the board,
        /// so it has 9 outputs total.
        /// </summary>
        public override int OutputCount
        {
            get { return 2; }
        }

        /// <summary>
        /// This constructor takes the experiment name and the configuration
        /// xml file name to Initialize everything directly.
        /// The constructor with no inputs is made private to ensure only this
        /// constructor is used.
        /// </summary>
        public MyExperiment(string experimentName, string xmlFile)
        {
            Initialize(experimentName, xmlFile);
        }
        private MyExperiment() {}

        /// <summary>
        /// Loads the configuration xml file and calls the initializer in the
        /// base class.
        /// </summary>
        void Initialize(string experimentName, string xmlFile)
        {
            XmlDocument xmlConfig = new XmlDocument();
            xmlConfig.Load(xmlFile);
            // The documentElement property returns the root node of the document.
            Initialize(experimentName, xmlConfig.DocumentElement);
        }
    }
}
