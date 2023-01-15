using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringSimulatorDesktop.UI.Prefabs;

namespace TuringSimulatorDesktop.UI
{
    public interface IView : IVisualElement
    {
        public string Title { get; }
        public int OpenFileID { get; }
        public Window OwnerWindow { get; set; }
    }
}
