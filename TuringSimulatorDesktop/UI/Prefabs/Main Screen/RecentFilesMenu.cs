using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringSimulatorDesktop.Files;
using TuringSimulatorDesktop.Input;

namespace TuringSimulatorDesktop.UI.Prefabs
{
    public class RecentFilesMenu : IVisualElement, IClosable
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
        VerticalLayoutBox LayoutBox;
        MainScreenView MainScreen;

        public RecentFilesMenu(MainScreenView Screen)
        {
            MainScreen = Screen;

            Header = new Icon(GlobalInterfaceData.Scheme.Header);
            Background = new Icon(GlobalInterfaceData.Scheme.InteractableAccent);

            Title = new Label();
            Title.FontSize = 16f;
            Title.Font = GlobalInterfaceData.StandardRegularFont;
            Title.FontColor = GlobalInterfaceData.Scheme.FontColorBright;
            Title.Text = "Recently Opened Projects";

            LayoutBox = new VerticalLayoutBox();
            LayoutBox.Spacing = 10;
            LayoutBox.Scrollable = true;
            LayoutBox.ScrollFactor = 0.2f;
            LayoutBox.ViewOffsetBoundsMin = new Vector2(15f, 5f);            
            LayoutBox.ViewOffset = new Vector2(15f, 5f);            
        }

        //Create display objects for each recently opened file entry
        public void DisplayRecentFiles()
        {
            for (int i = 0; i < GlobalProjectAndUserData.UserData.RecentlyAccessedFiles.Count; i++)
            {
                LayoutBox.AddElement(new RecentFileItem(GlobalProjectAndUserData.UserData.RecentlyAccessedFiles[i], MainScreen, LayoutBox.Group));
            }
            LayoutBox.UpdateLayout();
        }

        void MoveLayout()
        {
            Background.Position = position;
            Header.Position = position;
            Title.Position = new Vector2(position.X + 17, position.Y + Header.Bounds.Y * 0.5f);

            LayoutBox.Position = new Vector2(position.X, position.Y + 28);
        }

        void ResizeLayout()
        {
            Background.Bounds = bounds;
            Header.Bounds = new Point(bounds.X, 28);
            LayoutBox.Bounds = new Point(bounds.X, bounds.Y - 28);
        }

        public void Draw(Viewport? BoundPort = null)
        {
            Background.Draw();
            Header.Draw();
            Title.Draw();
            LayoutBox.Draw();
        }

        public void Close()
        {
            LayoutBox.Close();
        }
    }
}
