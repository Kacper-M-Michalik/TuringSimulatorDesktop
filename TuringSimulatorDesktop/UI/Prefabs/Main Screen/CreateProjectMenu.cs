using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringSimulatorDesktop.Input;

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

        ActionGroup Group;

        Icon Header;
        Icon Background;
        Label Title;

        Label ProjectTitle;
        InputBox ProjectTitleInputBox;

        Label ProjectLocationTitle;
        ColorButton ProjectLocationSelectionButton;
        InputBox ProjectLocationInputBox;

        Label ProjectSettingsTitle;
        TextureButton EmptyOption;
        TextureButton TemplateOption;

        TextureButton HostOption;
        Label ClientsTitle;
        InputBox ClientsInputBox;

        TextureButton CreateButton;

        MainScreenView MainScreen;

        public CreateProjectMenu(MainScreenView Screen)
        {
            Group = InputManager.CreateActionGroup();
            MainScreen = Screen;

            Header = new Icon(GlobalInterfaceData.Scheme.Header);
            Background = new Icon(GlobalInterfaceData.Scheme.InteractableAccent);

            Title = new Label();
            Title.FontSize = 16f;
            Title.Font = GlobalInterfaceData.StandardRegularFont;
            Title.FontColor = GlobalInterfaceData.Scheme.FontColorBright;
            Title.Text = "Create New Project";



            ProjectTitle = new Label();
            ProjectTitle.AutoSizeMesh = true;
            ProjectTitle.FontSize = 20f;
            ProjectTitle.FontColor = GlobalInterfaceData.Scheme.FontColor;
            ProjectTitle.Text = "Project Title";

            ProjectTitleInputBox = new InputBox(Group);
            ProjectTitleInputBox.BackgroundColor = GlobalInterfaceData.Scheme.Background;
            ProjectTitleInputBox.OutputLabel.DrawCentered = true;
            ProjectTitleInputBox.OutputLabel.FontSize = 20f;
            ProjectTitleInputBox.OutputLabel.FontColor = GlobalInterfaceData.Scheme.FontGrayedOutColor;


            ProjectLocationTitle = new Label();
            ProjectLocationTitle.AutoSizeMesh = true;
            ProjectLocationTitle.FontSize = 20f;
            ProjectLocationTitle.FontColor = GlobalInterfaceData.Scheme.FontColor;
            ProjectLocationTitle.Text = "Save Location";

            ProjectLocationInputBox = new InputBox(Group);
            ProjectLocationInputBox.BackgroundColor = GlobalInterfaceData.Scheme.Background;
            ProjectLocationInputBox.OutputLabel.DrawCentered = true;
            ProjectLocationInputBox.OutputLabel.FontSize = 20f;
            ProjectLocationInputBox.OutputLabel.FontColor = GlobalInterfaceData.Scheme.FontGrayedOutColor;





            ClientsTitle = new Label();
            ClientsTitle.AutoSizeMesh = true;
            ClientsTitle.FontSize = 20f;
            ClientsTitle.FontColor = GlobalInterfaceData.Scheme.FontColor;
            ClientsTitle.Text = "Maximum Clients";

            ClientsInputBox = new InputBox(Group);
            ClientsInputBox.BackgroundColor = GlobalInterfaceData.Scheme.Background;
            ClientsInputBox.OutputLabel.DrawCentered = true;
            ClientsInputBox.OutputLabel.FontSize = 20f;
            ClientsInputBox.OutputLabel.FontColor = GlobalInterfaceData.Scheme.FontColorBright;

        }


        void MoveLayout()
        {
            Group.X = UIUtils.ConvertFloatToInt(Position.X);
            Group.Y = UIUtils.ConvertFloatToInt(Position.Y);

            Background.Position = position;
            Header.Position = position;
            Title.Position = new Vector2(position.X + 17, position.Y + Header.Bounds.Y * 0.5f);

            ProjectTitle.Position = Position + new Vector2(17, 54);
            ProjectTitleInputBox.Position = Position + new Vector2(16, 80);

            ProjectLocationTitle.Position = Position + new Vector2(17, 150);
            ProjectLocationInputBox.Position = Position + new Vector2(16, 176);

            ClientsTitle.Position = Position + new Vector2(17, 480);
            ClientsInputBox.Position = Position + new Vector2(16, 506);
        }

        void ResizeLayout()
        {
            Group.Width = bounds.X;
            Group.Height = bounds.Y;            

            Background.Bounds = bounds;
            Header.Bounds = new Point(bounds.X, 28);

            //ProjectTitle.Bounds = new Point(300, 42);
            ProjectTitleInputBox.Bounds = new Point(300, 42);

            //ProjectLocationTitle.Bounds = new Point(300, 42);
            ProjectLocationInputBox.Bounds = new Point(300, 42);

            ClientsInputBox.Bounds = new Point(300, 42);
        }

        public void Draw(Viewport? BoundPort = null)
        {
            Background.Draw();
            Header.Draw();
            Title.Draw();

            ProjectTitle.Draw();
            ProjectTitleInputBox.Draw();

            ProjectLocationTitle.Draw();
            ProjectLocationInputBox.Draw();

            ClientsTitle.Draw();
            ClientsInputBox.Draw();
        }

        public void Close()
        {
            Group.IsMarkedForDeletion = true;

            //ProjectTitleInputBox.Close();
            //ProjectLocationInputBox.Close();

          //  ProjectLocationSelectionButton.Close();
        }

    }
}
