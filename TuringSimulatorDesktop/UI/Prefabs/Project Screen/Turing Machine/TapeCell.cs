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
    public class TapeCell : IVisualElement, ICanvasInteractable
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

        public void SetProjectionMatrix(Matrix projectionMatrix, Matrix inverseProjectionMatrix)
        {
            Seperator.SetProjectionMatrix(projectionMatrix, inverseProjectionMatrix);
            InputOutputLabel.SetProjectionMatrix(projectionMatrix, inverseProjectionMatrix);
        }

        public Icon Seperator;
        public InputBox InputOutputLabel;

        public static int ReferenceWidth = 91;
        public static int ReferenceHeight = 90;

        public TapeCell(ActionGroup Group)
        {
            Seperator = new Icon(GlobalInterfaceData.Scheme.NonInteractableAccent);
            InputOutputLabel = new InputBox(Group);
            InputOutputLabel.OutputLabel.DrawCentered = true;
            InputOutputLabel.BackgroundColor = GlobalInterfaceData.Scheme.DarkInteractableAccent;

            Bounds = new Point(ReferenceWidth, ReferenceHeight);
        }

        void MoveLayout()
        {
            InputOutputLabel.Position = position;
            Seperator.Position = InputOutputLabel.Position + new Vector2(InputOutputLabel.Bounds.X, 0);
        }

        void ResizeLayout()
        {
            InputOutputLabel.Bounds = new Point(bounds.X - 1, bounds.Y);            
            Seperator.Bounds = new Point(1, bounds.Y);
        }

        public void Draw(Viewport? BoundPort = null)
        {
            if (IsActive)
            {
                InputOutputLabel.Draw(BoundPort);
                //Background.Draw(BoundPort);
                Seperator.Draw(BoundPort);
            }
        }
    }
}
