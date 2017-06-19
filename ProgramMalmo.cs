// --------------------------------------------------------------------------------------------------
//  Copyright (c) 2016 Microsoft Corporation
//  
//  Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
//  associated documentation files (the "Software"), to deal in the Software without restriction,
//  including without limitation the rights to use, copy, modify, merge, publish, distribute,
//  sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//  
//  The above copyright notice and this permission notice shall be included in all copies or
//  substantial portions of the Software.
//  
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT
//  NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//  NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
//  DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// --------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Research.Malmo;
using Newtonsoft.Json;
using MalmoObservations;

namespace RunMission
{
	public static class ProgramMalmo
	{
		static AgentHost agentHost = new AgentHost();
		static MissionSpec mission;
		static MissionRecordSpec missionRecord;
		static WorldState worldState;
		static JsonObservations observations = new JsonObservations();
        static string fileName;
        static List<string> listOfCommmands;

        public static event EventHandler<ObservationEventArgs> ObservationsEvent;
        public static event EventHandler<ObservationEventArgs> MissionEndEvent;

        /// <summary>
        /// Runs a Minecraft simulation. The parameters of the simulation are in
        /// the minecraftWorldXML file.
        /// Using newFileName allows to save files in an specific path (otherwise
        /// the save path will not change and new calls will overwrite data!)
        /// </summary>
        /// <param name="newFileName">New file name.</param>
        public static void RunMalmo(string newFileName)
        {
            fileName = newFileName;
            RunMalmo();
        }
        public static void RunMalmo()
		{
			ReadCommandLineArgs();
			InitializeMission();
			TryStartMission();
			ConsoleOutputWhileMissionLoads();
			MainLoop();
			Console.WriteLine("Mission has stopped.");
            OnMissionEndEvent();
		}

