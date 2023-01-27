using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TuringCore;

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

        DraggableCanvas Canvas;
        TapeVisualItem VisualTape;

        Alphabet EditorAlphabet = new Alphabet()
        {
            EmptyCharacter = ""
        };

        public TapeEditorView(Guid FileToDisplay)
        {
            Canvas = new DraggableCanvas();

            VisualTape = new TapeVisualItem(Canvas.Group);

            Canvas.Elements.Add(VisualTape);

            IsActive = false;

            SwitchOpenedTape(FileToDisplay);
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

        public void ReceivedTapeData(Packet Data)
        {
            CustomLogging.Log("CLIENT: Window received Tape Template Data");

            if (Data.ReadGuid() != CurrentlyOpenedFileID)
            {
                CustomLogging.Log("CLIENT: Tape Editor Window Fatal Error, recived unwanted file data!");
                return;
            }

            Data.ReadInt();
            title = Data.ReadString();
            FileVersion = Data.ReadInt();

            try
            {
                OpenedFile = JsonSerializer.Deserialize<TapeTemplate>(Data.ReadByteArray());
            }
            catch
            {
                CustomLogging.Log("CLIENT: Window - Invalid Tape Template recieved");
                return;
            }
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

            TapeTemplate NewTemplate = new TapeTemplate();

            NewTemplate.FileID = CurrentlyOpenedFileID;
            NewTemplate.Data = ActivelyEditedTape.Data;
            NewTemplate.HighestIndex = ActivelyEditedTape.HighestIndex;
            NewTemplate.LowestIndex = ActivelyEditedTape.LowestIndex;

            Client.SendTCPData(ClientSendPacketFunctions.UpdateFile(FileVersion, JsonSerializer.SerializeToUtf8Bytes(NewTemplate, GlobalProjectAndUserData.JsonOptions)));
        }

        void MoveLayout()
        {
            Canvas.Position = position;
        }

        void ResizeLayout()
        {            
            Canvas.Bounds = bounds;
        }

        public void Draw(Viewport? BoundPort = null)
        {
            if (IsActive)
            {
                VisualTape.CameraMin = (Matrix.CreateTranslation(Canvas.Position.X, 0, 0) * Canvas.InverseMatrix).Translation.X;
                VisualTape.CameraMax = (Matrix.CreateTranslation(Canvas.Position.X + Canvas.Bounds.X, 0, 0) * Canvas.InverseMatrix).Translation.X;
                VisualTape.UpdateLayout();
                Canvas.Draw();
            }
        }

        public void Close()
        {
            Canvas.Close();
            IsActive = false;
        }
    }
}
