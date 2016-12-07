using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace CognitiveServices_Demo_Face
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // TO DO: Get your own Cognitive Services Face API key, don't use mine as I might have regenerated them.
        // Get your own at https://www.microsoft.com/cognitive-services/en-us/face-api.
        private readonly IFaceServiceClient faceServiceClient = new FaceServiceClient("42a0518d83254280813a6be663535131");
        FaceRectangle[] _faceRectangles;

        public MainPage()
        {
            this.InitializeComponent();
            UploadAndDetectFaces("NickLandry-headshot-sq.jpg");
        }

        async void UploadAndDetectFaces(string imageFilePath)
        {
            try
            {
                StorageFolder appInstalledFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                StorageFolder assets = await appInstalledFolder.GetFolderAsync("Assets");
                var storageFile = await assets.GetFileAsync(imageFilePath);
                var randomAccessStream = await storageFile.OpenReadAsync();

                using (Stream stream = randomAccessStream.AsStreamForRead())
                {
                    // This is the fragment where the face is recognized:
                    var faces = await faceServiceClient.DetectAsync(stream);
                    var faceRects = faces.Select(face => face.FaceRectangle);
                    _faceRectangles = faceRects.ToArray();
                    // Forces a redraw on the canvas control
                    CustomCanvas.Invalidate();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Draw a rectange around the face detected on the picture 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void canvasControl_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            if (_faceRectangles != null)
                if (_faceRectangles.Length > 0)
                {
                    foreach (var faceRect in _faceRectangles)
                    {
                        args.DrawingSession.DrawRectangle(faceRect.Left, faceRect.Top, faceRect.Width, faceRect.Height, Colors.Blue);
                    }
                }
        }

    }
}
