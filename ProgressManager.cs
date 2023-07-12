namespace AdCreativeCase
{
    public delegate void ProgressUpdatedEventHandler(int currentCount, int totalCount);

    public class ProgressManager
    {
        public event ProgressUpdatedEventHandler ProgressUpdated;

        public void UpdateProgress(int currentCount, int totalCount)
        {
            ProgressUpdated?.Invoke(currentCount, totalCount);
        }
    }
        
    public class ProgressWriter
    {
        private int totalCount;
        private int parallelism;
        public ProgressWriter(int totalCount, int parallelism)
        {
            this.totalCount = totalCount;
            this.parallelism = parallelism;
        }

        public void Start()
        {
            Console.WriteLine($"Downloading {totalCount} images ({parallelism} parallel downloads at most)");
        }

        public void UpdateProgress(int currentCount, int totalCount)
        {
            Console.SetCursorPosition(0, Console.CursorTop);

            Console.Write($"Progress: {currentCount}/{totalCount}", 0);
        }

        public void Complete()
        {
            Console.WriteLine();
        }
    }


}
