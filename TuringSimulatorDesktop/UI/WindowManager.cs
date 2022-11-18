using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TuringBackend;
using Microsoft.Xna.Framework.Graphics;
using TuringSimulatorDesktop.Input;

namespace TuringSimulatorDesktop.UI
{
    public class WindowManager
    {
        public List<Window> Windows;
        Window CurrentlyFocusedWindow;


        bool IsDragging;

        public WindowManager()
        {
            Windows = new List<Window>();
        }

        public void Update()
        {
            //deal with drag and snap

            //deal with focus

            //update windwos



            /*
            int i = Windows.Count - 1;
            bool AssignedFocus = false;
            if (!IsDragging)
            {
                while (!AssignedFocus && i > -1)
                {
                    if (Windows[i].IsMouseOverWindow())
                    {
                        AssignedFocus = true;
                        if (CurrentlyFocusedWindow != null) CurrentlyFocusedWindow.InFocus = false;
                        CurrentlyFocusedWindow = Windows[i];
                        CurrentlyFocusedWindow.InFocus = true;
                        DebugManager.CurrentWindow = CurrentlyFocusedWindow;

                        if (InputManager.LeftMousePressed)
                        {
                            Windows.RemoveAt(i);
                            Windows.Add(CurrentlyFocusedWindow);
                        }
                    }
                    i--;
                }
                if (!AssignedFocus) DebugManager.CurrentWindow = null;
            }
            */
            int i = Windows.Count - 1;
            bool AssignedFocus = false;
            if (!IsDragging)
            {
                while (!AssignedFocus && i > -1)
                {
                    if (Windows[i].IsMouseOverWindow())
                    {
                        AssignedFocus = true;
                        CurrentlyFocusedWindow = Windows[i];
                        DebugManager.CurrentWindow = CurrentlyFocusedWindow;

                        if (InputManager.LeftMousePressed)
                        {
                            Windows.RemoveAt(i);
                            Windows.Add(CurrentlyFocusedWindow);
                        }
                    }
                    i--;
                }
                if (!AssignedFocus) DebugManager.CurrentWindow = null;
            }

            if (IsDragging)
            {
                CurrentlyFocusedWindow.Position += InputManager.MouseDelta;

                if (InputManager.LeftMouseReleased)
                {
                    IsDragging = false;
                }
            }

            if (!IsDragging && InputManager.LeftMousePressed && CurrentlyFocusedWindow != null && CurrentlyFocusedWindow.IsMouseOverTab())
            {
                IsDragging = true;
            }

            for (int j = 0; j < Windows.Count; j++)
            {
                Windows[j].Update();
            }

             ////////////////////deal with movement and snmap of windows?
        }

        public void Draw(SpriteBatch Batch)
        {
            for (int i = 0; i < Windows.Count; i++)
            {
                Windows[i].Draw(Batch);
            }            
        }

        public void AddWindow()
        {
            Window Win = new Window(300, 400);
            Windows.Add(Win);
        }
    }

    public class WindowSnap
    {
        int X, Y;
        int Width, Height;

        float SnapX, SnapY;
        float SnapWidth, SnapHeight;
    }
}
