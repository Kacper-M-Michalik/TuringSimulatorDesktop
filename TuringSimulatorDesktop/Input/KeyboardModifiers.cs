﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuringSimulatorDesktop.Input
{
    //Used to limit text box inputs
    public class KeyboardModifiers
    {
        public bool AllowsShift = true;
        public bool AllowsNewLine = true;
        public bool AllowsNumbers = true;
        public bool AllowsCharacters = true;
        public bool AllowsSymbols = true;
    }
}
