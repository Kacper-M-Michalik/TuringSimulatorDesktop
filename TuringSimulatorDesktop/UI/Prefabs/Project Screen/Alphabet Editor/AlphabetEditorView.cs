using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TuringCore;
using TuringSimulatorDesktop.Input;

namespace TuringSimulatorDesktop.UI.Prefabs
{
    public class AlphabetEditorView : IView
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
                Group.IsActive = isActive;

                if (IsActive && OwnerWindow != null)
                {
                    //OwnerWindow.OwnerScreen.
                }
            }
        }

        Window ownerWindow;
        public Window OwnerWindow
        {
            get => ownerWindow;
            set => ownerWindow = value;
        }

        string title = "Empty Alphabet Editor View";
        public string Title => title;
        public int OpenFileID => CurrentlyOpenedFileID;

        ActionGroup Group;

        Icon Background;

        Label DefenitionIDTitle;
        Label EmptyCharacterTitle;
        Label WildcardCharacterTitle;
        Label AllowedCharactersTitle;

        InputBox DefenitionIDInputBox;
        InputBox EmptyCharacterInputBox;
        InputBox WildcardCharacterInputBox;
        InputBox AllowedCharactersInputBox;

        int CurrentlyOpenedFileID;
        int FileVersion;
        Alphabet OpenedFile;

        public bool IsMarkedForDeletion
        {
            get => false;
            set
            {
            }
        }

        public AlphabetEditorView(int FileToDisplay)
        {
            Group = InputManager.CreateActionGroup();

            Background = new Icon(GlobalInterfaceData.Scheme.Background);

            DefenitionIDTitle = new Label();
            DefenitionIDTitle.FontSize = GlobalInterfaceData.Scale(10);
            DefenitionIDTitle.FontColor = GlobalInterfaceData.Scheme.FontGrayedOutColor;
            DefenitionIDTitle.Text = "Alphabet ID";

            DefenitionIDInputBox = new InputBox(Group);
            DefenitionIDInputBox.OutputLabel.FontSize = GlobalInterfaceData.Scale(20);
            DefenitionIDInputBox.OutputLabel.FontColor = GlobalInterfaceData.Scheme.FontColor;
            DefenitionIDInputBox.OutputLabel.Text = "-";
            DefenitionIDInputBox.Modifiers.AllowsNewLine = false;

            EmptyCharacterTitle = new Label();
            EmptyCharacterTitle.FontSize = GlobalInterfaceData.Scale(10);
            EmptyCharacterTitle.FontColor = GlobalInterfaceData.Scheme.FontGrayedOutColor;
            EmptyCharacterTitle.Text = "Empty Character";

            EmptyCharacterInputBox = new InputBox(Group);
            EmptyCharacterInputBox.OutputLabel.FontSize = GlobalInterfaceData.Scale(20);
            EmptyCharacterInputBox.OutputLabel.FontColor = GlobalInterfaceData.Scheme.FontColor;
            EmptyCharacterInputBox.OutputLabel.Text = "-";
            EmptyCharacterInputBox.Modifiers.AllowsNewLine = false;

            WildcardCharacterTitle = new Label();
            WildcardCharacterTitle.FontSize = GlobalInterfaceData.Scale(10);
            WildcardCharacterTitle.FontColor = GlobalInterfaceData.Scheme.FontGrayedOutColor;
            WildcardCharacterTitle.Text = "Wildcard Character";

            WildcardCharacterInputBox = new InputBox(Group);
            WildcardCharacterInputBox.OutputLabel.FontSize = GlobalInterfaceData.Scale(20);
            WildcardCharacterInputBox.OutputLabel.FontColor = GlobalInterfaceData.Scheme.FontColor;
            WildcardCharacterInputBox.OutputLabel.Text = "-";
            WildcardCharacterInputBox.Modifiers.AllowsNewLine = false;

            AllowedCharactersTitle = new Label();
            AllowedCharactersTitle.FontSize = GlobalInterfaceData.Scale(20);
            AllowedCharactersTitle.FontColor = GlobalInterfaceData.Scheme.FontGrayedOutColor;
            AllowedCharactersTitle.Text = "Allowed Characters";

            AllowedCharactersInputBox = new InputBox(Group);
            AllowedCharactersInputBox.OutputLabel.FontSize = GlobalInterfaceData.Scale(12);
            AllowedCharactersInputBox.OutputLabel.FontColor = GlobalInterfaceData.Scheme.FontColor;
            AllowedCharactersInputBox.OutputLabel.Text = "-";

            IsActive = false;

            SwitchOpenedAlphabet(FileToDisplay);

            CurrentlyOpenedFileID = FileToDisplay;
        }

        public void SwitchOpenedAlphabet(int ID)
        {
            UIEventManager.Unsubscribe(CurrentlyOpenedFileID, AlphabetUpdated);
            Client.SendTCPData(ClientSendPacketFunctions.UnsubscribeFromFolderUpdates(CurrentlyOpenedFileID));
            CurrentlyOpenedFileID = ID;
            UIEventManager.Subscribe(CurrentlyOpenedFileID, AlphabetUpdated);
            Client.SendTCPData(ClientSendPacketFunctions.RequestFolderData(ID, true));
        }

        public void AlphabetUpdated(Packet Data)
        {
            CustomLogging.Log("CLIENT: Window received Alphabet Data");
            
            if (Data.ReadInt() != CurrentlyOpenedFileID)
            {
                CustomLogging.Log("CLIENT: Alphabet Editor Window Fatal Error, recived unwated file data!");
                return;
            }

            title = Data.ReadString();
            FileVersion = Data.ReadInt();

            try
            {
                OpenedFile = JsonSerializer.Deserialize<Alphabet>(Data.ReadByteArray());
            }
            catch
            {
                CustomLogging.Log("CLIENT: Window - Invalid Alphabet recieved");
                return;
            }
        }

        void MoveLayout()
        {
            Group.X = UIUtils.ConvertFloatToInt(position.X);
            Group.Y = UIUtils.ConvertFloatToInt(position.Y);

            Background.Position = position;

            DefenitionIDTitle.Position = position + GlobalInterfaceData.Scale(new Vector2());
            EmptyCharacterTitle.Position = position + GlobalInterfaceData.Scale(new Vector2());
            WildcardCharacterTitle.Position = position + GlobalInterfaceData.Scale(new Vector2());
            AllowedCharactersTitle.Position = position + GlobalInterfaceData.Scale(new Vector2());

            DefenitionIDInputBox.Position = position + GlobalInterfaceData.Scale(new Vector2());
            EmptyCharacterInputBox.Position = position + GlobalInterfaceData.Scale(new Vector2());
            WildcardCharacterInputBox.Position = position + GlobalInterfaceData.Scale(new Vector2());
            AllowedCharactersInputBox.Position = position + GlobalInterfaceData.Scale(new Vector2());
        }

        void ResizeLayout()
        {
            Group.Width = bounds.X;
            Group.Height = bounds.Y;

            Background.Bounds = bounds;

            DefenitionIDInputBox.Bounds = GlobalInterfaceData.Scale(new Point(10,10));
            EmptyCharacterInputBox.Bounds = GlobalInterfaceData.Scale(new Point(10, 10));
            WildcardCharacterInputBox.Bounds = GlobalInterfaceData.Scale(new Point(10, 10));
            AllowedCharactersInputBox.Bounds = GlobalInterfaceData.Scale(new Point(10, 10));
        }

        public void Draw(Viewport? BoundPort = null)
        {
            if (IsActive)
            {
                Background.Draw(BoundPort);

                DefenitionIDTitle.Draw(BoundPort);
                EmptyCharacterTitle.Draw(BoundPort);
                WildcardCharacterTitle.Draw(BoundPort);
                AllowedCharactersTitle.Draw(BoundPort);

                DefenitionIDInputBox.Draw(BoundPort);
                EmptyCharacterInputBox.Draw(BoundPort);
                WildcardCharacterInputBox.Draw(BoundPort);
                AllowedCharactersInputBox.Draw(BoundPort);
            }
        }

        public void Close()
        {
            
        }


    }
}
