using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using LazerEyez.GameModel.Game_Objects;
using LazerEyez.GameModel;
//This is the namespace of the camera component
using Dhpoware;

namespace LazerEyez.View
{
    class Game_View
    {
        //Need to make this just like winter wars
        private GraphicsDeviceManager graphics;
        //private CameraComponent cam
        private SortedDictionary<String, Model> model_dictionary;
        private ContentManager content;
        private CameraComponent camera;
        public int height;
        public int width;
        

        public Game_View(Game _game, GraphicsDeviceManager graphics_, ContentManager content_)
        {
            graphics = graphics_;
            content = content_;

            model_dictionary = new SortedDictionary<string, Model>();
            camera = new CameraComponent(_game);
            camera.Perspective(90, 4 / 3, 2f, 5000);
            camera.Position = new Vector3(100, 100, 100);            
           // camera.LookAt(Game_Model.Instance.mothership[0].Position);
           //camera.OrbitTarget = Game_Model.Instance.mothership[0].Position;

            //These don't work as yet
            camera.MapActionToKey(CameraComponent.Actions.MoveDownPrimary, Microsoft.Xna.Framework.Input.Keys.Down);
            camera.MapActionToKey(CameraComponent.Actions.MoveUpPrimary, Microsoft.Xna.Framework.Input.Keys.Up);
            camera.MapActionToKey(CameraComponent.Actions.FlightYawLeftPrimary, Microsoft.Xna.Framework.Input.Keys.Left);
            camera.MapActionToKey(CameraComponent.Actions.FlightYawRightPrimary, Microsoft.Xna.Framework.Input.Keys.Right);
            camera.MapActionToKey(CameraComponent.Actions.MoveForwardsPrimary, Microsoft.Xna.Framework.Input.Keys.W);
            camera.MapActionToKey(CameraComponent.Actions.MoveBackwardsPrimary, Microsoft.Xna.Framework.Input.Keys.S);
            camera.MapActionToKey(CameraComponent.Actions.YawLeftPrimary, Microsoft.Xna.Framework.Input.Keys.A);
            camera.MapActionToKey(CameraComponent.Actions.YawRightPrimary, Microsoft.Xna.Framework.Input.Keys.D);
            //apectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;

            height = graphics.GraphicsDevice.Viewport.Height;
            width = graphics.GraphicsDevice.Viewport.Width;
            
        }

        public void define_dictionary()
        {
            model_dictionary.Add("alien", content.Load<Model>("Models\\alien"));
            model_dictionary.Add("hero", content.Load<Model>("Models\\hero"));
        }

        public void draw_string(String str)
        {
            //Do this later

        }



        public void draw(List<Game_Object> objs)
        {
            //add stuff to this ( main loop)
            //Debug.Print("" + camera.Position);


            foreach (Game_Object go in objs)
            {
                Model model;
                if(!model_dictionary.TryGetValue(go.Model_Name, out model))
                    throw new Exception("Model Name " + go.Model_Name + "doesn't exist");


                Matrix[] transforms = new Matrix[model.Bones.Count];
                model.CopyAbsoluteBoneTransformsTo(transforms);

                foreach(ModelMesh mesh in model.Meshes)
                {
                    foreach(BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.World = transforms[mesh.ParentBone.Index] *
                            Matrix.CreateScale(go.Size) * Matrix.CreateTranslation(go.Position);

                        effect.View = camera.ViewMatrix;
                        effect.Projection = camera.ProjectionMatrix;
                    }
                    mesh.Draw();
                }
            }


        }

    }
}
