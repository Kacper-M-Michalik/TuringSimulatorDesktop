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
    public class DefenitionAlphabetInputItem : IVisualElement, IClickable, IDragListener
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

        Icon Background;

        public Label FileLabel;

        public FileData ReferenceFileData;

        public DefenitionAlphabetInputItem(ActionGroup group)
        {
            group.ClickableObjects.Add(this);

            Background = new Icon(GlobalInterfaceData.Scheme.InteractableAccent);

            FileLabel = new Label();
            FileLabel.FontColor = GlobalInterfaceData.Scheme.FontColor;
            FileLabel.Font = GlobalInterfaceData.StandardRegularFont;
            FileLabel.FontSize = GlobalInterfaceData.Scale(12);
            FileLabel.Text = "No Referenced Alphabet";
        }

        public void Clicked()
        {
        }

        public void ClickedAway()
        {

        }

        public bool IsMouseOver()
        {
            return (IsActive && InputManager.MouseData.X >= Position.X && InputManager.MouseData.X <= Position.X + bounds.X && InputManager.MouseData.Y >= Position.Y && InputManager.MouseData.Y <= Position.Y + bounds.Y);
        }

        public void RecieveDragData()
        {
            FileData Data = InputManager.DragData as FileData;
            if (Data != null && Data.Type == TuringCore.CoreFileType.Alphabet)
            {
                ReferenceFileData = Data;
                FileLabel.Text = ReferenceFileData.Name;
            }
        }
       
        void MoveLayout()
        {
            Background.Position = Position;
            FileLabel.Position = Position + GlobalInterfaceData.Scale(new Vector2(0, bounds.Y / 2f));
        }

        void ResizeLayout()
        {
            Background.Bounds = bounds;
        }

        public void Draw(Viewport? BoundPort = null)
        {
            if (IsActive)
            {
                Background.Draw(BoundPort);
                FileLabel.Draw(BoundPort);
            }
        }

    }
}
