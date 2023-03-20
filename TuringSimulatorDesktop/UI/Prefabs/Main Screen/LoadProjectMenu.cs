using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringSimulatorDesktop.Input;
using TuringServer;
using System.Windows.Forms;

namespace TuringSimulatorDesktop.UI
{
    public class LoadProjectMenu : IVisualElement, IClosable
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

        Label ProjectLocationTitle;
        TextureButton ProjectLocationSelectionButton;
        InputBox ProjectLocationInputBox;

        TextureButton HostOption;
        Label HostLabel;

        Label ClientsTitle;
        InputBox ClientsInputBox;

        TextureButton LoadButton;

        MainScreenView MainScreen;

        bool IsHosting = false;

        public LoadProjectMenu(MainScreenView Screen)
        {
            Group = InputManager.CreateActionGroup();
            MainScreen = Screen;

            Header = new Icon(GlobalInterfaceData.Scheme.Header);
            Background = new Icon(GlobalInterfaceData.Scheme.InteractableAccent);

            Title = new Label();
            Title.FontSize = 16f;
            Title.Font = GlobalInterfaceData.StandardRegularFont;
            Title.FontColor = GlobalInterfaceData.Scheme.FontColorBright;
            Title.Text = "Load Project";



            ProjectLocationTitle = new Label();
            ProjectLocationTitle.AutoSizeMesh = true;
            ProjectLocationTitle.FontSize = 20f;
            ProjectLocationTitle.FontColor = GlobalInterfaceData.Scheme.FontColor;
            ProjectLocationTitle.Text = "Project Location";

            ProjectLocationInputBox = new InputBox(Group);
            ProjectLocationInputBox.Modifiers.AllowsNewLine = false;
            ProjectLocationInputBox.BackgroundColor = GlobalInterfaceData.Scheme.Background;
            ProjectLocationInputBox.OutputLabel.FontSize = 20f;
            ProjectLocationInputBox.OutputLabel.FontColor = GlobalInterfaceData.Scheme.FontGrayedOutColor;

            ProjectLocationSelectionButton = new TextureButton(Group);
            ProjectLocationSelectionButton.BaseTexture = GlobalInterfaceData.TextureLookup[UILookupKey.LoadFile];
            ProjectLocationSelectionButton.OnClickedEvent += SelectLocation;

            HostOption = new TextureButton(Group);
            HostOption.BaseTexture = GlobalInterfaceData.TextureLookup[UILookupKey.TickboxUnticked];
            HostOption.OnClickedEvent += ToggleHost;

            HostLabel = new Label();
            HostLabel.AutoSizeMesh = true;
            HostLabel.FontSize = 20f;
            HostLabel.FontColor = GlobalInterfaceData.Scheme.FontColor;
            HostLabel.Text = "Host Publicly";

            ClientsTitle = new Label();
            ClientsTitle.AutoSizeMesh = true;
            ClientsTitle.FontSize = 20f;
            ClientsTitle.FontColor = GlobalInterfaceData.Scheme.FontGrayedOutColor;
            ClientsTitle.Text = "Maximum Clients";
            ClientsTitle.IsActive = false;

            ClientsInputBox = new InputBox(Group);
            ClientsInputBox.Modifiers.AllowsCharacters = false;
            ClientsInputBox.Modifiers.AllowsSymbols = false;
            ClientsInputBox.Modifiers.AllowsNewLine = false;
            ClientsInputBox.BackgroundColor = GlobalInterfaceData.Scheme.Background;
            ClientsInputBox.OutputLabel.DrawCentered = true;
            ClientsInputBox.OutputLabel.FontSize = 20f;
            ClientsInputBox.OutputLabel.FontColor = GlobalInterfaceData.Scheme.FontGrayedOutColor;
            ClientsInputBox.EditEvent += SanitiseClientCount;
            ClientsInputBox.IsActive = false;

            LoadButton = new TextureButton(Group);
            LoadButton.BaseTexture = GlobalInterfaceData.TextureLookup[UILookupKey.LoadButton];
            LoadButton.HighlightTexture = GlobalInterfaceData.TextureLookup[UILookupKey.LoadButtonHighlight];
            LoadButton.HighlightOnMouseOver = true;
            LoadButton.OnClickedEvent += LoadProject;
        }

