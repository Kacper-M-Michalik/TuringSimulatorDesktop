using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TuringSimulatorDesktop.Input;

namespace TuringSimulatorDesktop.UI
{
    public delegate void OnClick(Button Sender);

    public class Button : ImageElement, IClickable
    {
        public string Text;

        public int BoundLeft;
        public int BoundRight;
        public int BoundUp;
        public int BoundDown;

        public event OnClick Clicked;

        public Button(Texture2D SetSprite, Vector2 SetPosition, string SetText = "") : base(SetSprite, SetPosition)
        {
            if (Sprite.Width % 2 == 1) 
                BoundRight = (Sprite.Width / 2) + 1;
            else 
                BoundRight = (Sprite.Width / 2);

            BoundLeft = Sprite.Width / 2;

            if (Sprite.Height % 2 == 1)
                BoundDown = (Sprite.Height / 2) + 1;
            else
                BoundDown = (Sprite.Height / 2);

            BoundUp = Sprite.Height / 2;

            Text = SetText;

            InputManager.AllClickableObjects.Add(this);
        }
        public Button(Texture2D SetSprite, Vector2 SetPosition, int SetBoundLeft, int SetBoundRight, int SetBoundUp, int SetBoundDown) : base(SetSprite, SetPosition)
        {
            BoundLeft = SetBoundLeft;
            BoundRight = SetBoundRight;
            BoundUp = SetBoundUp;
            BoundDown = SetBoundDown;

            InputManager.AllClickableObjects.Add(this);
        }

        public override void Draw(SpriteBatch OwnerSpriteBatch, RenderTarget2D CurrentRenderTarget)
        {
            base.Draw(OwnerSpriteBatch, CurrentRenderTarget);
            OwnerSpriteBatch.DrawString(GlobalGraphicsData.Font, Text, Position, Color.White);
        }

        void IClickable.Clicked()
        {
            Clicked?.Invoke(this);
        }

        public bool IsMouseOver()
        {
            return (InputManager.LeftMousePressed && InputManager.MouseData.X >= Position.X - BoundLeft && InputManager.MouseData.X <= Position.X + BoundRight && InputManager.MouseData.Y >= Position.Y - BoundUp && InputManager.MouseData.Y <= Position.Y + BoundDown);
        }
    }
}
