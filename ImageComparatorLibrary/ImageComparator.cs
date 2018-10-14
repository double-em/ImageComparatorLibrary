using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ImageComparatorLibrary
{
    public class ImageComparator
    {
        Bitmap source;
        public Bitmap imageGrayscaled;
        public Bitmap Source
        {
            get
            {
                return source;
            }
            set
            {
                source = value;
                imageGrayscaled = GrayscaleImage();
            }
        }
        int widthOFImage;
        public int WidthOFImage
        {
            get
            {
                return widthOFImage;
            }
            set
            {
                if (value > 0)
                {
                    widthOFImage = value;
                }
            }
        }
        int heightOFImage;
        public int HeightOFImage
        {
            get
            {
                return heightOFImage;
            }
            set
            {
                if (value > 0)
                {
                    heightOFImage = value;
                }
            }
        }
        public ImageComparator(string path, int width = 16, int height = 16)
        {
            source = new Bitmap(path);
            WidthOFImage = width;
            HeightOFImage = height;
            imageGrayscaled = GrayscaleImage();


        }
        Bitmap GrayscaleImage()
        {
            Bitmap sourceTemp = new Bitmap(source, widthOFImage, heightOFImage);
            for (int i = 0; i < sourceTemp.Width; i++)
            {
                for (int j = 0; j < sourceTemp.Height; j++)
                {
                    Color pixelColor = sourceTemp.GetPixel(j, i);
                    //https://en.wikipedia.org/wiki/Grayscale from thread https://stackoverflow.com/questions/2265910/convert-an-image-to-grayscale
                    int grayScale = (int)((pixelColor.R * 0.299) + (pixelColor.G * 0.587) + (pixelColor.B * 0.114));
                    Color grayColor = Color.FromArgb(pixelColor.A, grayScale, grayScale, grayScale);
                    sourceTemp.SetPixel(j, i, grayColor);
                }
            }
            return sourceTemp;
        }

        public List<double> GetBrightness()
        {
            List<double> results = new List<double>();

            for (int i = 0; i < widthOFImage; i++)
            {
                for (int j = 0; j < heightOFImage; j++)
                {
                    results.Add(imageGrayscaled.GetPixel(j, i).GetBrightness());
                }
            }
            return results;
        }

        public double CompareImages (List<double> imageTOCompare, bool draw = false)
        {
            List<double> sourceBrightness = GetBrightness();
            double difference = 0;

            if (sourceBrightness.Count == imageTOCompare.Count)
            {
                for (int i = 0; i < sourceBrightness.Count; i++)
                {
                    double result = Math.Sqrt(Math.Pow(sourceBrightness[i] - imageTOCompare[i], 2));
                    result *= 100;
                    difference += result;
                }
            }
            else
            {
                throw new Exception("Images wasen't the same size");

            }
            return difference;
        }

        public List<double> CompareImagesList (List<double> imageTOCompare, bool draw = false)
        {
            List<double> sourceBrightness = GetBrightness();
            List<double> differences = new List<double>();

            if (sourceBrightness.Count == imageTOCompare.Count)
            {
                for (int i = 0; i < sourceBrightness.Count; i++)
                {
                    double result = Math.Sqrt(Math.Pow(sourceBrightness[i] - imageTOCompare[i], 2));
                    result *= 100;
                    differences.Add(result);
                }
            }
            else
            {
                throw new Exception("Images wasen't the same size");
            }
            return differences;
        }

        public void DrawDifference (List<double> differences, string path, int offsetX, int offsetY, Graphics graphics)
        {
            int offsetXTemp = offsetX;
            Image imageTemp = Image.FromFile(path);
            Graphics graphicsObj = Graphics.FromImage(imageTemp);
            graphicsObj = graphics;
            Pen pen1 = new Pen(Color.Black, 1);
            SolidBrush brush1 = new SolidBrush(Color.FromArgb(0, 255, 255, 255));
            SolidBrush brushText = new SolidBrush(Color.FromArgb(0, 0, 0));
            Font font1 = new Font("Arial", 10);

            int rows = 0;
            for (int i = 0; i < differences.Count; i++)
            {
                Rectangle rectangle1 = new Rectangle(offsetXTemp + (i * 20), offsetY, 20, 20);
                brush1 = new SolidBrush(Color.FromArgb((int)Math.Round(differences[i], 0), 255, 0, 0));
                graphicsObj.DrawString(Math.Round(differences[i], 0).ToString(), font1, brushText, rectangle1);
                graphicsObj.FillRectangle(brush1, rectangle1);
                if ((i + 1) / Math.Sqrt(differences.Count) == rows + 1)
                {
                    offsetY += 20;
                    offsetXTemp -= ((int)Math.Sqrt(differences.Count) * 20);
                    rows++;
                }
            }
            graphicsObj.Dispose();
        }
    }
}
