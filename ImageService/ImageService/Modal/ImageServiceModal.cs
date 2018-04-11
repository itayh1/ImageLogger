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
        public string AddFile(string src, out bool result)
        {
            string year, month, dstPath, t_src, message = String.Empty;

            try
            {
                if (!File.Exists(src))
                {
                    throw new Exception("File missing!");
                }
                DateTime dateTime = File.GetCreationTime(src);
                year = dateTime.Year.ToString();
                month = dateTime.Month.ToString();
                dstPath = Path.Combine(this.outputFolder, year, month);
                // make output directory hidden
                DirectoryInfo di = Directory.CreateDirectory(dstPath);
                di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
                
                if (!Directory.Exists(this.outputFolder + @"\Thumbnails"))
                {
                    Directory.CreateDirectory(this.outputFolder + @"\Thumbnails");
                }
                Directory.CreateDirectory(this.outputFolder + "\\" + year + "\\" + month + "\\");
                Directory.CreateDirectory(Path.Combine(this.outputFolder + @"\Thumbnails", year, month));

                message += AddToFolder(src, dstPath, Path.GetFileName(src));
                message += AddToTumbnailFolder(src, this.outputFolder, Path.GetFileName(src), dateTime);

                result = true;
                return message;
                
            } catch (Exception e)
            {
                result = false;
                return e.Message.ToString();
            }

        }

        private string AddToFolder(string source, string dstPath, string fileName)
        {
            string message = "";
            if (!File.Exists(Path.Combine(dstPath, fileName)))
            {
                File.Copy(source, Path.Combine(dstPath, fileName));
                message += String.Format("{0} added successfuly to {1}", fileName, dstPath);
            }
            else
            {
                message += AddToFolder(source, dstPath, "cpy_" + fileName);
            }
            return message;
        }

        private string AddToTumbnailFolder(string source, string dstPath, string fileName, DateTime dt)
        {
            string message = "";
            string output = dstPath + "\\Thumbnails";
            output = Path.Combine(output, dt.Year.ToString(), dt.Month.ToString(), fileName);
            if (!File.Exists(output))
            {
                Image image = Image.FromFile(source);
                Image thumb = image.GetThumbnailImage(this.thumbSize, this.thumbSize, () => false, IntPtr.Zero);
               
                thumb.Save(output);
                Path.ChangeExtension(output, ".thumbnail");
                message += String.Format("\n{0} added successfuly to {1}", fileName, output);
            }
            else
            {
                message += AddToTumbnailFolder(source, dstPath, "cpy_" + fileName, dt);
            }
            return message;
        }
    }

}
