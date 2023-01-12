using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using TuringCore;
using TuringSimulatorDesktop.Input;
using TuringSimulatorDesktop.UI;
using TuringSimulatorDesktop.UI.Prefabs;
using TuringCore;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TuringSimulatorDesktop.UI
{    
    public class TextProgrammingView : IView
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
                TransitionLayout.Group.IsActive = isActive;
            }
        }       
        
        Icon Background;
        VerticalLayoutBox TransitionLayout;
        Label TestLabel;
        ActionGroup Group;

        string title = "Empty Programming View";
        public string Title => title;
        Window ownerWindow;
        public Window OwnerWindow 
        { 
            get => ownerWindow; 
            set => ownerWindow = value; 
        }

        int CurrentlyOpenedFileID;
        int FileVersion;
        TransitionFile OpenedFile;

        public TextProgrammingView(int FileToDisplay)
        {
            Group = InputManager.CreateActionGroup();

            Background = new Icon(GlobalInterfaceData.Scheme.Background);
            TransitionLayout = new VerticalLayoutBox();
            TransitionLayout.Scrollable = true;
            TransitionLayout.Spacing = 5f;

            TestLabel = new Label();

            IsActive = false;

            CurrentlyOpenedFileID = FileToDisplay;
            SwitchOpenedFile(FileToDisplay);
        }

        public void LoadStateTransitionTable(Packet Data)
        {            
            //file id
            Data.ReadInt();

            if ((CreateFileType)Data.ReadInt() != CreateFileType.TransitionFile) throw new Exception("Opened File is not a transition file!");

            title = Data.ReadString();
            FileVersion = Data.ReadInt();
            try
            {
                OpenedFile = JsonSerializer.Deserialize<TransitionFile>(Data.ReadByteArray(false));
            }
            catch
            {
                CustomLogging.Log("CLIENT: Window - Invalid Transition table recieved");
                return;
            }

            TestLabel.Text = "VERSION: " + FileVersion.ToString() + "/n" + Encoding.ASCII.GetString(Data.ReadByteArray()) + "/n" + OpenedFile.DefinitionAlphabetID;

            for (int i = 0; i < OpenedFile.Transitions.Count; i++)
            {
                //add transitions here
            }
        }

        public void SwitchOpenedFile(int ID)
        {
            UIEventManager.Unsubscribe(CurrentlyOpenedFileID, LoadStateTransitionTable);
            Client.SendTCPData(ClientSendPacketFunctions.UnsubscribeFromFileUpdates(CurrentlyOpenedFileID));
            CurrentlyOpenedFileID = ID;
            UIEventManager.Subscribe(CurrentlyOpenedFileID, LoadStateTransitionTable);
            Client.SendTCPData(ClientSendPacketFunctions.RequestFile(ID, true));
        }

        void MoveLayout()
        {
            Group.X = UIUtils.ConvertFloatToInt(position.X);
            Group.Y = UIUtils.ConvertFloatToInt(position.Y);

            Background.Position = position;
            TransitionLayout.Position = new Vector2(position.X, position.Y + 20);
            TestLabel.Position = position;
        }

        void ResizeLayout()
        {
            Group.Width = bounds.X;
            Group.Height = bounds.Y;

            Background.Bounds = bounds;
            TransitionLayout.Bounds = new Point(bounds.X, bounds.Y - 20);
            TestLabel.Bounds = bounds;
        }

        public void Draw(Viewport? BoundPort = null)
        {
            if (IsActive)
            {
                Background.Draw(BoundPort);
                TestLabel.Draw(BoundPort);
                TransitionLayout.Draw(BoundPort);
            }
        }

        public void Close()
        {
            TransitionLayout.Close();
            Group.IsMarkedForDeletion = true;
        }
    }
}
