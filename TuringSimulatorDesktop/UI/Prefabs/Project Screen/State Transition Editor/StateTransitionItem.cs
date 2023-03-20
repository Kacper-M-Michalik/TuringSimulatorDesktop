using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TuringSimulatorDesktop.Input;

namespace TuringSimulatorDesktop.UI.Prefabs
{
    public class StateTransitionItem: IVisualElement, ICanvasInteractable
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
                return;
            }
        }

        public void SetProjectionMatrix(Matrix projectionMatrix, Matrix inverseProjectionMatrix)
        {
            Background.SetProjectionMatrix(projectionMatrix, inverseProjectionMatrix);
            Arrow.SetProjectionMatrix(projectionMatrix, inverseProjectionMatrix);
            CurrentStateTextBox.SetProjectionMatrix(projectionMatrix, inverseProjectionMatrix);
            TapeValueTextBox.SetProjectionMatrix(projectionMatrix, inverseProjectionMatrix);
            NewStateTextBox.SetProjectionMatrix(projectionMatrix, inverseProjectionMatrix);
            NewTapeValueTextBox.SetProjectionMatrix(projectionMatrix, inverseProjectionMatrix);
            MoveDirectionTextBox.SetProjectionMatrix(projectionMatrix, inverseProjectionMatrix);
        }

        public bool IsActive { get; set; } = true;

        public ColorButton Background;
        public InputBox CurrentStateTextBox;
        public InputBox TapeValueTextBox;
        public InputBox NewStateTextBox;
        public InputBox NewTapeValueTextBox;
        public InputBox MoveDirectionTextBox;
        public Icon Arrow;        

        public StateTransitionItem(ActionGroup group)
        {
            Background = new ColorButton(group);
            Background.BaseColor = GlobalInterfaceData.Scheme.Background;
            Background.OnClickedEvent += Clicked;
            Background.OnClickedAwayEvent += ClickedAway;

            Arrow = new Icon();
            Arrow.Bounds = new Point(37, 10);

            CurrentStateTextBox = new InputBox(54, 34, group);
            TapeValueTextBox = new InputBox(54, 34, group);
            NewStateTextBox = new InputBox(54, 34, group);
            NewTapeValueTextBox = new InputBox(54, 34, group);
            MoveDirectionTextBox = new InputBox(54, 34, group);

            CurrentStateTextBox.EditEvent += EditBoxResize;
            TapeValueTextBox.EditEvent += EditBoxResize;
            NewStateTextBox.EditEvent += EditBoxResize;
            NewTapeValueTextBox.EditEvent += EditBoxResize;
            MoveDirectionTextBox.EditEvent += EditBoxResize;
        }

        public void Clicked(Button Sender)
        {
            Background.BaseColor = GlobalInterfaceData.Scheme.DarkInteractableAccent;
        }
        public void ClickedAway(Button Sender)
        {
            Background.BaseColor = GlobalInterfaceData.Scheme.Background;
        }

        void EditBoxResize(InputBox Sender)
        {
            if (Sender.OutputLabel.RichText.Size.X > Sender.Bounds.X)
            {
                Sender.Bounds = new Point(Sender.OutputLabel.RichText.Size.X + 2, Sender.Bounds.Y);
            }

            MoveLayout();
        }

        void MoveLayout()
        {
            Background.Position = position;
            CurrentStateTextBox.Position = position + new Vector2(12, 12);
            TapeValueTextBox.Position = CurrentStateTextBox.Position + new Vector2(CurrentStateTextBox.Bounds.X + 12, 0);

            Arrow.Position = TapeValueTextBox.Position + new Vector2(TapeValueTextBox.Bounds.X + 9, 0);

            NewStateTextBox.Position = Arrow.Position + new Vector2(Arrow.Bounds.X + 9, 0);
            NewTapeValueTextBox.Position = NewStateTextBox.Position + new Vector2(NewStateTextBox.Bounds.X + 12, 0);
            MoveDirectionTextBox.Position = NewTapeValueTextBox.Position + new Vector2(NewTapeValueTextBox.Bounds.X + 12, 0);

            bounds = new Point(115 + CurrentStateTextBox.Bounds.X + TapeValueTextBox.Bounds.X + NewStateTextBox.Bounds.X + NewTapeValueTextBox.Bounds.X + MoveDirectionTextBox.Bounds.X, 56);
            Background.Bounds = bounds;
        }

        public void Draw(Viewport? BoundPort = null)
        {
            Background.Draw(BoundPort);
            Arrow.Draw(BoundPort);

            CurrentStateTextBox.Draw(BoundPort);
            TapeValueTextBox.Draw(BoundPort);
            NewStateTextBox.Draw(BoundPort);
            NewTapeValueTextBox.Draw(BoundPort);
            MoveDirectionTextBox.Draw(BoundPort);
        }

    }
}
