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
    public class RecentFileItem : IVisualElement
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

        public bool IsActive { get; set; } = true;

        ColorButton Background;
        Label FileName;
        Label FileLocation;
        Label FileLastAccessed;

        FileInfoWrapper FileInfo;
        MainScreenView MainScreen;

        public static int ReferenceWidth = 420;
        public static int ReferenceHeight = 60;

        public RecentFileItem(FileInfoWrapper Info, MainScreenView Screen, ActionGroup group)
        {
            FileInfo = Info;
            MainScreen = Screen;

            Background = new ColorButton(ReferenceWidth, ReferenceHeight, group);
            Background.BaseColor = GlobalInterfaceData.Scheme.Background;
            Background.HighlightColor = GlobalInterfaceData.Scheme.DarkInteractableAccent;
            Background.HighlightOnMouseOver = true;
            Background.OnClickedEvent += LoadRecentProject;

            FileName = new Label();
            FileName.FontSize = 20;

            FileLocation = new Label();
            FileLastAccessed = new Label();

            FileName.Text = FileInfo.FileName;

            //Limit displayed file path length as to not overflow out of the edge of this UI element
            if (FileInfo.FullPath.Length > 65)
            {
                FileLocation.Text = FileInfo.FullPath.Substring(0, 65) + "...";
            }
            else
            {
                FileLocation.Text = FileInfo.FullPath;
            }

            FileLastAccessed.Text = FileInfo.LastAccessed.ToString("g");

            bounds = new Point(ReferenceWidth, ReferenceHeight);
            Position = Vector2.Zero;
        }

        //When clicked, load this recent file
        void LoadRecentProject(Button Sender)
        {
            MainScreen.SelectedProject(FileInfo.FullPath, 1);
        }

        void MoveLayout()
        {
            Background.Position = Position;
            FileName.Position = new Vector2(Position.X + 8, Position.Y + 19);
            FileLocation.Position = new Vector2(Position.X + 8, Position.Y + 45);
            FileLastAccessed.Position = new Vector2(Position.X + ReferenceWidth - FileLastAccessed.Bounds.X - 10, Position.Y + 19);
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
