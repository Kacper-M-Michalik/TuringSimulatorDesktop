using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuringSimulatorDesktop.UI
{
    public interface IRunnable
    {
        public string Title { get; }
        public int OpenFileID { get; }
    }
}
