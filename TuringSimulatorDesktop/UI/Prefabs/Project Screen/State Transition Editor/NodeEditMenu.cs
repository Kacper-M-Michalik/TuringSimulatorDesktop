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
    public class NodeEditMenu : IVisualElement, IClosable
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
        public ColorButton DeleteNodeButton;
        public ColorButton CloneNodeButton;
        Label DeleteNodeLabel;
        Label CloneNodeLabel;

        Icon Divider1;

        ColorButton CreateTransitionButton;        
        Label CreateTransitionLabel;

        TextProgrammingView Editor;

        StateTransitionItem CurrentNode;

        public NodeEditMenu(TextProgrammingView editor, StateTransitionItem Node)
        {
            CurrentNode = Node;
            Editor = editor;
            Group = InputManager.CreateActionGroup();

            Background = new Icon(GlobalInterfaceData.Scheme.CanvasContextMenu);

            DeleteNodeButton = new ColorButton(Group);
            DeleteNodeButton.BaseColor = GlobalInterfaceData.Scheme.CanvasContextMenu;
            DeleteNodeButton.HighlightColor = GlobalInterfaceData.Scheme.CanvasContextMenuSelected;
            DeleteNodeButton.HighlightOnMouseOver = true;
            DeleteNodeButton.OnClickedEvent += Delete;

            CloneNodeButton = new ColorButton(Group);
            CloneNodeButton.BaseColor = GlobalInterfaceData.Scheme.CanvasContextMenu;
            CloneNodeButton.HighlightColor = GlobalInterfaceData.Scheme.CanvasContextMenuSelected;
            CloneNodeButton.HighlightOnMouseOver = true;
            CloneNodeButton.OnClickedEvent += Clone;

            DeleteNodeLabel = new Label();
            DeleteNodeLabel.FontSize = 12;
            DeleteNodeLabel.FontColor = GlobalInterfaceData.Scheme.FontColor;
            DeleteNodeLabel.Text = "Delete Transition";

            CloneNodeLabel = new Label();
            CloneNodeLabel.FontSize = 12;
            CloneNodeLabel.FontColor = GlobalInterfaceData.Scheme.FontColor;
            CloneNodeLabel.Text = "Clone Transition";

            Divider1 = new Icon(GlobalInterfaceData.Scheme.NonInteractableAccent);

            CreateTransitionButton = new ColorButton(Group);
            CreateTransitionButton.BaseColor = GlobalInterfaceData.Scheme.CanvasContextMenu;
            CreateTransitionButton.HighlightColor = GlobalInterfaceData.Scheme.CanvasContextMenuSelected;
            CreateTransitionButton.HighlightOnMouseOver = true;
            CreateTransitionButton.OnClickedEvent += Editor.AddNewTransition;

            CreateTransitionLabel = new Label();
            CreateTransitionLabel.FontSize = 12;
            CreateTransitionLabel.FontColor = GlobalInterfaceData.Scheme.FontColor;
            CreateTransitionLabel.Text = "Create Transition";


            bounds = GlobalInterfaceData.Scale(new Point(225, 83));
            ResizeLayout();
            Position = Vector2.Zero;
        }

        public void Delete(Button Sender)
        {
            Editor.DeleteTransition(CurrentNode);
        }
        public void Clone(Button Sender)
        {
            Editor.CloneTransition(CurrentNode);
        }


        void MoveLayout()
        {
            Group.X = UIUtils.ConvertFloatToInt(position.X);
            Group.Y = UIUtils.ConvertFloatToInt(position.Y);

            Background.Position = Position;

            DeleteNodeButton.Position = Position + GlobalInterfaceData.Scale(new Vector2(0, 5));
            DeleteNodeLabel.Position = Position + new Vector2(30, 16);
            CloneNodeButton.Position = Position + GlobalInterfaceData.Scale(new Vector2(0, 29));
            CloneNodeLabel.Position = Position + new Vector2(30, 40);

            Divider1.Position = Position + GlobalInterfaceData.Scale(new Vector2(0, 54));

            CreateTransitionButton.Position = Position + GlobalInterfaceData.Scale(new Vector2(0, 55));
            CreateTransitionLabel.Position = Position + GlobalInterfaceData.Scale(new Vector2(30, 66));
        }

        void ResizeLayout()
        {
            Group.Width = bounds.X;
            Group.Height = bounds.Y;

            Background.Bounds = bounds;

            Divider1.Bounds = GlobalInterfaceData.Scale(new Point(225, 1));

            CloneNodeButton.Bounds = GlobalInterfaceData.Scale(new Point(225, 24));
            DeleteNodeButton.Bounds = GlobalInterfaceData.Scale(new Point(225, 24));
            CreateTransitionButton.Bounds = GlobalInterfaceData.Scale(new Point(225, 24));

            float FontSize = GlobalInterfaceData.Scale(12);
            DeleteNodeLabel.FontSize = FontSize;
            CloneNodeLabel.FontSize = FontSize;
            CreateTransitionLabel.FontSize = FontSize;
        }

        public void Draw(Viewport? BoundPort = null)
        {
            if (IsActive)
            {
                Background.Draw();

                DeleteNodeButton.Draw();
                CloneNodeButton.Draw();

                Divider1.Draw();

                CreateTransitionButton.Draw();

                DeleteNodeLabel.Draw();
                CloneNodeLabel.Draw();
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
