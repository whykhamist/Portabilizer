using System;
using System.Collections.Generic;
using System.Text;

namespace Systems
{
    public class FolderInformation
    {
        /// <summary>
        /// The full path of the folder
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Total number of files in the folder and subfolders
        /// </summary>
        public int FileCount { get; set; } = 0;

        /// <summary>
        /// Total number of folders and subfolders
        /// </summary>
        public int FolderCount { get; set; } = -1;

        /// <summary>
        /// Total size in bytes of the contents of the folder
        /// </summary>
        public long Size { get; set; } = 0L;
    }
}
