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
                Group.IsActive = value;
                TransitionCanvas.IsActive = value;
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

        public DraggableCanvas TransitionCanvas;

        bool FullyLoadedFile;
        Guid CurrentlyOpenedFileID;
        int FileVersion;
        TransitionFile OpenedFile;

        public IClosable OpenMenu;

        public TextProgrammingView(Guid FileToDisplay)
        {
            TransitionCanvas = new DraggableCanvas();
            TransitionCanvas.OnClickedEvent += Clicked;
            TransitionCanvas.OnClickedAwayEvent += ClickedAway;

            Group = InputManager.CreateActionGroup();
            Group.PollableObjects.Add(this);
            //Group.ClickableObjects.Add(this);

            Background = new Icon(GlobalInterfaceData.Scheme.CanvasBackground);

            DefenitionAlphabetTitle = new Label();
            DefenitionAlphabetTitle.Text = "Defenition Alphabet ID:";

            DefenitionAlphabetBox = new DefenitionAlphabetInputItem(Group);
            
            HaltStatesTitle = new Label();
            HaltStatesTitle.Text = "Halt States:";

            HaltStateInputBox = new InputBox(Group);
            HaltStateInputBox.BackgroundColor = GlobalInterfaceData.Scheme.Background;


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
                StateTransitionItem Item = new StateTransitionItem(TransitionCanvas.Group, this);
                Item.Position = new Vector2(OpenedFile.Transitions[i].X, OpenedFile.Transitions[i].Y);
                Item.CurrentStateTextBox.Text = OpenedFile.Transitions[i].CurrentState;
                Item.EditBoxResize(Item.CurrentStateTextBox);
                Item.TapeValueTextBox.Text = OpenedFile.Transitions[i].TapeValue;
                Item.EditBoxResize(Item.TapeValueTextBox);
                Item.NewStateTextBox.Text = OpenedFile.Transitions[i].NewState;
                Item.EditBoxResize(Item.NewStateTextBox);
                Item.NewTapeValueTextBox.Text = OpenedFile.Transitions[i].NewTapeValue;
                Item.EditBoxResize(Item.NewTapeValueTextBox);

                if (OpenedFile.Transitions[i].MoveDirection == MoveHeadDirection.Left)
                {
                    Item.MoveDirectionTextBox.Text = "L";
                }
                else if (OpenedFile.Transitions[i].MoveDirection == MoveHeadDirection.Right)
                {
                    Item.MoveDirectionTextBox.Text = "R";
                }
                else
                {
                    Item.MoveDirectionTextBox.Text = "";
                }
                
                TransitionCanvas.Elements.Add(Item);
            }

            StringBuilder Builder = new StringBuilder();
            foreach (string HaltState in OpenedFile.HaltStates)
            {
                Builder.Append(HaltState);
                Builder.Append("/n");
            }
            HaltStateInputBox.Text = Builder.ToString();

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

            DefenitionAlphabetBox.ChangeAlphabet(new FileData(AlphabetFileName, AlphabetID, CoreFileType.Alphabet));


         //   DefenitionAlphabetBox.FileLabel.Text = AlphabetFileName;
           // DefenitionAlphabetBox.Bounds = new Point(DefenitionAlphabetBox.FileLabel.RichText.Size.X + 4 , 15);

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

            string[] HaltStates = HaltStateInputBox.Text.Split("/n");

            for (int i = 0; i < HaltStates.Length; i++)
            {
                NewFile.HaltStates.Add(HaltStates[i]);
            }

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

                if (TransitionSource.MoveDirectionTextBox.Text.ToUpper() == "L")
                {
                    NewTransition.MoveDirection = MoveHeadDirection.Left;
                }
                else if (TransitionSource.MoveDirectionTextBox.Text.ToUpper() == "R")
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

        public void Clicked(DraggableCanvas Sender)
        {
            if (InputManager.RightMousePressed)
            {
                OpenMenu?.Close();
                OpenMenu = new NodeCreationMenu(this);
                OpenMenu.Position = new Vector2(InputManager.MouseData.X, InputManager.MouseData.Y);
            }
        }

        public void ClickedAway(DraggableCanvas Sender)
        {
            OpenMenu?.Close();
        }

        public void AddNewTransition(Button Sender)
        {
            Matrix CanvasPos = TransitionCanvas.InverseMatrix * Matrix.CreateTranslation(OpenMenu.Position.X, OpenMenu.Position.Y, 0);
            StateTransitionItem NewTransition = new StateTransitionItem(TransitionCanvas.Group, this);
            NewTransition.Position = new Vector2(CanvasPos.Translation.X, CanvasPos.Translation.Y);
            TransitionCanvas.Elements.Add(NewTransition);
            OpenMenu?.Close();
        }

        public void OpenNodeEditMenu(StateTransitionItem Node)
        {
            OpenMenu?.Close();
            OpenMenu = new NodeEditMenu(this, Node);
            OpenMenu.Position = new Vector2(InputManager.MouseData.X, InputManager.MouseData.Y);
        }

        public void CloneTransition(StateTransitionItem Item)
        {
            Matrix CanvasPos = TransitionCanvas.InverseMatrix * Matrix.CreateTranslation(OpenMenu.Position.X, OpenMenu.Position.Y, 0);
            StateTransitionItem NewTransition = new StateTransitionItem(TransitionCanvas.Group, this);
            NewTransition.CurrentStateTextBox.Text = Item.CurrentStateTextBox.Text;
            NewTransition.TapeValueTextBox.Text = Item.TapeValueTextBox.Text;
            NewTransition.NewTapeValueTextBox.Text = Item.NewTapeValueTextBox.Text;
            NewTransition.NewStateTextBox.Text = Item.NewStateTextBox.Text;
            NewTransition.MoveDirectionTextBox.Text = Item.MoveDirectionTextBox.Text;
            NewTransition.Position = new Vector2(CanvasPos.Translation.X, CanvasPos.Translation.Y);
            TransitionCanvas.Elements.Add(NewTransition);
            OpenMenu?.Close();
        }

        public void DeleteTransition(StateTransitionItem Item)
        {
            TransitionCanvas.Elements.Remove(Item);
            Item.Close();
            TransitionCanvas.Group.IsDirtyPollable = true;
            TransitionCanvas.Group.IsDirtyClickable = true;
            OpenMenu?.Close();
        }

        public void  MoveTansition(StateTransitionItem Item, Matrix Offset)
        {
            Matrix CanvasPos = TransitionCanvas.InverseMatrix * Matrix.CreateTranslation(InputManager.MouseData.X, InputManager.MouseData.Y, 0);
            Item.Position = new Vector2(CanvasPos.Translation.X + Offset.Translation.X, CanvasPos.Translation.Y + Offset.Translation.Y);
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

            TransitionCanvas.Bounds = bounds;

            //DefenitionAlphabetTitle.Bounds = new Point(70, 15);
            //DefenitionAlphabetBox.Bounds = new Point(70, 15);

            //HaltStatesTitle.Bounds = new Point(70, 15);
            //HaltStatesLayout.Bounds = new Point(45, 80);
            HaltStateInputBox.Bounds = new Point(120, 90);
        
        }

        public void Draw(Viewport? BoundPort = null)
        {
            if (IsActive)
            {
                Background.Draw(BoundPort);

                TransitionCanvas.Draw(BoundPort);

                DefenitionAlphabetTitle.Draw(BoundPort);
                DefenitionAlphabetBox.Draw(BoundPort);

                HaltStatesTitle.Draw(BoundPort);
                HaltStateInputBox.Draw(BoundPort);

                OpenMenu?.Draw();
            }
        }

        public void Close()
        {
            TransitionCanvas.Close();
            IsActive = false;
        }
    }
}
