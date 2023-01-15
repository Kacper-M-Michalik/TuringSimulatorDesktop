using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using TuringCore;
using TuringSimulatorDesktop.Input;

namespace TuringSimulatorDesktop.UI.Prefabs
{    
    public class TuringMachineView : IView, IPollable
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
        public string Title => "Turing Machine";
        public int OpenFileID => -1;

        Window ownerWindow;
        public Window OwnerWindow
        {
            get => ownerWindow;
            set => ownerWindow = value;
        }

        public bool IsMarkedForDeletion { get; set; }

        Icon InfoBackground;
        Label CurrentStateTableTitle;
        Label CurrentStateTableLabel;
        Label CurrentTapeTitle;
        Label CurrentTapeLabel;

        Label NotificationLabel;


        Icon Background;

        TextureButton ExecuteButton;
        TextureButton StepButton;
        TextureButton RestartButton;

        TextureButton AutoStepButton;
        TextureButton PauseButton;
        TextureButton Speed1Button;
        TextureButton Speed2Button;
        TextureButton Speed3Button;



        Icon ReadHead;
        Label CurrentStateLabel;
        VerticalLayoutBox CurrentInstructionCollectionLayout;
        Icon CurrentInstructionPointer;

        ActionGroup Group;

        int CurrentlyOpenedFileID = -1;
        int CurrentlyOpenedAlphabetID = -1;
        int CurrentlyOpenedTapeID = -1;
        TuringMachine Machine;

        CompilableFile TempFile;
        Alphabet TempAlphabet;
        StateTable TempStateTable;
        bool IsStateTableOutdated;

        bool IsAutoStepActive;
        double TimeLeftToNextStep;
        const double BaseTimeBetweenSteps = 1000;
        double TimeBetweenStepsMS;

        public TuringMachineView()
        {
            Machine = new TuringMachine();
            TimeBetweenStepsMS = BaseTimeBetweenSteps;
        }

        public void LoadStateTableSource(int FileID)
        {
            if (CurrentlyOpenedAlphabetID != -1)
            {
                UIEventManager.Unsubscribe(CurrentlyOpenedAlphabetID, ReceivedAlphabetData);
                Client.SendTCPData(ClientSendPacketFunctions.UnsubscribeFromFileUpdates(CurrentlyOpenedAlphabetID));
                CurrentlyOpenedAlphabetID = -1;
                TempAlphabet = null;
            }
            if (CurrentlyOpenedFileID != -1)
            {
                UIEventManager.Unsubscribe(CurrentlyOpenedFileID, ReceivedStateTableSourceData);
                Client.SendTCPData(ClientSendPacketFunctions.UnsubscribeFromFileUpdates(CurrentlyOpenedFileID));
                TempFile = null;
            }

            CurrentlyOpenedFileID = FileID;

            UIEventManager.Subscribe(CurrentlyOpenedFileID, ReceivedStateTableSourceData);
            Client.SendTCPData(ClientSendPacketFunctions.RequestFile(CurrentlyOpenedFileID, true));

            /*
             state tl references alphabet

            how deal with coding statetale
            how deal with change to alphaet

            when compile check validity, throw errors -> auto picsk up invalid alphaet
            updated comes -> gets recompiled by turing view -> if error in compile then display in view

            when load tape to edit -> check if contains non valid alphabet stuff
            when laod tape in turing machine -> also run invalidity check, if so display on turing view and prevent run
                         
             */
        }

        public void ReceivedStateTableSourceData(Packet Data)
        {
            //get rid of fileID
            if (CurrentlyOpenedFileID != Data.ReadInt())
            {
                //new request was sent, throw error or whatver
                return;
            }

            CoreFileType File = ((CoreFileType)Data.ReadInt());

            if (File != CoreFileType.TransitionFile && File != CoreFileType.SlateFile)
            {
                //diosplay error to UI HERE
                CustomLogging.Log("Client: Turing Machine Window Fatal Error, received an unexpected non table source data!");
                return;            
            }

            CurrentStateTableLabel.Text = Data.ReadString();
            //get rid of file version
            Data.ReadInt();

            //deserialize on seperate thread later
            try
            {
                if (File == CoreFileType.TransitionFile)
                {
                    TempFile = JsonSerializer.Deserialize<TransitionFile>(Data.ReadByteArray());                
                }
                else
                {
                    TempFile = JsonSerializer.Deserialize<SlateFile>(Data.ReadByteArray());
                }
            }
            catch
            {
                //display error to ui here
                CustomLogging.Log("Client: Turing Machine Window error, received corrupted transition/slate file");
                return;
            }

            //FIGURE OUT LINKED ALPHABET ----
            //CurrentlyOpenedAlphabetID = FileID;
            UIEventManager.Subscribe(CurrentlyOpenedAlphabetID, ReceivedAlphabetData);
            Client.SendTCPData(ClientSendPacketFunctions.RequestFile(CurrentlyOpenedAlphabetID, true));
        }

