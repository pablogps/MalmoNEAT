using System;
using SharpNeat.Core;
using SharpNeat.Phenomes;
using RunMission;
using MalmoObservations;

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
        private double fitness;

        public MalmoEvaluator()
        {
            ProgramMalmo.ObservationsEvent += new EventHandler<ObservationEventArgs>(WhenObservationsEvent);
            ProgramMalmo.MissionEndEvent += new EventHandler<ObservationEventArgs>(WhenMissionEndEvent);
        }

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

        #endregion

		/// <summary>
		/// Implement here the task. In our case, this should be something like
        /// creating a serires of Minecraft simulations for each brain and get
        /// a fitness in each case.
        /// 
        /// If no name is given for the simulation (calling only Evaluate(box))
        /// then en empty name will be used.
		/// </summary>
		public FitnessInfo Evaluate(IBlackBox brain)
		{
            return Evaluate(brain, "");
		}
        public FitnessInfo Evaluate(IBlackBox brain, string simulationName)
        {
            // The brain to Malmo controller will be subscribed to the update
            // events of ProgramMalmo, and it will controll the actions of the
            // Minecraft character. Here we only care about evaluating the results
            // of these actions.
            BrainToMalmoController brainToMalmoController = new BrainToMalmoController(brain);
            // Resets fitness and updates the evaluations counter
            fitness = 0;
        	++_evalCount;
        	Console.WriteLine("Enter evaluation " + _evalCount);
            //fitness = brainToMalmoController.TestObservationsToCommands(false);
            simulationName = "provisionalName" + _evalCount.ToString();
        	ProgramMalmo.RunMalmo(simulationName);
            // Note ProgramMalmo will use events to notify this evaluator when
            // the simulation is updated or finished. These events will update
            // fitness. WhenObservationsEvent/WhenMissionEndEvent
            CheckStopCondition();
            // FitnessInfo takes an "alternative fitness", so here we simply pass
            // the same fitness value twice.
        	return new FitnessInfo(fitness, fitness);
        }

        void CheckStopCondition()
        {
            //if (fitness >= 0.9 || _evalCount >= 200)
            if (fitness >= 19.0)
            {
                Console.WriteLine("Condition satisfied at " + _evalCount);
                Console.WriteLine("With fitness " + fitness);
                _stopConditionSatisfied = true;
            }           
        }

		/// <summary>
		/// Reset the internal state of the evaluation scheme if any exists.
		/// Note. The TicTacToe problem domain has no internal state. This method does nothing.
		/// </summary>
		public void Reset()
		{}

        void WhenObservationsEvent(object sender, ObservationEventArgs eventArguments)
        {}

        void WhenMissionEndEvent(object sender, ObservationEventArgs eventArguments)
        {
            UpdateFitnessUsingObservations(eventArguments.observations);
        }

        void UpdateFitnessUsingObservations(JsonObservations observations)
        {
            fitness = 0;
            fitness -= observations.XPos;
            fitness -= observations.ZPos;
            EnsurePositiveFitness();
            Console.WriteLine("Fitness:        " + fitness);
        }

        void EnsurePositiveFitness()
        {
            if (fitness < 0.0)
            {
                fitness = 0.0;
            }
        }
	}
}