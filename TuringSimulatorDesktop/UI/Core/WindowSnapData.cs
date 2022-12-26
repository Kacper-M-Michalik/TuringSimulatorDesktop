using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuringSimulatorDesktop.UI
{
    public class WindowSnapData
    {
        //Hitbox
        int X, Y;
        int Width, Height;

        //Anchor point
        float SnapX, SnapY;
    }

    public class WindowGroupData
    {
        public List<WindowGroupData> SubGroups;
        public WindowView ChildWindow;
        public GroupType TilingOrientation;

        int X, Y;
        int Width, Height;

        public WindowGroupData()
        {
            SubGroups = new List<WindowGroupData>();
        }

        public void Resize(int X, int Y, int Width, int Height)
        {
            if (TilingOrientation == GroupType.Horizontal)
            {

            }
            else
            {

            }
        }
    }

    public class WindowGroupConnection
    {
        public WindowGroupData Left;
        public WindowGroupData Right;
        public GroupConnectionType Orientation;
    }

    public enum GroupConnectionType { Vertical, Horizontal }
    public enum GroupType { Vertical, Horizontal }
}
