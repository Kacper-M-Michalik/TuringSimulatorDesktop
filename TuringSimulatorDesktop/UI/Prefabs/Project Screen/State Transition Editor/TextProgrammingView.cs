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
using TuringCore.TextProgramming;
using TuringCore.Files;
using TuringCore.Networking;
using TuringSimulatorDesktop.Debugging;
using TuringSimulatorDesktop.Networking;

namespace TuringSimulatorDesktop.UI.Prefabs
{    
    public class TextProgrammingView : IView, IPollable, IRunnable, ISaveable, IUndoRedoable
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

        TextureButton HelpMenuButton;
        List<Texture2D> HelpMenus;

        Label DefenitionAlphabetTitle;
        DefinitionAlphabetInputItem DefenitionAlphabetBox;

        Label HaltStatesTitle;
        InputBox HaltStateInputBox;

        public DraggableCanvas TransitionCanvas;

        bool FullyLoadedFile;
        Guid CurrentlyOpenedFileID;
        int FileVersion;
        TransitionFile OpenedFile;

        //Implements the undo/redo feature
        Stack<(InputBox, string)> StateTransitionChangeUndoStack = new Stack<(InputBox, string)>();
        Stack<(InputBox, string)> StateTransitionChangeRedoStack = new Stack<(InputBox, string)>();

        public IClosable OpenMenu;

        //Constructor
        //Takes in GUID of file to display
        public TextProgrammingView(Guid FileToDisplay)
        {
            TransitionCanvas = new DraggableCanvas();
            TransitionCanvas.OnClickedEvent += Clicked;
            TransitionCanvas.OnClickedAwayEvent += ClickedAway;

            Group = InputManager.CreateActionGroup();
            Group.PollableObjects.Add(this);

            Background = new Icon(GlobalInterfaceData.Scheme.CanvasProgrammingBackground);

            HelpMenuButton = new TextureButton(Group);
            HelpMenuButton.BaseTexture = GlobalInterfaceData.TextureLookup[UILookupKey.HelpButton];

            DefenitionAlphabetTitle = new Label();
            DefenitionAlphabetTitle.Text = "Definition Alphabet ID:";

            DefenitionAlphabetBox = new DefinitionAlphabetInputItem(Group);
            
            HaltStatesTitle = new Label();
            HaltStatesTitle.Text = "Halt States:";

            HaltStateInputBox = new InputBox(Group);
            HaltStateInputBox.BackgroundColor = GlobalInterfaceData.Scheme.Background;


            IsActive = false;

            //Switches to display the file
            CurrentlyOpenedFileID = FileToDisplay;
            SwitchOpenedFile(FileToDisplay);
        }

