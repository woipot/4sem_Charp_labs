using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PictureCW_01
{
    class Program
    {
        static void Main(string[] args)
        {
           
            Console.WriteLine("Enter directory");
            var patch = Console.ReadLine();

            try
            {
                var imagesWuthPatch = GetImagesWithPath(patch);

                var counter = 0;
                var sb = new StringBuilder();
                foreach (var image in imagesWuthPatch)
                {
                    sb.Append(++counter+ ") " + image.Value + "\n");
                }
                Console.WriteLine($"Found {counter} files:\n" + sb);

                Console.Write("Enter number of image to open:");
                var numberInString = Console.ReadLine();

                try
                {
                    int.TryParse(numberInString, out var number);
                    var images = imagesWuthPatch.Keys.ToList();
                    var needImage = images[number - 1];
                    OpenImage(imagesWuthPatch[needImage]);
                }
                catch (Exception)
                {
                    Console.WriteLine("Error number... OOOkk ... Go to next step");
                }

                Console.WriteLine("Enter comment for images (copyrigths)");
                var copyrigths = Console.ReadLine();

                foreach (var image in imagesWuthPatch.Keys)
                {
                    image.Tag = copyrigths;
                }

                Console.Write("Patch to save: ");
                var savePatch = Console.ReadLine();
                SaveImages(imagesWuthPatch.Keys, savePatch);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            

            Console.WriteLine("Enter eny key to exit");
            Console.ReadKey();
        }

        private static Dictionary<Image, string> GetImagesWithPath(string patch)
        {
            var filesInDirectory = Directory.GetFiles(patch);
            var imageList = new Dictionary<Image, string>();

            foreach (var file in filesInDirectory)
            {
                try
                {
                    var newImage = Image.FromFile(file);
                    imageList[newImage] = file;

                }
                catch (Exception)
                {
                    // ignored
                }
            }

            return imageList;
        }

        private static void OpenImage(string path)
        {
            var processStartInfo = new ProcessStartInfo(path)
            {
                Arguments = Path.GetFileName(path),
                UseShellExecute = true,
                WorkingDirectory = Path.GetDirectoryName(path) ?? throw new Exception("#Error: Path is lost"),
                Verb = "OPEN"

            };
            Process.Start(processStartInfo);
        }

        private static void SaveImages(IEnumerable<Image> images, string patch)
        {
            var counter = 0;
            foreach (var image in images)
            {
                var newTask = new Task
                    (() => SaveImage(image, patch, counter++.ToString()));
                newTask.Start();
            }
        }

        private static void SaveImage(Image image, string patch , string name)
        {
            try
            { 
                image.Save(patch + "\\" + name);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
