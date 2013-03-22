using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace LazerEyez.GameModel.Game_Objects
{
	public class Enemy : Game_Object
	{
		private static float line_speed = 0.01f;

		//This might belong in game object 
		//-- Buildings have health because you can destroy them
		private Health life;

		private PointPath path;

		public PointPath Path
		{
			get
			{
				return path;
			}
			set
			{
				path = value;
			}
		}

		public Enemy(Vector3 pos_) : this(pos_, new Vector3(1,1,1), new Quaternion())  {}
		public Enemy(Vector3 pos_, Vector3 scale_) : this(pos_, scale_, new Quaternion()) { }
		public Enemy(Vector3 pos_, Vector3 scale_, Quaternion quat_) 
			: base(pos_, scale_, quat_)
		{
			Model_Name = "alien";
			life = new Health();
			path = new PointPath(Enemy_Manager.spawn_pt);
			//Essentially a dummy path - should be rewritten
		}

		public void take_damage(float hit)
		{
            hit = float.MaxValue;
			life.minusHealth(hit);
			if(life.getStatus() == HealthStatus.DEAD){
				Alive = false;
			}
		}

		//Movement is complcated
		//Current basic types: Point Path, Orbit, and Flocking
		//Starting with only implementing Point Path

		/// <summary>
		/// Used to change acceleration and velocity vectors appropriately
		/// Eventually may involve more complications and state Machines
		/// </summary>
		private void calculate_movement()
		{
			if (path.Single_Point)
				Position = path.get_next_target(); //teleport it to it's spot

		
			//Will change small path eventually
			if (path.Long_Path || path.Small_Path)
			{
				if (path.distance_to_target(Position) < 0.1)
				{
					path.get_next_target();
				}
				Vector3 sight_line = path.get_current_target() - Position;
                sight_line.Normalize();
				Velocity = sight_line * line_speed;
			}
		}


		public override void Update()
		{
			calculate_movement();           
			
			//Base class game_object just uses accel and velocity to move this
			base.Update();
		}




		public override void render() { 
			//Need to work with view on this one
			//Thinking possibly toss them a model
			//Would involve loading models everywhere though...messy
		}
	}


	public class PointPath
	{
		private List<Vector3> path;
		//if only one point, it is stored in left (INVARIANT)
		private Vector3 left;
		private Vector3 right;
		private bool single_pt;
		private bool circular_path;

		//I don't like this pointer implementation, but it is simple for now
		//Would prefer to upgrade to pointers or something like it later
		private int current_pt;


		#region Public Properties

		//Single_Point, Small_Path, and Long path are all mutually exclusive

		/// <summary>
		/// Returns true only when there is only one point in a path.
		/// For either stationary, or "orbiting" enemies
		/// </summary>
		public Boolean Single_Point
		{
			get
			{
				return single_pt;
			}
			set{}
		}
		/// <summary>
		/// Returns true when there are only two points in a path
		/// Representing moving back and forth between them
		/// </summary>
		public Boolean Small_Path
		{
			get
			{
				if (path.Count == 2)
					return true;
				return false;
			}
			set{}
		}
		/// <summary>
		/// Returns true only when there are 3 or more points in the path,
		/// requiring more complex movement handling
		/// </summary>
		public Boolean Long_Path
		{
			get
			{
				if (path.Count > 2)
					return true;
				return false;
			}
			set{}
		}
		/// <summary>
		/// True when the enemy should move from the last point in the path
		/// to the first point, as in a circle. 
		/// Default behavior just reverses direction 
		/// </summary>
		public Boolean Circular_Path
		{
			get
			{
				return circular_path;
			}
			set
			{
				circular_path = value;
			}
		}


		#endregion

		public PointPath(Vector3 origin, Vector3 direction, float length) :
			this(origin, length * (origin + direction)) { }
		public PointPath(Vector3 start, Vector3 end)
		{
			path =new List<Vector3>();
			left = start;
			right = end;
			path.Add(left);
			path.Add(right);
			single_pt = false;
			circular_path = false;

		}

		public PointPath(List<Vector3> _path)
		{
			if(_path.Count == 0)
				throw new Exception("Paths must have a non-zero number of points");
			
			//Happen regardless of size of path (assuming > 0)
			circular_path = false;
			single_pt = false;
			path = _path;
			//if path.Count > 2, The rest of these blocks are sufficent

			if (_path.Count == 2)
			{
				left = _path.First();
				right = _path.Last();
			}
			else if(_path.Count == 1)
			{
				single_pt = true;
				left = _path.First<Vector3>();
				right = Vector3.Zero;
			}

		}

		public PointPath(Vector3 _single)
		{
			left = _single;
			right = Vector3.Zero;
			single_pt = true;
			circular_path = false;
		}

		/// <summary>
		/// gives distance to start point of path
		/// </summary>
		/// <param name="_position"></param>
		/// <returns>float</returns>
		public float distance_to_left(Vector3 _position)
		{
			Vector3 diff = left - _position;
			return diff.Length();
		}
		/// <summary>
		/// gives distance to end point of path
		/// </summary>
		/// <param name="_position"></param>
		/// <returns>float</returns>
		public float distance_to_right(Vector3 _position)
		{
			//So... this might be bad. depends on use so be careful
			//Alternative solution would be to throw an exception
			if(Single_Point)
				return distance_to_left(_position);

			Vector3 diff = right - _position;
			return diff.Length();
		}

		/// <summary>
		/// Moves to the next point in the list, handles looping behavior
		/// </summary>
		/// <returns>Vector3</returns>
		public Vector3 get_next_target()
		{
			if (Single_Point)
				return left;
			
			if (Small_Path)
			{
				if (current_pt == 0)
				{
					current_pt = 1;
					return right;
				}
				else
				{
					current_pt = 0;
					return left;
				}
			}
			

			//Only gets here when Long Path is true
			if (current_pt == path.Count - 1) //End of the list
			{
				if (Circular_Path)  //reset the loop, currently at start -1
					current_pt = -1;
				else
				{//If it is a "linear" path the go back the way you came
					path.Reverse();
					current_pt = 0;
				}
			}
			//Always advance to next point and return it
			current_pt++;
			return path[current_pt];
		}

		/// <summary>
		/// Gives the distance to the current target point in the path
		/// </summary>
		/// <param name="_position"></param>
		/// <returns>float</returns>
		public float distance_to_target(Vector3 _position)
		{
			float diff;

			if (Single_Point)
				diff = distance_to_left(_position);

			if (Small_Path)
			{
				if (current_pt == 0)
					diff = distance_to_left(_position);
				else
					diff = distance_to_right(_position);
			}
			else
			{
				diff = Vector3.Subtract(path[current_pt], _position).Length();

			}

			return diff;
		}

		public Vector3 get_current_target()
		{
			if (Small_Path)
			{
				if (current_pt == 0)
					return left;
				return right;
			}

			if (Long_Path)
				return path[current_pt];

			//Will only get here if is single Point
			return left;
		}





	}
}
