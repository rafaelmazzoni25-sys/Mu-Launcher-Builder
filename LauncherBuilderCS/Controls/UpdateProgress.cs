namespace LauncherBuilderCS.Controls
{
    internal readonly struct UpdateProgress
    {
        public UpdateProgress(int completed, int total, string currentFile)
        {
            Completed = completed;
            Total = total;
            CurrentFile = currentFile;
        }

        public int Completed { get; }

        public int Total { get; }

        public string CurrentFile { get; }
    }
}
