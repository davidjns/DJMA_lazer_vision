using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LazerEyez.GameModel.Game_Objects
{
	/// <summary>
	/// This is a class that can hold all the information we want to track
	/// Later we may want to add funcitonality for multiple players
	/// </summary>
	public class Game_Stats
	{
		//currently it only has enemies killed, 
		//but we could add things like time spent looking at enemies
		//vs time spent looking away. 

		private int kills;
		/// <summary>
		/// Represents the number of enemies destroyed
		/// </summary>
		public int Kills
		{
			get { return kills; }
			set { kills = value; }
		}
		



	}
}
