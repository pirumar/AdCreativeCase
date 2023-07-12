using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdCreativeCase.Downloader
{
    public interface IImageDownloader
    {
        void Start();
        void ReadInput();
        void DownloadImages();
        void Cleanup();
    }
}
