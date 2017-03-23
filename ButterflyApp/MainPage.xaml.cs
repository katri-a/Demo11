using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ButterflyApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        // Butterfly
        private Butterfly butterfly;

        //Flowers
        private List<Flower> flowers;

        // game loop timer
        private DispatcherTimer timer;

        //which keys are pressed
        private bool UpPressed;
        private bool LeftPressed;
        private bool RightPressed;
        // audio
        private MediaElement mediaElement;
        

        public MainPage()
        {
            this.InitializeComponent();

            // add one butterfly
            butterfly = new Butterfly
            {
                LocationX = MyCanvas.Width / 2,
                LocationY = MyCanvas.Height / 2
            };
            MyCanvas.Children.Add(butterfly);

            // initialise list of flowers
            flowers = new List<Flower>();

            
            
            // key listener
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
            Window.Current.CoreWindow.KeyUp += CoreWindow_KeyUp;

            // mouse listener
            Window.Current.CoreWindow.PointerPressed += CoreWindow_PointerPressed;


            // game loop timer
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1000 / 60);
            timer.Tick += Timer_Tick;
            timer.Start(); //"start game"

            
        }

        private async void InitAudio()
        {
        
        StorageFolder folder =
            await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets");
        StorageFile file = await folder.GetFileAsync("ding.wav");
        var stream = await file.OpenAsync(FileAccessMode.Read);
        mediaElement = new MediaElement();
        mediaElement.AutoPlay = false;
        mediaElement.SetSource(stream, file.ContentType);

        }

        private void CoreWindow_PointerPressed(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.PointerEventArgs args)
        {
            //create new flower
            Flower flower = new Flower();
            //set locationwith mouse position
            flower.LocationX = args.CurrentPoint.Position.X - flower.Width / 2;
            flower.LocationY = args.CurrentPoint.Position.Y - flower.Height / 2;
            // add to game canvas
            MyCanvas.Children.Add(flower);
            //set flower loccation in canvas
            flower.SetLocation();
            // add to flowers list (for collision checking)
            flowers.Add(flower);
            //does these intersect

            
        }

        /// <summary>
        /// Game Loop
        /// </summary>
        private void Timer_Tick(object sender, object e)
        {
            //move butterfly
            if (UpPressed) butterfly.Move();

            //rotate butterfly
            if (LeftPressed) butterfly.Rotate(-1);
            if (RightPressed) butterfly.Rotate(1);


            //update location
            butterfly.SetLocation();

            //collision to flower
            CheckCollision();
        }

        private void CheckCollision()
        {
            //check all flowers
            foreach(Flower flower in flowers)
            {
                Rect BRect = new Rect(butterfly.LocationX, butterfly.LocationY, butterfly.ActualWidth, butterfly.ActualHeight);
                Rect FRect = new Rect(flower.LocationX, flower.LocationY, flower.ActualWidth, flower.ActualHeight);

                BRect.Intersect(FRect);
                // is rect empty?
                if (!BRect.IsEmpty)
                {
                    MyCanvas.Children.Remove(flower);
                    flowers.Remove(flower);
                    mediaElement.Play();
                    break;
                }
            }
        }

        private void CoreWindow_KeyUp(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            switch (args.VirtualKey)
            {
                case VirtualKey.Up:
                    UpPressed = false;
                    break;
                case VirtualKey.Left:
                    LeftPressed = false;
                    break;
                case VirtualKey.Right:
                    RightPressed = false;
                    break;
            }
        }

        private void CoreWindow_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            switch (args.VirtualKey)
            {
                case VirtualKey.Up:
                    UpPressed = true;
                    break;
                case VirtualKey.Left:
                    LeftPressed = true;
                    break;
                case VirtualKey.Right:
                    RightPressed = true;
                    break;
            }
        }
    }
}
