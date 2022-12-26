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
    public enum ElementCreateType {Persistent, Regular}
    public delegate void OnClickButton(OldButton Sender);

    public class OldButton : UIElement, IClickable
    {
        UIMesh MeshData;

        public string Text;
        public bool HighlightOnMouseOver;

        int BoundRight;
        int BoundDown;

        public event OnClickButton ClickEvent;

        public OldButton(Vector2 SetPosition, UIMesh SetMeshData, ActionGroup Group, string SetText = "")
        {
            Position = SetPosition;

            MeshData = SetMeshData;

            Text = SetText;

            BoundRight = Convert.ToInt32(MathF.Round(MeshData.GetFurthestRightVertexPoint(), MidpointRounding.AwayFromZero));

            BoundDown = Convert.ToInt32(MathF.Round(MeshData.GetFurthestDownVertexPoint(), MidpointRounding.AwayFromZero)); ;

            Group.ClickableObjects.Add(this);
        }

        void IClickable.Clicked()
        {
            ClickEvent?.Invoke(this);
        }

        void IClickable.ClickedAway()
        {

        }

        public bool IsMouseOver()
        {
            return (InputManager.LeftMousePressed && InputManager.MouseData.X >= Position.X && InputManager.MouseData.X <= Position.X + BoundRight && InputManager.MouseData.Y >= Position.Y && InputManager.MouseData.Y <= Position.Y + BoundDown);
        }

    }
}
