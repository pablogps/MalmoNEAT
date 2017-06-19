﻿using System;
using SharpNeat.Phenomes;
using MalmoObservations;

namespace RunMission
{
    public class BrainToMalmoController
    {
        IBlackBox brain;

        public BrainToMalmoController(IBlackBox givenBrain)
        {
			SetDotAsDecimalSeparator();
            brain = givenBrain;
            ProgramMalmo.ObservationsEvent += new EventHandler<ObservationEventArgs>(WhenObservationsEvent);
        }

        /// <summary>
        /// This class will take numbers and pass them as commands to ProgramMalmo.
        /// But Malmo works with dot as separator (0.9) and we must make sure
        /// numbers are not printed using comas (0,9) which would not work.
        /// </summary>
        void SetDotAsDecimalSeparator()
        {
            System.Globalization.CultureInfo customCulture =
		            (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;            
        }

        /// <summary>
        /// Program Malmo will execute Minecraft. At some points it will create
        /// an update event, so which this controller is subscribed. This event
        /// passes the observations as parameters (observations are the state
        /// of the Minecraft simulation, such as position or the type of blocks
        /// in the surroundings).
        /// This class, the brain, will take these observations and translate
        /// them into commands for the Minecraft character (using a neural network).
        /// This will be done every time the event is called.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="eventArguments">Event arguments.</param>
        void WhenObservationsEvent(object sender, ObservationEventArgs eventArguments)
        {
            ObservationsToCommands(eventArguments.observations);
        }

        void ObservationsToCommands(JsonObservations observations)
        {
        	ObservationsToBrainInputs();
        	brain.Activate();
        	OutputsToCommands();
        }

        void ObservationsToBrainInputs()
        {
            brain.InputSignalArray[0] = 1;
            brain.InputSignalArray[1] = 1;
        }

        void OutputsToCommands()
        {
            string frontSpeed = OutputToMovementStringValue(
                    brain.OutputSignalArray[0], brain.OutputSignalArray[1]);
            string lateralSpeedString = OutputToMovementStringValue(
                    brain.OutputSignalArray[2], brain.OutputSignalArray[3]);
            ProgramMalmo.AddCommandToList("move " + frontSpeed);
            ProgramMalmo.AddCommandToList("strafe " + lateralSpeedString);
        }

        string OutputToMovementStringValue(double speedValue, double directionValue)
        {
            // Outputs are from 0 to 1
            if (speedValue > 0.5)
            {
                if (directionValue > 0.5)
                {
                    return "1.0";
                }
                else
                {
                    return "-1.0";
                }
            }
            else
            {
                return "0.0";
            }
        }

// Test stuff-------------------------------------------------------------------
        public double TestObservationsToCommands(bool testThis)
        {
        	ObservationsToBrainInputs();
        	brain.Activate();
        	return TestOutputsToCommands();
        }

        double TestOutputsToCommands()
        {
        	double frontSpeed = brain.OutputSignalArray[0];
        	double lateralSpeed = brain.OutputSignalArray[1];
            double fitness;
            fitness = 200;
            fitness -= 100.0 * Math.Abs(frontSpeed - 0.5);
            fitness -= 100.0 * Math.Abs(lateralSpeed - 0.5);
            Console.WriteLine("Ahead value " + frontSpeed.ToString());
            Console.WriteLine("Lateral value " + lateralSpeed.ToString());
            Console.WriteLine("Fitness " + fitness.ToString());
            if (fitness < 0.0)
            {
                fitness = 0.0;
            }
            return fitness;
        }
    }
}
