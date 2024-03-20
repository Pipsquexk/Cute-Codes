using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Cute_Codes
{
    internal class Program
    {

        private static FileInfo fileToConv;

        [STAThread]
        static void Main(string[] args)
        {
            Logger.Instance.Log(ConsoleColor.Green, "Started Cute Codes!");
            if (args.Length < 1)
            {
                FileInput();
            }
            else
            {
                fileToConv = new(args[0]);
                Logger.Instance.Log(ConsoleColor.Yellow, $"Are you sure you want to convert the file at '{fileToConv.FullName}'?\n\ny/n:");
                string qResp = Console.ReadLine();
                Logger.Instance.LogToFile(qResp);

                if (!qResp.ToLower().Contains("y"))
                {
                    FileInput();
                    return;
                }
            }

            if (fileToConv.Extension.ToLower() == ".png" || fileToConv.Extension.ToLower() == ".jpg" || fileToConv.Extension.ToLower() == ".bmp")
            {
                ProcessImage();
                return;
            }

            Logger.Instance.Log($"File Size: {fileToConv.Length}");

            byte[] byteArr = File.ReadAllBytes(fileToConv.FullName);

            double lenSqrt = Math.Sqrt(byteArr.Length);
            Logger.Instance.Log(ConsoleColor.Green, $"Square root of Byte Array ({byteArr.Length}): {lenSqrt}");
            int dimen = (int)Math.Ceiling(lenSqrt);
            Logger.Instance.Log(ConsoleColor.Green, $"Rounded up square root of Byte Array ({lenSqrt}): {dimen}");

            Bitmap bMP = new(dimen, dimen, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

            Logger.Instance.Log(ConsoleColor.Blue, $"Converting {fileToConv.Name} to an image...");

            for (int y = 0; y < dimen; ++y)
            {
                for (int x = 0; x < dimen; ++x)
                {
                    int index = (byteArr.Length - 1) - ((byteArr.Length - 1) - (x + y * dimen));
                    if (index > byteArr.Length - 1)
                    {
                        bMP.SetPixel(x, y, Color.FromArgb(255, 255, 255, 255));
                    }
                    else
                    {
                        Color col = Color.FromArgb(255, byteArr[index], 0, 0);
                        //Logger.Instance.Log(col.R.ToString());
                        bMP.SetPixel(x, y, col);
                    }
                }
            }

            //for (int i = 0; i < byteArr.Length; ++i)
            //{
            //    Logger.Instance.Log(bMP.GetPixel(i, 0).ToString());
            //}

            bMP.Save($"{fileToConv.Name}.png", ImageFormat.Png);

            Logger.Instance.Log(ConsoleColor.Green, $"Saved {fileToConv.Name}.png!");

            Console.ReadKey(false);
        }

        /// <summary>
        /// Get user's input for the file path
        /// </summary>
        private static void FileInput()
        {
            Logger.Instance.Log("File Path:");
            string filePath = "[EMPTY]";
            OpenFileDialog openFileDialog = new()
            {
                Filter = "Image File (*.png;*.jpg;*.bmp)|*.png;*.jpg;*.bmp|All Files (*.*)|*.*",
                FilterIndex = 2,
                InitialDirectory = Environment.CurrentDirectory,
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK) filePath = openFileDialog.FileName;

            filePath = string.IsNullOrWhiteSpace(filePath) ? "[EMPTY]" : filePath;

            Logger.Instance.LogToFile(filePath);

            fileToConv = new(filePath);

            if (!fileToConv.Exists)
            {
                Logger.Instance.Log(ConsoleColor.DarkRed, $"Failed to find file at '{filePath}'. Make sure to include the entire file path.");
                FileInput();
                return;
            }

            Logger.Instance.Log(ConsoleColor.Yellow, $"Are you sure you want to convert the file at '{filePath}'?\n\ny/n:");
            string qResp = Console.ReadLine();
            Logger.Instance.LogToFile(qResp);

            if (!qResp.ToLower().Contains("y"))
            {
                FileInput();
                return;
            }

        }

        private static void ProcessImage()
        {
            Logger.Instance.Log(ConsoleColor.Blue, $"Converting {fileToConv.Name} to a file...");

            byte[] data = new byte[] { };

            Bitmap bMP = new(fileToConv.FullName);
            for (int y = 0; y < bMP.Height; ++y)
            {
                for (int x = 0; x < bMP.Width; ++x)
                {
                    Color pixelData = bMP.GetPixel(x, y);
                    if (pixelData != Color.FromArgb(255, 255, 255, 255))
                    {
                        byte newByteData = pixelData.R;
                        List<byte> byteList = data.ToList();
                        byteList.Add(newByteData);
                        data = byteList.ToArray();
                        //Logger.Instance.Log(newByteData.ToString());
                    }
                }
            }

            bMP.Dispose();

            File.WriteAllBytes("EntityWorld.exe", data);

            Logger.Instance.Log(ConsoleColor.Green, $"Done!");

        }

    }
}
