using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuringCore
{
    public abstract class TuringAction
    {
        public abstract void Execute(TuringMachine Machine);
    }
}
