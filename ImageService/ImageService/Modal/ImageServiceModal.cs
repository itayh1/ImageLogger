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


        /*
         * Construct ImageServiceMidal by destenation path and size of thumbnail
         */
        public ImageServiceModal(string target, int thumbnail)
        {
            this.outputFolder = target;
            this.thumbSize = thumbnail;
        }


        /*
         * The functions adding file by getting its src and its dst from constructor's class.
         * It opens relevant directories derives from the path and puts the file in the relevant 
         * year and date directories relying on the file's creation date.
         * It also creates a thumbnails picture in different directory.
         */
        public string AddFile(string src, out bool result)
        {
            string year, month, dstPath, message = String.Empty;

            try
            {
                if (!File.Exists(src))
                {
                    throw new Exception("File missing!");
                }
                DirectoryInfo mainFolder = Directory.CreateDirectory(this.outputFolder);//new DirectoryInfo(this.outputFolder);
                if ((mainFolder.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                {
                    mainFolder.Attributes |= FileAttributes.Hidden;
                }
                

                DateTime dateTime = File.GetCreationTime(src);
                year = dateTime.Year.ToString();
                month = dateTime.Month.ToString();
                dstPath = Path.Combine(this.outputFolder, year, month);
                // make output directory hidden
                DirectoryInfo di = Directory.CreateDirectory(dstPath);
               // di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
                
                if (!Directory.Exists(this.outputFolder + @"\Thumbnails"))
                {
                    Directory.CreateDirectory(this.outputFolder + @"\Thumbnails");
                }
                Directory.CreateDirectory(this.outputFolder + "\\" + year + "\\" + month + "\\");
                Directory.CreateDirectory(Path.Combine(this.outputFolder + @"\Thumbnails", year, month));
                
                //copy source file as thumbnail
                message += AddToTumbnailFolder(src, this.outputFolder, Path.GetFileName(src), dateTime);
                //move source file to target folder
                message += AddToFolder(src, dstPath, Path.GetFileName(src));

                result = true;
                return message;
                
            } catch (Exception e)
            {
                result = false;
                return e.Message.ToString();
            }

        }

        /*
         * The function adds a file to specific diretory relying on src and dst pathes
         */ 
        private string AddToFolder(string source, string dstPath, string fileName)
        {
            string message = "";
            if (!File.Exists(Path.Combine(dstPath, fileName)))
            {
                File.Move(source, Path.Combine(dstPath, fileName));
                message += String.Format("\n{0} added successfuly to {1}", fileName, dstPath);
            }
            else
            {
                message += AddToFolder(source, dstPath, "cpy_" + fileName);
            }
            return message;
        }


        /*
        * The function adds a thumbnail file to specific diretory relying on src and dst pathes
        */
        private string AddToTumbnailFolder(string source, string dstPath, string fileName, DateTime dt)
        {
            string message = "";
            string output = dstPath + "\\Thumbnails";
            output = Path.Combine(output, dt.Year.ToString(), dt.Month.ToString(), fileName);
            if (!File.Exists(output))
            {
                using (Image image = Image.FromFile(source))
                {
                    Image thumb = image.GetThumbnailImage(this.thumbSize, this.thumbSize, () => false, IntPtr.Zero);

                    thumb.Save(output);
                    Path.ChangeExtension(output, ".thumbnail");
                }
                message += String.Format("{0} added successfuly to {1}", fileName, output);
            }
            else
            {
                message += AddToTumbnailFolder(source, dstPath, "cpy_" + fileName, dt);
            }
            return message;
        }
    }

}
