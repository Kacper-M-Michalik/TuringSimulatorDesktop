﻿using Microsoft.Xna.Framework;
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
                TransitionLayout.Group.IsActive = isActive;
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
        InputBox DefenitionAlphabetInputBox;

        Label HaltStatesTitle;

        //VerticalLayoutBox HaltStatesLayout;
        //List<InputBox> HaltStateInputBoxes;
        InputBox HaltStateInputBox;

        Label TransitionsTitle;
        ColorButton AddTransitionButton;

        VerticalLayoutBox TransitionLayout;
        List<StateTransitionItem> TransitionItems;


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

            DefenitionAlphabetInputBox = new InputBox(Group);


            HaltStatesTitle = new Label();
            HaltStatesTitle.Text = "Halt States:";

            HaltStateInputBox = new InputBox(Group);

            TransitionsTitle = new Label();
            TransitionsTitle.Text = "Transitions:";

            AddTransitionButton = new ColorButton(Group);
            AddTransitionButton.BaseColor = GlobalInterfaceData.Scheme.InteractableAccent;
            AddTransitionButton.OnClickedEvent += AddTransitionItem;

            TransitionLayout = new VerticalLayoutBox();
            TransitionLayout.Scrollable = true;
            TransitionLayout.Spacing = 5f;

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

        public void AddTransitionItem(Button Sender)
        {
            StateTransitionItem Item = new StateTransitionItem(TransitionLayout.Group);
            TransitionLayout.AddElement(Item);
            TransitionItems.Add(Item);
            TransitionLayout.UpdateLayout();    
        }

        public void SwitchOpenedFile(Guid ID)
        {
            FullyLoadedFile = false;
            UIEventManager.Unsubscribe(CurrentlyOpenedFileID, ReceivedStateTransitionTable);
            Client.SendTCPData(ClientSendPacketFunctions.UnsubscribeFromFileUpdates(CurrentlyOpenedFileID));
            CurrentlyOpenedFileID = ID;
            UIEventManager.Subscribe(CurrentlyOpenedFileID, ReceivedStateTransitionTable);
            Client.SendTCPData(ClientSendPacketFunctions.RequestFile(ID, true));
        }

        public void ReceivedStateTransitionTable(Packet Data)
        {
            TransitionItems = new List<StateTransitionItem>();

            //file id
            Data.ReadGuid();

            if ((CoreFileType)Data.ReadInt() != CoreFileType.TransitionFile) throw new Exception("Opened File is not a transition file!");

            title = Data.ReadString();
            FileVersion = Data.ReadInt();
            try
            {
                OpenedFile = JsonSerializer.Deserialize<TransitionFile>(Data.ReadByteArray());
            }
            catch
            {
                CustomLogging.Log("CLIENT: Window - Invalid Transition table recieved");
                return;
            }

            //TestLabel.Text = "VERSION: " + FileVersion.ToString() + "/n" + Encoding.ASCII.GetString(Data.ReadByteArray()) + "/n" + OpenedFile.DefinitionAlphabetID;

            TransitionLayout.Clear();

            for (int i = 0; i < OpenedFile.Transitions.Count; i++)
            {
                StateTransitionItem Item = new StateTransitionItem(TransitionLayout.Group);
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
                
                TransitionLayout.AddElement(Item);
                TransitionItems.Add(Item);
            }

            TransitionLayout.UpdateLayout();
            FullyLoadedFile = true;
        }

        public void Save()
        {
            if (!FullyLoadedFile)
            {
                return;
            }

            TransitionFile NewFile = new TransitionFile();

            NewFile.FileID = CurrentlyOpenedFileID;

            NewFile.HaltStates.AddRange(HaltStateInputBox.Text.Split("/n"));

            //for (int i = 0; i < HaltStateInputBoxes.Count; i++)
            //{
            //    NewFile.HaltStates.Add(HaltStateInputBoxes[i].Text);
           // }

            for (int i = 0; i < TransitionItems.Count; i++)
            {
                Transition NewTransition = new Transition();

                NewTransition.CurrentState = TransitionItems[i].CurrentStateTextBox.Text;
                NewTransition.TapeValue = TransitionItems[i].TapeValueTextBox.Text;
                NewTransition.NewState = TransitionItems[i].NewStateTextBox.Text;
                NewTransition.NewTapeValue = TransitionItems[i].NewTapeValueTextBox.Text;

                if (TransitionItems[i].CurrentStateTextBox.Text == "L")
                {
                    NewTransition.MoveDirection = MoveHeadDirection.Left;
                }
                else if (TransitionItems[i].CurrentStateTextBox.Text == "R")
                {
                    NewTransition.MoveDirection = MoveHeadDirection.Right;
                }
                else
                {
                    NewTransition.MoveDirection = MoveHeadDirection.Empty;
                }

                NewFile.Transitions.Add(NewTransition);
            }

            Client.SendTCPData(ClientSendPacketFunctions.UpdateFile(FileVersion, JsonSerializer.SerializeToUtf8Bytes(NewFile, GlobalProjectAndUserData.JsonOptions)));
        }

        void MoveLayout()
        {
            Group.X = UIUtils.ConvertFloatToInt(position.X);
            Group.Y = UIUtils.ConvertFloatToInt(position.Y);

            Background.Position = position;

            DefenitionAlphabetTitle.Position = new Vector2(position.X + 10, position.Y + 10);
            DefenitionAlphabetInputBox.Position = new Vector2(position.X + 10, position.Y + 30);

            HaltStatesTitle.Position = new Vector2(position.X + 10, position.Y + 60);
            //HaltStatesLayout.Position = new Vector2(position.X + 10, position.Y + 80);
            HaltStateInputBox.Position = new Vector2(position.X + 10, position.Y + 80);

            TransitionsTitle.Position = new Vector2(position.X + 100, position.Y + 10); 
            AddTransitionButton.Position = new Vector2(position.X + 100, position.Y+5);
            TransitionLayout.Position = new Vector2(position.X + 100, position.Y + 30);
        }

        void ResizeLayout()
        {
            Group.Width = bounds.X;
            Group.Height = bounds.Y;

            Background.Bounds = bounds;

            DefenitionAlphabetTitle.Bounds = new Point(70, 15);
            DefenitionAlphabetInputBox.Bounds = new Point(70, 15);

            HaltStatesTitle.Bounds = new Point(70, 15);
            //HaltStatesLayout.Bounds = new Point(45, 80);
            HaltStateInputBox.Bounds = new Point(70, 80);

            TransitionsTitle.Bounds = new Point(70, 15); 
            AddTransitionButton.Bounds = new Point(15, 15);
            TransitionLayout.Bounds = new Point(300, 300);            
        }

        public void Draw(Viewport? BoundPort = null)
        {
            if (IsActive)
            {
                Background.Draw(BoundPort); 
                DefenitionAlphabetTitle.Draw(BoundPort);
                DefenitionAlphabetInputBox.Draw(BoundPort);

                HaltStatesTitle.Draw(BoundPort);
                HaltStateInputBox.Draw(BoundPort);

                TransitionsTitle.Draw(BoundPort);
                AddTransitionButton.Draw(BoundPort);
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
