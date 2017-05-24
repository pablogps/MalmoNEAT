using SharpNeat.Core;
using SharpNeat.Phenomes;

namespace SharpNeat
{
	/// <summary>
	/// Class used to evaluate neural networks that play Tic-Tac-Toe against
	/// random and optimal agents.
	/// </summary>
	public class MalmoEvaluator : IPhenomeEvaluator<IBlackBox>
	{
		private ulong _evalCount;
		private bool _stopConditionSatisfied;

		#region IPhenomeEvaluator<IBlackBox> Members

		/// <summary>
		/// Gets the total number of evaluations that have been performed.
		/// </summary>
		public ulong EvaluationCount
		{
			get { return _evalCount; }
		}

		/// <summary>
		/// Gets a value indicating whether some goal fitness has been achieved and that
		/// the the evolutionary algorithm/search should stop. This property's value can remain false
		/// to allow the algorithm to run indefinitely.
		/// </summary>
		public bool StopConditionSatisfied
		{
			get { return _stopConditionSatisfied; }
		}

		/// <summary>
		/// Implement here the task. In our case, this should be something like
        /// creating a serires of Minecraft simulations for each brain and get
        /// a fitness in each case.
		/// </summary>
		public FitnessInfo Evaluate(IBlackBox box)
		{
            ++_evalCount;

            System.Console.WriteLine("enter evaluation " + _evalCount);

			double fitness = 0;

            // Typically here inputs and outputs for an ANN are declared and
            // box is activated (producing outputs from the inputs).
            // In our case this will take place in Minecraft using the Malmo
            // platform.
            fitness = _evalCount;

            if (fitness >= 32)
            {
                _stopConditionSatisfied = true;
            }

            return new FitnessInfo(fitness, fitness);
			//return new FitnessInfo(fitness, fitness);
		}

		/// <summary>
		/// Reset the internal state of the evaluation scheme if any exists.
		/// Note. The TicTacToe problem domain has no internal state. This method does nothing.
		/// </summary>
		public void Reset()
		{
		}

		#endregion
	}
}