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
        public bool IsActive = true;
        public bool IsMarkedForDeletion;
        public bool IsPersistant;

        public int X, Y, Width, Height;
        
        public List<IPollable> PollableObjects = new List<IPollable>();
        public List<IClickable> ClickableObjects = new List<IClickable>();

        public ActionGroup()
        {
        }
        public ActionGroup(int SetX, int SetY, int SetWidth, int SetHeight)
        {
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
