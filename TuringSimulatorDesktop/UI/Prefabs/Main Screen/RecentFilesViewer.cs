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

        public RecentFilesViewer(int width, int height, Vector2 position)
        {
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

            Bounds = new Point(width, height);
            Position = position;
        }

        public void DisplayRecentFiles()
        {
            for (int i = 0; i < GlobalProjectAndUserData.UserData.RecentlyAccessedFiles.Count; i++)
            {
                LayoutBox.AddElement(new RecentFileCard(GlobalProjectAndUserData.UserData.RecentlyAccessedFiles[i]));
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
    }

    public class RecentFileCard : IVisualElement
    {
        Icon Background;
        Label FileName;
        Label FileLocation;
        Label FileLastAccessed;

        public RecentFileCard(FileInfoWrapper FileInfo)
        {
            Background = new Icon(478, 80, Vector2.Zero, GlobalRenderingData.SubHeaderColor);

            FileName = new Label();
            FileLocation = new Label();
            FileLastAccessed = new Label();

            FileName.Text = FileInfo.FileName;
            FileLocation.Text = FileInfo.FullPath;
            FileLastAccessed.Text = FileInfo.LastAccessed.ToString("g");
        }

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
            get => new Point(478, 80); 
            set 
            { 
                return;
            } 
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