        public void SelectLocation(Button Sender)
        {
            OpenFileDialog Dialog = new OpenFileDialog
            {
                InitialDirectory = @"C:\",
                Title = "Browse tproj Files",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "tproj",
                Filter = "tproj files (*.tproj)|*.tproj",
                FilterIndex = 2,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if (Dialog.ShowDialog() == DialogResult.OK)
            {
                ProjectLocationInputBox.Text = Dialog.FileName;
                //MainScreen.SelectedProject(Dialog.FileName, 1);
            }
        }

        public void LoadProject(Button Sender)
        {
            if (ProjectLocationInputBox.Text.Length < 6) return;

            if (IsHosting)
            {
                MainScreen.SelectedProject(ProjectLocationInputBox.Text, int.Parse(ClientsInputBox.Text));
            }
            else
            {
                MainScreen.SelectedProject(ProjectLocationInputBox.Text, 1);
            }
        }

        public void ToggleHost(Button Sender)
        {
            IsHosting = !IsHosting;

            if (IsHosting)
            {
                HostOption.BaseTexture = GlobalInterfaceData.TextureLookup[UILookupKey.TickboxTicked];
                ClientsTitle.IsActive = true;
                ClientsInputBox.IsActive = true;
            }
            else
            {
                HostOption.BaseTexture = GlobalInterfaceData.TextureLookup[UILookupKey.TickboxUnticked];
                ClientsTitle.IsActive = false;
                ClientsInputBox.IsActive = false;
            }
        }

        public void SanitiseClientCount(InputBox Sender)
        {
            if (!int.TryParse(ClientsInputBox.Text, out int Result) || Result < 1) ClientsInputBox.Text = "1";
        }

        void MoveLayout()
        {
            Group.X = UIUtils.ConvertFloatToInt(Position.X);
            Group.Y = UIUtils.ConvertFloatToInt(Position.Y);

            Background.Position = position;
            Header.Position = position;
            Title.Position = new Vector2(position.X + 17, position.Y + Header.Bounds.Y * 0.5f);

            ProjectLocationTitle.Position = Position + new Vector2(17, 54);
            ProjectLocationInputBox.Position = Position + new Vector2(16, 80);
            ProjectLocationSelectionButton.Position = Position + new Vector2(392, 80);

            HostOption.Position = Position + new Vector2(16, 150);
            HostLabel.Position = Position + new Vector2(54, 163);

            ClientsTitle.Position = Position + new Vector2(17, 200);
            ClientsInputBox.Position = Position + new Vector2(16, 226);

            LoadButton.Position = Position + new Vector2(16, 630);
        }

        void ResizeLayout()
        {
            Group.Width = bounds.X;
            Group.Height = bounds.Y;

            Background.Bounds = bounds;
            Header.Bounds = new Point(bounds.X, 28);

            //ProjectLocationTitle.Bounds = new Point(300, 42);
            ProjectLocationInputBox.Bounds = new Point(376, 42);
            ProjectLocationSelectionButton.Bounds = new Point(42, 42);

            HostOption.Bounds = new Point(25, 25);
            LoadButton.Bounds = new Point(150, 40);

            ClientsInputBox.Bounds = new Point(300, 42);
        }

        public void Draw(Viewport? BoundPort = null)
        {
            Background.Draw();
            Header.Draw();
            Title.Draw();

            ProjectLocationTitle.Draw();
            ProjectLocationInputBox.Draw();
            ProjectLocationSelectionButton.Draw();

            HostOption.Draw();
            HostLabel.Draw();

            ClientsTitle.Draw();
            ClientsInputBox.Draw();

            LoadButton.Draw();
        }

        public void Close()
        {
            Group.IsMarkedForDeletion = true;
        }

    }
}
