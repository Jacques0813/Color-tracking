using System;
using System.Drawing;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Imaging.Filters;
using AForge.Imaging;

namespace Camera_New
{
    public partial class Form1 : Form
    {
        static FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
        // create video source
        VideoCaptureDevice videoSource;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //
        }

        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap original = new Bitmap(eventArgs.Frame);
            Bitmap filtered = new Bitmap(eventArgs.Frame);
            // create a filter
            EuclideanColorFiltering filter = new EuclideanColorFiltering();

            // set color ranges to keep
            filter.CenterColor = new RGB(200, 0, 0); // red color
            filter.Radius = 100;

            // apply the filter
            filter.ApplyInPlace(filtered);

            // locate blobs
            BlobCounter bc = new BlobCounter();
            bc.MinHeight= 100;
            bc.MinWidth= 100;
            bc.ProcessImage(filtered);

            Blob[] blobs = bc.GetObjectsInformation();
            int MaxIndex = 0;

            for (int i = 0; i < blobs.Length; i++)
            {
                if (blobs[i].Area > blobs[MaxIndex].Area)
                {
                    MaxIndex = i;
                }
            }

            if (MaxIndex < blobs.Length)
            {
                Rectangle rect = blobs[MaxIndex].Rectangle;
                using (Graphics g = Graphics.FromImage(original))
                {
                    g.DrawRectangle(new Pen(Color.Yellow), rect);
                }
            }

            // display the filtered image
            pictureBox1.Image = (Bitmap)original.Clone();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (videoDevices.Count > 0)
            {
                videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
                // set NewFrame event handler
                videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);

                // start the video source
                videoSource.Start();
            }
        }
    }
}
