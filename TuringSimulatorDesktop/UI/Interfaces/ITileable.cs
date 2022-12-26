﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuringSimulatorDesktop.UI
{
    public interface ITileable
    {
        public Vector2 Position { get; set; }
        public Vector2 GetBounds { get; }
    }
}
