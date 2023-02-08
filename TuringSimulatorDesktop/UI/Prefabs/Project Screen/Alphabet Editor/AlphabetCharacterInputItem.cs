using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TuringSimulatorDesktop.Input;

namespace TuringSimulatorDesktop.UI
{
    public class AlphabetCharacterInputItem : IVisualElement
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

        public bool IsActive { get; set; } = true;

        public bool IsMarkedForDeletion { get; set; }

        ActionGroup Group;

        ColorButton Background;
        VerticalLayoutBox LayoutBox;
        public List<InputBox> InputBoxes;

        public AlphabetCharacterInputItem(ActionGroup group)
        {
            Group = group;

            Background = new ColorButton(Group);
            Background.BaseColor = (GlobalInterfaceData.Scheme.InteractableAccent);
            Background.OnClickedEvent += Clicked;

            LayoutBox = new VerticalLayoutBox();
            LayoutBox.DrawBounded = true;

            InputBoxes = new List<InputBox>();

            LastInputBoxEdited(null);
        }

        public void Clicked(Button Sender)
        {
            InputManager.ManuallyClickElement(InputBoxes[InputBoxes.Count - 1]);
        }

        public void LastInputBoxEdited(InputBox Sender)
        {
            if (InputBoxes.Count > 0) InputBoxes[InputBoxes.Count - 1].EditEvent -= LastInputBoxEdited;

            InputBox NewInput = new InputBox(Group);
            NewInput.BackgroundColor = GlobalInterfaceData.Scheme.InteractableAccent;
            NewInput.OutputLabel.AutoSizeMesh = true;
            NewInput.Modifiers.AllowsNewLine = false;
            NewInput.OutputLabel.FontColor = GlobalInterfaceData.Scheme.FontColor;
            NewInput.OutputLabel.FontSize = 16;
            NewInput.Text = "";
            NewInput.EditEvent += LastInputBoxEdited;

            InputBoxes.Add(NewInput);
            LayoutBox.AddElement(NewInput);
            LayoutBox.UpdateLayout();
        }

        void MoveLayout()
        {
            Background.Position = Position;

            LayoutBox.Position = Position;
        }

        void ResizeLayout()
        {
            Background.Bounds = bounds;
            LayoutBox.Bounds = bounds;
        }

        public void Draw(Viewport? BoundPort = null)
        {
            if (IsActive)
            {
                Background.Draw(BoundPort);
                LayoutBox.Draw(BoundPort);
            }
        }
    }
}
