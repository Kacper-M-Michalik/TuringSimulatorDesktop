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
    public class NodeCreationMenu : IVisualElement, IClosable
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
            }
        }

        bool isActive = true;
        public bool IsActive
        {
            get => isActive;
            set
            {
                isActive = value;
                Group.IsActive = isActive;
            }
        }

        ActionGroup Group;

        Icon Background;
        ColorButton CreateTransitionButton;
        Label CreateTransitionLabel;

        TextProgrammingView Editor;

        public NodeCreationMenu(TextProgrammingView editor)
        {
            Editor = editor;
            Group = InputManager.CreateActionGroup();

            Background = new Icon(GlobalInterfaceData.Scheme.CanvasContextMenu);

            CreateTransitionButton = new ColorButton(Group);
            CreateTransitionButton.BaseColor = GlobalInterfaceData.Scheme.CanvasContextMenu;
            CreateTransitionButton.HighlightColor = GlobalInterfaceData.Scheme.CanvasContextMenuSelected;
            CreateTransitionButton.HighlightOnMouseOver = true;
            CreateTransitionButton.OnClickedEvent += Editor.AddNewTransition;

            CreateTransitionLabel = new Label();
            CreateTransitionLabel.FontSize = 12;
            CreateTransitionLabel.FontColor = GlobalInterfaceData.Scheme.FontColor;
            CreateTransitionLabel.Text = "Create Transition";

            bounds = GlobalInterfaceData.Scale(new Point(225, 34));
            ResizeLayout();
            Position = Vector2.Zero;
        }

        void MoveLayout()
        {
            Group.X = UIUtils.ConvertFloatToInt(position.X);
            Group.Y = UIUtils.ConvertFloatToInt(position.Y);

            Background.Position = Position;

            CreateTransitionButton.Position = Position + GlobalInterfaceData.Scale(new Vector2(0, 5));

            CreateTransitionLabel.Position = Position + GlobalInterfaceData.Scale(new Vector2(30, 16));
        }

        void ResizeLayout()
        {
            Group.Width = bounds.X;
            Group.Height = bounds.Y;

            Background.Bounds = bounds;

            CreateTransitionButton.Bounds = GlobalInterfaceData.Scale(new Point(225, 24));

            float FontSize = GlobalInterfaceData.Scale(12);
            CreateTransitionLabel.FontSize = FontSize;
        }

        public void Draw(Viewport? BoundPort = null)
        {
            if (IsActive)
            {
                Background.Draw();

                CreateTransitionButton.Draw();
                CreateTransitionLabel.Draw();
            }
        }

        public void Close()
        {
            IsActive = false;
            Group.IsMarkedForDeletion = true;
        }
    }
}
