using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace LazerEyez.GameModel.Game_Objects
{
    public abstract class Game_Object
    {
        //Represents the base class for all objects in our game
        //all functions should be virtual

        private Vector3 position;		// Upper left corner? or center?
		private Vector3 velocity;       // 
		private Vector3 acceleration;	// 		
        private Vector3 scale;			// size box
        private Quaternion rotation;	// determines orientation

        protected BoundingSphere body;
        private string model_name;

        private bool alive;        // True only while object should be in model
                

        #region Public Properties

        public string Model_Name
        {
            get
            {
                return model_name;
            }
            protected set
            {
                model_name = value;
            }
        }


        /// <summary>
        /// The position of the ?top left corner? of this object in 3D space
        /// </summary>
        public Vector3 Position
        {
            get
            {
                return position;
            }
            protected set 
			{
				position = value;
			}
        }
		/// <summary>
		/// Current velocity; applied to object position each frame
		/// </summary>
		public Vector3 Velocity
		{
			get
			{
				return velocity;
			}
			protected set
			{
				velocity = value;
			}
		}
		/// <summary>
		/// Current Accel, applied once an update, only for certain situations
		/// </summary>
		public Vector3 Accel
		{
			get
			{
				return acceleration;
			}
			protected set
			{
				acceleration = value;
			}
		}
		/// <summary>
        /// The XYZ dimensions of the size of this object, 
        /// Size.length() gives corner to corner length
        /// </summary>
        public Vector3 Size
        {
            get
            {
                return scale;
            }
			protected set
			{
				if(scale.Length() > 0)
					scale = value;
			}
        }
		/// <summary>
        /// Represents the actual Collsion body in 3D space of this object
        /// </summary>
        public BoundingSphere Body
        {
            get
            {
                return body;
            }
            protected set
            {//I'll leave this as setable but Objects shouldnr really change body
				body = value;
			}
        }
        /// <summary>
        /// True while the object is in the model, Set this to false and
		/// it will be removed from the game by the next frame
        /// </summary>
		public Boolean Alive
		{
			get
			{
				return alive;
			}
			set
			{//Only allowed to set to false, to enforce consistency
				if (!value)
					alive = false;
			}
		}
		
        #endregion


        public Game_Object() : this(new Vector3(0, 0, 0), new Vector3(1, 1, 1), new Quaternion()) { }
        public Game_Object(Vector3 pos_, Vector3 scale_, Quaternion quat_)
        {
            position = pos_;
			velocity = Vector3.Zero;
			acceleration = Vector3.Zero;
            scale = scale_;
            rotation = quat_;
            body = new BoundingSphere(position, scale_.Length());
			alive = true;      
        }

        public virtual bool collides(BoundingSphere rhs)
        {
            if (body.Intersects(rhs))
                return true;
            return false;
        }

        public virtual void Update()
        {
			position += velocity * Game_Model.Instance.Time_Step.Milliseconds;
			velocity += acceleration * Game_Model.Instance.Time_Step.Milliseconds;

        }


        public virtual void render() { }        
    }
}
