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
    public class TextProgrammingView : IView, ISaveable
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
        
        public int OpenFileID => CurrentlyOpenedFileID;


        ActionGroup Group;
        Icon Background;

        Label DefenitionAlphabetTitle;
        InputBox DefenitionAlphabetInputBox;

        Label HaltStatesTitle;

        VerticalLayoutBox HaltStatesLayout;
        List<InputBox> HaltStateInputBoxes;

        Label TransitionsTitle;

        VerticalLayoutBox TransitionLayout;
        List<StateTransitionItem> TransitionItems;

        bool FullyLoadedFile;
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



            IsActive = false;

            CurrentlyOpenedFileID = FileToDisplay;
            SwitchOpenedFile(FileToDisplay);
        }

        public void SwitchOpenedFile(int ID)
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
            //file id
            Data.ReadInt();

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
                else
                {
                    Item.CurrentStateTextBox.Text = "R";
                }

                TransitionLayout.AddElement(Item);
            }

            TransitionLayout.UpdateLayout();
            FullyLoadedFile = true;
        }

        void MoveLayout()
        {
            Group.X = UIUtils.ConvertFloatToInt(position.X);
            Group.Y = UIUtils.ConvertFloatToInt(position.Y);

            Background.Position = position;
            TransitionLayout.Position = new Vector2(position.X, position.Y + 20);
            //update this

        }

        void ResizeLayout()
        {
            Group.Width = bounds.X;
            Group.Height = bounds.Y;

            Background.Bounds = bounds;
            TransitionLayout.Bounds = new Point(bounds.X, bounds.Y - 20);
            
        }

        public void Save()
        {
            if (!FullyLoadedFile)
            {
                return;
            }

            TransitionFile NewFile = new TransitionFile();

            for (int i = 0; i < HaltStateInputBoxes.Count; i++)
            {
                NewFile.HaltStates.Add(HaltStateInputBoxes[i].Text);
            }

            for (int i = 0; i < TransitionItems.Count; i++)
            {
                Transition NewTransition = new Transition();

                NewTransition.CurrentState = TransitionItems[i].CurrentStateTextBox.Text;
                NewTransition.TapeValue = TransitionItems[i].CurrentStateTextBox.Text;
                NewTransition.NewState = TransitionItems[i].CurrentStateTextBox.Text;
                NewTransition.NewTapeValue = TransitionItems[i].CurrentStateTextBox.Text;

                if (TransitionItems[i].CurrentStateTextBox.Text == "L")
                {
                    NewTransition.MoveDirection = MoveHeadDirection.Left;
                }
                else
                {
                    NewTransition.MoveDirection = MoveHeadDirection.Right;
                }

                NewFile.Transitions.Add(NewTransition);
            }

            Client.SendTCPData(ClientSendPacketFunctions.UpdateFile(CurrentlyOpenedFileID, FileVersion, JsonSerializer.SerializeToUtf8Bytes(OpenedFile, GlobalProjectAndUserData.JsonOptions)));
        }

        public void Draw(Viewport? BoundPort = null)
        {
            if (IsActive)
            {
                Background.Draw(BoundPort);
                //TestLabel.Draw(BoundPort);
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
