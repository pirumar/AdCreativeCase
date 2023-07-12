using System.Net;

namespace AdCreativeCase.Downloader
{
    public class ImageDownloader : IImageDownloader
    {

        public static ImageDownloader Instance = new();

        private int totalImages { get; set; }
        private string savePath { get; set; }
        private int downloadedCount { get; set; }
        private int parallelism { get; set; }
        private bool isCancelled { get; set; }
        ProgressManager progressManager { get; set; }
        bool cleanUpCompleted { get; set; }

        public void Start()
        {
            ReadInput();
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                isCancelled = true;
                cleanUpCompleted = false;
                Cleanup();
            };

            DownloadImages();
        }

        public void ReadInput()
        {
            Console.WriteLine("Enter the number of images to download:");
            totalImages = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Enter the number of parallel download:");
            parallelism = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Enter the save path (default: ./outputs):");
            savePath = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(savePath))
                savePath = "./outputs/";
        }

        public void DownloadImages()
        {
            ProgressWriter progressWriter = new ProgressWriter(totalImages, parallelism);
            progressManager = new ProgressManager();
            progressManager.ProgressUpdated += progressWriter.UpdateProgress;

            progressWriter.Start();
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;

            List<Task> tasks = new List<Task>();
            for (int i = 0; i < totalImages; i++)
            {
                if (isCancelled)
                    break;

                string imageUrl = $"https://picsum.photos/400/400?{i}";
                string imagePath = Path.Combine(savePath, $"{i + 1}.png");

                tasks.Add(Task.Run(() => DownloadImage(i, imageUrl, imagePath)));

                if (tasks.Count >= parallelism || i == totalImages - 1)
                    Task.WaitAny(tasks.ToArray());
                tasks.RemoveAll(t => t.IsCompleted);

            }
            progressWriter.Complete();
            if (isCancelled)
            {
                cts.Cancel();

                while (!cleanUpCompleted)
                {
                    Thread.Sleep(100);
                }
                Console.WriteLine("Operation Cancelled From User");
            }
            else
            {
                Console.WriteLine("All images downloaded successfully.");

            }


        }

        public void DownloadImage(int i, string imageUrl, string imagePath)
        {
            using (WebClient client = new WebClient())
            {
                client.DownloadFile(imageUrl, imagePath);
                downloadedCount++;
                progressManager.UpdateProgress(i + 1, totalImages);

            }
        }

        public void Cleanup()
        {
            for (int i = 1; i <= downloadedCount; i++)
            {
                string imagePath = Path.Combine(savePath, $"{i}.png");

                if (File.Exists(imagePath))
                {
                    try
                    {
                        File.Delete(imagePath);
                    }
                    catch (Exception E)
                    {

                    }
                }
            }
            cleanUpCompleted = true;
        }


    }
}
