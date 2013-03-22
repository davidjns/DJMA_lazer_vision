using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.FaceTracking;
using LazerEyez.GameModel;
using LazerEyez.GameModel.Game_Objects;


namespace LazerEyez
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Game1 : Microsoft.Xna.Framework.Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
        KinectSensor kinectSensor;
        FaceTracker faceTracker;
        private byte[] colorPixelData;
        private short[] depthPixelData;
        private Skeleton[] skeletonData;
        float yaw, pitch;
        EnumIndexableCollection<FeaturePoint, PointF> facePoints;
        Dictionary<string, Texture2D> tex;
        float topOfTheScreen;
        float leftOfScreen;
        float screenWidth;
        float screenHeight;
        float prevPitch = 0;
        float prevYaw = 0;
        float xThresh = 10;
        float yThresh = 7;
        float xOffset = -4;
        float yOffset = 7;

		public Enemy Number_One;


		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
            screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = (int)screenHeight;
            graphics.PreferredBackBufferWidth = (int)screenWidth;
			Content.RootDirectory = "Content";
            // For a KinectSensor to be detected, we can plug it in after the application has been started.
            KinectSensor.KinectSensors.StatusChanged += KinectSensors_StatusChanged;
            // Or it's already plugged in, so we will look for it.
            var kinect = KinectSensor.KinectSensors.FirstOrDefault(k => k.Status == KinectStatus.Connected);
            if (kinect != null)
            {
                OpenKinect(kinect);
            }
		}

        void KinectSensors_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            if (e.Status == KinectStatus.Connected)
            {
                OpenKinect(e.Sensor);
            }
        }

        private void OpenKinect(KinectSensor newSensor)
        {
            kinectSensor = newSensor;

            // Initialize all the necessary streams:
            // - ColorStream with default format
            // - DepthStream with Near mode
            // - SkeletonStream with tracking in NearReange and Seated mode.

            kinectSensor.ColorStream.Enable();

            kinectSensor.DepthStream.Range = DepthRange.Near;
            kinectSensor.DepthStream.Enable(DepthImageFormat.Resolution80x60Fps30);

            kinectSensor.SkeletonStream.EnableTrackingInNearRange = true;
            kinectSensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
            kinectSensor.SkeletonStream.Enable(
                new TransformSmoothParameters()
                {
                    Correction = 0.75f,
                    JitterRadius = 1f,
                    MaxDeviationRadius = 0.00f,
                    Prediction = 0.05f,
                    Smoothing = 0.04f
                });

            // Listen to the AllFramesReady event to receive KinectSensor's data.
            kinectSensor.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(kinectSensor_AllFramesReady);

            // Initialize data arrays
            colorPixelData = new byte[kinectSensor.ColorStream.FramePixelDataLength];
            depthPixelData = new short[kinectSensor.DepthStream.FramePixelDataLength];
            skeletonData = new Skeleton[6];

            // Starts the Sensor
            kinectSensor.Start();

            // Initialize a new FaceTracker with the KinectSensor
            faceTracker = new FaceTracker(kinectSensor);
        }

        void kinectSensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            // Retrieve each single frame and copy the data
            ColorImageFrame colorImageFrame = e.OpenColorImageFrame();

            if (colorImageFrame == null)
                return;
            colorImageFrame.CopyPixelDataTo(colorPixelData);


            DepthImageFrame depthImageFrame = e.OpenDepthImageFrame();

            if (depthImageFrame == null)
                return;
            depthImageFrame.CopyPixelDataTo(depthPixelData);



            SkeletonFrame skeletonFrame = e.OpenSkeletonFrame();

            if (skeletonFrame == null)
                return;
            skeletonFrame.CopySkeletonDataTo(skeletonData);


            // Retrieve the first tracked skeleton if any. Otherwise, do nothing.
            var skeleton = skeletonData.FirstOrDefault(s => s.TrackingState == SkeletonTrackingState.Tracked);
            if (skeleton == null)
                return;

            // Make the faceTracker processing the data.
            FaceTrackFrame faceFrame = faceTracker.Track(kinectSensor.ColorStream.Format, colorPixelData,
                                              kinectSensor.DepthStream.Format, depthPixelData,
                                              skeleton);

            // If a face is tracked, then we can use it.
            if (faceFrame.TrackSuccessful)
            {


                /*var head = skeleton.Joints[JointType.Head];
                var depthImagePoint = depthImageFrame.MapFromSkeletonPoint(head.Position);
                facePoints = faceFrame.GetProjected3DShape();
                PointF center = facePoints[87];
                topOfTheScreen = (float)Math.Atan(0.2032/(depthImagePoint.Depth/1000.0));*/
                //^^depth functions were not working very well so I tweaked numbers until I got an acceptable threshold

                //-4F and +7F are the rough offsets of kinect data 

                pitch = faceFrame.Rotation.X + xOffset;
                yaw = faceFrame.Rotation.Y + yOffset;

                float pitchMult = 1;
                float yawMult = 1;
                if (Math.Abs(pitch - prevPitch) < Math.Abs(yaw - prevYaw))
                    pitchMult = 1.5F;
                else
                    yawMult = 1.5F;

                //do not accept small fidgets in pitch and yaw, more restrictive for the less move one
                if (Math.Abs(pitch - prevPitch) < .25 * pitchMult)
                    pitch = prevPitch;
                if (Math.Abs(yaw - prevYaw) < .4 * yawMult)
                    yaw = prevYaw;

                //I average the values with their previous value to smooth the animations
                //this will cause decreasing affectivness of turning head as the user goes in one direction.  
                yaw = (yaw + prevYaw) / 2F;
                pitch = (pitch + prevPitch) / 2F;

                prevPitch = pitch;
                prevYaw = yaw;
                //limits on how far left and right they can look, 
                //a ratio of their pitch and yaw over xThresh and yThresh is used to decide screen positions
                if (pitch > yThresh)
                    pitch = yThresh;
                if (yaw > xThresh)
                    yaw = xThresh;
                if (pitch < -yThresh)
                    pitch = -yThresh;
                if (yaw < -xThresh)
                    yaw = -xThresh;

                Debug.WriteLine("pitch: " + pitch + "   yaw: " + yaw);
                //Debug.WriteLine("topPitch:  " + topOfTheScreen);
            }

            colorImageFrame.Dispose();
            depthImageFrame.Dispose();
            skeletonFrame.Dispose();
        }

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			// TODO: Add your initialization logic here

			base.Initialize();
			Number_One = new Enemy(new Vector3(0, 0, 0));

		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		/// 
		//view code
		Model hero;
		Model alien;
		float aspectRatio;
		SoundEffect mySong;
		SoundEffect beepTest;
		SoundEffectInstance songInstance;
		SoundEffectInstance beepInstance;
		Texture2D cityTexture;
		Texture2D crosshair;
		Vector2 spritePosition = Vector2.Zero;
		//end view code
		protected override void LoadContent()
		{
			Game_Model.Instance.Create_View(this, graphics, Content);
			Game_Model.Instance.Load_Content();
			
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);


			mySong = Content.Load<SoundEffect>("Sounds//alpha_song_v2");
			beepTest = Content.Load<SoundEffect>("Sounds//laser-effect-shortened");
			songInstance = mySong.CreateInstance();
			beepInstance = beepTest.CreateInstance();



			//view code

			hero = Content.Load<Model>("Models\\hero");
			alien = Content.Load<Model>("Models\\alien");
			aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;

			cityTexture = Content.Load<Texture2D>("Textures\\city");
			crosshair = Content.Load<Texture2D>("Textures\\crosshair");

			//end view code

			// TODO: use this.Content to load your game content here
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			if (beepInstance.State == SoundState.Stopped)
			{
				songInstance.IsLooped = true;
				beepInstance.IsLooped = true;
				beepInstance.Volume = 1.0f;
				songInstance.Volume = 0.5f;
				beepInstance.Play();
				songInstance.Play();
			}

			else
			{
				beepInstance.Resume();
			}
			try
			{
				//This is the only time that the Model Time should be set
				Game_Model.Instance.Time_Step = gameTime.ElapsedGameTime;

				/*** for now, just getting the position of the mouse for the crosshair*/
                float drawY = (float)((screenHeight / 2F) * (1F - pitch / (float)yThresh));
                float drawX = (float)((screenWidth / 2F) * (1F - yaw / (float)xThresh));
                Vector2 crosshairPosition = new Vector2(drawX, drawY);
				//Vector2 crosshairPosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
				Game_Model.Instance.Update_Crosshair(crosshairPosition);
				Debug.WriteLine("Mouse X coordinate: " + Mouse.GetState().X + "\nMouse Y coordinate: " + Mouse.GetState().Y);
				Game_Model.Instance.Update();

				// Allows the game to exit
				if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
					this.Exit();

				// TODO: Add your update logic here 
					//Should really be in game model, only if neccesary put it here

				base.Update(gameTime);
			}
			catch (Exception e)
			{
				Debug.Print(e.Message);
				//And Continue;				
			}
			finally
			{}
			
		}
		//view code
		Vector3 heroPosition = new Vector3(-5.0f,-3.0f,0.0f);
		List<Enemy> alienList = Game_Model.Instance.mothership.enemies;
		float modelRotation = 0.0f;

		Vector3 cameraPosition = new Vector3(0.0f, 0.0f, 10.0f);

		//end view code

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);
			/*** in here should call View.draw() to draw the appropriate view*/

			//view code
			//drawing the city as the background
			spriteBatch.Begin();
			spriteBatch.Draw(cityTexture, spritePosition, Color.White);
			spriteBatch.End();

			Matrix[] transforms = new Matrix[hero.Bones.Count];
			hero.CopyAbsoluteBoneTransformsTo(transforms);
			//drawing the first hero and making it rotate

			
			foreach(ModelMesh mesh in hero.Meshes)
			{
				foreach(BasicEffect effect in mesh.Effects)
				{
					effect.EnableDefaultLighting();
					effect.World = transforms[mesh.ParentBone.Index] *
						Matrix.CreateRotationY(modelRotation) *
						Matrix.CreateTranslation(heroPosition);
					effect.View = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);
					effect.Projection = Matrix.CreatePerspectiveFieldOfView(
						MathHelper.ToRadians(45.0f), aspectRatio,
						1.0f, 10000.0f);
				}
				mesh.Draw();
			}

			foreach (Enemy myEnemy in alienList)
			{
				foreach (ModelMesh mesh in alien.Meshes)
				{
					foreach (BasicEffect effect in mesh.Effects)
					{

							effect.EnableDefaultLighting();
							effect.World = transforms[mesh.ParentBone.Index] *
								Matrix.CreateRotationY(modelRotation) *
								Matrix.CreateTranslation(myEnemy.Position);
							effect.View = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);
							effect.Projection = Matrix.CreatePerspectiveFieldOfView(
								MathHelper.ToRadians(45.0f), aspectRatio,
								1.0f, 10000.0f);
						}
					mesh.Draw();
					}
					
			}
			
			//end view code

            

			//Crosshair
			spriteBatch.Begin();
            spriteBatch.Draw(crosshair, Game_Model.Instance.beam.Crosshair2D, Color.White);
            
            //Rendering Kill Count String
            SpriteFont gmfnt = Content.Load<SpriteFont>("gamefont");
            String killcnt = 
                "Aliens Destroyed: " + Game_Model.Instance.stats_p1.Kills;
            Vector2 FontMsr = gmfnt.MeasureString(killcnt);
            spriteBatch.DrawString(gmfnt, killcnt, new Vector2(0, 0), Color.Black,
                0, new Vector2(0,0), 0.5f, SpriteEffects.None, 0);
                
            
            
			spriteBatch.End();


			//Game_Model.Instance.Draw();
		
			// TODO: Add your drawing code here

			base.Draw(gameTime);
		}
	}
}
