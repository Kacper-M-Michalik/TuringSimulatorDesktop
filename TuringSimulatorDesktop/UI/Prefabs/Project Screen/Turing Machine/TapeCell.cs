using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringSimulatorDesktop.Input;

namespace TuringSimulatorDesktop.UI.Prefabs
{
    public class TapeCell : IVisualElement
    {
        Vector2 position;
        public Vector2 Position
        {
            get => position;
            set
            {
                position = value;
                MoveLayout();
            }
        }

        Point bounds;
        public Point Bounds
        {
            get => bounds;
            set
            {
                bounds = value;
                ResizeLayout();
            }
        }

        bool isActive;
        public bool IsActive
        {
            get => isActive;
            set
            {
                isActive = value;
            }
        }

        public Icon Seperator;
        public InputBox InputOutputLabel;

        public TapeCell(ActionGroup Group)
        {
            Seperator = new Icon(GlobalInterfaceData.Scheme.NonInteractableAccent);
            InputOutputLabel = new InputBox(Group);
        }

        void MoveLayout()
        {
            InputOutputLabel.Position = position;
            Seperator.Position = InputOutputLabel.Position + new Vector2(1, 0);
        }

        void ResizeLayout()
        {
            InputOutputLabel.Bounds = bounds;            
            Seperator.Bounds = new Point(1, bounds.Y);
        }

        public void Draw(Viewport? BoundPort = null)
        {
            if (IsActive)
            {
                //Background.Draw(BoundPort);
                Seperator.Draw(BoundPort);
                InputOutputLabel.Draw(BoundPort);
            }
        }
    }
}
