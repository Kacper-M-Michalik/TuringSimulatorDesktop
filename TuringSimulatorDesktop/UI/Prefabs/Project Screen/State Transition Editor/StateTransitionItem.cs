using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TuringSimulatorDesktop.Input;

namespace TuringSimulatorDesktop.UI
{
    public class StateTransitionItem: IVisualElement
    {
        Vector2 position;
        public Vector2 Position
        {
            get => position;
            set
            {
                position = value;
                ResizeLayout();
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

        Icon Background;
        public InputBox CurrentStateTextBox;
        public InputBox TapeValueTextBox;
        public InputBox NewStateTextBox;
        public InputBox NewTapeValueTextBox;
        public InputBox MoveDirectionTextBox;
        Label TransitionLabel;

        public StateTransitionItem(ActionGroup group)
        {
            Background = new Icon(GlobalRenderingData.BackgroundColor);

            CurrentStateTextBox = new InputBox(15,10,group);
            TapeValueTextBox = new InputBox(15, 10, group);
            NewStateTextBox = new InputBox(15, 10, group);
            NewTapeValueTextBox = new InputBox(15, 10, group);
            MoveDirectionTextBox = new InputBox(15, 10, group);
            CurrentStateTextBox.EditEvent += EditBoxResize;
            TapeValueTextBox.EditEvent += EditBoxResize;
            NewStateTextBox.EditEvent += EditBoxResize;
            NewTapeValueTextBox.EditEvent += EditBoxResize;
            MoveDirectionTextBox.EditEvent += EditBoxResize;

            TransitionLabel = new Label();
            TransitionLabel.Text = ") => (";
        }

        void EditBoxResize(InputBox Sender)
        {
            ResizeLayout();
        }

        void ResizeLayout()
        {
            Background.Position = position;
            CurrentStateTextBox.Position = position;
            TapeValueTextBox.Position = CurrentStateTextBox.Position + new Vector2(CurrentStateTextBox.Bounds.X + 2, CurrentStateTextBox.Bounds.Y);

            TransitionLabel.Position = TapeValueTextBox.Position + new Vector2(TapeValueTextBox.Bounds.X + 2, TapeValueTextBox.Bounds.Y);

            NewStateTextBox.Position = TransitionLabel.Position + new Vector2(TransitionLabel.Bounds.X + 2, TransitionLabel.Bounds.Y);
            NewTapeValueTextBox.Position = NewStateTextBox.Position + new Vector2(NewStateTextBox.Bounds.X + 2, NewStateTextBox.Bounds.Y);
            MoveDirectionTextBox.Position = NewTapeValueTextBox.Position + new Vector2(NewTapeValueTextBox.Bounds.X + 2, NewTapeValueTextBox.Bounds.Y);

            Vector2 Size = MoveDirectionTextBox.Position + new Vector2(MoveDirectionTextBox.Bounds.X, MoveDirectionTextBox.Bounds.Y) - position;
            bounds = new Point(UIUtils.ConvertFloatToInt(Size.X), 15);
        }

        public void Draw(Viewport? BoundPort = null)
        {
            Background.Draw(BoundPort);
            CurrentStateTextBox.Draw(BoundPort);
            TapeValueTextBox.Draw(BoundPort);
            NewStateTextBox.Draw(BoundPort);
            NewTapeValueTextBox.Draw(BoundPort);
            MoveDirectionTextBox.Draw(BoundPort);
        }

    }
}
