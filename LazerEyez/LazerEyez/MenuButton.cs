using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace LazerEyez
{
    class MenuButton
    {
        public bool selected = false;
        public string text = "Menu Button";
        public Rectangle area;
        public Texture2D buttonBG;

        public MenuButton(Texture2D buttonBackground, Rectangle Area, string Text)
        {
            buttonBG = buttonBackground;
            area = Area;
            text = Text;
        }
        public void MouseOver(MouseState clicked)
        {
            if (clicked.X > area.X && clicked.X < (area.X + area.Width)
                && clicked.Y > area.Y && clicked.Y < (area.Y + area.Height))
                selected = true;
            else
                selected = false;
        }



        public void Draw(SpriteBatch spriteBatch, SpriteFont font, Texture2D buttonBackground)
        {

            buttonBG = buttonBackground;
            spriteBatch.Draw(buttonBG, new Vector2(area.X, area.Y), new Rectangle(area.X, area.Y, area.Width, area.Height), Color.White);
            if(selected == false)
                spriteBatch.DrawString(font, text, new Vector2(area.X, area.Y), Color.OrangeRed);
            if(selected == true)
                spriteBatch.DrawString(font, text, new Vector2(area.X, area.Y), Color.DarkOrange);
            
            
         }

        
    }
}
