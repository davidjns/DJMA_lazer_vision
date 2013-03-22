using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;

namespace LazerEyez.GameModel.Game_Objects
{
    public class Weapon : Game_Object
    {
       //We need screen coordinates to find an enemy to shoot
        public void find_target(Vector2 screen_location)
        {
            int screen_height = Game_Model.Instance.g_view.height;
            int screen_width = Game_Model.Instance.g_view.width;
            
            crosshair = new Vector3((screen_location.X - screen_width / 2) / 80, (-screen_location.Y + screen_height / 2) / 80, -1);
            Crosshair2D = screen_location;
        }

        public bool targeting(Game_Object possible_target){
            int dummy;
            double f = Math.Pow(Math.Pow((crosshair.X - possible_target.Position.X),2) + Math.Pow((crosshair.Y - possible_target.Position.Y),2), 0.5);
            Debug.WriteLine("Translated Crosshair X: " + crosshair.X + "  Translated Crosshair Y: " + crosshair.Y);
            Debug.WriteLine("Enemy X: " + possible_target.Position.X + "Enemy Y: " + possible_target.Position.Y);
            //return (f < possible_target.Size.X);
            if (f < 2)
                dummy = 0;
            return (f < 2);
         }

        public float calculate_damage()
        {
            return Game_Model.Instance.Time_Step.Milliseconds * payload; 
        }
        //pub void Launch(velocity, direction)

        private Vector3 target;
        private int payload;    //Damage over time or instant??
        private Ray beam;

        //%%%%
        public Vector2 Crosshair2D;
        public Vector3 crosshair;
    }
}