        //Polls if the user has interacted with this view, if so, sets itself as the last focused editor
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
            //Unsubscribe from UI events regarding receiving responses regarding previously displayed Transition Program
            FullyLoadedFile = false;
            UIEventManager.Unsubscribe(CurrentlyOpenedFileID, ReceivedStateTransitionFile);
            Client.SendTCPData(ClientSendPacketFunctions.UnsubscribeFromFileUpdates(CurrentlyOpenedFileID));
            CurrentlyOpenedFileID = ID;
            //Unsubscribe for UI events regarding responses regarding newly requested Transition Program
            UIEventManager.Subscribe(CurrentlyOpenedFileID, ReceivedStateTransitionFile);
            //Create request packet and send to server
            Client.SendTCPData(ClientSendPacketFunctions.RequestFile(ID, true));
        }

        public void ReceivedStateTransitionFile(object Data)
        {
            CustomLogging.Log("CLIENT: Window received State Transition Data");

            FileDataMessage Message = (FileDataMessage)Data;

            //If a metadata update is received, we only update the metadata relevant data/UI elements
            if ((ServerSendPackets)Message.RequestType == ServerSendPackets.SentFileMetadata)
            {
                title = Message.Name;
                FileVersion = Message.Version;
                return;
            }

            //Security check, ensures the received JSON object represents the wanted file
            if (Message.GUID != CurrentlyOpenedFileID)
            {
                CustomLogging.Log("CLIENT: State Transition Editor Fatal Error, received unwanted file data!");
                return;
            }

            title = Message.Name;
            FileVersion = Message.Version;

            //Security check, ensures the received Transition File is valid
            try
            {
                OpenedFile = JsonSerializer.Deserialize<TransitionFile>(Message.Data);
            }
            catch
            {
                CustomLogging.Log("CLIENT: Window - Invalid Transition table received");
                return;
            }

            //Reset UI elements
            DefenitionAlphabetBox.ReferenceFileData = null;
            DefenitionAlphabetBox.FileLabel.Text = "";

            //Request metadata regarding the referenced Definition Alphabet
            if (OpenedFile.DefinitionAlphabetFileID != Guid.Empty)
            {
                UIEventManager.Subscribe(OpenedFile.DefinitionAlphabetFileID, ReceiveAlphabetMetaData);
                Client.SendTCPData(ClientSendPacketFunctions.RequestFileMetadata(OpenedFile.DefinitionAlphabetFileID));
            }
            else
            {
                //If there is no Definition Alphabet, indicate in the UI as such
                DefenitionAlphabetBox.ReferenceFileData = null;
                DefenitionAlphabetBox.FileLabel.Text = "No Referenced Alphabet";
                FullyLoadedFile = true;
            }

            //Translate all transitions in the Transition File as nodes on the programming canvas
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

            //Populate halt states UI
            StringBuilder Builder = new StringBuilder();
            foreach (string HaltState in OpenedFile.HaltStates)
            {
                Builder.Append(HaltState);
                Builder.Append("/n");
            }
            if (Builder.Length > 0) Builder.Remove(Builder.Length - 2, 2);
            HaltStateInputBox.Text = Builder.ToString();

            TransitionCanvas.ApplyMatrices();
        }

        //Process Alphabet Metadat Resposne
        public void ReceiveAlphabetMetaData(object Data)
        {
            CustomLogging.Log("CLIENT: Window received State Alphabet Data");

            FileDataMessage Message = (FileDataMessage)Data;
            
            //Ignore file contents requests
            if ((ServerSendPackets)Message.RequestType == ServerSendPackets.SentOrUpdatedFile) return;

            //Secuirty check, ensure received JSON object represents the file we requested
            if (Message.GUID != OpenedFile.DefinitionAlphabetFileID)
            {
                CustomLogging.Log("CLIENT: State Transition Editor Fatal Error, received unwanted alphabet file data!");
                return;
            }
            
           // UIEventManager.Unsubscribe(OpenedFile.DefinitionAlphabetFileID, ReceiveAlphabetMetaData);

            Guid AlphabetID = Message.GUID;
            string AlphabetFileName = Message.Name;
            int AlphabetFileVersion = Message.Version;

            //Update UI element
            DefenitionAlphabetBox.ChangeAlphabet(new FileData(AlphabetFileName, AlphabetID, CoreFileType.Alphabet));
            
            FullyLoadedFile = true;
        }

        public void Save()
        {
            if (!FullyLoadedFile)
            {
                return;
            }

            //Generate Transition File Object and populate with data based on UI elements, such as programming canvas and halt state input box
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

            //Generate and send File Update Request using generated object
            Client.SendTCPData(ClientSendPacketFunctions.UpdateFile(CurrentlyOpenedFileID, FileVersion, JsonSerializer.SerializeToUtf8Bytes(NewFile, GlobalProjectAndUserData.JsonOptions)));
        }

        //Close any previous context menu when right clicked and display the NodeCreation context menu
        public void Clicked(DraggableCanvas Sender)
        {
            if (InputManager.RightMousePressed)
            {
                OpenMenu?.Close();
                OpenMenu = new NodeCreationMenu(this);
                OpenMenu.Position = new Vector2(InputManager.MouseData.X, InputManager.MouseData.Y);
            }
        }

        //Close cotnext menu when clicked away from
        public void ClickedAway(DraggableCanvas Sender)
        {
            OpenMenu?.Close();
        }

        //Generate new transition node on canvas
        public void AddNewTransition(Button Sender)
        {
            Matrix CanvasPos = TransitionCanvas.InverseMatrix * Matrix.CreateTranslation(OpenMenu.Position.X, OpenMenu.Position.Y, 0);
            StateTransitionItem NewTransition = new StateTransitionItem(TransitionCanvas.Group, this);
            NewTransition.Position = new Vector2(CanvasPos.Translation.X, CanvasPos.Translation.Y);
            TransitionCanvas.Elements.Add(NewTransition);
            OpenMenu?.Close();
        }

        //Close any previous context menu and display the NodeEdit context menu
        public void OpenNodeEditMenu(StateTransitionItem Node)
        {
            OpenMenu?.Close();
            OpenMenu = new NodeEditMenu(this, Node);
            OpenMenu.Position = new Vector2(InputManager.MouseData.X, InputManager.MouseData.Y);
        }

        //Clones a target transition node onto the canvas
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

        //Deletes a transition node off the canvas
        public void DeleteTransition(StateTransitionItem Item)
        {
            TransitionCanvas.Elements.Remove(Item);
            Item.Close();
            TransitionCanvas.Group.IsDirtyPollable = true;
            TransitionCanvas.Group.IsDirtyClickable = true;
            OpenMenu?.Close();
        }

        //Updates the position of a transition node on the canvas
        public void  MoveTansition(StateTransitionItem Item, Matrix Offset)
        {
            Matrix CanvasPos = TransitionCanvas.InverseMatrix * Matrix.CreateTranslation(InputManager.MouseData.X, InputManager.MouseData.Y, 0);
            Item.Position = new Vector2(CanvasPos.Translation.X + Offset.Translation.X, CanvasPos.Translation.Y + Offset.Translation.Y);
        }       

        //Undos the last transition node edit
        public void Undo()
        {
            //Check an action to undo exists
            if (StateTransitionChangeUndoStack.Count == 0) return;

            //Pop action off of stack
            (InputBox, string) Pair = StateTransitionChangeUndoStack.Pop();

            //Add action to redo stack
            StateTransitionChangeRedoStack.Push((Pair.Item1, Pair.Item1.Text));

            //Apply undo
            Pair.Item1.Text = Pair.Item2;
        }

        //Redos the last transition node edit
        public void Redo()
        {
            //Check an action to redo exists
            if (StateTransitionChangeRedoStack.Count == 0) return;

            //Pop action off of stack
            (InputBox, string) Pair = StateTransitionChangeRedoStack.Pop();

            //Add action to undo stack
            StateTransitionChangeUndoStack.Push((Pair.Item1, Pair.Item1.Text));

            //Apply redo
            Pair.Item1.Text = Pair.Item2;
        }

        //Adding an undo clears and redo actions and adds the uno action onto the stack
        public void AddUndo(InputBox Box)
        {
            StateTransitionChangeRedoStack.Clear();
            StateTransitionChangeUndoStack.Push((Box, Box.Text));
        }


        void MoveLayout()
        {
            Group.X = UIUtils.ConvertFloatToInt(position.X);
            Group.Y = UIUtils.ConvertFloatToInt(position.Y);

            Background.Position = position;

            HelpMenuButton.Position = Position + new Vector2(bounds.X - 37, 7);

            DefenitionAlphabetTitle.Position = new Vector2(position.X + 10, position.Y + 10);
            DefenitionAlphabetBox.Position = new Vector2(position.X + 10, position.Y + 30);

            HaltStatesTitle.Position = new Vector2(position.X + 10, position.Y + 60);
            HaltStateInputBox.Position = new Vector2(position.X + 10, position.Y + 80);
            
            TransitionCanvas.Position = position;
        }

        void ResizeLayout()
        {
            Group.Width = bounds.X;
            Group.Height = bounds.Y;

            Background.Bounds = bounds;

            HelpMenuButton.Bounds = new Point(30, 30);

            TransitionCanvas.Bounds = bounds;

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

                HelpMenuButton.Draw(BoundPort);

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
