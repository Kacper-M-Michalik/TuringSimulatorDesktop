using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringSimulatorDesktop.UI;

namespace TuringSimulatorDesktop.UI
{    
    public class TextProgrammingView : View
    {
        //OldButton SaveButton;
        //OldButton CompileButton;

        List<StateTransitionView> TransitionList;

        public TextProgrammingView()
        {

        }

        public void SaveFile()
        {

        }

        public void LoadFile()
        {

        }

        public void AddTransition()
        {
            //StateTransitionPrefab Transition = new StateTransitionPrefab();
            //TransitionList.Add(Transition);
            //Tiler.Elements.Add(Transition);
            //Tiler.Tile();
        }

        public void RemoveTransition()
        {

        }

        public override void Draw()
        {

        }

        public override void ViewResize(int NewWidth, int NewHeight)
        {

        }

        public override void ViewPositionSet(int X, int Y)
        {
            throw new NotImplementedException();
        }

        /*
        public override void Draw(SpriteBatch OwnerSpriteBatch, RenderTarget2D PreviousRenderTarget)
        {
            for (int i = 0; i < TransitionList.Count; i++)
            {
                TransitionList[i].BackgroundLabel.Draw(OwnerSpriteBatch, PreviousRenderTarget);
                TransitionList[i].CurrentStateTextBox.Draw(OwnerSpriteBatch, PreviousRenderTarget);
                TransitionList[i].TapeValueTextBox.Draw(OwnerSpriteBatch, PreviousRenderTarget);
                TransitionList[i].NewStateTextBox.Draw(OwnerSpriteBatch, PreviousRenderTarget);
                TransitionList[i].NewTapeValueTextBox.Draw(OwnerSpriteBatch, PreviousRenderTarget);
                TransitionList[i].MoveDirectionTextBox.Draw(OwnerSpriteBatch, PreviousRenderTarget);
            }
        }
        */
    }
}
