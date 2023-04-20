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
    public class HelpMenu : IView
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

        bool isActive;
        public bool IsActive
        {
            get => isActive;
            set
            {
                isActive = value;
            }
        }

        Window ownerWindow;
        public Window OwnerWindow
        {
            get => ownerWindow;
            set => ownerWindow = value;
        }

        public bool IsMarkedForDeletion
        {
            get => false;
            set
            {
            }
        }

        string title = "Help Menu";
        public string Title => title;

        public Guid OpenFileID => Guid.Empty;

        ActionGroup Group;

        //Help menus are like slideshows, the help contents is a series of images the help menu displays and lets the user cycle through
        List<Texture2D> Views;
        int CurrentView = 0;

        Icon MenuIcon;
        Label CurrentMenuLabel;
        TextureButton LeftButton;
        TextureButton RightButton;

        //Constructor
        //Creating a help menu requires the creating object to pass in a series of textures (images/slides) the help menu will display
        public HelpMenu(List<Texture2D> Textures)
        {
            //Ensure there it at least 1 base slide to display
            if (Textures.Count == 0)
            {
                throw new Exception("Cannot supply 0 slides to a Help Menu");
            }

            Views = Textures;
            Group = InputManager.CreateActionGroup();

            MenuIcon = new Icon();
            MenuIcon.DrawTexture = Views[0];

            CurrentMenuLabel = new Label();
            CurrentMenuLabel.FontSize = 24;
            CurrentMenuLabel.FontColor = GlobalInterfaceData.Scheme.FontColor;
            CurrentMenuLabel.Text = (CurrentView + 1).ToString() + "/" + Views.Count;

            LeftButton = new TextureButton(Group);
            LeftButton.BaseTexture = GlobalInterfaceData.TextureLookup[UILookupKey.ButtonLeft];
            LeftButton.OnClickedEvent += ChangeMenuLeft;

            RightButton = new TextureButton(Group);
            RightButton.BaseTexture = GlobalInterfaceData.TextureLookup[UILookupKey.ButtonRight];
            RightButton.OnClickedEvent += ChangeMenuRight;

            IsActive = false;
        }

        void MoveLayout()
        {
            Group.X = UIUtils.ConvertFloatToInt(position.X);
            Group.Y = UIUtils.ConvertFloatToInt(position.Y);

            MenuIcon.Position = position;
            LeftButton.Position = position + new Vector2(bounds.X, bounds.Y) - new Vector2(136.5f, 52.5f);
            RightButton.Position = position + new Vector2(bounds.X, bounds.Y) - new Vector2(97.5f, 52.5f);
            CurrentMenuLabel.Position = position + new Vector2(bounds.X, bounds.Y) - new Vector2(58.9f, 37f);
        }

        void ResizeLayout()
        {
            Group.Width = bounds.X;
            Group.Height = bounds.Y;

            MenuIcon.Bounds = bounds;
            LeftButton.Bounds = new Point(26, 31);
            RightButton.Bounds = new Point(26, 31);
        }

        //Changes slide one left one if available
        public void ChangeMenuLeft(Button Sender)
        {
            if (CurrentView > 0) CurrentView--;
            MenuIcon.DrawTexture = Views[CurrentView];
            CurrentMenuLabel.Text = (CurrentView + 1).ToString() + "/" + Views.Count;
        }

        //Changes slide one right one if available
        public void ChangeMenuRight(Button Sender)
        {
            if (CurrentView < Views.Count - 1) CurrentView++;
            MenuIcon.DrawTexture = Views[CurrentView];
            CurrentMenuLabel.Text = (CurrentView + 1).ToString() + "/" + Views.Count;
        }

        public void Draw(Viewport? BoundPort = null)
        {
            if (IsActive)
            {
                MenuIcon.Draw(BoundPort);
                CurrentMenuLabel.Draw(BoundPort);
                LeftButton.Draw(BoundPort);
                RightButton.Draw(BoundPort);
            }
        }

        public void Close()
        {
            IsActive = false;
        }
    }   
}
