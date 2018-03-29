//sing ImageService.Infrastructure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ImageService.Modal
{
    public class ImageServiceModal : IImageServiceModal
    {        
        private string outputFolder { get; set; }
        private int thumbSize { get; set; }

        public ImageServiceModal(string target, int thumbnail)
        {
            this.outputFolder = target;
            this.thumbSize = thumbnail;
        }
        public string AddFile(string path, out bool result)
        {
            // string fileName = "test.txt";
            string year, month, dstPath;

            try
            {
                if (!File.Exists(path))
                {
                    throw new Exception("File missing!");
                }
                DateTime dateTime = File.GetCreationTime(path);
                year = dateTime.Year.ToString();
                month = dateTime.Month.ToString();

                // check if directory exist

                Directory.CreateDirectory(outputFolder);
                Directory.CreateDirectory(outputFolder + "\\" + "Thubnails");

                // create year folder
                
                dstPath = this.outputFolder + "\\" + year + "\\" + month + "\\";
                File.Copy(path, dstPath + Path.GetFileName(path));

                Image image  = Image.FromFile(path);
                Image thumb = image.GetThumbnailImage(this.thumbSize, this.thumbSize, () => false, IntPtr.Zero);
                thumb.Save(this.outputFolder + "\\Thumbnails" + "\\" + year + "\\" + month + "\\" + Path.GetFileName(path));//Path.ChangeExtension(path, "thumb"));
                result = true;
                return Path.GetFileName(path) + "added successfuly";
                
            } catch (Exception e)
            {
                result = false;
                return e.Message.ToString();
            }

        }

    }

}
