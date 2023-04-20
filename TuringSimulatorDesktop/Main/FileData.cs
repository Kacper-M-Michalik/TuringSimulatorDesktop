using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringCore;

namespace TuringSimulatorDesktop
{
    public class FileData
    {
        public string Name;
        public int ID;
        public Guid GUID;
        public CoreFileType Type;
        public bool IsFolder;

        public FileData(string SetName, Guid SetGUID, CoreFileType SetType)
        {
            Name = SetName;
            GUID = SetGUID;
            Type = SetType;
            IsFolder = false;
        }

        public FileData(string SetName, int SetID)
        {
            Name = SetName;
            ID = SetID;
            Type = CoreFileType.Other;
            IsFolder = true;
        }
    }
}