        public void ReceivedAlphabetData(Packet Data)
        {
            if (CurrentlyOpenedAlphabetID != Data.ReadInt())
            {
                //new request was sent, throw error or whatver
                return;
            }

            CoreFileType File = ((CoreFileType)Data.ReadInt());

            if (File != CoreFileType.Alphabet)
            {
                CustomLogging.Log("Client: Turing Machine Window Fatal Error, received an unexpected non alphabet!");
                return;
            }

            Data.ReadString();
            Data.ReadInt();

            try
            {                
                TempAlphabet = JsonSerializer.Deserialize<Alphabet>(Data.ReadByteArray());               
            }
            catch
            {
                //display error to ui here
                CustomLogging.Log("Client: Turing Machine Window error, received corrupted alphabet file");
                return;
            }

            CompileSourceFile();
        }

        public void CompileSourceFile()
        {
            TempStateTable = TempFile.Compile(TempAlphabet);
            if (TempStateTable == null)
            {
                NotificationLabel.Text = "Failed to compile state table: Check it programmed correctly!";
                CustomLogging.Log("Cllient: Failed to compile file, invalid transition/slate file!");
                return;
            }
            IsStateTableOutdated = true;
        }

        public void LoadTape(int FileID)
        {
            if (CurrentlyOpenedTapeID != -1)
            {
                UIEventManager.Unsubscribe(CurrentlyOpenedTapeID, ReceivedTapeData);
                Client.SendTCPData(ClientSendPacketFunctions.UnsubscribeFromFileUpdates(CurrentlyOpenedTapeID));
            }

            CurrentlyOpenedTapeID = FileID;
            UIEventManager.Subscribe(CurrentlyOpenedTapeID, ReceivedStateTableSourceData);
            Client.SendTCPData(ClientSendPacketFunctions.RequestFile(CurrentlyOpenedTapeID, true));
        }

        public void ReceivedTapeData(Packet Data)
        {
            if (CurrentlyOpenedTapeID != Data.ReadInt())
            {
                //new request was sent, throw error or whatver
                return;
            }

            CoreFileType File = ((CoreFileType)Data.ReadInt());

            if (File != CoreFileType.Tape)
            {
                CustomLogging.Log("Client: Turing Machine Window Fatal Error, received an unexpected non tape!");
                return;
            }

            Data.ReadString();
            Data.ReadInt();

            TapeTemplate Tape;
            try
            {
                Tape = JsonSerializer.Deserialize<TapeTemplate>(Data.ReadByteArray());
            }
            catch
            {
                //display error to ui here
                CustomLogging.Log("Client: Turing Machine Window error, received corrupted tape file");
                return;
            }

            Machine.SetActiveTape(Tape);
        }

        public void PollInput(bool IsInActionGroupFrame)
        {
            if (IsAutoStepActive)
            {
                if (TimeLeftToNextStep < -1) TimeLeftToNextStep = -1;

                TimeLeftToNextStep -= GlobalInterfaceData.Time.ElapsedGameTime.TotalMilliseconds;

                if (TimeLeftToNextStep < 0)
                {
                    Step(null);
                    TimeLeftToNextStep = TimeBetweenStepsMS; 
                }
            }
        }

        public int StartMachine()
        {
            //Add Checks HERE
            //read input boxs for input

            if (IsStateTableOutdated)
            {
                Machine.SetActiveStateTable(TempStateTable, TempAlphabet);
                IsStateTableOutdated = false;
            }

            int Code = 0;//Machine.Start();
            if (Code == 1)
            {
                //set error ui
            }
            if (Code == 2)
            {
                //set error ui
            }
            if (Code == 3)
            {
                //set error ui
            }
            if (Code == 4)
            {
                //set error ui
            }

            return Code;
        }

        public void Execute(TextureButton Sender)
        {
            if (StartMachine() != 0) return;
            //need to add limit of steps
            while (Machine.IsActive) Machine.StepProgram();
            UpdateUI();
        }
        public void Step(TextureButton Sender)
        {
            if (Machine.IsActive)
            {
                Machine.StepProgram();
            }
            else
            {
                if (StartMachine() != 0) return;
            }
            UpdateUI();
        }
        public void Restart(TextureButton Sender)
        {
            StartMachine();
        }
        public void AutoStep(TextureButton Sender)
        {
            if (!Machine.IsActive)
            {
                if (StartMachine() != 0) return;
            }

            TimeLeftToNextStep = TimeBetweenStepsMS;
            IsAutoStepActive = true;
        }
        public void Pause(TextureButton Sender)
        {
            IsAutoStepActive = false;
        }
        public void SetSpeed1(TextureButton Sender)
        {
            TimeBetweenStepsMS = BaseTimeBetweenSteps;
        }
        public void SetSpeed2(TextureButton Sender)
        {
            TimeBetweenStepsMS = BaseTimeBetweenSteps / 2;
        }
        public void SetSpeed3(TextureButton Sender)
        {
            TimeBetweenStepsMS = BaseTimeBetweenSteps / 4;
        }

        public void UpdateUI()
        {

        }

        void MoveLayout()
        {
            Group.X = UIUtils.ConvertFloatToInt(position.X);
            Group.Y = UIUtils.ConvertFloatToInt(position.Y);

            Background.Position = Position;
        }

        void ResizeLayout()
        {
            Group.Width = bounds.X;
            Group.Height = bounds.Y;

            Background.Bounds = bounds;
        }

        public void Draw(Viewport? BoundPort = null)
        {
            if (IsActive)
            {
                Background.Draw(BoundPort);
                
            }
        }
        public void Close()
        {
            Group.IsMarkedForDeletion = true;
        }
    }    
}
