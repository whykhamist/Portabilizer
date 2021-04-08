
namespace Systems
{
    public class FolderCopyProgressInfo : FolderInformation
    {
        public FileCopyProgressInfo FileCopyProgressInfo = new();

        /// <summary>
        /// The filename currently being copied.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// The filename including path currently being copied.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// The destination of file copied.
        /// </summary>
        public string Destination { get; set; }

        /// <summary>
        /// The number of files successfully copied.
        /// </summary>
        public int FinishedCount { get; set; }

        /// <summary>
        /// The Progress (in percentage value) of the copy task.
        /// </summary>
        public double Progress { get; set; }


        /// <summary>
        /// Check if operation has been cancelled
        /// </summary>
        public bool Cancelled { get; set; } = false;
    }
}
