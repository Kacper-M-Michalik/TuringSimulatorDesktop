using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuringSimulatorDesktop.UI
{
    public interface ICanvasInteractable : IVisualElement
    {
        public void SetProjectionMatrix(Matrix projectionMatrix, Matrix inverseProjectionMatrix);
    }
}
