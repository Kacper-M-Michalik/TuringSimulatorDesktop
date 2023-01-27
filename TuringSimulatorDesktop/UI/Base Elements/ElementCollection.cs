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
    public class ElementCollection : IVisualElement
    {
        Vector2 position;
        public Vector2 Position
        {
            get => position;
            set
            {
                position = value;
                MoveLayout();
            }
        }

        Point bounds;
        public Point Bounds
        {
            get => bounds;
            set
            {
                bounds = value;
            }
        }

        bool isActive = true;
        public bool IsActive
        {
            get => isActive;
            set
            {
                isActive = value;
                for (int i = 0; i < Elements.Count; i++)
                {
                    Elements[i].IsActive = value;
                }
            }
        }

        public List<IVisualElement> Elements;
        public List<Vector2> Offsets;

        public ElementCollection()
        {
            Elements = new List<IVisualElement>();
            Offsets = new List<Vector2>();

            Bounds = Point.Zero;
            Position = Vector2.Zero;
        }

        public void AddElement(IVisualElement Element)
        {
            Elements.Add(Element);
            Offsets.Add(new Vector2());
        }

        void MoveLayout()
        {
            for (int i = 0; i < Elements.Count; i++)
            {
                Elements[i].Position = position + Offsets[i];
            }
        }

        public void Draw(Viewport? BoundPort = null)
        {
            if (IsActive)
            {
                for (int i = 0; i < Elements.Count; i++)
                {
                    Elements[i].Draw(BoundPort);
                }
            }
        }

      //  public void Close()
      //  {
          //  for (int i = 0; i < Elements.Count; i++)
         //   {
          //      Elements[i].Close();
          //  }
      //  }
    }
}
