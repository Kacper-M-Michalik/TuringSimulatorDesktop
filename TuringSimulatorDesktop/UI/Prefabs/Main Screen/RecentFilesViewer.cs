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
    public class RecentFilesViewer : IVisualElement
    {
        Vector2 position;
        public Vector2 Position
        {
            get => position;
            set
            {
                position = value;

                Background.Position = position;
                Header.Position = new Vector2(position.X + 1, position.Y + 1);
                ForeGround.Position = new Vector2(position.X + 1, position.Y + 25);
                Title.Position = new Vector2(position.X + 1, position.Y + 1);

                LayoutBox.Position = new Vector2(position.X + 1, position.Y + 25);
            }
        }

        Point bounds;
        public Point Bounds
        {
            get => bounds;
            set
            {
                bounds = value;

                Background.Bounds = bounds;
                Header.Bounds = new Point(bounds.X - 2, 24);
                ForeGround.Bounds = new Point(bounds.X - 2, bounds.Y - 26);

                LayoutBox.Bounds = new Point(bounds.X - 2, bounds.Y - 26);
            }
        }

        Icon Background;
        Icon Header;
        Icon ForeGround;
        Label Title;
        VerticalLayoutBox LayoutBox;
        MainScreenView MainScreen;

        public RecentFilesViewer(MainScreenView Screen)
        {
            MainScreen = Screen;

            Background = new Icon(GlobalRenderingData.DarkAccentColor);
            Header = new Icon(GlobalRenderingData.HeaderColor);
            ForeGround = new Icon(GlobalRenderingData.BackgroundColor);

            Title = new Label();
            Title.Text = "Recently Opened Projects";

            LayoutBox = new VerticalLayoutBox();
            LayoutBox.Spacing = 10;
            LayoutBox.Scrollable = true;
            LayoutBox.ScrollFactor = 0.2f;
            LayoutBox.ViewOffsetBoundsMin = new Vector2(0f, 5f);
            LayoutBox.ViewOffset = new Vector2(0f, 5f);

            //Bounds = new Point(width, height);
            //Position = position;
        }

        public void DisplayRecentFiles()
        {
            for (int i = 0; i < GlobalProjectAndUserData.UserData.RecentlyAccessedFiles.Count; i++)
            {
                LayoutBox.AddElement(new RecentFileCard(GlobalProjectAndUserData.UserData.RecentlyAccessedFiles[i], MainScreen, LayoutBox.Group));
            }
            LayoutBox.UpdateLayout();
        }

        public void Draw(Viewport? BoundPort = null)
        {
            Background.Draw();
            Header.Draw();
            ForeGround.Draw();
            Title.Draw();
            LayoutBox.Draw();
        }

        public void Close()
        {
            LayoutBox.Close();
        }
    }

    public class RecentFileCard : IVisualElement
    {
        Vector2 position;
        public Vector2 Position 
        { 
            get => position;
            set 
            {
                position = value;
                Background.Position = Position;
                FileName.Position = new Vector2(Position.X + 5, Position.Y + 5);
                FileLocation.Position = new Vector2(Position.X + 5, Position.Y + 35);
                FileLastAccessed.Position = new Vector2(Position.X + 50, Position.Y + 5);
            } 
        }

        public Point Bounds 
        {
            get => Background.Bounds; 
            set 
            { 
                return;
            } 
        }

        Button Background;
        Label FileName;
        Label FileLocation;
        Label FileLastAccessed;

        FileInfoWrapper FileInfo;
        MainScreenView MainScreen;

        public RecentFileCard(FileInfoWrapper Info, MainScreenView Screen, ActionGroup group)
        {
            FileInfo = Info;
            MainScreen = Screen;

            Background = new Button(450, 80, group);
            Background.BaseTexture = GlobalRenderingData.TextureLookup[UILookupKey.SubHeader];//, ,Vector2.Zero, GlobalRenderingData.SubHeaderColor);
            Background.OnClickedEvent += LoadRecentProject;

            FileName = new Label();
            FileLocation = new Label();
            FileLastAccessed = new Label();

            FileName.Text = FileInfo.FileName;
            FileLocation.Text = FileInfo.FullPath;
            FileLastAccessed.Text = FileInfo.LastAccessed.ToString("g");
        }

        void LoadRecentProject(Button Sender)
        {
            MainScreen.SelectedProject(FileInfo.FullPath);
        }

        public void Draw(Viewport? BoundPort = null)
        {
            Background.Draw(BoundPort);
            FileName.Draw(BoundPort);
            FileLocation.Draw(BoundPort);
            FileLastAccessed.Draw(BoundPort);
        }
    }
}
