using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringSimulatorDesktop.UI;

namespace TuringSimulatorDesktop.Input
{
    public static class InputManager
    {
        static List<ActionGroup> ActionGroups = new List<ActionGroup>(); 
        static IClickable PreviouslyClickedObject;

        public static MouseState MouseData;

        public static int MouseDeltaX;
        public static int MouseDeltaY;
        public static int PreviousMouseX;
        public static int PreviousMouseY;

        public static bool LeftMousePressed;
        public static bool LeftMouseReleased;
        static ButtonState PreviousLeftClickState;
        public static bool RightMousePressed;
        public static bool RightMouseReleased;
        static ButtonState PreviousRightClickState;

        public static int PreviousScrollWheel;
        public static int ScrollWheelDelta;

        public static void Update()
        {
            MouseData = Mouse.GetState();

            LeftMousePressed = false;
            LeftMouseReleased = false;
            if (MouseData.LeftButton == ButtonState.Pressed && PreviousLeftClickState == ButtonState.Released) LeftMousePressed = true;
            if (MouseData.LeftButton == ButtonState.Released && PreviousLeftClickState == ButtonState.Pressed) LeftMouseReleased = true;
            PreviousLeftClickState = MouseData.LeftButton;
            RightMousePressed = false;
            RightMouseReleased = false;
            if (MouseData.RightButton == ButtonState.Pressed && PreviousRightClickState == ButtonState.Released) RightMousePressed = true;
            if (MouseData.RightButton == ButtonState.Released && PreviousRightClickState == ButtonState.Pressed) RightMouseReleased = true;
            PreviousRightClickState = MouseData.RightButton;

            MouseDeltaX = MouseData.X - PreviousMouseX;
            MouseDeltaY = MouseData.Y - PreviousMouseY;
            PreviousMouseX = MouseData.X;
            PreviousMouseY = MouseData.Y;

            ScrollWheelDelta = MouseData.ScrollWheelValue - PreviousScrollWheel;
            PreviousScrollWheel = MouseData.ScrollWheelValue;

            if (LeftMousePressed)
            {
                int i = ActionGroups.Count-1;
                bool RecepientFound = false;
                while (i > -1 && !RecepientFound)
                {
                    if (ActionGroups[i].IsActive && ActionGroups[i].IsMouseInBounds())
                    {
                        int j = ActionGroups[i].ClickableObjects.Count - 1;
                        while (j > -1 && !RecepientFound)                            
                        {
                            if (ActionGroups[i].ClickableObjects[j].IsMouseOver())
                            {
                                ActionGroups[i].ClickableObjects[j].Clicked();
                                if (PreviouslyClickedObject != null && ActionGroups[i].ClickableObjects[j] != PreviouslyClickedObject) PreviouslyClickedObject.ClickedAway();
                                PreviouslyClickedObject = ActionGroups[i].ClickableObjects[j];
                                RecepientFound = true;                                
                            }

                            j--;

                        }
                    }

                    i--;
                }

                if (PreviouslyClickedObject != null && RecepientFound == false) PreviouslyClickedObject.ClickedAway();
            }

            for (int i = 0; i < ActionGroups.Count; i++)
            {
                if (ActionGroups[i].IsActive)
                {
                    if (ActionGroups[i].IsMouseInBounds())
                    {
                        for (int j = 0; j < ActionGroups[i].PollableObjects.Count; j++)
                        {
                            ActionGroups[i].PollableObjects[j].PollInput(true);
                        }
                    }
                    else
                    {
                        for (int j = 0; j < ActionGroups[i].PollableObjects.Count; j++)
                        {
                            ActionGroups[i].PollableObjects[j].PollInput(false);
                        }
                    }
                }    
            }

            for (int i = ActionGroups.Count - 1; i > -1; i--)
            {
                if (ActionGroups[i].IsMarkedForDeletion) ActionGroups.RemoveAt(i);
            }
        }

        public static ActionGroup CreateActionGroup(int X, int Y, int Width, int Height)
        {
            ActionGroup NewGroup = new ActionGroup(X, Y, Width, Height);
            ActionGroups.Add(NewGroup);

            return NewGroup;
        }

        public static ActionGroup CreateActionGroup()
        {
            ActionGroup NewGroup = new ActionGroup();
            ActionGroups.Add(NewGroup);

            return NewGroup;
        }

        public static void DeleteAllActionGroups()
        {
            for (int i = 0; i < ActionGroups.Count; i++) ActionGroups[i].IsMarkedForDeletion = true;
            PreviouslyClickedObject = null;
        }
    }
}
