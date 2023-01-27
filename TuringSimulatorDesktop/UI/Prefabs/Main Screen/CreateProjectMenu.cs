using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuringSimulatorDesktop.UI
{
    public class CreateProjectMenu : IVisualElement
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

        Icon Header;
        Icon Background;
        Label Title;

        Label ProjectTitle;
        InputBox ProjectTitleInputBox;

        Label ProjectLocationTitle;
        ColorButton ProjectLocationSelectionButton;
        Label ProjectLocation;

        MainScreenView MainScreen;

        public CreateProjectMenu(MainScreenView Screen)
        {
            MainScreen = Screen;

            Header = new Icon(GlobalInterfaceData.Scheme.Header);
            Background = new Icon(GlobalInterfaceData.Scheme.InteractableAccent);

            Title = new Label();
            Title.FontSize = 12f;
            Title.Font = GlobalInterfaceData.StandardBoldFont;
            Title.FontColor = GlobalInterfaceData.Scheme.FontColorBright;
            Title.Text = "Create New Project";




        }


        void MoveLayout()
        {
            Background.Position = position;
            Header.Position = position;
            Title.Position = new Vector2(position.X, position.Y);


        }

        void ResizeLayout()
        {
            Background.Bounds = bounds;
            Header.Bounds = new Point(bounds.X, 24);

        }

        public void Draw(Viewport? BoundPort = null)
        {
            Background.Draw();
            Header.Draw();
            Title.Draw();

        }

        public void Close()
        {
            ProjectTitleInputBox.Close();
            ProjectLocationSelectionButton.Close();
        }

    }
}
