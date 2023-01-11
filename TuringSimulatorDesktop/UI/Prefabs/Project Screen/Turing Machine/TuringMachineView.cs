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

        Button StepButton;
        Button ExecuteButton;

        ActionGroup Group;

        TuringMachine Machine;

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

        public void LoadStateTable(StateTable Table)
        {            
            //UIEventManager.Subscribe(GlobalProjectAndUserData.ProjectData.AlphabetToFileLookup[Table.DefenitionAlphabetID], );
            //Client.SendTCPData(ClientSendPacketFunctions.RequestFile(GlobalProjectAndUserData.ProjectData.AlphabetToFileLookup[Table.DefenitionAlphabetID], false));

        }

        public void ReceivedAlphabet(Packet Data)
        {
            //deserialize
        }

        public void LoadTape(int FileID)
        {

        }

        //udpate local soruce tape, state table call set

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
