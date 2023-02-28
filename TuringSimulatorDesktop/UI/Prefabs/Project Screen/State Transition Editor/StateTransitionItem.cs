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

        }

        public bool IsActive { get; set; } = true;

        public Icon Background;
        public InputBox CurrentStateTextBox;
        public InputBox TapeValueTextBox;
        public InputBox NewStateTextBox;
        public InputBox NewTapeValueTextBox;
        public InputBox MoveDirectionTextBox;
        public Icon Arrow;        

        public StateTransitionItem(ActionGroup group)
        {
            Background = new Icon(GlobalInterfaceData.Scheme.Background);
            Arrow = new Icon();

            CurrentStateTextBox = new InputBox(25, 20,group);

            TapeValueTextBox = new InputBox(25, 20, group);
            NewStateTextBox = new InputBox(25, 20, group);
            NewTapeValueTextBox = new InputBox(25, 20, group);
            MoveDirectionTextBox = new InputBox(25, 20, group);
            CurrentStateTextBox.EditEvent += EditBoxResize;
            TapeValueTextBox.EditEvent += EditBoxResize;
            NewStateTextBox.EditEvent += EditBoxResize;
            NewTapeValueTextBox.EditEvent += EditBoxResize;
            MoveDirectionTextBox.EditEvent += EditBoxResize;
        }

        void EditBoxResize(InputBox Sender)
        {
            MoveLayout();
        }

        void MoveLayout()
        {
            Background.Position = position;
            CurrentStateTextBox.Position = position + new Vector2(5, 5);
            TapeValueTextBox.Position = CurrentStateTextBox.Position + new Vector2(CurrentStateTextBox.Bounds.X + 2, 0);

            Arrow.Position = TapeValueTextBox.Position + new Vector2(TapeValueTextBox.Bounds.X + 2, 0);

            NewStateTextBox.Position = Arrow.Position + new Vector2(Arrow.Bounds.X + 2, 0);
            NewTapeValueTextBox.Position = NewStateTextBox.Position + new Vector2(NewStateTextBox.Bounds.X + 2, 0);
            MoveDirectionTextBox.Position = NewTapeValueTextBox.Position + new Vector2(NewTapeValueTextBox.Bounds.X + 2, 0);

            Vector2 Size = MoveDirectionTextBox.Position + new Vector2(MoveDirectionTextBox.Bounds.X, MoveDirectionTextBox.Bounds.Y) - position;
            bounds = new Point(UIUtils.ConvertFloatToInt(Size.X), 100); 

            Background.Bounds = new Point(UIUtils.ConvertFloatToInt(Size.X), 100);
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
