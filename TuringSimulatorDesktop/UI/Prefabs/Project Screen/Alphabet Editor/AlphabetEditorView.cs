using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TuringCore;
using TuringCore.Files;
using TuringCore.Networking;
using TuringSimulatorDesktop.Debugging;
using TuringSimulatorDesktop.Input;
using TuringSimulatorDesktop.Networking;

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

        //Title used by window for headers
        string title = "Empty Alphabet Editor View";
        public string Title => title;
        public Guid OpenFileID => CurrentlyOpenedFileID;

        ActionGroup Group;

        Icon Background;

        TextureButton HelpMenuButton;
        List<Texture2D> HelpMenus;

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
        
        //Constructor
        //Arguement supplied point to which file to display
        public AlphabetEditorView(Guid FileToDisplay)
        {
            Group = InputManager.CreateActionGroup();

            //Set up UI elements
            Background = new Icon(GlobalInterfaceData.Scheme.Background);

            HelpMenuButton = new TextureButton(Group);
            HelpMenuButton.BaseTexture = GlobalInterfaceData.TextureLookup[UILookupKey.HelpButton];

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

            //Request file if a valid GUID is given
            if (FileToDisplay != Guid.Empty) SwitchOpenedAlphabet(FileToDisplay);
        }

        //Request Alphabet
        public void SwitchOpenedAlphabet(Guid ID)
        {
            //Mark as waiting for response and unsubscribe from any previous file loaded
            FullyLoadedFile = false;
            UIEventManager.Unsubscribe(CurrentlyOpenedFileID, ReceivedAlphabetData);
            Client.SendTCPData(ClientSendPacketFunctions.UnsubscribeFromFileUpdates(CurrentlyOpenedFileID));
            CurrentlyOpenedFileID = ID;
            //Subscribe to UI event for when response arrives
            UIEventManager.Subscribe(CurrentlyOpenedFileID, ReceivedAlphabetData);
            //Send request to server
            Client.SendTCPData(ClientSendPacketFunctions.RequestFile(ID, true));
        }

        //Processing response
        public void ReceivedAlphabetData(object Data)
        {
            CustomLogging.Log("CLIENT: Window received Alphabet Data");

            FileDataMessage Message = (FileDataMessage)Data;

            //Ensure response is one with file data, not just metadata
            if ((ServerSendPackets)Message.RequestType == ServerSendPackets.SentFileMetadata) return;

            //Secuirty check, make sure file received is the one requested 
            if (Message.GUID != CurrentlyOpenedFileID)
            {
                CustomLogging.Log("CLIENT: Alphabet Editor Window Fatal Error, received unwanted file data!");
                return;
            }

            //Security check, check received JSON object is valid, if not, discard the response
            try
            {
                OpenedFile = JsonSerializer.Deserialize<Alphabet>(Message.Data);
            }
            catch
            {
                //Request file again
                Client.SendTCPData(ClientSendPacketFunctions.RequestFile(CurrentlyOpenedFileID, true));
                CustomLogging.Log("CLIENT: Window - Invalid Alphabet received");
                return;
            }

            //Update UI with newly received alphabet model
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
            if (Builder.Length > 0) Builder.Remove(Builder.Length - 2, 2);
            CharacterInputItem.Text = Builder.ToString();

            //Finish loading the file
            FullyLoadedFile = true;
        }

        //Save current alphabet file
        public void Save()
        {
            if (!FullyLoadedFile)
            {
                return;
            }

            //Generate new JSON object based on user inputs
            Alphabet NewAlphabet = new Alphabet();
            NewAlphabet.EmptyCharacter = EmptyCharacterInputBox.Text;
            NewAlphabet.WildcardCharacter = WildcardCharacterInputBox.Text;

            HashSet<string> AllowedCharacters = new HashSet<string>();

            string[] Symbols = CharacterInputItem.Text.Split("/n");
            for (int i = 0; i < Symbols.Length; i++)
            {
                AllowedCharacters.Add(Symbols[i]);
            }
            //Error correction, make sure the symbols the user set as empty and wildcard symbols are indeed contained in the alphabet definition set
            AllowedCharacters.Add(EmptyCharacterInputBox.Text);
            AllowedCharacters.Add(WildcardCharacterInputBox.Text);

            NewAlphabet.Characters = AllowedCharacters;

            //Send file update request o server with new JSON object data
            Client.SendTCPData(ClientSendPacketFunctions.UpdateFile(CurrentlyOpenedFileID, FileVersion, JsonSerializer.SerializeToUtf8Bytes(NewAlphabet, GlobalProjectAndUserData.JsonOptions)));
        }

        //Update UI element positions on screen whenever this view is moved
        void MoveLayout()
        {
            Group.X = UIUtils.ConvertFloatToInt(position.X);
            Group.Y = UIUtils.ConvertFloatToInt(position.Y);

            Background.Position = position;

            HelpMenuButton.Position = Position + new Vector2(bounds.X - 37, 7);

            EmptyCharacterTitle.Position = position + GlobalInterfaceData.Scale(new Vector2(22, 18));
            EmptyCharacterInputBox.Position = position + GlobalInterfaceData.Scale(new Vector2(20, 30));

            WildcardCharacterTitle.Position = position + GlobalInterfaceData.Scale(new Vector2(22, 82));
            WildcardCharacterInputBox.Position = position + GlobalInterfaceData.Scale(new Vector2(20, 94));


            AllowedCharactersTitle.Position = position + GlobalInterfaceData.Scale(new Vector2(185, 18));
            CharacterInputItem.Position = position + GlobalInterfaceData.Scale(new Vector2(183, 30));
        }

        //Update UI element sizes on screen whenever this view is resized
        void ResizeLayout()
        {
            Group.Width = bounds.X;
            Group.Height = bounds.Y;

            Background.Bounds = bounds;

            HelpMenuButton.Bounds = new Point(30, 30);

            EmptyCharacterInputBox.Bounds = GlobalInterfaceData.Scale(new Point(120, 30));
            WildcardCharacterInputBox.Bounds = GlobalInterfaceData.Scale(new Point(120, 30));
            CharacterInputItem.Bounds = GlobalInterfaceData.Scale(new Point(300, 500));
        }

        //Draw view
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

                HelpMenuButton.Draw(BoundPort);
            }
        }

        //Shut down view resources
        public void Close()
        {
            Group.IsMarkedForDeletion = true;
        }
    }
}
