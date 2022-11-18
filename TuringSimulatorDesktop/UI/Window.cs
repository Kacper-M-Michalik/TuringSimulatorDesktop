using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringSimulatorDesktop.Input;

namespace TuringSimulatorDesktop.UI
{
    public class Window : IClickable
    {
        public Vector2 Position;

        public List<UIElement> Views;
        public List<Button> ViewButtons;

        UIElement CurrentView;

        RenderTarget2D TabTexture;
        RenderTarget2D ViewTexture;

        int Width { get { return ViewTexture.Width; } }
        int Height { get { return TabTexture.Height + ViewTexture.Height; } }

        public Window(int SetWidth, int SetHeight)
        {
            TabTexture = new RenderTarget2D(GlobalGraphicsData.Device, SetWidth, 20);
            ViewTexture = new RenderTarget2D(GlobalGraphicsData.Device, SetWidth, SetHeight);

            Views = new List<UIElement>();
            ViewButtons = new List<Button>();

            InputManager.AllClickableObjects.Add(this);
        }

        //created list of button visual elements, let them deal with setting active view?

        public void Update()
        {
            //deal with assigning active view
            //call update of view here?
        }
        
        public void Draw(SpriteBatch Batch)
        {
            GlobalGraphicsData.Device.SetRenderTarget(TabTexture);
            GlobalGraphicsData.Device.Clear(Color.White);

            for (int i = 0; i < ViewButtons.Count; i++)
            {
                ViewButtons[i].Draw(Batch, TabTexture);
            }

            GlobalGraphicsData.Device.SetRenderTarget(ViewTexture);
            GlobalGraphicsData.Device.Clear(Color.Black);

            if (CurrentView != null) CurrentView.Draw(Batch, ViewTexture);

            Batch.Draw(TabTexture, Position, Color.White);
            Batch.Draw(ViewTexture, new Vector2(Position.X, Position.Y + TabTexture.Height), Color.White);
        }

        public void AddView()
        {

        }

        public bool IsMouseOverWindow()
        {
            if (InputManager.MouseData.Y < ((int)Position.Y + Height) && InputManager.MouseData.Y > (int)Position.Y && InputManager.MouseData.X < ((int)Position.X + Width) && InputManager.MouseData.X > (int)Position.X) return true;
            return false;
        }

        public bool IsMouseOverTab()
        {
            if (InputManager.MouseData.Y < ((int)Position.Y + TabTexture.Height) && InputManager.MouseData.Y > (int)Position.Y && InputManager.MouseData.X < ((int)Position.X + Width) && InputManager.MouseData.X > (int)Position.X) return true;
            return false;
        }

        public void Clicked()
        {
            //intercept and send off to 
        }

        public bool IsMouseOver()
        {
            return IsMouseOverWindow();
        }
    }
}
