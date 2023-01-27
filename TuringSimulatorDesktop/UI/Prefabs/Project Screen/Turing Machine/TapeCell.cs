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
            //might not wanan draw like this tough
            Seperator.SetProjectionMatrix(projectionMatrix, inverseProjectionMatrix);
            InputOutputLabel.SetProjectionMatrix(projectionMatrix, inverseProjectionMatrix);
            IndexLabel.SetProjectionMatrix(projectionMatrix, inverseProjectionMatrix);
        }

        public Icon Seperator;
        public InputBox InputOutputLabel;
        public Label IndexLabel;

        public TapeVisualItem OwnerTape;

        public int Index;

        public static int ReferenceCellWidth = 90;
        public static int ReferenceTotalWidth = 91;
        public static int ReferenceCellHeight = 90;
        public static int ReferenceTotalHeight = 120;

        public TapeCell(TapeVisualItem ownerTape, ActionGroup Group)
        {
            Seperator = new Icon(GlobalInterfaceData.Scheme.NonInteractableAccent);

            InputOutputLabel = new InputBox(Group);
            InputOutputLabel.OutputLabel.DrawCentered = true;
            InputOutputLabel.BackgroundColor = GlobalInterfaceData.Scheme.DarkInteractableAccent;            
            InputOutputLabel.EditEvent += UpdatedCell;

            IndexLabel = new Label();
            IndexLabel.FontSize = 14f;
            IndexLabel.FontColor = GlobalInterfaceData.Scheme.FontGrayedOutColor;
            IndexLabel.Text = "";

            Bounds = new Point(ReferenceTotalWidth, ReferenceTotalHeight);
            OwnerTape = ownerTape;
        }

        public void UpdatedCell(InputBox Sender)
        {
            OwnerTape.UpdateTapeContents(Index, InputOutputLabel);
        }

        void MoveLayout()
        {
            InputOutputLabel.Position = position;
            Seperator.Position = InputOutputLabel.Position + new Vector2(InputOutputLabel.Bounds.X, 0);
            IndexLabel.Position = Position + new Vector2(35, 110);
        }

        void ResizeLayout()
        {
            InputOutputLabel.Bounds = new Point(ReferenceCellWidth, ReferenceCellHeight);            
            Seperator.Bounds = new Point(1, ReferenceCellHeight);
        }

        public void Draw(Viewport? BoundPort = null)
        {
            if (IsActive)
            {
                InputOutputLabel.Draw(BoundPort);
                //Background.Draw(BoundPort);
                Seperator.Draw(BoundPort);
                IndexLabel.Draw(BoundPort);
            }
        }

        public void Close()
        {
            InputOutputLabel.IsMarkedForDeletion = true;
            IsActive = false;
        }
    }
}
