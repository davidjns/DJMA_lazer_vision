using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using LazerEyez.GameModel.Game_Objects;
using LazerEyez.View;

namespace LazerEyez.GameModel
{

	//Singleton
	class Game_Model
	{
		
		//Weapons container
		//Player
		//Buildings container

		private TimeSpan clock;
		private TimeSpan countdown;
		//Multiple lasers eventually?
		private List<Weapon> weapons;
		private List<Environment_GM> level;
		private Player player;
		public Weapon beam;	//The laser from the player
		public Enemy_Manager mothership;	//Enemy container abstraction
		//%%% PUBLIC FOR TESTING

		public Game_View g_view;


		//Not so sure of using this since we have an enemy manager
		private List<Game_Object> all_things;	//Every object in the Model is in here
		//Should be changed to a hashset

		public Game_Stats stats_p1;


		private Vector2 crosshair_location{get; set;}

		#region Public Properties
		/// <summary>
		/// Abstraction for a data structure to hold information about
		/// the player. Updated by anything
		/// </summary>
		public Game_Stats Stats_p1
		{
			get
			{
				return stats_p1;
			}
			set
			{
				stats_p1 = value;
			}
		}
		/// <summary>
		/// Gives the TimeSpan since the last frame. 
		/// XNA attempts to hold this to about 16ms
		/// </summary>
		/// Use this for any calculations involving time
		public TimeSpan Time_Step
		{
			get
			{
				return clock;
			}
			set
			{
				clock = value;
			}
		}
		/// <summary>
		/// Time remaining in the Current game
		/// </summary>
		public TimeSpan Remaining_Time
		{
			get
			{
				return countdown;
			}
			private set
			{
				countdown = value;
			}
		}

		#endregion


		private Game_Model()
		{
			//Initialzie stuff here
			clock = new TimeSpan();
			countdown = new TimeSpan(0, 5, 0);
			weapons = new List<Weapon>();
			level = new List<Environment_GM>();
			player = new Player();
			beam = new Weapon();
			mothership = new Enemy_Manager(beam);

			all_things = new List<Game_Object>();
			stats_p1 = new Game_Stats();
			
		}

		//part of singletong partern
		//represents actual game model
		private static Game_Model instance;
		/// <summary>
		/// Use this to get access to all the public funtions of the model
		/// This is the only model that is ever present
		/// </summary>
		public static Game_Model Instance
		{
			get
			{
				if (instance == null)
					instance = new Game_Model();
				return instance;
			}
		}


		#region add game objects

		public void Add_Game_Object(Weapon w)
		{
			weapons.Add(w);
			all_things.Add(w);
		}
		public void Add_Game_Object(Enemy e)
		{
			all_things.Add(e);
		}
		public void Add_Game_Object(Player p)
		{
			player = p;
			all_things.Add(p);
		}
		public void Add_Game_Object(Environment_GM e)
		{
			level.Add(e);
			all_things.Add(e);
		}
		#endregion

		
		


		private void Check_for_Collisions()
		{
			foreach (Game_Object go1 in all_things)
			{
				foreach (Game_Object go2 in all_things)
				{
					if (go1.collides(go2.Body))
					{
						//Handle collisions
						//This is needs conceptual work
					}
				}
			}
		}


		public void Create_View(Game game_, GraphicsDeviceManager graphics_, ContentManager content_)
		{
			g_view = new Game_View(game_, graphics_, content_);

		}

		public void Load_Content()
		{
			g_view.define_dictionary();
		}

		public void Draw()
		{
			g_view.draw(all_things);
		}

		public void Update_Crosshair(Vector2 crosshair_loc_)
		{
			crosshair_location = crosshair_loc_;
		}

		public void Update()
		{
			try
			{
				//timer updates
				Remaining_Time -= Time_Step;
				//Do game logic here

				//Movement - of non-enemies (Effects?)
				
				//Eventuatlly
				//Check_for_Collisions();  - of what?

				//Check/deploy laser
				beam.find_target(crosshair_location);

				//Enemey Manager Update
				mothership.Update();
			}
			catch (Exception e)
			{
				//Catch all -- this is definitely not the best way to handle it
				//Prefere to have our catch blocks in game model, and
				//only the back up catch all in Game1
				Debug.Print(e.Message);
				//carries on to next update
			}

		}

	}
}
