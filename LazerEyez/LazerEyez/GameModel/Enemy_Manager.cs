using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using LazerEyez.GameModel.Game_Objects;

namespace LazerEyez.GameModel
{
	public class Enemy_Manager
	{
		/*** this shouldn't be here forever*/
		public List<Enemy> enemies;
		Stopwatch Last_Generation;
		private static int Max_On_Screen = 5;
		private TimeSpan Generation_Charge_Time;

		public static Vector3 spawn_pt = new Vector3(0f, 0f, -10f);
		private Weapon Laser_Ref;
		//Difficulty measuer /level index?

		public Enemy this[int index]
		{
			get
			{
				if (index >= 0 && index < enemies.Count)
					return enemies.ElementAt(index);
				else
					throw new IndexOutOfRangeException("That index is out of range for enemies");
			}
			set	
			{ 
				throw new NotImplementedException("Only the enemy manager can create or set new Enemies");
			}
			
		}

		public Enemy_Manager(Weapon laser_)
		{
			Laser_Ref = laser_;
			enemies = new List<Enemy>();
			Last_Generation = new Stopwatch();
            Generation_Charge_Time = new TimeSpan(0, 0, 0, 3, 0);
		}

		private void Control_Enemy_Movement()
		{
			//Essentially this will be where any complicated movement features
			// are controlled. For now this is irrelevant, but this is where
			// to put grouping, and path changing etc in the future.

			//Right now heavy lifting is in the enemy class
            Last_Generation.Start();
		}

		/// <summary>
		/// Ensures that there are at most three enemies, and that they
		/// are only generated every few seconds
		/// </summary>
		private void Manage_Enemy_Count()
		{
			if (enemies.Count < Max_On_Screen 
			   && Last_Generation.Elapsed.Seconds > Generation_Charge_Time.Seconds)
			{
				Enemy new_e = new Enemy(spawn_pt, new Vector3(10,10,10));
				List<Vector3> my_path = new List<Vector3>();
                my_path.Add(new Vector3(-10, 0, -10));
                my_path.Add(new Vector3(0, 7, -10));
                my_path.Add(new Vector3(10, 0, -10));
                my_path.Add(new Vector3(0, -7, -10));
				//new_e.Path = new PointPath(new Vector3(-10,0,-10), new Vector3(10,0,-10));
                new_e.Path = new PointPath(my_path);
                new_e.Path.Circular_Path = true;
				enemies.Add(new_e);
				Game_Model.Instance.Add_Game_Object(new_e);
				Last_Generation.Reset();
				Last_Generation.Start();
			}

		}

		private void Update_Health()
		{
			foreach (Enemy alien in enemies)
			{
				if (Laser_Ref.targeting(alien)){
					alien.take_damage(Laser_Ref.calculate_damage());
				}
			}
		}

		private void Handle_Enemy_Specific_Collistion_Detection()
		{
			//This function is more of a placeholder concept for now
			// that will likely be added to later.
		}

		/// <summary>
		/// This function runs through the list of enemies and removes the 
		/// dead ones. As it does so it will do whatever is neccesary for creating
		/// their death effects. explosions maybe.
		/// </summary>
		private void Remove_Corpses()
		{
			foreach(Enemy alien in enemies.Reverse<Enemy>())
			{
				if (!alien.Alive)
				{
					enemies.Remove(alien);
					//%%% MUST REMOVE FROM GAME MODEL
					Game_Model.Instance.Stats_p1.Kills++;
					//Do explosion stuff here to, whatever that entails
				}
			}
		}



		/// <summary>
		/// Handles creating, destroying, moving, and collision detection of 
		/// enemies
		/// </summary>
		public void Update()
		{
			Manage_Enemy_Count();

			Control_Enemy_Movement();

			Update_Health();

			Handle_Enemy_Specific_Collistion_Detection();

			Remove_Corpses();

			foreach (Enemy zurg in enemies)
			{
				zurg.Update();
			}
			
		}
	}
}
