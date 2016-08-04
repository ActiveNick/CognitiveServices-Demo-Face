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
        private readonly IFaceServiceClient faceServiceClient = new FaceServiceClient("42a0518d83254280813a6be663535131");
        FaceRectangle[] _faceRectangles;

        public MainPage()
        {
            this.InitializeComponent();
            UploadAndDetectFaces("ms-appx:///Assets/StoreLogo.png");
        }

        async void UploadAndDetectFaces(string imageFilePath)
        {
            try
            {
                StorageFolder appInstalledFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                StorageFolder assets = await appInstalledFolder.GetFolderAsync("Assets");
                var storageFile = await assets.GetFileAsync("NickLandry-headshot-sq.jpg");
                var randomAccessStream = await storageFile.OpenReadAsync();

                using (Stream stream = randomAccessStream.AsStreamForRead())
                {
                    //this is the fragment where face is recognized:
                    var faces = await faceServiceClient.DetectAsync(stream);
                    var faceRects = faces.Select(face => face.FaceRectangle);
                    _faceRectangles = faceRects.ToArray();
                    CustomCanvas.Invalidate();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

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
