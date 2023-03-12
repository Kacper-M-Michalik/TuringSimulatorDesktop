using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuringSimulatorDesktop.UI.Prefabs
{
    public class LoadProjectMenu : IVisualElement
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


        MainScreenView MainScreen;

        public LoadProjectMenu(MainScreenView Screen)
        {
            MainScreen = Screen;

            Header = new Icon(GlobalInterfaceData.Scheme.Header);
            Background = new Icon(GlobalInterfaceData.Scheme.InteractableAccent);

            Title = new Label();
            Title.FontSize = 16f;
            Title.Font = GlobalInterfaceData.StandardBoldFont;
            Title.FontColor = GlobalInterfaceData.Scheme.FontColorBright;
            Title.Text = "Load Project";

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
            Header.Bounds = new Point(bounds.X, 28);

        }

        public void Draw(Viewport? BoundPort = null)
        {
            Background.Draw();
            Header.Draw();
            Title.Draw();
        }

        public void Close()
        {

        }

    }
}
