using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic; 
using System.Text;
using System.Runtime.CompilerServices;

namespace TuringSimulatorDesktop.UI
{
    public static class UIUtils
    {
        //Marks to compiler to inline this function
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ConvertFloatToMinInt(float Value, float Min)
        {
            return Convert.ToInt32(Math.Clamp(MathF.Round(Value /*, MidpointRounding.AwayFromZero*/), Min, float.PositiveInfinity));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ConvertFloatToInt(float Value)
        {
            return Convert.ToInt32(Value);//MathF.Round(Value, MidpointRounding.AwayFromZero));
        }

        //Generates overlapping rectangle of two input rectangles
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Viewport CalculateOverlapPort(Viewport Left, Viewport Right)
        {
            int LeftLeft = Left.X;
            int LeftRight = Left.X + Left.Width;
            int RightLeft = Right.X;
            int RightRight = Right.X + Right.Width;

            if (LeftRight < RightLeft || RightRight < LeftLeft) return new Viewport(0, 0, 0, 0);

            Viewport Port = new Viewport();
            
            Port.X = Math.Max(LeftLeft, RightLeft);

            if (LeftRight > RightRight)
            {                
                Port.Width = RightRight - Port.X;
            }
            else
            {
                Port.Width = LeftRight - Port.X;
            }

            LeftLeft = Left.Y;
            LeftRight = Left.Y + Left.Height;
            RightLeft = Right.Y;
            RightRight = Right.Y + Right.Height;

            Port.Y = Math.Max(LeftLeft, RightLeft);

            if (LeftRight > RightRight)
            {
                Port.Height = RightRight - Port.Y;
            }
            else
            {
                Port.Height = LeftRight - Port.Y;
            }
            return Port;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix CreateTranslation(Vector2 Position)
        {
            return Matrix.CreateTranslation(new Vector3(Position, 0));//, Vector3.Forward, Vector3.Up);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDefaultViewport(Viewport Port)
        {
            return (Port.X == 0 && Port.Y == 0 && Port.Width == 0 && Port.Height == 0);
        }
    }

}
