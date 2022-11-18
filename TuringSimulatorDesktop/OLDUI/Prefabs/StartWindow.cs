using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuringSimulatorDesktop.UI
{
    public class StartWindow : UIElement
    {
        public StartWindow()
        {
            NewProjectButton = new Button(UIElement.TextureLookup[TextureLookupKey.StateNodeBackground], new Vector2(50, 200));
            LoadProjectButton = new Button(UIElement.TextureLookup[TextureLookupKey.StateNodeBackground], new Vector2(50, 280));
            JoinProjectButton = new Button(UIElement.TextureLookup[TextureLookupKey.StateNodeBackground], new Vector2(50, 360));

            ImageElement Background = new ImageElement(UIElement.TextureLookup[TextureLookupKey.StateNodeBackground], new Vector2(400, 360));

            NewProjectButton.Clicked += NewProjectWindow;
            LoadProjectButton.Clicked += LoadProjectWindow;
            JoinProjectButton.Clicked += JoinProjectWindow;
        }

        Button NewProjectButton;
        Button LoadProjectButton;
        Button JoinProjectButton;

        public void NewProjectWindow(Button Sender)
        {

        }

        public void LoadProjectWindow(Button Sender)
        {

        }

        public void JoinProjectWindow(Button Sender)
        {

        }

        public override void Draw(SpriteBatch OwnerSpriteBatch, RenderTarget2D CurrentRenderTarget)
        {
            throw new NotImplementedException();
        }

    }
}
