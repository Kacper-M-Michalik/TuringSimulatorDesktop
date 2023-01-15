using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
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

            if (LeftMousePressed || RightMousePressed)
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
                if (ActionGroups[i].IsMarkedForDeletion && !ActionGroups[i].IsPersistant) ActionGroups.RemoveAt(i);
                else
                {
                    if (ActionGroups[i].IsDirtyClickable)
                    {
                        for (int j = ActionGroups[i].ClickableObjects.Count - 1; j > -1; j--)
                        {
                            if (ActionGroups[i].ClickableObjects[j].IsMarkedForDeletion) ActionGroups[i].ClickableObjects.RemoveAt(j);
                        }
                        ActionGroups[i].IsDirtyClickable = false;
                    }
                    if (ActionGroups[i].IsDirtyPollable)
                    {
                        for (int j = ActionGroups[i].PollableObjects.Count - 1; j > -1; j--)
                        {
                            if (ActionGroups[i].PollableObjects[j].IsMarkedForDeletion) ActionGroups[i].PollableObjects.RemoveAt(j);
                        }
                        ActionGroups[i].IsDirtyPollable = false;
                    }
                }
            }
        }

        public static void DrawActionGroups()
        {
            for (int i = 0; i < ActionGroups.Count; i++)
            {
                Icon debug = new Icon();
                if (i % 3 == 0)
                {
                    debug.DrawColor = new Color(new Color(GlobalInterfaceData.Scheme.UIOverlayDebugColor4.ToVector3()), 20);
                }
                else if (i % 2 == 0)
                {
                    debug.DrawColor = new Color(new Color(GlobalInterfaceData.Scheme.UIOverlayDebugColor1.ToVector3()), 20);
                }
                else
                {
                    debug.DrawColor = new Color(new Color(GlobalInterfaceData.Scheme.UIOverlayDebugColor3.ToVector3()), 20);
                }

                debug.Bounds = new Point(ActionGroups[i].Width, ActionGroups[i].Height);
                debug.Position = new Vector2(ActionGroups[i].X, ActionGroups[i].Y);
                debug.Draw();
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

        public static void SendActionGroupToFront(ActionGroup Group)
        {
            int Index = ActionGroups.IndexOf(Group);
            ActionGroups.Add(ActionGroups[Index]);
            ActionGroups.RemoveAt(Index);
        }

        public static void DeleteAllActionGroups()
        {
            for (int i = 0; i < ActionGroups.Count; i++) ActionGroups[i].IsMarkedForDeletion = true;
            PreviouslyClickedObject = null;
        }

        //Depracted
        /*
        public static char? KeyToChar(Keys Key, KeyboardModifiers Modifiers, bool Shift, bool Control, bool Alt)
        {
            //TODO integrate allows charcter and numbers and symbols parts
            bool IsShiftDown = Modifiers.AllowsShift && Shift;
            bool IsAltGearDown = false;
            if (!IsShiftDown) IsAltGearDown = (Modifiers.AllowsAltGear && Control) || (Modifiers.AllowsAltGear && Alt);

            if (IsAltGearDown && Modifiers.AllowsSymbols)
            {
                //international keyboards
                if (Key == Keys.V) return '@';
                if (Key == Keys.D2) return '@';
                if (Key == Keys.Q) return '@';
            }

            if (Modifiers.AllowsCharacters)
            {
                switch (Key)
                {
                    case Keys.A:
                        return IsShiftDown ? 'A' : 'a';
                    case Keys.B:
                        return IsShiftDown ? 'B' : 'b';
                    case Keys.C:
                        return IsShiftDown ? 'C' : 'c';
                    case Keys.D:
                        return IsShiftDown ? 'D' : 'd';
                    case Keys.E:
                        return IsShiftDown ? 'E' : 'e';
                    case Keys.F:
                        return IsShiftDown ? 'F' : 'f';
                    case Keys.G:
                        return IsShiftDown ? 'G' : 'g';
                    case Keys.H:
                        return IsShiftDown ? 'H' : 'h';
                    case Keys.I:
                        return IsShiftDown ? 'I' : 'i';
                    case Keys.J:
                        return IsShiftDown ? 'J' : 'j';
                    case Keys.K:
                        return IsShiftDown ? 'K' : 'k';
                    case Keys.L:
                        return IsShiftDown ? 'L' : 'l';
                    case Keys.M:
                        return IsShiftDown ? 'M' : 'm';
                    case Keys.N:
                        return IsShiftDown ? 'N' : 'n';
                    case Keys.O:
                        return IsShiftDown ? 'O' : 'o';
                    case Keys.P:
                        return IsShiftDown ? 'P' : 'p';
                    case Keys.Q:
                        return IsShiftDown ? 'Q' : 'q';
                    case Keys.R:
                        return IsShiftDown ? 'R' : 'r';
                    case Keys.S:
                        return IsShiftDown ? 'S' : 's';
                    case Keys.T:
                        return IsShiftDown ? 'T' : 't';
                    case Keys.U:
                        return IsShiftDown ? 'U' : 'u';
                    case Keys.V:
                        return IsShiftDown ? 'V' : 'v';
                    case Keys.W:
                        return IsShiftDown ? 'W' : 'w';
                    case Keys.X:
                        return IsShiftDown ? 'X' : 'x';
                    case Keys.Y:
                        return IsShiftDown ? 'Y' : 'y';
                    case Keys.Z:
                        return IsShiftDown ? 'Z' : 'z';
                    case Keys.Space:
                        return ' ';
                    default:
                        break;
                }
            }

            if (!IsShiftDown && Modifiers.AllowsNumbers)
            {
                switch (Key)
                {
                    case Keys.D0:
                        return '0';
                    case Keys.D1:
                        return '1';
                    case Keys.D2:
                        return '2';
                    case Keys.D3:
                        return '3';
                    case Keys.D4:
                        return '4';
                    case Keys.D5:
                        return '5';
                    case Keys.D6:
                        return '6';
                    case Keys.D7:
                        return '7';
                    case Keys.D8:
                        return '8';
                    case Keys.D9:
                        return '9';
                    case Keys.NumPad0:
                        return '0';
                    case Keys.NumPad1:
                        return '1';
                    case Keys.NumPad2:
                        return '2';
                    case Keys.NumPad3:
                        return '3';
                    case Keys.NumPad4:
                        return '4';
                    case Keys.NumPad5:
                        return '5';
                    case Keys.NumPad6:
                        return '6';
                    case Keys.NumPad7:
                        return '7';
                    case Keys.NumPad8:
                        return '8';
                    case Keys.NumPad9:
                        return '9';
                    default:
                        break;
                }
            }

            if (IsShiftDown && Modifiers.AllowsSymbols)
            {
                switch (Key)
                {
                    case Keys.D0:
                        return ')';
                    case Keys.D1:
                        return '!';
                    case Keys.D2:
                        return '"';
                    case Keys.D3:
                        return '£';
                    case Keys.D4:
                        return '$';
                    case Keys.D5:
                        return '%';
                    case Keys.D6:
                        return '^';
                    case Keys.D7:
                        return '&';
                    case Keys.D8:
                        return '*';
                    case Keys.D9:
                        return '(';
                    default:
                        break;
                }
            }

            //TODO rewrite this part
            if (Modifiers.AllowsSymbols)
            {
                switch (Key)
                {
                    case Keys.Add:
                        return '+';
                    case Keys.Decimal:
                        return '.';
                    case Keys.Divide:
                        return '/';
                    case Keys.Multiply:
                        return '*';
                    case Keys.Subtract:
                        return '-';
                    case Keys.OemBackslash:
                        return '\\';
                    default:
                        break;
                }
                if ((Key == Keys.OemComma) && !IsShiftDown) return ',';
                if ((Key == Keys.OemComma) && IsShiftDown) return '<';
                //if ((Key == Keys.) && !IsShiftDown) return ',';
                //if ((Key == Keys.OemComma) && IsShiftDown) return '<';
                if ((Key == Keys.OemOpenBrackets) && !IsShiftDown) return '[';
                if ((Key == Keys.OemOpenBrackets) && IsShiftDown) return '{';
                if ((Key == Keys.OemCloseBrackets) && !IsShiftDown) return ']';
                if ((Key == Keys.OemCloseBrackets) && IsShiftDown) return '}';
                if ((Key == Keys.OemPeriod) && !IsShiftDown) return '.';
                if ((Key == Keys.OemPeriod) && IsShiftDown) return '>';
                if ((Key == Keys.OemPipe) && !IsShiftDown) return '\\';
                if ((Key == Keys.OemPipe) && IsShiftDown) return '|';
                if ((Key == Keys.OemPlus) && !IsShiftDown) return '=';
                if ((Key == Keys.OemPlus) && IsShiftDown) return '+';
                if ((Key == Keys.OemMinus) && !IsShiftDown) return '-';
                if ((Key == Keys.OemMinus) && IsShiftDown) return '_';
                if ((Key == Keys.OemQuestion) && !IsShiftDown) return '/';
                if ((Key == Keys.OemQuestion) && IsShiftDown) return '?';
                if ((Key == Keys.OemQuotes) && !IsShiftDown) return '\'';
                if ((Key == Keys.OemQuotes) && IsShiftDown) return '@';
                if ((Key == Keys.OemSemicolon) && !IsShiftDown) return ';';
                if ((Key == Keys.OemSemicolon) && IsShiftDown) return ':';
                if ((Key == Keys.OemTilde) && !IsShiftDown) return '`';
                if ((Key == Keys.OemTilde) && IsShiftDown) return '~';
            }
            return null;
        }
        */
    }
}
