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

namespace TuringSimulatorDesktop.UI.Prefabs
{    
    public class TextProgrammingView : IView, IPollable, IRunnable, ISaveable
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
                TransitionCanvas.Group.IsActive = isActive;
            }
        }       

        Window ownerWindow;
        public Window OwnerWindow 
        { 
            get => ownerWindow; 
            set => ownerWindow = value; 
        }

        string title = "Empty Programming View";
        public string Title => title;
        
        public Guid OpenFileID => CurrentlyOpenedFileID;

        public bool IsMarkedForDeletion
        {
            get => false;
            set
            {
            }
        }

        ActionGroup Group;


        Icon Background;

        Label DefenitionAlphabetTitle;
        DefenitionAlphabetInputItem DefenitionAlphabetBox;

        Label HaltStatesTitle;
        InputBox HaltStateInputBox;

        DraggableCanvas TransitionCanvas;



        bool FullyLoadedFile;
        Guid CurrentlyOpenedFileID;
        int FileVersion;
        TransitionFile OpenedFile;


        public TextProgrammingView(Guid FileToDisplay)
        {
            Group = InputManager.CreateActionGroup();
            Group.PollableObjects.Add(this);


            Background = new Icon(GlobalInterfaceData.Scheme.Background);

            DefenitionAlphabetTitle = new Label();
            DefenitionAlphabetTitle.Text = "Defenition Alphabet ID:";

            DefenitionAlphabetBox = new DefenitionAlphabetInputItem(Group);
            
            HaltStatesTitle = new Label();
            HaltStatesTitle.Text = "Halt States:";

            HaltStateInputBox = new InputBox(Group);

            TransitionCanvas = new DraggableCanvas();


            IsActive = false;

            CurrentlyOpenedFileID = FileToDisplay;
            SwitchOpenedFile(FileToDisplay);
        }

        public void PollInput(bool IsInActionGroupFrame)
        {
            if ((InputManager.LeftMousePressed || InputManager.RightMousePressed) && OwnerWindow.OwnerScreen.ActiveEditorView != this && IsMouseOver())
            {
                OwnerWindow.OwnerScreen.SetActiveEditorWindow(this);
            }
        }

        public bool IsMouseOver()
        {
            return (IsActive && InputManager.MouseData.X >= Position.X && InputManager.MouseData.X <= Position.X + bounds.X && InputManager.MouseData.Y >= Position.Y && InputManager.MouseData.Y <= Position.Y + bounds.Y);
        }

        public void SwitchOpenedFile(Guid ID)
        {
            FullyLoadedFile = false;
            UIEventManager.Unsubscribe(CurrentlyOpenedFileID, ReceivedStateTransitionFile);
            Client.SendTCPData(ClientSendPacketFunctions.UnsubscribeFromFileUpdates(CurrentlyOpenedFileID));
            CurrentlyOpenedFileID = ID;
            UIEventManager.Subscribe(CurrentlyOpenedFileID, ReceivedStateTransitionFile);
            Client.SendTCPData(ClientSendPacketFunctions.RequestFile(ID, true));
        }

        public void ReceivedStateTransitionFile(object Data)
        {
            CustomLogging.Log("CLIENT: Window received State Transition Data");

            FileDataMessage Message = (FileDataMessage)Data;

            if ((ServerSendPackets)Message.RequestType == ServerSendPackets.SentFileMetadata) return;

            if (Message.GUID != CurrentlyOpenedFileID)
            {
                CustomLogging.Log("CLIENT: State Transition Editor Fatal Error, recived unwanted file data!");
                return;
            }

            title = Message.Name;
            FileVersion = Message.Version;

            try
            {
                OpenedFile = JsonSerializer.Deserialize<TransitionFile>(Message.Data);
            }
            catch
            {
                CustomLogging.Log("CLIENT: Window - Invalid Transition table recieved");
                return;
            }

            //TestLabel.Text = "VERSION: " + FileVersion.ToString() + "/n" + Encoding.ASCII.GetString(Data.ReadByteArray()) + "/n" + OpenedFile.DefinitionAlphabetID;

            DefenitionAlphabetBox.ReferenceFileData = null;
            DefenitionAlphabetBox.FileLabel.Text = "";

            //request metadata
            if (OpenedFile.DefinitionAlphabetFileID != Guid.Empty)
            {
                UIEventManager.Subscribe(OpenedFile.DefinitionAlphabetFileID, ReceiveAlphabetMetaData);
                Client.SendTCPData(ClientSendPacketFunctions.RequestFileMetadata(OpenedFile.DefinitionAlphabetFileID));
            }
            else
            {
                DefenitionAlphabetBox.ReferenceFileData = null;
                DefenitionAlphabetBox.FileLabel.Text = "No Referenced Alphabet";
                FullyLoadedFile = true;
            }


            TransitionCanvas.Clear();

            for (int i = 0; i < OpenedFile.Transitions.Count; i++)
            {
                StateTransitionItem Item = new StateTransitionItem(TransitionCanvas.Group);
                Item.CurrentStateTextBox.Text = OpenedFile.Transitions[i].CurrentState;
                Item.TapeValueTextBox.Text = OpenedFile.Transitions[i].CurrentState;
                Item.NewStateTextBox.Text = OpenedFile.Transitions[i].CurrentState;
                Item.NewTapeValueTextBox.Text = OpenedFile.Transitions[i].CurrentState;

                if (OpenedFile.Transitions[i].MoveDirection == MoveHeadDirection.Left)
                {
                    Item.CurrentStateTextBox.Text = "L";
                }
                else if (OpenedFile.Transitions[i].MoveDirection == MoveHeadDirection.Right)
                {
                    Item.CurrentStateTextBox.Text = "R";
                }
                else
                {
                    Item.CurrentStateTextBox.Text = "";
                }
                
                TransitionCanvas.Elements.Add(Item);
            }

            TransitionCanvas.ApplyMatrices();
        }

        public void ReceiveAlphabetMetaData(object Data)
        {
            CustomLogging.Log("CLIENT: Window received State Alphabet Data");

            FileDataMessage Message = (FileDataMessage)Data;

            if ((ServerSendPackets)Message.RequestType == ServerSendPackets.SentOrUpdatedFile) return;

            if (Message.GUID != OpenedFile.DefinitionAlphabetFileID)
            {
                CustomLogging.Log("CLIENT: State Transition Editor Fatal Error, recived unwanted alphabet file data!");
                return;
            }
            
            //may want to change in future
            UIEventManager.Unsubscribe(OpenedFile.DefinitionAlphabetFileID, ReceiveAlphabetMetaData);

            Guid AlphabetID = Message.GUID;
            string AlphabetFileName = Message.Name;
            int AlphabetFileVersion = Message.Version;

            DefenitionAlphabetBox.ReferenceFileData = new FileData(AlphabetFileName, AlphabetID, CoreFileType.Alphabet);
            DefenitionAlphabetBox.FileLabel.Text = AlphabetFileName;

            //may have to move back 
            FullyLoadedFile = true;
        }

        public void Save()
        {
            if (!FullyLoadedFile)
            {
                return;
            }

            TransitionFile NewFile = new TransitionFile();

            if (DefenitionAlphabetBox.ReferenceFileData != null)
            {
                NewFile.DefinitionAlphabetFileID = DefenitionAlphabetBox.ReferenceFileData.GUID;
            }
            else
            {
                NewFile.DefinitionAlphabetFileID = Guid.Empty;
            }

            NewFile.HaltStates.AddRange(HaltStateInputBox.Text.Split("/n"));

            //for (int i = 0; i < HaltStateInputBoxes.Count; i++)
            //{
            //    NewFile.HaltStates.Add(HaltStateInputBoxes[i].Text);
           // }

            for (int i = 0; i < TransitionCanvas.Elements.Count; i++)
            {
                Transition NewTransition = new Transition();

                StateTransitionItem TransitionSource = (StateTransitionItem)TransitionCanvas.Elements[i];

                NewTransition.X = TransitionSource.Position.X;
                NewTransition.Y = TransitionSource.Position.Y;
                NewTransition.CurrentState = TransitionSource.CurrentStateTextBox.Text;
                NewTransition.TapeValue = TransitionSource.TapeValueTextBox.Text;
                NewTransition.NewState = TransitionSource.NewStateTextBox.Text;
                NewTransition.NewTapeValue = TransitionSource.NewTapeValueTextBox.Text;

                if (TransitionSource.CurrentStateTextBox.Text == "L")
                {
                    NewTransition.MoveDirection = MoveHeadDirection.Left;
                }
                else if (TransitionSource.CurrentStateTextBox.Text == "R")
                {
                    NewTransition.MoveDirection = MoveHeadDirection.Right;
                }
                else
                {
                    NewTransition.MoveDirection = MoveHeadDirection.Empty;
                }

                NewFile.Transitions.Add(NewTransition);
            }

            Client.SendTCPData(ClientSendPacketFunctions.UpdateFile(CurrentlyOpenedFileID, FileVersion, JsonSerializer.SerializeToUtf8Bytes(NewFile, GlobalProjectAndUserData.JsonOptions)));
        }

        void MoveLayout()
        {
            Group.X = UIUtils.ConvertFloatToInt(position.X);
            Group.Y = UIUtils.ConvertFloatToInt(position.Y);

            Background.Position = position;

            DefenitionAlphabetTitle.Position = new Vector2(position.X + 10, position.Y + 10);
            DefenitionAlphabetBox.Position = new Vector2(position.X + 10, position.Y + 30);

            HaltStatesTitle.Position = new Vector2(position.X + 10, position.Y + 60);
            //HaltStatesLayout.Position = new Vector2(position.X + 10, position.Y + 80);
            HaltStateInputBox.Position = new Vector2(position.X + 10, position.Y + 80);
            
            TransitionCanvas.Position = position;
        }

        void ResizeLayout()
        {
            Group.Width = bounds.X;
            Group.Height = bounds.Y;

            Background.Bounds = bounds;

            DefenitionAlphabetTitle.Bounds = new Point(70, 15);
            DefenitionAlphabetBox.Bounds = new Point(70, 15);

            HaltStatesTitle.Bounds = new Point(70, 15);
            //HaltStatesLayout.Bounds = new Point(45, 80);
            HaltStateInputBox.Bounds = new Point(70, 80);
        
        }

        public void Draw(Viewport? BoundPort = null)
        {
            if (IsActive)
            {
                Background.Draw(BoundPort); 
                DefenitionAlphabetTitle.Draw(BoundPort);
                DefenitionAlphabetBox.Draw(BoundPort);

                HaltStatesTitle.Draw(BoundPort);
                HaltStateInputBox.Draw(BoundPort);

            }
        }

        public void Close()
        {
            TransitionCanvas.Close();
            Group.IsMarkedForDeletion = true;
        }
    }
}
