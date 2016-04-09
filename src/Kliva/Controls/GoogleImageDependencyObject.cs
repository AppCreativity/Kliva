using System;
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
            if (e.NewValue != null && e.NewValue is Map)
                DownloadGoogleImage((ImageBrush)d, (Map)e.NewValue);
        }

        private static async void DownloadGoogleImage(ImageBrush image, Map googleMap)
        {
            string fileName = string.Concat(googleMap.Id, ".map");
            FileRandomAccessStream fileStream = await ServiceLocator.Current.GetInstance<IOService>().GetFile(fileName);

            if (fileStream == null)
            {
                if (!string.IsNullOrEmpty(googleMap.GoogleImageApiUrl))
                {
                    HttpClient client = new HttpClient();
                    try
                    {
                        var result = await Task.Run(() => client.GetByteArrayAsync(googleMap.GoogleImageApiUrl));
                        await ServiceLocator.Current.GetInstance<IOService>().SaveFile(fileName, result);
                        var stream = new InMemoryRandomAccessStream();
                        await stream.WriteAsync(result.AsBuffer());
                        stream.Seek(0);

                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            var bitmap = new BitmapImage();
                            bitmap.DecodePixelHeight = 190;
                            bitmap.DecodePixelWidth = 480;
                            bitmap.SetSource(stream);
                            image.ImageSource = bitmap;
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
                    image.ImageSource = bitmap;
                });
            }
        }
    }
}
