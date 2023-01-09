using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringCore;
using TuringSimulatorDesktop.Input;
using TuringSimulatorDesktop.UI;
using TuringSimulatorDesktop.UI.Prefabs;

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

        public TextProgrammingView(int FileToDisplay)
        {
            Group = InputManager.CreateActionGroup();

            Background = new Icon(GlobalRenderingData.BackgroundColor);
            TransitionLayout = new VerticalLayoutBox();
            TransitionLayout.Scrollable = true;
            TransitionLayout.Spacing = 5f;

            IsActive = false;

            SwitchOpenedFile(FileToDisplay);
        }

        public void LoadStateTransitionTable(Packet Data)
        {
            //deserialze here into a transitiontabel as defined in turingcore
            Data.ReadInt();
            title = Data.ReadString();
            TransitionLayout.Clear();
            //TransitionLayout.Add(new ());
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

            Background.Position = Position;
            TransitionLayout.Position = new Vector2(Position.X, Position.Y + 20);
        }

        void ResizeLayout()
        {
            Group.Width = bounds.X;
            Group.Height = bounds.Y;

            Background.Bounds = bounds;
            TransitionLayout.Bounds = new Point(bounds.X, bounds.Y - 20);
        }

        public void Draw(Viewport? BoundPort = null)
        {
            if (IsActive)
            {
                Background.Draw(BoundPort);
                TransitionLayout.Draw(BoundPort);
            }
        }
    }
}
