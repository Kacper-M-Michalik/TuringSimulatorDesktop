using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuringSimulatorDesktop.UI
{
    public interface IView : IVisualElement
    {
        public string Title { get; }
        public bool IsActive { get; set; }
    }
}
