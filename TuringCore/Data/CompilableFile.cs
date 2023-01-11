using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuringCore
{
    public abstract class CompilableFile
    {
        public abstract StateTable Compile();
    }
}
