﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuringSimulatorDesktop.UI.Prefabs
{
    public class TapeItem : IVisualElement
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
                ResizeLayout();
            }
        }

        bool isActive;
        public bool IsActive
        {
            get => isActive;
            set
            {
                isActive = value;
            }
        }

        HorizontalLayoutBox TapeContents;
        public float CameraMin, CameraMax;

        void MoveLayout()
        {
        }

        void ResizeLayout()
        {
        }

        public void Draw(Viewport? BoundPort = null)
        {
            throw new NotImplementedException();
        }
    }
}
