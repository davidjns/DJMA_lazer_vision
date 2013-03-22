using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LazerEyez.GameModel.Game_Objects
{
    public class Player : Game_Object
    {
        private bool shooting;
        //Needs to know 
        //Am I shooting?
        //Am I being hit by an enemy right now.
        //
    

        public Player() {
            Model_Name = "hero";
        }

        public override void Update()
        {
            base.Update();
            throw new NotImplementedException();
        }


        public override void render() { 
     
        
        }
    }
}
