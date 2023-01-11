using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using TuringCore;
using TuringSimulatorDesktop.Input;

namespace TuringSimulatorDesktop.UI.Prefabs
{    
    public class TuringMachineView : IView
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
        Window ownerWindow;
        public Window OwnerWindow
        {
            get => ownerWindow;
            set => ownerWindow = value;
        }

        Icon Background;

        Button RestartButton;
        Button StepButton;

        Button ExecuteButton;
        Button PauseButton;
        Button Speed1Button;
        Button Speed2Button;
        Button Speed3Button;

        ActionGroup Group;

        TuringMachine Machine;

        public TuringMachineView()
        {

        }

        public void LoadStateTableSource(int File)
        {            
            //request file here

            //UIEventManager.Subscribe(GlobalProjectAndUserData.ProjectData.AlphabetToFileLookup[Table.DefenitionAlphabetID], );
            //Client.SendTCPData(ClientSendPacketFunctions.RequestFile(GlobalProjectAndUserData.ProjectData.AlphabetToFileLookup[Table.DefenitionAlphabetID], false));

        }

        public void ReceivedStateTableSourceData(Packet Data)
        {
            //serialize state table
            //request alphabet ddata
        }

        public void ReceivedAlphabetData(Packet Data)
        {
            //deserialize
        }

        public void LoadTape(int FileID)
        {

        }
        public void ReceivedTapeData(Packet Data)
        {

        }



        public void Step(Button Sender)
        {

        }
        public void Execute(Button Sender)
        {

        }
        public void Pause(Button Sender)
        {

        }
        public void SetSpeed1(Button Sender)
        {

        }
        public void SetSpeed2(Button Sender)
        {

        }
        public void SetSpeed3(Button Sender)
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
