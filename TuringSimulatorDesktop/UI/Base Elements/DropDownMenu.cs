using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringSimulatorDesktop.Input;

namespace TuringSimulatorDesktop.UI
{
    public class DropDownMenu : IVisualElement
    {
        Vector2 position;
        public Vector2 Position { get => position; set { position = value;  DropDownHeader.Position = value; LayoutBox.Position = value + MenuOffset; } }
        public Vector2 GetBounds { get => new Vector2(); }
        public Point Bounds { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsActive = true;

        public int HeaderWidth, HeaderHeight;
        public int MenuWidth, MenuHeight;
        public Vector2 MenuOffset;
        public bool HighlightOnMouseOver;

        ActionGroup Group;
        ButtonIcon DropDownHeader;
        VerticalLayoutBox LayoutBox;
        List<ButtonIcon> MenuButtons;

        public DropDownMenu(int headerWidth, int headerHeight, Vector2 position, ActionGroup group)
        {
            HeaderWidth = headerWidth;
            HeaderHeight = headerHeight;
            Group = group;

            //DropDownHeader = new Button(HeaderWidth, HeaderHeight, position, Group);            
            //LayoutBox = new VerticalLayoutBox(MenuWidth, MenuHeight);

            Position = position;
        }

        public void AddMenuButton(ButtonIcon button)
        {


           //. Button NewButton = new Button(MenuWidth, )
            //MenuButtons.Add(new )
        }

        public void Draw(Viewport? BoundPort = null)
        {
            if (IsActive)
            {
                LayoutBox.Draw(BoundPort);                
            }
        }
    }
}
