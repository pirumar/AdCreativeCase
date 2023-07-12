using System.Net;

namespace AdCreativeCase
{
    internal class ImageDownloader
    {

        public static ImageDownloader Instance = new();

        private int totalImages;
        private string savePath;
        private int downloadedCount;
        private int parallelism;
        private bool isCancelled;
        ProgressManager progressManager;
        bool cleanUpCompleted = false;
        public void Start()
        {
            ReadInput();

            // Setup cancellation handler
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                isCancelled = true;
                cleanUpCompleted = false;
                Cleanup();
            };

            DownloadImages();
        }

        void ReadInput()
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

        void DownloadImages()
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

        void DownloadImage(int i, string imageUrl, string imagePath)
        {
            using (WebClient client = new WebClient())
            {
                client.DownloadFile(imageUrl, imagePath);
                downloadedCount++;
                progressManager.UpdateProgress(i + 1, totalImages);

            }
        }

        void Cleanup()
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
