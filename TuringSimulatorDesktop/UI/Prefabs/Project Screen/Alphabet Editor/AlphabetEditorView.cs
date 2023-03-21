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
    public class AlphabetEditorView : IView, ISaveable
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
        public Guid OpenFileID => CurrentlyOpenedFileID;

        ActionGroup Group;

        Icon Background;

        Label EmptyCharacterTitle;
        Label WildcardCharacterTitle;
        Label AllowedCharactersTitle;

        InputBox EmptyCharacterInputBox;
        InputBox WildcardCharacterInputBox;

        InputBox CharacterInputItem;

        bool FullyLoadedFile;
        Guid CurrentlyOpenedFileID;
        int FileVersion;
        Alphabet OpenedFile;

        public bool IsMarkedForDeletion
        {
            get => false;
            set
            {
            }
        }

        public AlphabetEditorView(Guid FileToDisplay)
        {
            Group = InputManager.CreateActionGroup();

            Background = new Icon(GlobalInterfaceData.Scheme.Background);

            EmptyCharacterTitle = new Label();
            EmptyCharacterTitle.FontSize = GlobalInterfaceData.Scale(12);
            EmptyCharacterTitle.FontColor = GlobalInterfaceData.Scheme.FontGrayedOutColor;
            EmptyCharacterTitle.Text = "Empty Character";

            EmptyCharacterInputBox = new InputBox(Group);
            EmptyCharacterInputBox.BackgroundColor = GlobalInterfaceData.Scheme.InteractableAccent;
            EmptyCharacterInputBox.OutputLabel.FontSize = GlobalInterfaceData.Scale(20);
            EmptyCharacterInputBox.OutputLabel.FontColor = GlobalInterfaceData.Scheme.FontColor;
            EmptyCharacterInputBox.OutputLabel.Text = "";
            EmptyCharacterInputBox.Modifiers.AllowsNewLine = false;

            WildcardCharacterTitle = new Label();
            WildcardCharacterTitle.FontSize = GlobalInterfaceData.Scale(12);
            WildcardCharacterTitle.FontColor = GlobalInterfaceData.Scheme.FontGrayedOutColor;
            WildcardCharacterTitle.Text = "Wildcard Character";

            WildcardCharacterInputBox = new InputBox(Group);
            WildcardCharacterInputBox.BackgroundColor = GlobalInterfaceData.Scheme.InteractableAccent;
            WildcardCharacterInputBox.OutputLabel.FontSize = GlobalInterfaceData.Scale(20);
            WildcardCharacterInputBox.OutputLabel.FontColor = GlobalInterfaceData.Scheme.FontColor;
            WildcardCharacterInputBox.OutputLabel.Text = "*";
            WildcardCharacterInputBox.Modifiers.AllowsNewLine = false;

            AllowedCharactersTitle = new Label();
            AllowedCharactersTitle.FontSize = GlobalInterfaceData.Scale(12);
            AllowedCharactersTitle.FontColor = GlobalInterfaceData.Scheme.FontGrayedOutColor;
            AllowedCharactersTitle.Text = "Allowed Characters";

            CharacterInputItem = new InputBox(Group);
            CharacterInputItem.BackgroundColor = GlobalInterfaceData.Scheme.InteractableAccent;


            IsActive = false;

            if (FileToDisplay != Guid.Empty) SwitchOpenedAlphabet(FileToDisplay);
        }

        public void SwitchOpenedAlphabet(Guid ID)
        {
            FullyLoadedFile = false;
            UIEventManager.Unsubscribe(CurrentlyOpenedFileID, ReceivedAlphabetData);
            Client.SendTCPData(ClientSendPacketFunctions.UnsubscribeFromFileUpdates(CurrentlyOpenedFileID));
            CurrentlyOpenedFileID = ID;
            UIEventManager.Subscribe(CurrentlyOpenedFileID, ReceivedAlphabetData);
            Client.SendTCPData(ClientSendPacketFunctions.RequestFile(ID, true));
        }

        public void ReceivedAlphabetData(object Data)
        {
            CustomLogging.Log("CLIENT: Window received Alphabet Data");

            FileDataMessage Message = (FileDataMessage)Data;

            if ((ServerSendPackets)Message.RequestType == ServerSendPackets.SentFileMetadata) return;

            if (Message.GUID != CurrentlyOpenedFileID)
            {
                CustomLogging.Log("CLIENT: Alphabet Editor Window Fatal Error, recived unwanted file data!");
                return;
            }

            try
            {
                OpenedFile = JsonSerializer.Deserialize<Alphabet>(Message.Data);
            }
            catch
            {
                CustomLogging.Log("CLIENT: Window - Invalid Alphabet recieved");
                return;
            }

            title = Message.Name;
            FileVersion = Message.Version;

            EmptyCharacterInputBox.Text = OpenedFile.EmptyCharacter;
            WildcardCharacterInputBox.Text = OpenedFile.WildcardCharacter;

            StringBuilder Builder = new StringBuilder();
            foreach (string Character in OpenedFile.Characters)
            {
                Builder.Append(Character);
                Builder.Append("/n");
            }
            Builder.Remove(Builder.Length - 2, 2);
            CharacterInputItem.Text = Builder.ToString();
            FullyLoadedFile = true;
        }

        public void Save()
        {
            if (!FullyLoadedFile)
            {
                return;
            }

            Alphabet NewAlphabet = new Alphabet();
            NewAlphabet.EmptyCharacter = EmptyCharacterInputBox.Text;
            NewAlphabet.WildcardCharacter = WildcardCharacterInputBox.Text;

            HashSet<string> AllowedCharacters = new HashSet<string>();

            string[] Symbols = CharacterInputItem.Text.Split("/n");
            for (int i = 0; i < Symbols.Length; i++)
            {
                AllowedCharacters.Add(Symbols[i]);
            }
            AllowedCharacters.Add(EmptyCharacterInputBox.Text);
            AllowedCharacters.Add(WildcardCharacterInputBox.Text);

            NewAlphabet.Characters = AllowedCharacters;

            Client.SendTCPData(ClientSendPacketFunctions.UpdateFile(CurrentlyOpenedFileID, FileVersion, JsonSerializer.SerializeToUtf8Bytes(NewAlphabet, GlobalProjectAndUserData.JsonOptions)));
        }

        void MoveLayout()
        {
            Group.X = UIUtils.ConvertFloatToInt(position.X);
            Group.Y = UIUtils.ConvertFloatToInt(position.Y);

            Background.Position = position;

            EmptyCharacterTitle.Position = position + GlobalInterfaceData.Scale(new Vector2(22, 18));
            EmptyCharacterInputBox.Position = position + GlobalInterfaceData.Scale(new Vector2(20, 30));

            WildcardCharacterTitle.Position = position + GlobalInterfaceData.Scale(new Vector2(22, 82));
            WildcardCharacterInputBox.Position = position + GlobalInterfaceData.Scale(new Vector2(20, 94));


            AllowedCharactersTitle.Position = position + GlobalInterfaceData.Scale(new Vector2(185, 18));
            CharacterInputItem.Position = position + GlobalInterfaceData.Scale(new Vector2(183, 30));
        }

        void ResizeLayout()
        {
            Group.Width = bounds.X;
            Group.Height = bounds.Y;

            Background.Bounds = bounds;

            EmptyCharacterInputBox.Bounds = GlobalInterfaceData.Scale(new Point(120, 30));
            WildcardCharacterInputBox.Bounds = GlobalInterfaceData.Scale(new Point(120, 30));
            CharacterInputItem.Bounds = GlobalInterfaceData.Scale(new Point(300, 500));
        }

        public void Draw(Viewport? BoundPort = null)
        {
            if (IsActive)
            {
                Background.Draw(BoundPort);

                EmptyCharacterTitle.Draw(BoundPort);
                EmptyCharacterInputBox.Draw(BoundPort);

                WildcardCharacterTitle.Draw(BoundPort);
                WildcardCharacterInputBox.Draw(BoundPort);

                AllowedCharactersTitle.Draw(BoundPort);
                CharacterInputItem.Draw(BoundPort);
            }
        }

        public void Close()
        {
            Group.IsMarkedForDeletion = true;
        }
    }
}
