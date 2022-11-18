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
        public static List<IClickable> AllClickableObjects = new List<IClickable>();

        public static MouseState MouseData;

        static object MouseOwner;
        static bool IsMouseBusy;

        public static Vector2 CurrentMousePosition;
        public static Vector2 PreviousMousePosition;
        public static Vector2 MouseDelta;

        public static bool LeftMousePressed;
        public static bool LeftMouseReleased;
        static ButtonState PreviousLeftClickState;
        public static bool RightMousePressed;
        public static bool RightMouseReleased;
        static ButtonState PreviousRightClickState;

        public static int PreviousScrollWheel;
        public static int ScrollWheelDelta;

        public static ButtonState VPWScrollInput;
        public static float VPWZoomInput;
        public static ButtonState VPWActionMenuInput;

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

            PreviousMousePosition = CurrentMousePosition;
            //get rid off
            CurrentMousePosition = new Vector2(MouseData.X, MouseData.Y);
            MouseDelta = CurrentMousePosition - PreviousMousePosition;

            ScrollWheelDelta = MouseData.ScrollWheelValue - PreviousScrollWheel;
            PreviousScrollWheel = MouseData.ScrollWheelValue;

            VPWScrollInput = MouseData.MiddleButton;
            VPWActionMenuInput = MouseData.RightButton;

            VPWZoomInput = (float)ScrollWheelDelta * 0.001f;

            if (LeftMousePressed)
            {
                int i = AllClickableObjects.Count-1;
                bool RecepientFound = false;
                while (i > -1 && !RecepientFound)
                {
                    if (AllClickableObjects[i].IsMouseOver())
                    {
                        AllClickableObjects[i].Clicked();
                        RecepientFound = true;
                    }

                    i--;
                }
            }
        }

        public static void ReserveMouse(object Reserver)
        {
            IsMouseBusy = true;
            MouseOwner = Reserver;
        }

        public static void FreeMouse()
        {
            IsMouseBusy = false;
        }

        public static bool IsMouseAvailable(object Querier)
        {
            if (!IsMouseBusy) return true;
            return MouseOwner == Querier;
        }
    }
}
