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
        public int OpenFileID => CurrentlyOpenedFileID;

        bool FullyLoadedFile;
        int CurrentlyOpenedFileID;
        int FileVersion;
        TapeTemplate OpenedFile;

        DraggableCanvas Canvas;
        TapeVisualItem VisualTape;


        public TapeEditorView(int FileToDisplay)
        {
            Canvas = new DraggableCanvas();

            VisualTape = new TapeVisualItem(Canvas.Group);

            Canvas.Elements.Add(VisualTape);

            IsActive = false;

            CurrentlyOpenedFileID = FileToDisplay;
            //call switch here
        }

        public void SwitchOpenedTape(int ID)
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

            if (Data.ReadInt() != CurrentlyOpenedFileID)
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

            //VisualTape.SetTapeData(OpenedFile.Clone());

            FullyLoadedFile = true;
        }

        public void Save()
        {

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
                VisualTape.CameraMin = (Matrix.CreateTranslation(Position.X, 0, 0) * Canvas.InverseMatrix).Translation.X;
                VisualTape.CameraMax = (Matrix.CreateTranslation(Position.X + Bounds.X, 0, 0) * Canvas.InverseMatrix).Translation.X;
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
