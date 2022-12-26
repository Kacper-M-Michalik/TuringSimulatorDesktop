using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuringSimulatorDesktop.Input
{
    public class ActionGroup
    {
        public bool IsActive;
        public bool IsMarkedForDeletion;

        public int X, Y, Width, Height;
        
        public List<IPollable> PollableObjects;
        public List<IClickable> ClickableObjects;

        public ActionGroup()
        {
            IsActive = true;

            PollableObjects = new List<IPollable>();
            ClickableObjects = new List<IClickable>();
        }
        public ActionGroup(int SetX, int SetY, int SetWidth, int SetHeight)
        {
            IsActive = true;

            PollableObjects = new List<IPollable>();
            ClickableObjects = new List<IClickable>();

            X = SetX;
            Y = SetY;
            Width = SetWidth;
            Height = SetHeight;
        }

        public bool IsMouseInBounds()
        {
            return (InputManager.MouseData.X > X && InputManager.MouseData.X < X + Width && InputManager.MouseData.Y > Y && InputManager.MouseData.Y < Y + Height);
        }
    }
}
