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
using TuringSimulatorDesktop.Networking;

namespace TuringSimulatorDesktop.UI.Prefabs
{
    public class TapeEditorView : IView, ISaveable
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
                Canvas.IsActive = value;
                
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

        string title = "Empty Tape Editor View";
        public string Title => title;
        public Guid OpenFileID => CurrentlyOpenedFileID;

        bool FullyLoadedFile;
        Guid CurrentlyOpenedFileID;
        int FileVersion;
        TapeTemplate OpenedFile;
        Tape ActivelyEditedTape;

        Icon Background;

        TextureButton HelpMenuButton;
        List<Texture2D> HelpMenus;

        DraggableCanvas Canvas;
        TapeVisualItem VisualTape;

        Alphabet EditorAlphabet = new Alphabet()
        {
            EmptyCharacter = ""
        };

        //Constructor
        //Takes in GUID of file to display
        public TapeEditorView(Guid FileToDisplay)
        {
            Background = new Icon(GlobalInterfaceData.Scheme.CanvasBackground);

            Canvas = new DraggableCanvas();

            HelpMenuButton = new TextureButton(Canvas.Group);
            HelpMenuButton.BaseTexture = GlobalInterfaceData.TextureLookup[UILookupKey.HelpButton];

            VisualTape = new TapeVisualItem(Canvas.Group);

            Canvas.Elements.Add(VisualTape);

            IsActive = false;

            if (FileToDisplay != Guid.Empty) SwitchOpenedTape(FileToDisplay);
        }

        public void SwitchOpenedTape(Guid ID)
        {
            FullyLoadedFile = false;
            UIEventManager.Unsubscribe(CurrentlyOpenedFileID, ReceivedTapeData);
            Client.SendTCPData(ClientSendPacketFunctions.UnsubscribeFromFileUpdates(CurrentlyOpenedFileID));
            CurrentlyOpenedFileID = ID;
            UIEventManager.Subscribe(CurrentlyOpenedFileID, ReceivedTapeData);
            Client.SendTCPData(ClientSendPacketFunctions.RequestFile(ID, true));
        }

        public void ReceivedTapeData(object Data)
        {
            CustomLogging.Log("CLIENT: Window received Tape Template Data");

            FileDataMessage Message = (FileDataMessage)Data;

            if ((ServerSendPackets)Message.RequestType == ServerSendPackets.SentFileMetadata) return;

            if (Message.GUID != CurrentlyOpenedFileID)
            {
                CustomLogging.Log("CLIENT: Tape Editor Window Fatal Error, received unwanted file data!");
                return;
            }

            try
            {
                OpenedFile = JsonSerializer.Deserialize<TapeTemplate>(Message.Data);
            }
            catch
            {
                CustomLogging.Log("CLIENT: Window - Invalid Tape Template recieved");
                return;
            }

            title = Message.Name;
            FileVersion = Message.Version;

            //Update UI elements
            ActivelyEditedTape = OpenedFile.Clone(EditorAlphabet);
            VisualTape.SetSourceTape(ActivelyEditedTape);

            FullyLoadedFile = true;
        }

        public void Save()
        {
            if (!FullyLoadedFile)
            {
                return;
            }

            //Generate new TapeTempalte Object
            //Populate with data from UI
            TapeTemplate NewTemplate = new TapeTemplate();

            foreach (KeyValuePair<int, string> Pair in ActivelyEditedTape.Data)
            {
                if (Pair.Value == "")
                {
                    ActivelyEditedTape.Data.Remove(Pair.Key);
                }
            }

            NewTemplate.Data = ActivelyEditedTape.Data;
            NewTemplate.HighestIndex = ActivelyEditedTape.HighestIndex;
            NewTemplate.LowestIndex = ActivelyEditedTape.LowestIndex;

            //Send File Update Request to server
            Client.SendTCPData(ClientSendPacketFunctions.UpdateFile(CurrentlyOpenedFileID, FileVersion, JsonSerializer.SerializeToUtf8Bytes(NewTemplate, GlobalProjectAndUserData.JsonOptions)));
        }

        void MoveLayout()
        {
            Background.Position = position;
            Canvas.Position = position;
            HelpMenuButton.Position = Position + new Vector2(bounds.X - 37, 7);
        }

        void ResizeLayout()
        {
            Background.Bounds = bounds;
            Canvas.Bounds = bounds;
            HelpMenuButton.Bounds = new Point(30, 30);
        }

        public void Draw(Viewport? BoundPort = null)
        {
            if (IsActive)
            {
                Background.Draw(BoundPort);

                //Calculate where the camera/views edges lie on the canvas coordinate space, and feed this data to the tape instance
                //Required for tape to render appropriate tape cells (tape is infinite so rendering every cell is impossible, using previous calculations we can figure out which nodes are visible and the render them when calling the VisualTape objects Draw() function) 
                VisualTape.CameraMin = (Matrix.CreateTranslation(Canvas.Position.X, 0, 0) * Canvas.InverseMatrix).Translation.X;
                VisualTape.CameraMax = (Matrix.CreateTranslation(Canvas.Position.X + Canvas.Bounds.X, 0, 0) * Canvas.InverseMatrix).Translation.X;
                VisualTape.UpdateLayout();
                Canvas.Draw(BoundPort);

                HelpMenuButton.Draw(BoundPort);
            }
        }

        public void Close()
        {
            Canvas.Close();
            IsActive = false;
        }
    }
}
