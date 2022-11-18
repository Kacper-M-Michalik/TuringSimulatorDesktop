using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringBackend.Systems.VisualProgramming;
using TuringSimulatorDesktop.Input;

namespace TuringSimulatorDesktop.UI
{
    public class VisualProgrammingWindow : ParentWindow
    {
        public VisualProgrammingProject LoadedProject;

        VPActionMenu ActionMenu;
        Vector2 CameraPosition;
        float Zoom;

        public VisualProgrammingWindow(GraphicsDevice graphicsDevice, int SetWidth, int SetHeight) : base(graphicsDevice, SetWidth, SetHeight)
        {
            ActionMenu = new VPActionMenu();
            CameraPosition = Vector2.Zero;
            Zoom = 1f;
        }

        public override void GetView()
        {
            RendererGraphicsDevice.SetRenderTarget(ViewRenderTarget);
            RendererGraphicsDevice.Clear(Color.Yellow);

            LocalSpriteBatch.Begin(); 

            if (LoadedProject != null)
            {
                foreach (Node node in LoadedProject.StoredNodes)
                {
                    Texture2D Texture = TextureLookup[TextureLookupKey.StateNodeBackground];
                    LocalSpriteBatch.Draw(Texture, new Vector2(node.X * Zoom + CameraPosition.X, node.Y * Zoom + CameraPosition.Y), null, Color.White, 0f, new Vector2(Texture.Width / 2f, Texture.Height / 2f), Zoom, SpriteEffects.None, 0f);
                }
            }

            ActionMenu.Draw(LocalSpriteBatch, ViewRenderTarget);

            LocalSpriteBatch.End();            
        }

        /*
        public override void Update(GameTime gameTime)
        {
            Zoom = MathHelper.Clamp(Zoom + InputManager.VPWZoomInput, 0.1f, 2f);

            if (MouseWithinBounds())
            {
                if (InputManager.VPWScrollInput == ButtonState.Pressed)
                {
                    CameraPosition += InputManager.MouseDelta;
                }

                if (InputManager.VPWActionMenuInput == ButtonState.Pressed)
                {
                    ActionMenu.Position = InputManager.CurrentMousePosition;
                    ActionMenu.Hidden = false;
                }
            }            
        }
        */
    }
}
