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
    public class StateTransitionItem: IVisualElement, ICanvasInteractable, IPollable
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
        public bool IsMarkedForDeletion { get; set; }

        public ColorButton Background;
        public InputBox CurrentStateTextBox;
        public InputBox TapeValueTextBox;
        public InputBox NewStateTextBox;
        public InputBox NewTapeValueTextBox;
        public InputBox MoveDirectionTextBox;
        public Icon Arrow;

        TextProgrammingView ProgrammingView;

        bool LeftClickedOnce = false;

        Matrix Offset;

        ActionGroup Group;

        public StateTransitionItem(ActionGroup group, TextProgrammingView view)
        {
            Group = group;
            group.PollableObjects.Add(this);
            ProgrammingView = view;

            Background = new ColorButton(group);
            Background.BaseColor = GlobalInterfaceData.Scheme.Background;
            Background.OnClickedEvent += Clicked;
            Background.OnClickedAwayEvent += ClickedAway;
            Background.ClickListenType = ClickType.Both;

            Arrow = new Icon();
            Arrow.Bounds = new Point(37, 10);
            Arrow.DrawTexture = GlobalInterfaceData.TextureLookup[UILookupKey.Arrow];

            CurrentStateTextBox = new InputBox(54, 34, group);
            CurrentStateTextBox.OutputLabel.FontSize = 16;
            CurrentStateTextBox.OutputLabel.DrawCentered = true;
            CurrentStateTextBox.BackgroundColor = GlobalInterfaceData.Scheme.InteractableAccent;
            TapeValueTextBox = new InputBox(54, 34, group);
            TapeValueTextBox.OutputLabel.FontSize = 16;
            TapeValueTextBox.OutputLabel.DrawCentered = true;
            TapeValueTextBox.BackgroundColor = GlobalInterfaceData.Scheme.InteractableAccent;
            NewStateTextBox = new InputBox(54, 34, group);
            NewStateTextBox.OutputLabel.FontSize = 16;
            NewStateTextBox.OutputLabel.DrawCentered = true;
            NewStateTextBox.BackgroundColor = GlobalInterfaceData.Scheme.InteractableAccent;
            NewTapeValueTextBox = new InputBox(54, 34, group);
            NewTapeValueTextBox.OutputLabel.FontSize = 16;
            NewTapeValueTextBox.OutputLabel.DrawCentered = true;
            NewTapeValueTextBox.BackgroundColor = GlobalInterfaceData.Scheme.InteractableAccent;
            MoveDirectionTextBox = new InputBox(54, 34, group);
            MoveDirectionTextBox.OutputLabel.FontSize = 16;
            MoveDirectionTextBox.OutputLabel.DrawCentered = true;
            MoveDirectionTextBox.BackgroundColor = GlobalInterfaceData.Scheme.InteractableAccent;

            CurrentStateTextBox.EditEvent += EditBoxResize;
            TapeValueTextBox.EditEvent += EditBoxResize;
            NewStateTextBox.EditEvent += EditBoxResize;
            NewTapeValueTextBox.EditEvent += EditBoxResize;
            MoveDirectionTextBox.EditEvent += EditBoxResize;
        }

        public void Clicked(Button Sender)
        {
            if (LeftClickedOnce)
            {
                ClickedAway(null);
                return;
            }

            Offset = Matrix.CreateTranslation(position.X, position.Y, 0) - ProgrammingView.TransitionCanvas.InverseMatrix * Matrix.CreateTranslation(InputManager.MouseData.X, InputManager.MouseData.Y, 0);

            Background.BaseColor = GlobalInterfaceData.Scheme.DarkInteractableAccent;
            ProgrammingView.TransitionCanvas.Draggable = false;

            if (InputManager.RightMousePressed)
            {
                ProgrammingView.OpenNodeEditMenu(this);
            }
            else
            {
                LeftClickedOnce = true;
            }
        }
        public void ClickedAway(Button Sender)
        {
            LeftClickedOnce = false;
            Offset = Matrix.CreateTranslation(0, 0, 0);
            Background.BaseColor = GlobalInterfaceData.Scheme.Background;
            ProgrammingView.TransitionCanvas.Draggable = true;
        }

        public void EditBoxResize(InputBox Sender)
        {
            if (Sender.OutputLabel.RichText.Size.X > Sender.Bounds.X)
            {
                Sender.Bounds = new Point(Sender.OutputLabel.RichText.Size.X + 4, Sender.Bounds.Y);
            }

            MoveLayout();
        }

        void MoveLayout()
        {
            Background.Position = position;
            CurrentStateTextBox.Position = position + new Vector2(12, 12);
            TapeValueTextBox.Position = CurrentStateTextBox.Position + new Vector2(CurrentStateTextBox.Bounds.X + 12, 0);

            Arrow.Position = new Vector2(TapeValueTextBox.Position.X + TapeValueTextBox.Bounds.X + 9, position.Y + 23);

            NewStateTextBox.Position = new Vector2(Arrow.Position.X + Arrow.Bounds.X + 9, TapeValueTextBox.Position.Y);
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

        public void PollInput(bool IsInActionGroupFrame)
        {
            if (IsInActionGroupFrame && LeftClickedOnce)
            {
                ProgrammingView.MoveTansition(this, Offset);
            }
        }

        public void Close()
        {
            IsMarkedForDeletion = true;
            IsActive = false;

            Background.Close();
            CurrentStateTextBox.Close();
            TapeValueTextBox.Close();
            NewTapeValueTextBox.Close();
            NewStateTextBox.Close();
            MoveDirectionTextBox.Close();
        }
    }
}
