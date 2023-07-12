using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdCreativeCase.Downloader
{
    public interface IImageDownloader
    {
       public void Start();
        public void ReadInput();
        public void DownloadImages();
        public void Cleanup();
    }
}
