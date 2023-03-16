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
    public class JoinProjectMenu : IVisualElement, IClosable
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

        ActionGroup Group;

        Icon Header;
        Icon Background;
        Label Title;

        Label HostIPLabel;
        InputBox HostIPInputBox;

        TextureButton JoinButton;
        //Label JoinLabel;

        MainScreenView MainScreen;

        public JoinProjectMenu(MainScreenView Screen)
        {
            Group = InputManager.CreateActionGroup();
            MainScreen = Screen;

            Header = new Icon(GlobalInterfaceData.Scheme.Header);
            Background = new Icon(GlobalInterfaceData.Scheme.InteractableAccent);

            Title = new Label();
            Title.FontSize = 16f;
            Title.Font = GlobalInterfaceData.StandardRegularFont;
            Title.FontColor = GlobalInterfaceData.Scheme.FontColorBright;
            Title.Text = "Join Project";

            HostIPLabel = new Label();
            HostIPLabel.AutoSizeMesh = true;
            HostIPLabel.FontSize = 20f;
            HostIPLabel.FontColor = GlobalInterfaceData.Scheme.FontColor;
            HostIPLabel.Text = "Host IP";

            HostIPInputBox = new InputBox(Group);
            HostIPInputBox.BackgroundColor = GlobalInterfaceData.Scheme.Background;
            HostIPInputBox.OutputLabel.DrawCentered = true;
            HostIPInputBox.OutputLabel.FontSize = 20f;
            HostIPInputBox.OutputLabel.FontColor = GlobalInterfaceData.Scheme.FontGrayedOutColor;

            JoinButton = new TextureButton(Group);
            JoinButton.OnClickedEvent += Join;
        }

        public void Join(Button Sender)
        {
            if (System.Net.IPAddress.TryParse(HostIPInputBox.Text, out System.Net.IPAddress IP))
            {
                MainScreen.ConnectToOtherDevice(IP);
            }
        }

        void MoveLayout()
        {
            Group.X = UIUtils.ConvertFloatToInt(Position.X);
            Group.Y = UIUtils.ConvertFloatToInt(Position.Y);

            Background.Position = position;
            Header.Position = position;
            Title.Position = new Vector2(position.X + 17, position.Y + Header.Bounds.Y * 0.5f);

            HostIPLabel.Position = Position + new Vector2(17, 54);
            HostIPInputBox.Position = Position + new Vector2(16, 80);
            JoinButton.Position = Position + new Vector2(16, 630);

        }

        void ResizeLayout()
        {
            Group.Width = bounds.X;
            Group.Height = bounds.Y;

            Background.Bounds = bounds;
            Header.Bounds = new Point(bounds.X, 28);

            HostIPInputBox.Bounds = new Point(418, 42);

            JoinButton.Bounds = new Point(150, 40);
        }

        public void Draw(Viewport? BoundPort = null)
        {
            Background.Draw();
            Header.Draw();
            Title.Draw();

            HostIPLabel.Draw();
            HostIPInputBox.Draw();

            JoinButton.Draw();
            //JoinLabel.Draw();
        }

        public void Close()
        {

        }

    }
}
