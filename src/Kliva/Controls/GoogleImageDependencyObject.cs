using System;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using GalaSoft.MvvmLight.Threading;
using Kliva.Models;
using Kliva.Services;
using Microsoft.Practices.ServiceLocation;

namespace Kliva.Controls
{
    /// <summary>
    /// Due to perfomance hits we are no longer using this approach ( here for Dependency property example reference )
    /// </summary>
    public static class GoogleImageDependencyObject
    {
        public static readonly DependencyProperty GoogleImageUrlProperty = DependencyProperty.RegisterAttached(
            "GoogleImageUrl",
            typeof(Map), typeof(GoogleImageDependencyObject),
            new PropertyMetadata("", OnUrlChanged));

        public static Map GetGoogleImageUrl(DependencyObject d)
        {
            return (Map)d.GetValue(GoogleImageUrlProperty);
        }

        public static void SetGoogleImageUrl(DependencyObject d, Map value)
        {
            d.SetValue(GoogleImageUrlProperty, value);
        }

        private static void OnUrlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ImageBrush imageBrush = (ImageBrush)d;

            if (e.NewValue != null && e.NewValue is Map)
            {
                Map googleMap = (Map)e.NewValue;

                if (googleMap.GoogleImage != null)
                {
                    if(imageBrush.ImageSource != googleMap.GoogleImage)
                        imageBrush.ImageSource = googleMap.GoogleImage;
                }
                else
                    DownloadGoogleImage((ImageBrush) d, (Map) e.NewValue);
            }
        }

        private static async void DownloadGoogleImage(ImageBrush image, Map googleMap)
        {
            string fileName = string.Concat(googleMap.Id, ".map");
            FileRandomAccessStream fileStream = await ServiceLocator.Current.GetInstance<IOService>().GetFileAsync(fileName);

            if (fileStream == null)
            {
                if (!string.IsNullOrEmpty(googleMap.GoogleImageApiUrl))
                {
                    HttpClient client = new HttpClient();
                    try
                    {
                        var result = await Task.Run(() => client.GetByteArrayAsync(googleMap.GoogleImageApiUrl));
                        await ServiceLocator.Current.GetInstance<IOService>().SaveFileAsync(fileName, result);
                        var stream = new InMemoryRandomAccessStream();
                        await stream.WriteAsync(result.AsBuffer());
                        stream.Seek(0);

                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            var bitmap = new BitmapImage();
                            bitmap.DecodePixelHeight = 190;
                            bitmap.DecodePixelWidth = 480;
                            bitmap.SetSource(stream);
                            image.ImageSource = googleMap.GoogleImage = bitmap;
                        });
                    }
                    catch (Exception e)
                    {
                        //TODO: Show exception
                    }
                }
            }
            else
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    var bitmap = new BitmapImage();
                    bitmap.DecodePixelHeight = 190;
                    bitmap.DecodePixelWidth = 480;
                    bitmap.SetSource(fileStream);
                    image.ImageSource = googleMap.GoogleImage = bitmap;
                });
            }
        }
    }
}
