using System;
using System.Collections.Generic;
using TuringCore;

namespace TuringServer.Data
{
    public class CacheFileData
    {
        public byte[] FileData;
        public long ExpiryTimer;

        public CacheFileData(byte[] SetFileData)
        {
            FileData = SetFileData;
            ExpiryTimer = 0;
        }

        public void ResetExpiryTimer()
        {
            ExpiryTimer = 0;
        }
    }

    public class ProjectData
    {
        //Cache System
        public Dictionary<int, CacheFileData> CacheDataLookup;
        public Dictionary<int, DirectoryFile> FileDataLookup;
        public Dictionary<int, DirectoryFolder> FolderDataLookup;

        //Data
        public string ProjectName;
        public string BasePath;
        public string ProjectFilePath;
        public TuringProjectType TuringTypeRule;

        public DirectoryFolder BaseDirectoryFolder;
        public Dictionary<Guid, int> GuidFileLookup;

        //Rules get sent to client -> client Ui responsible for telling uiser cant run turing with these rules, unless is server simulated turing
        //Allow server to simualte hese windwos as to have mutiple users have access to same window not only file:
        //windows
        //turing machine
        //data visualization
        //flow diagram viewer
        //alphabet manager
        //project settings
    }
}
