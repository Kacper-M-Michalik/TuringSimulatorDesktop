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
    public class RunProjectItem : IVisualElement
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
            }
        }

        Icon RunIcon;
        ColorButton BackgroundButton;
        Label CurrentFileTitle;

        public RunProjectItem(ProjectScreenView view, ActionGroup group)
        {
            BackgroundButton = new ColorButton(group);
            BackgroundButton.BaseColor = GlobalInterfaceData.Scheme.InteractableAccent;
            BackgroundButton.HighlightColor = GlobalInterfaceData.Scheme.DarkInteractableAccent;
            BackgroundButton.HighlightOnMouseOver = true;
            BackgroundButton.OnClickedEvent += view.Run;

            RunIcon = new Icon(GlobalInterfaceData.TextureLookup[UILookupKey.RunIcon]);

            CurrentFileTitle = new Label();
            CurrentFileTitle.FontSize = 12;
            CurrentFileTitle.FontColor = GlobalInterfaceData.Scheme.FontColor;
            CurrentFileTitle.Text = "Nothing Selected";

            bounds = GlobalInterfaceData.Scale(new Point(25 + CurrentFileTitle.Bounds.X + 8, 20));
            ResizeLayout();
            Position = Vector2.Zero;
        }

        public void UpdateTarget(string title)
        {
            CurrentFileTitle.Text = title;
            bounds = GlobalInterfaceData.Scale(new Point(25 + CurrentFileTitle.Bounds.X + 8, 20));
            ResizeLayout();
        }

        void MoveLayout()
        {
            BackgroundButton.Position = Position;

            RunIcon.Position = Position + GlobalInterfaceData.Scale(new Vector2(10, 10));

            CurrentFileTitle.Position = Position + GlobalInterfaceData.Scale(new Vector2(15, 17));
        }

        public void ResizeLayout()
        {
            BackgroundButton.Bounds = bounds;

            RunIcon.Bounds = GlobalInterfaceData.Scale(new Point(20, 20));

            float FontSize = GlobalInterfaceData.Scale(12);
            CurrentFileTitle.FontSize = FontSize;
            CurrentFileTitle.UpdateLabel();
        }

        public void Draw(Viewport? BoundPort = null)
        {
            if (IsActive)
            {
                BackgroundButton.Draw();
                RunIcon.Draw();
                CurrentFileTitle.Draw();
            }
        }

        public void Close()
        {
            IsActive = false;
        }
    }
}