        public static void AddCommandToList(string newCommand)
        {
            listOfCommmands.Add(newCommand);
        }

//----------------------------------------------------------------------------------------------------------------------
		static void ReadCommandLineArgs()
		{
			try
			{
				agentHost.parse(new StringVector(Environment.GetCommandLineArgs()));
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine("ERROR: {0}", ex.Message);
				Console.Error.WriteLine(agentHost.getUsage());
				Environment.Exit(1);
			}
			if (agentHost.receivedArgument("help"))
			{
				Console.Error.WriteLine(agentHost.getUsage());
				Environment.Exit(0);
			}
		}

//----------------------------------------------------------------------------------------------------------------------
        static void InitializeMission()
		{
            string rawMissionXML = System.IO.File.ReadAllText("C:\\Users\\pago\\Documents\\Malmo\\CSharp_MyExperiments\\MalmoNEAT\\minecraftWorldXML.txt");

            // NOT WORKING YET
            //string rawMissionXML = RawXMLmissionFactory.xmlMission;

            mission = new MissionSpec(rawMissionXML, true);
            //mission.forceWorldReset();
            string savePath = MakeSavePath();
			missionRecord = new MissionRecordSpec(savePath);
			missionRecord.recordCommands();
			missionRecord.recordMP4(20, 400000);
			missionRecord.recordRewards();
			missionRecord.recordObservations();
		}
        static string MakeSavePath()
        {
            if (fileName != null)
            {
                return "./_" + fileName + "SavedData.tgz";
            }
            else
            {
                return "./_savedData.tgz";  
            }
        }

//----------------------------------------------------------------------------------------------------------------------
		static void TryStartMission()
		{
			try
			{
				agentHost.startMission(mission, missionRecord);
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine("Error starting mission: {0}", ex.Message);
				Environment.Exit(1);
			}
		}

//----------------------------------------------------------------------------------------------------------------------
		static void ConsoleOutputWhileMissionLoads()
		{
			Console.WriteLine("Waiting for the mission to start");
			UpdateWorldState();
			while (!worldState.has_mission_begun)
			{
				Console.Write(".");
				Thread.Sleep(100);
				UpdateWorldState();
			}
			Console.WriteLine();
		}

//----------------------------------------------------------------------------------------------------------------------
		static void UpdateWorldState()
		{
			worldState = agentHost.getWorldState();
			foreach (TimestampedString error in worldState.errors)
			{
				Console.Error.WriteLine("Error: {0}", error.text);
			}
		}

//----------------------------------------------------------------------------------------------------------------------
		static void MainLoop()
		{
            ResetCommands();
			while (worldState.is_mission_running)
			{
				worldState = agentHost.getWorldState();
				UpdateObservations();
                OnObservationsEvent();
                ExecuteCommands();
                ResetCommands();
				Thread.Sleep(500);
				WriteFeedback();
			}
		}

//----------------------------------------------------------------------------------------------------------------------
		static void UpdateObservations()
		{
			if (worldState.number_of_observations_since_last_state > 0)
			{
				ParseObervations();
				FourTiles neighbourTiles = new FourTiles();
				neighbourTiles.SetTileIndicesGivenObs(observations);
				/*agentHost.sendCommand(string.Format("chat Neighbour tiles: {0}, {1}, {2}, {3}",
													neighbourTiles.Ahead,
													neighbourTiles.Left,
													neighbourTiles.Right,
													neighbourTiles.Back));*/
                /*agentHost.sendCommand(string.Format("chat Position: {0}, {1}, {2}",
                                                    observations.XPos,
                                                    observations.YPos,
                                                    observations.ZPos));*/            
            }
		}

//----------------------------------------------------------------------------------------------------------------------
        static void OnObservationsEvent()
        {
            // Always check that there are listeners for the event
            if (null != ObservationsEvent)
            {
                // Creates here the arguments for the event
                ObservationEventArgs myArguments = new ObservationEventArgs();
                myArguments.observations = observations;
                // Catches exceptions thrown by event listeners. This prevents 
                // listener exceptions from terminating the algorithm thread.
                try
                {
                    ObservationsEvent(null, myArguments);
                }
                catch (Exception ex)
                {
                    //__log.Error("ObservationsEvent listener threw exception", ex);
                    Console.WriteLine("ObservationsEvent listener threw exception" + ex);
                }
            }
        }

//----------------------------------------------------------------------------------------------------------------------
        static void OnMissionEndEvent()
        {
            // Always check that there are listeners for the event
            if (null != MissionEndEvent)
            {
                // Creates here the arguments for the event
                ObservationEventArgs myArguments = new ObservationEventArgs();
                myArguments.observations = observations;

                // Catch exceptions thrown by event listeners. This prevents 
                // listener exceptions from terminating the algorithm thread.
                try
                {
                    MissionEndEvent(null, myArguments);
                }
                catch (Exception ex)
                {
                    //__log.Error("MissionEndEvent listener threw exception", ex);
                    Console.WriteLine("MissionEndEvent listener threw exception" + ex);
                }
            }
        }

//----------------------------------------------------------------------------------------------------------------------
        static void ResetCommands()
        {
            listOfCommmands = new List<string>();
        }

//----------------------------------------------------------------------------------------------------------------------
        static void ExecuteCommands()
        {
            foreach (string command in listOfCommmands)
            {
                agentHost.sendCommand(command); 
            }
        }

//----------------------------------------------------------------------------------------------------------------------
		static void ParseObervations()
		{
			int numberObservations = worldState.observations.Count;
			string msg = worldState.observations[numberObservations - 1].text;
			observations = JsonConvert.DeserializeObject<JsonObservations>(msg);
		}

//----------------------------------------------------------------------------------------------------------------------
		static void WriteFeedback()
		{
            /*Console.WriteLine("video, observations, rewards received: {0}, {1}, {2}",
							  worldState.number_of_video_frames_since_last_state,
							  worldState.number_of_observations_since_last_state,
							  worldState.number_of_rewards_since_last_state);*/
            //Console.WriteLine(".");
			WriteRewards();
			WriteErrors();
		}

		static void WriteRewards()
		{
			foreach (TimestampedReward reward in worldState.rewards)
			{
				Console.Error.WriteLine("Summed reward: {0}", reward.getValue());
			}
		}

		static void WriteErrors()
		{
			foreach (TimestampedString error in worldState.errors)
			{
				Console.Error.WriteLine("Error: {0}", error.text);
			}
		}
	}
}