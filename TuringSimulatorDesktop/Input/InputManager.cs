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
        //will be probnlem in future -> clikc priority -> maybe group ui elements for click priorty?
        //group would be index via an id, lets yuou move whoel iwndow priority at a time, also would support grou bounds -> means eeach ui elementdoesnt have to store a scissor area

        static List<IClickable> ClickableObjects = new List<IClickable>();
        static IClickable PreviouslyClickedObject;
        static List<IPoll> PollableObjects = new List<IPoll>();

        static List<IClickable> ClickableObjectsToAdd = new List<IClickable>();
        static List<IPoll> PollableObjectsToAdd = new List<IPoll>();
        static List<IClickable> ClickableObjectsToRemove = new List<IClickable>();
        static List<IPoll> PollableObjectsToRemove = new List<IPoll>();
        static List<IClickable> ClickableObjectsToAddPersistent = new List<IClickable>();
        static List<IPoll> PollableObjectsToAddPersistent = new List<IPoll>();

        static bool ClearOnQueue;

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

            MouseDeltaX = MouseData.X - PreviousMouseX;
            MouseDeltaY = MouseData.Y - PreviousMouseY;
            PreviousMouseX = MouseData.X;
            PreviousMouseY = MouseData.Y;

            ScrollWheelDelta = MouseData.ScrollWheelValue - PreviousScrollWheel;
            PreviousScrollWheel = MouseData.ScrollWheelValue;

            VPWScrollInput = MouseData.MiddleButton;
            VPWActionMenuInput = MouseData.RightButton;

            VPWZoomInput = (float)ScrollWheelDelta * 0.001f;

            if (LeftMousePressed)
            {
                int i = ClickableObjects.Count-1;
                bool RecepientFound = false;
                while (i > -1 && !RecepientFound)
                {
                    if (ClickableObjects[i].IsMouseOver())
                    {
                        bool PassThrough = ClickableObjects[i].Clicked();

                        if (!PassThrough)
                        {
                            if (PreviouslyClickedObject != null && ClickableObjects[i] != PreviouslyClickedObject) PreviouslyClickedObject.ClickedAway();

                            PreviouslyClickedObject = ClickableObjects[i];
                            RecepientFound = true;
                        }
                    }

                    i--;
                }

                if (PreviouslyClickedObject != null && RecepientFound == false) PreviouslyClickedObject.ClickedAway();

            }

            for (int i = 0; i < PollableObjects.Count; i++)
            {
                PollableObjects[i].PollInput();                
            }

            if (!ClearOnQueue)
            {
                for (int i = 0; i < ClickableObjectsToRemove.Count; i++)
                {
                    ClickableObjects.Remove(ClickableObjectsToRemove[i]);
                }
                for (int i = 0; i < PollableObjectsToRemove.Count; i++)
                {
                    PollableObjects.Remove(PollableObjectsToRemove[i]);
                }

                for (int i = 0; i < ClickableObjectsToAdd.Count; i++)
                {
                    ClickableObjects.Add(ClickableObjectsToAdd[i]);
                }
                for (int i = 0; i < PollableObjectsToAdd.Count; i++)
                {
                    PollableObjects.Add(PollableObjectsToAdd[i]);
                }
                ClickableObjectsToRemove.Clear();
                PollableObjectsToRemove.Clear();
                ClickableObjectsToAdd.Clear();
                PollableObjectsToAdd.Clear();

            }
            else
            {
                ClickableObjects.Clear();
                PollableObjects.Clear();
                ClickableObjectsToRemove.Clear();
                PollableObjectsToRemove.Clear();
                ClickableObjectsToAdd.Clear();
                PollableObjectsToAdd.Clear();
                PreviouslyClickedObject = null;
                ClearOnQueue = false;
            }

            for (int i = 0; i < ClickableObjectsToAddPersistent.Count; i++)
            {
                ClickableObjects.Add(ClickableObjectsToAddPersistent[i]);
            }
            ClickableObjectsToAddPersistent.Clear();
            for (int i = 0; i < PollableObjectsToAddPersistent.Count; i++)
            {
                PollableObjects.Add(PollableObjectsToAddPersistent[i]);
            }
            PollableObjectsToAddPersistent.Clear();
        }

        public static void RegisterClickableObjectOnQueue(IClickable Object)
        {
            ClickableObjectsToAdd.Add(Object);
        }

        public static void RegisterPollableObjectOnQueue(IPoll Object)
        {
            PollableObjectsToAdd.Add(Object);
        }

        public static void RegisterClickableObjectOnQueuePersistent(IClickable Object)
        {
            ClickableObjectsToAddPersistent.Add(Object);
        }

        public static void RegisterPollableObjectOnQueuePersistent(IPoll Object)
        {
            PollableObjectsToAddPersistent.Add(Object);
        }


        public static void RemoveAllListenersUnsafe()
        {
            ClickableObjects.Clear();
            PollableObjects.Clear();
            ClickableObjectsToRemove.Clear();
            PollableObjectsToRemove.Clear();
            ClickableObjectsToAdd.Clear();
            PollableObjectsToAdd.Clear();

            PreviouslyClickedObject = null;
        }

        public static void RemoveAllListenersOnQueue()
        {
            ClearOnQueue = true;
        }

        public static void RemoveClickableObjectOnQueue(IClickable Object)
        {
            ClickableObjectsToRemove.Add(Object);
        }

        public static void RemovePollableObjectOnQueue(IPoll Object)
        {
            PollableObjectsToRemove.Add(Object);
        }

    }
}
