using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuringServer
{
    public struct CreateProjectReturnData
    {
        public bool Success;
        public string SolutionPath;

        //Constructor
        public CreateProjectReturnData(bool success, string solutionPath)
        {
            Success = success;
            SolutionPath = solutionPath;
        }
    }
}
