using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using TuringSimulatorDesktop.Input;

namespace TuringSimulatorDesktop.UI
{
    public delegate void OnClick(Button Sender);
    public enum ElementCreateType {Persistent, Regular}

    public class Button : UIElement, IClickable
    {
        public string Text;

        public int BoundRight;
        public int BoundDown;

        public event OnClick ClickEvent;

        public Button(Vector2 SetPosition, Mesh SetMeshData, ElementCreateType Type = ElementCreateType.Regular, string SetText = "")
        {
            Position = SetPosition;

            MeshData = SetMeshData;

            Text = SetText;

            BoundRight = Convert.ToInt32(MathF.Round(MeshData.GetFurthestRightVertexPoint(), MidpointRounding.AwayFromZero));

            BoundDown = Convert.ToInt32(MathF.Round(MeshData.GetFurthestDownVertexPoint(), MidpointRounding.AwayFromZero)); ;

            if (Type == ElementCreateType.Regular) InputManager.RegisterClickableObjectOnQueue(this);
            else InputManager.RegisterClickableObjectOnQueuePersistent(this);
        }

        bool IClickable.Clicked()
        {
            ClickEvent?.Invoke(this);
            return false;
        }

        void IClickable.ClickedAway()
        {

        }

        public bool IsMouseOver()
        {
            return (InputManager.LeftMousePressed && InputManager.MouseData.X >= Position.X && InputManager.MouseData.X <= Position.X + BoundRight && InputManager.MouseData.Y >= Position.Y && InputManager.MouseData.Y <= Position.Y + BoundDown);
        }

        public override Vector2 GetBounds()
        {
            return new Vector2(BoundRight, BoundDown);
        }
    }
}
