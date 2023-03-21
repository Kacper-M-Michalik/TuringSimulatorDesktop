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
    public class TuringMachineView : IView, IPollable, IDragListener
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
                Canvas.IsActive = value;
                Canvas.Group.IsActive = value;
            }
        }
        public string Title => "Turing Machine";
        public Guid OpenFileID => Guid.Empty;

        Window ownerWindow;
        public Window OwnerWindow
        {
            get => ownerWindow;
            set => ownerWindow = value;
        }

        public bool IsMarkedForDeletion { get; set; }

        ActionGroup Group;

        Icon Background;

        Label CurrentStateTableTitle;
        Label CurrentStateTableLabel;
        Label CurrentTapeTitle;
        Label CurrentTapeLabel;

        Label StartStateTitle;
        InputBox StartStateInputBox;
        Label StartIndexTitle;
        InputBox StartIndexInputBox;

        Label NotificationLabel;

        HorizontalLayoutBox ControlBox1;
        TextureButton AutoStepButton;
        TextureButton PauseButton;
        TextureButton StepButton;
        TextureButton Speed1Button;
        TextureButton Speed2Button;
        TextureButton Speed3Button;

        HorizontalLayoutBox ControlBox2;
        TextureButton ExecuteButton;
        TextureButton RestartButton;

        DraggableCanvas Canvas;

        Icon ReadHead;
        Label CurrentStateLabel;

        VerticalLayoutBox CurrentInstructionCollectionLayout;
        Icon CurrentInstructionPointer;
        Icon CurrentInstructionBackground;

        TapeVisualItem VisualTape;



        Guid CurrentlyOpenedFileID = Guid.Empty;
        Guid CurrentlyOpenedAlphabetID = Guid.Empty;
        Guid CurrentlyOpenedTapeID = Guid.Empty;
        TuringMachine Machine;

        CompilableFile TempFile;
        StateTable TempStateTable;
        Alphabet TempAlphabet;
        bool IsStateTableOutdated;

        Tape TempTape;

        bool IsAutoStepActive;
        double TimeLeftToNextStep;
        const double BaseTimeBetweenSteps = 800;
        double TimeBetweenStepsMS;

        Alphabet EmptyAlphabet = new Alphabet() { EmptyCharacter = "" };

        public TuringMachineView(Guid FileToDisplay)
        {
            Canvas = new DraggableCanvas();
            VisualTape = new TapeVisualItem(Canvas.Group);

            Background = new Icon(GlobalInterfaceData.Scheme.Background);

            Group = InputManager.CreateActionGroup();
            Group.ClickableObjects.Add(this);
            Group.PollableObjects.Add(this);

            CurrentStateTableTitle = new Label();
            CurrentStateTableTitle.FontSize = 11;
            CurrentStateTableTitle.FontColor = GlobalInterfaceData.Scheme.FontGrayedOutColor;
            CurrentStateTableTitle.Text = "Current State Table";

            CurrentStateTableLabel = new Label();
            CurrentStateTableLabel.FontSize = 20;
            CurrentStateTableLabel.FontColor = GlobalInterfaceData.Scheme.FontColor;
            CurrentStateTableLabel.Text = "NONE";

            CurrentTapeTitle = new Label();
            CurrentTapeTitle.FontSize = 11;
            CurrentTapeTitle.FontColor = GlobalInterfaceData.Scheme.FontGrayedOutColor;
            CurrentTapeTitle.Text = "Current Tape";

            CurrentTapeLabel = new Label();
            CurrentTapeLabel.FontSize = 20;
            CurrentTapeLabel.FontColor = GlobalInterfaceData.Scheme.FontColor;
            CurrentTapeLabel.Text = "DEFAULT";


            StartStateTitle = new Label();
            StartStateTitle.FontSize = 11;
            StartStateTitle.FontColor = GlobalInterfaceData.Scheme.FontGrayedOutColor;
            StartStateTitle.Text = "Start State";

            StartStateInputBox = new InputBox(Group);
            StartStateInputBox.OutputLabel.FontSize = 20;
            StartStateInputBox.OutputLabel.FontColor = GlobalInterfaceData.Scheme.FontColor;
            StartStateInputBox.Text = "";
            StartStateInputBox.EditEvent += ResizeInputBox;
            StartStateInputBox.BackgroundColor = GlobalInterfaceData.Scheme.InteractableAccent;

            StartIndexTitle = new Label();
            StartIndexTitle.FontSize = 11;
            StartIndexTitle.FontColor = GlobalInterfaceData.Scheme.FontGrayedOutColor;
            StartIndexTitle.Text = "Start Tape Index";

            StartIndexInputBox = new InputBox(Group);
            StartIndexInputBox.OutputLabel.FontSize = 20;
            StartIndexInputBox.OutputLabel.FontColor = GlobalInterfaceData.Scheme.FontColor;
            StartIndexInputBox.Text = "0";
            StartIndexInputBox.EditEvent += ResizeInputBox;
            StartIndexInputBox.BackgroundColor = GlobalInterfaceData.Scheme.InteractableAccent;

            StartIndexInputBox.Bounds = new Point(200, 26);
            StartStateInputBox.Bounds = new Point(200, 26);

            NotificationLabel = new Label();

            ControlBox1 = new HorizontalLayoutBox();
            ControlBox1.DrawBounded = false;
            ControlBox1.Scrollable = false;

            AutoStepButton = new TextureButton(Group);
            AutoStepButton.BaseTexture = GlobalInterfaceData.TextureLookup[UILookupKey.AutoStep];
            AutoStepButton.HighlightTexture = GlobalInterfaceData.TextureLookup[UILookupKey.AutoStepHighlight];
            AutoStepButton.HighlightOnMouseOver = true;
            AutoStepButton.OnClickedEvent += AutoStep;
            PauseButton = new TextureButton(Group);
            PauseButton.BaseTexture = GlobalInterfaceData.TextureLookup[UILookupKey.Pause];
            PauseButton.HighlightTexture = GlobalInterfaceData.TextureLookup[UILookupKey.PauseHighlight];
            PauseButton.HighlightOnMouseOver = true;
            PauseButton.OnClickedEvent += Pause;
            StepButton = new TextureButton(Group);
            StepButton.BaseTexture = GlobalInterfaceData.TextureLookup[UILookupKey.Step];
            StepButton.HighlightTexture = GlobalInterfaceData.TextureLookup[UILookupKey.StepHighlight];
            StepButton.HighlightOnMouseOver = true;
            StepButton.OnClickedEvent += Step;

            Speed1Button = new TextureButton(Group);
            Speed1Button.BaseTexture = GlobalInterfaceData.TextureLookup[UILookupKey.SpeedOne];
            Speed1Button.HighlightTexture = GlobalInterfaceData.TextureLookup[UILookupKey.SpeedOneHighlight];
            Speed1Button.HighlightOnMouseOver = true;
            Speed1Button.OnClickedEvent += SetSpeed1;
            Speed2Button = new TextureButton(Group);
            Speed2Button.BaseTexture = GlobalInterfaceData.TextureLookup[UILookupKey.SpeedTwo];
            Speed2Button.HighlightTexture = GlobalInterfaceData.TextureLookup[UILookupKey.SpeedTwoHighlight];
            Speed2Button.HighlightOnMouseOver = true;
            Speed2Button.OnClickedEvent += SetSpeed2;
            Speed3Button = new TextureButton(Group);
            Speed3Button.BaseTexture = GlobalInterfaceData.TextureLookup[UILookupKey.SpeedThree];
            Speed3Button.HighlightTexture = GlobalInterfaceData.TextureLookup[UILookupKey.SpeedThreeHighlight];
            Speed3Button.HighlightOnMouseOver = true;
            Speed3Button.OnClickedEvent += SetSpeed3;

            ControlBox1.AddElement(AutoStepButton);
            ControlBox1.AddElement(PauseButton);
            ControlBox1.AddElement(Speed1Button);
            ControlBox1.AddElement(Speed2Button);
            ControlBox1.AddElement(Speed3Button);


            ControlBox2 = new HorizontalLayoutBox();
            ControlBox2.DrawBounded = false;
            ControlBox2.Scrollable = false;

            ExecuteButton = new TextureButton(Group);
            ExecuteButton.BaseTexture = GlobalInterfaceData.TextureLookup[UILookupKey.Execute];
            ExecuteButton.HighlightTexture = GlobalInterfaceData.TextureLookup[UILookupKey.ExecuteHighlight];
            ExecuteButton.HighlightOnMouseOver = true;
            ExecuteButton.OnClickedEvent += Execute;
            RestartButton = new TextureButton(Group);
            RestartButton.BaseTexture = GlobalInterfaceData.TextureLookup[UILookupKey.Restart];
            RestartButton.HighlightTexture = GlobalInterfaceData.TextureLookup[UILookupKey.RestartHighlight];
            RestartButton.HighlightOnMouseOver = true;
            RestartButton.OnClickedEvent += Restart;

            ControlBox2.AddElement(ExecuteButton);
            ControlBox2.AddElement(RestartButton);
            ControlBox2.AddElement(StepButton);

            CurrentStateLabel = new Label();
            CurrentStateLabel.FontSize = 48;
            CurrentStateLabel.FontColor = GlobalInterfaceData.Scheme.FontColor;
            CurrentStateLabel.Text = "-";


            ReadHead = new Icon();
            ReadHead.DrawTexture = GlobalInterfaceData.TextureLookup[UILookupKey.Header];
            ReadHead.Position = VisualTape.GetIndexWorldPosition(0);

            Canvas.Elements.Add(CurrentStateLabel);
            Canvas.Elements.Add(VisualTape);
            Canvas.Elements.Add(ReadHead);

            Machine = new TuringMachine();
            TimeBetweenStepsMS = BaseTimeBetweenSteps;

            Machine.SetActiveTape(new TapeTemplate());

            IsActive = false;

            if (FileToDisplay != Guid.Empty) LoadStateTableSource(FileToDisplay);
        }

        public void LoadStateTableSource(Guid FileID)
        {            
            if (CurrentlyOpenedAlphabetID != Guid.Empty)
            {
                UIEventManager.Unsubscribe(CurrentlyOpenedAlphabetID, ReceivedAlphabetData);
                Client.SendTCPData(ClientSendPacketFunctions.UnsubscribeFromFileUpdates(CurrentlyOpenedAlphabetID));
                CurrentlyOpenedAlphabetID = Guid.Empty;
                TempAlphabet = null;
            }
            
            if (CurrentlyOpenedFileID != Guid.Empty)
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

        public void ReceivedStateTableSourceData(object Data)
        {
            FileDataMessage Message = (FileDataMessage)Data;

            if ((ServerSendPackets)Message.RequestType == ServerSendPackets.SentFileMetadata) return;

            if (Message.GUID != CurrentlyOpenedFileID)
            {
                //new request was sent, throw error or whatver
                return;
            }

            //unnecessary check?
            if (Message.FileType != CoreFileType.TransitionFile && Message.FileType != CoreFileType.SlateFile)
            {
                //diosplay error to UI HERE
                CustomLogging.Log("Client: Turing Machine Window Fatal Error, received an unexpected non table source data!");
                return;            
            }

            //deserialize on seperate thread later
            try
            {
                if (Message.FileType == CoreFileType.TransitionFile)
                {
                    TempFile = JsonSerializer.Deserialize<TransitionFile>(Message.Data);                
                }
                else
                {
                    TempFile = JsonSerializer.Deserialize<SlateFile>(Message.Data);
                }
            }
            catch
            {
                //display error to ui here
                CustomLogging.Log("Client: Turing Machine Window error, received corrupted transition/slate file");
                return;
            }

            CurrentStateTableLabel.Text = Message.Name;

            CurrentlyOpenedAlphabetID = TempFile.DefinitionAlphabetFileID;
            UIEventManager.Subscribe(CurrentlyOpenedAlphabetID, ReceivedAlphabetData);
            Client.SendTCPData(ClientSendPacketFunctions.RequestFile(CurrentlyOpenedAlphabetID, true));
        }
                
        public void ReceivedAlphabetData(object Data)
        {
            FileDataMessage Message = (FileDataMessage)Data;
            if ((ServerSendPackets)Message.RequestType == ServerSendPackets.SentFileMetadata) return;

            if (Message.GUID != CurrentlyOpenedAlphabetID)
            {
                //new request was sent, throw error or whatver
                return;
            }

            //unnecessary check?
            if (Message.FileType != CoreFileType.Alphabet)
            {
                //diosplay error to UI HERE
                CustomLogging.Log("Client: Turing Machine Window Fatal Error, received an unexpected non alphabet!");
                return;
            }

            try
            {                
                TempAlphabet = JsonSerializer.Deserialize<Alphabet>(Message.Data);               
            }
            catch
            {
                //display error to ui here
                CustomLogging.Log("Client: Turing Machine Window error, received corrupted alphabet file");
                return;
            }

            TempTape = Machine.OriginalTape.Clone(TempAlphabet);
            VisualTape.SetSourceTape(TempTape);

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

        public void LoadTape(Guid FileID)
        {
            if (CurrentlyOpenedTapeID != Guid.Empty)
            {
                UIEventManager.Unsubscribe(CurrentlyOpenedTapeID, ReceivedTapeData);
                Client.SendTCPData(ClientSendPacketFunctions.UnsubscribeFromFileUpdates(CurrentlyOpenedTapeID));
            }

            CurrentlyOpenedTapeID = FileID;
            UIEventManager.Subscribe(CurrentlyOpenedTapeID, ReceivedTapeData);
            Client.SendTCPData(ClientSendPacketFunctions.RequestFile(CurrentlyOpenedTapeID, false));
        }

        public void ReceivedTapeData(object Data)
        {
            FileDataMessage Message = (FileDataMessage)Data;
            if ((ServerSendPackets)Message.RequestType == ServerSendPackets.SentFileMetadata) return;

            if (Message.GUID != CurrentlyOpenedTapeID)
            {
                //new request was sent, throw error or whatver
                return;
            }

            if (Message.FileType != CoreFileType.Tape)
            {
                //diosplay error to UI HERE
                CustomLogging.Log("Client: Turing Machine Window Fatal Error, received an unexpected non tape!");
                return;
            }

            TapeTemplate Tape;
            try
            {
                Tape = JsonSerializer.Deserialize<TapeTemplate>(Message.Data);
            }
            catch
            {
                //display error to ui here
                CustomLogging.Log("Client: Turing Machine Window error, received corrupted tape file");
                return;
            }

            Machine.SetActiveTape(Tape);

            if (TempAlphabet != null)
            {
                TempTape = Machine.OriginalTape.Clone(TempAlphabet);
            }
            else
            {
                TempTape = Machine.OriginalTape.Clone(EmptyAlphabet);
            }
            VisualTape.SetSourceTape(TempTape);

            CurrentTapeLabel.Text = Message.Name;
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
                UpdateUI();
            }
        }

        void ResizeInputBox(InputBox Sender)
        {
            if (Sender.OutputLabel.RichText.Size.X > Sender.Bounds.X - 4) Sender.Bounds = new Point(Sender.OutputLabel.RichText.Size.X + 4, 26);

            if (Sender.Bounds.X > 200 && Sender.OutputLabel.RichText.Size.X + 4 < Sender.Bounds.X) Sender.Bounds = new Point(200, 26);
        }

        public int StartMachine()
        {
            //Add Checks HERE
                //does start state and idnex exist
                //alphabet comaptibilty chekcs?

            if (IsStateTableOutdated)
            {
                Machine.SetActiveStateTable(TempStateTable, TempAlphabet);
                IsStateTableOutdated = false;
            }
               
            int Code = Machine.Start(StartStateInputBox.Text, Convert.ToInt32(StartIndexInputBox.Text));
           
            if (Code == 0) VisualTape.SetSourceTape(Machine.ActiveTape); 
            
            if (Code == 1)
            {
                //Outdated error code
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

        public void Execute(Button Sender)
        {
            if (Machine.ReachedHaltState) return;

            if (StartMachine() != 0) return;
            //need to add limit of steps
            while (Machine.IsActive) Machine.StepProgram();
            UpdateUI();
        }
        public void Step(Button Sender)
        {
            if (Machine.ReachedHaltState)
            {
                IsAutoStepActive = false;
                return;
            }

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
        public void Restart(Button Sender)
        {
            IsAutoStepActive = false;
            StartMachine();
            UpdateUI();
        }
        public void AutoStep(Button Sender)
        {
            if (!Machine.IsActive)
            {
                if (StartMachine() != 0) return;
            }

            TimeLeftToNextStep = TimeBetweenStepsMS;
            IsAutoStepActive = true;
        }
        public void Pause(Button Sender)
        {
            IsAutoStepActive = false;
        }
        public void SetSpeed1(Button Sender)
        {
            TimeBetweenStepsMS = BaseTimeBetweenSteps;
        }
        public void SetSpeed2(Button Sender)
        {
            TimeBetweenStepsMS = BaseTimeBetweenSteps / 2;
        }
        public void SetSpeed3(Button Sender)
        {
            TimeBetweenStepsMS = BaseTimeBetweenSteps / 4;
        }

        public void UpdateUI()
        {
            if (Machine.CurrentState != "Null") CurrentStateLabel.Text = Machine.CurrentState;
            else CurrentStateLabel.Text = "-";
            Vector2 NewHeadPos = VisualTape.GetIndexWorldPosition(Machine.HeadPosition);
            CurrentStateLabel.Position = NewHeadPos + new Vector2(-CurrentStateLabel.Bounds.X * 0.5f, 115);
            ReadHead.Position = VisualTape.GetIndexWorldPosition(Machine.HeadPosition) - new Vector2(ReadHead.Bounds.X * 0.5f, 100.5f);
        }

        public void RecieveDragData()
        {
            FileData Data = InputManager.DragData as FileData;
            if (Data != null && Data.Type == CoreFileType.Tape)
            {
                LoadTape(Data.GUID);
            }
        }

        public void Clicked()
        {

        }

        public void ClickedAway()
        {

        }

        public bool IsMouseOver()
        {
            return (IsActive && InputManager.MouseData.X >= Position.X && InputManager.MouseData.X <= Position.X + bounds.X && InputManager.MouseData.Y >= Position.Y && InputManager.MouseData.Y <= Position.Y + bounds.Y);
        }

        void MoveLayout()
        {
            Group.X = UIUtils.ConvertFloatToInt(position.X);
            Group.Y = UIUtils.ConvertFloatToInt(position.Y);

            Background.Position = Position;

            Canvas.Position = Position;

            CurrentStateTableTitle.Position = Position + new Vector2(22, 27);
            CurrentStateTableLabel.Position = Position + new Vector2(22, 52);
            CurrentTapeTitle.Position = Position + new Vector2(22, 80);
            CurrentTapeLabel.Position = Position + new Vector2(22, 105);

            StartStateTitle.Position = Position + new Vector2(22, 133);
            StartStateInputBox.Position = Position + new Vector2(22, 145);
            StartIndexTitle.Position = Position + new Vector2(22, 186);
            StartIndexInputBox.Position = Position + new Vector2(22, 198);

            ControlBox1.Position = Position + new Vector2(520, 20);
            ControlBox1.UpdateLayout();

            ControlBox2.Position = Position + new Vector2(300, 20);
            ControlBox2.UpdateLayout();

        }

        void ResizeLayout()
        {
            Group.Width = bounds.X;
            Group.Height = bounds.Y;

            Background.Bounds = bounds;           

            ExecuteButton.Bounds = new Point(65, 65);
            RestartButton.Bounds = new Point(65, 65);
            StepButton.Bounds = new Point(65, 65);
            AutoStepButton.Bounds = new Point(65, 65);
            PauseButton.Bounds = new Point(65, 65);
            Speed1Button.Bounds = new Point(65, 65);
            Speed2Button.Bounds = new Point(65, 65);
            Speed3Button.Bounds = new Point(65, 65);

            ReadHead.Bounds = new Point(155, 177);

            UpdateUI();

            Canvas.Bounds = bounds;
        }

        public void Draw(Viewport? BoundPort = null)
        {
            if (IsActive)
            {
                Background.Draw(BoundPort); 
                VisualTape.CameraMin = (Matrix.CreateTranslation(Canvas.Position.X, 0, 0) * Canvas.InverseMatrix).Translation.X;
                VisualTape.CameraMax = (Matrix.CreateTranslation(Canvas.Position.X + Canvas.Bounds.X, 0, 0) * Canvas.InverseMatrix).Translation.X;
                VisualTape.UpdateLayout();
                Canvas.Draw(BoundPort);

                CurrentStateTableTitle.Draw(BoundPort);
                CurrentStateTableLabel.Draw(BoundPort);
                CurrentTapeTitle.Draw(BoundPort);
                CurrentTapeLabel.Draw(BoundPort);

                StartStateTitle.Draw(BoundPort);
                StartStateInputBox.Draw(BoundPort);
                StartIndexTitle.Draw(BoundPort);
                StartIndexInputBox.Draw(BoundPort);

                NotificationLabel.Draw(BoundPort);

                ControlBox1.Draw(BoundPort);
                ControlBox2.Draw(BoundPort);
            }
        }

        public void Close()
        {
            Canvas.Close();
            Group.IsMarkedForDeletion = true;
        }

    }    
}
