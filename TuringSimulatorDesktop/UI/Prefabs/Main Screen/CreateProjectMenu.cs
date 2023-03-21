﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringSimulatorDesktop.Input;
using TuringServer;
using System.Windows.Forms;
using System.IO;

namespace TuringSimulatorDesktop.UI
{
    public class CreateProjectMenu : IVisualElement, IClosable
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
        TextureButton ProjectLocationSelectionButton;
        InputBox ProjectLocationInputBox;

        Label ProjectSettingsTitle;
        TextureButton EmptyOption;
        TextureButton TemplateOption;

        TextureButton HostOption;
        Label HostLabel;

        Label ClientsTitle;
        InputBox ClientsInputBox;

        TextureButton CreateButton;

        MainScreenView MainScreen;

        bool IsHosting = false;
        bool IsEmptyProject = true;

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

            ProjectLocationSelectionButton = new TextureButton(Group);
            ProjectLocationSelectionButton.BaseTexture = GlobalInterfaceData.TextureLookup[UILookupKey.LoadFile];
            ProjectLocationSelectionButton.HighlightOnMouseOver = false;
            ProjectLocationSelectionButton.OnClickedEvent += SelectLocation;

            ProjectSettingsTitle = new Label();
            ProjectSettingsTitle.AutoSizeMesh = true;
            ProjectSettingsTitle.FontSize = 20f;
            ProjectSettingsTitle.FontColor = GlobalInterfaceData.Scheme.FontColor;
            ProjectSettingsTitle.Text = "Project Settings";

            EmptyOption = new TextureButton(Group);
            EmptyOption.BaseTexture = GlobalInterfaceData.TextureLookup[UILookupKey.EmptyProjectSelected];
            EmptyOption.OnClickedEvent += SelectEmptyProject;

            TemplateOption = new TextureButton(Group);
            TemplateOption.BaseTexture = GlobalInterfaceData.TextureLookup[UILookupKey.TemplateProject];
            TemplateOption.OnClickedEvent += SelectTemplateProject;

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
            ClientsInputBox.BackgroundColor = GlobalInterfaceData.Scheme.Background;
            ClientsInputBox.OutputLabel.DrawCentered = true;
            ClientsInputBox.OutputLabel.FontSize = 20f;
            ClientsInputBox.OutputLabel.FontColor = GlobalInterfaceData.Scheme.FontColor;
            ClientsInputBox.Text = "Count";
            ClientsInputBox.IsActive = false;
            ClientsInputBox.ClickEvent += ClearCount;
            ClientsInputBox.ClickAwayEvent += ClearCount;
            ClientsInputBox.EditEvent += SanitiseClientCount;

            CreateButton = new TextureButton(Group);
            CreateButton.BaseTexture = GlobalInterfaceData.TextureLookup[UILookupKey.CreateButton];
            CreateButton.HighlightTexture = GlobalInterfaceData.TextureLookup[UILookupKey.CreateButtonHighlight];
            CreateButton.HighlightOnMouseOver = true;
            CreateButton.OnClickedEvent += CreateProject;
        }

        public void SelectLocation(Button Sender)
        {            
            FolderBrowserDialog Dialog = new FolderBrowserDialog
            {
                InitialDirectory = @"C:\",
            };

            if (Dialog.ShowDialog() == DialogResult.OK)
            {
                ProjectLocationInputBox.Text = Dialog.SelectedPath;
                //MainScreen.SelectedProject(Dialog.FileName, 1);
            }
        }

        public void SelectEmptyProject(Button Sender)
        {
            IsEmptyProject = true;
            EmptyOption.BaseTexture = GlobalInterfaceData.TextureLookup[UILookupKey.EmptyProjectSelected];
            TemplateOption.BaseTexture = GlobalInterfaceData.TextureLookup[UILookupKey.TemplateProject];
        }

        public void SelectTemplateProject(Button Sender)
        {
            IsEmptyProject = false;
            EmptyOption.BaseTexture = GlobalInterfaceData.TextureLookup[UILookupKey.EmptyProject];
            TemplateOption.BaseTexture = GlobalInterfaceData.TextureLookup[UILookupKey.TemplateProjectSelected];
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

        public void CreateProject(Button Sender)
        {
            CreateProjectReturnData Result = FileManager.CreateProject(ProjectTitleInputBox.Text, ProjectLocationInputBox.Text, TuringCore.TuringProjectType.NonClassical);
         
            if (!Result.Success)
            {
                throw new Exception("Failed To create new project!");
            }

            if (!IsEmptyProject)
            {
                string Dir = ProjectLocationInputBox.Text + Path.DirectorySeparatorChar + ProjectTitleInputBox.Text + Path.DirectorySeparatorChar + ProjectTitleInputBox.Text + "Data" + Path.DirectorySeparatorChar + "Templates";
                Directory.CreateDirectory(Dir);

                string[] Templates = Directory.GetFiles(Environment.CurrentDirectory + @"\Assets\Templates\");
                    
                foreach (string TemplatePath in Templates)
                {
                    string FileName = TemplatePath.Substring(TemplatePath.LastIndexOf("\\"));
                    FileStream Stream = File.Create(Dir + FileName);
                    Stream.Write(File.ReadAllBytes(TemplatePath));
                    Stream.Close();
                }
            }

            if (IsHosting)
            {
                MainScreen.SelectedProject(Result.SolutionPath, int.Parse(ClientsInputBox.Text));               
            }
            else
            {
                MainScreen.SelectedProject(Result.SolutionPath, 1);
            }
        }

        public void ClearCount(InputBox Sender)
        {
            if (!int.TryParse(ClientsInputBox.Text, out int Result)) ClientsInputBox.Text = "Count";
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

            ProjectTitle.Position = Position + new Vector2(17, 54);
            ProjectTitleInputBox.Position = Position + new Vector2(16, 80);

            ProjectLocationTitle.Position = Position + new Vector2(17, 150);
            ProjectLocationInputBox.Position = Position + new Vector2(16, 176);
            ProjectLocationSelectionButton.Position = Position + new Vector2(316, 176);

            ProjectSettingsTitle.Position = position + new Vector2(17, 246);

            EmptyOption.Position = Position + new Vector2(16, 272);
            TemplateOption.Position = Position + new Vector2(232, 272);

            HostOption.Position = Position + new Vector2(16, 410);
            HostLabel.Position = Position + new Vector2(54, 423);

            ClientsTitle.Position = Position + new Vector2(17, 460);
            ClientsInputBox.Position = Position + new Vector2(16, 486);

            //create  utton label
            CreateButton.Position = Position + new Vector2(16, 630);
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
            ProjectLocationSelectionButton.Bounds = new Point(42, 42);

            EmptyOption.Bounds = new Point(200, 105);
            TemplateOption.Bounds = new Point(200, 105);
            HostOption.Bounds = new Point(25, 25);
            CreateButton.Bounds = new Point(150, 40);

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
            ProjectLocationSelectionButton.Draw();

            ProjectSettingsTitle.Draw();
            EmptyOption.Draw();
            TemplateOption.Draw();

            HostOption.Draw();
            HostLabel.Draw();

            ClientsTitle.Draw();
            ClientsInputBox.Draw();

            CreateButton.Draw();
        }

        public void Close()
        {
            Group.IsMarkedForDeletion = true;
        }

    }
}
