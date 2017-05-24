using System;
using System.Collections.Generic;

namespace RunMission
{
	public class FourTiles
	{
		private string ahead;
		private string left;
		private string right;
		private string back;

		static List<List<int>> indicesGivenYaw;
		static bool isIndicesListCreated = false;

		public string Ahead 
		{
			get { return ahead; }
		}
		public string Left
		{
			get { return left; }
		}
		public string Right
		{
			get { return right; }
		}
		public string Back
		{
			get { return back; }
		}

		public FourTiles()
		{
			if (!isIndicesListCreated)
			{
				InitializeIndicesList();
				isIndicesListCreated = true;
			}
		}

		public void SetTileIndicesGivenObs(JsonObservations observations)
		{
			int yaw = (int)YawBetweenZeroAndThreeSixty(observations.Yaw);
			ahead = observations.floor3x3[indicesGivenYaw[yaw][0]];
			left = observations.floor3x3[indicesGivenYaw[yaw][1]];
			right = observations.floor3x3[indicesGivenYaw[yaw][2]];
			back = observations.floor3x3[indicesGivenYaw[yaw][3]];
		}

		double YawBetweenZeroAndThreeSixty(double rawYaw)
		{
			double newYaw = rawYaw;
			if (newYaw < 0)
			{
				newYaw += 360;
			}
			return newYaw;
		}

		/// <summary>
		/// This class needs to initialize indicesGivenYaw.
		/// This object stores the indices for the neighbours depending on
		/// the orientation (yaw). Neighbours are given in a 3x3 grid so
		/// we can discretisize yaw values and consider only 8 directions,
		/// the borders of which happen at odd number * 360 degrees / 16 (pieces)
		/// </summary>
		void InitializeIndicesList()
		{
			// The best way would probably be to load this from a file.
			// But this looks fast enough:
			indicesGivenYaw = new List<List<int>>();
			for (int discreteYaw = 0; discreteYaw < 360; ++discreteYaw)
			{
				List<int> neighboursIndicesList = new List<int>();
				if (discreteYaw < Math.Floor(1.0 * 360 / 16))
				{
					neighboursIndicesList = new List<int> { 7, 5, 3, 1 };
				}
				else if (discreteYaw < Math.Floor(3.0 * 360 / 16))
				{
					neighboursIndicesList = new List<int> { 6, 8, 0, 2 };
				}
				else if (discreteYaw < Math.Floor(5.0 * 360 / 16))
				{
					neighboursIndicesList = new List<int> { 3, 7, 1, 5 };
				}
				else if (discreteYaw < Math.Floor(7.0 * 360 / 16))
				{
					neighboursIndicesList = new List<int> { 0, 6, 2, 8 };
				}
				else if (discreteYaw < Math.Floor(9.0 * 360 / 16))
				{
					neighboursIndicesList = new List<int> { 1, 3, 5, 7 };
				}
				else if (discreteYaw < Math.Floor(11.0 * 360 / 16))
				{
					neighboursIndicesList = new List<int> { 2, 0, 8, 6 };
				}
				else if (discreteYaw < Math.Floor(13.0 * 360 / 16))
				{
					neighboursIndicesList = new List<int> { 5, 1, 7, 3 };
				}
				else if (discreteYaw < Math.Floor(15.0 * 360 / 16))
				{
					neighboursIndicesList = new List<int> { 8, 2, 6, 0 };
				}
				else
				{
					neighboursIndicesList = new List<int> { 7, 5, 3, 1 };
				}
				indicesGivenYaw.Add(neighboursIndicesList);
			}
		}
	}
}
