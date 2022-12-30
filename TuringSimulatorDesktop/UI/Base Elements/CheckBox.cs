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
    public delegate void OnCheckBoxClick(CheckBox Sender);

    public class CheckBox : IVisualElement, IClickable, IPollable
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public bool IsActive = true;

        public bool HighlightOnMouseOver = true;
        public Texture2D BaseUncheckedTexture;
        public Texture2D BaseCheckedTexture;
        public Texture2D HighlightUncheckedTexture;
        public Texture2D HighlightCheckedTexture;

        public UIMesh Background;

        Vector2 position;
        public Vector2 Position { get => position; set { position = value; Background.MeshTransformations = Matrix.CreateWorld(new Vector3(position.X, position.Y, 0), Vector3.Forward, Vector3.Up); } }
        public Vector2 GetBounds { get => new Vector2(Width, Height); }

        public bool Checked = false;
        public event OnCheckBoxClick OnClickedEvent;
        public ActionGroup Group { get; private set; }

        public CheckBox(int width, int height, Vector2 position, ActionGroup group)
        {
//#if DEBUG
            //Background = UIMesh.CreateRectangle(Vector2.Zero, width, height, GlobalInterfaceData.UIOverlayDebugColor1);
//#else
            //Background = UIMesh.CreateRectangle(Vector2.Zero, width, height, Color.Transparent);
//#endif
            Width = width;
            Height = height;
            Position = position;

            Group = group;
            group.ClickableObjects.Add(this);
            group.PollableObjects.Add(this);
        }

        public void UpdateSize(int SetWidth, int SetHeight)
        {
            Width = SetWidth;
            Height = SetHeight;
            Background.UpdateMesh(UIMesh.CreateRectangle(Vector2.Zero, Width, Height));
        }

        public void Clicked()
        {
            Checked = !Checked;
            OnClickedEvent?.Invoke(this);
        }

        public void ClickedAway()
        {

        }

        public bool IsMouseOver()
        {
            return (IsActive && InputManager.MouseData.X >= Position.X && InputManager.MouseData.X <= Position.X + Width && InputManager.MouseData.Y >= Position.Y && InputManager.MouseData.Y <= Position.Y + Height);
        }

        public void PollInput(bool IsInActionGroupFrame)
        {
            if (IsInActionGroupFrame && HighlightOnMouseOver && IsMouseOver())
            {      
                if (Checked)
                {
                   // if (HighlightCheckedTexture == null) Background.OverlayColor = GlobalInterfaceData.UIOverlayDebugColor4;
                   // else Background.Texture = HighlightCheckedTexture;
                }
                else
                {
                   // if (HighlightUncheckedTexture == null) Background.OverlayColor = GlobalInterfaceData.UIOverlayDebugColor2;
                  //  else Background.Texture = HighlightUncheckedTexture;
                }
                
            }
            else
            {
                if (Checked)
                {
                    //if (BaseCheckedTexture == null) Background.OverlayColor = GlobalInterfaceData.UIOverlayDebugColor3;
                   // else Background.Texture = BaseCheckedTexture;
                }
                else
                {
                    //if (BaseUncheckedTexture == null) Background.OverlayColor = GlobalInterfaceData.UIOverlayDebugColor1;
                    //else Background.Texture = BaseUncheckedTexture;
                }
            }
        }

        public void Draw(Viewport BoundPort = default)
        {
            if (IsActive)
            {
                if (UIUtils.IsDefaultViewport(BoundPort))
                {
                    GlobalUIRenderer.Draw(Background);
                }
                else
                {
                    GlobalUIRenderer.Draw(Background, BoundPort);
                }
            }
        }
    }
}
