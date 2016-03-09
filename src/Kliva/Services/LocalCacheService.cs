using System;
using System.Threading.Tasks;
using Windows.Storage;
using System.IO;

namespace Kliva.Services
{
    internal class LocalCacheService // TODO make a singleton instance
    {
        public static async void PersistCacheData(string json, string name) // TODO Bart: review async void
        {
            StorageFolder folder = ApplicationData.Current.LocalCacheFolder;
            StorageFile file = await folder.CreateFileAsync(name, CreationCollisionOption.ReplaceExisting);
            var stream = await file.OpenAsync(FileAccessMode.ReadWrite);
            StreamWriter streamWriter = new StreamWriter(stream.AsStreamForWrite());
            streamWriter.Write(json);
            streamWriter.Flush();
            streamWriter.Dispose();
        }

        public static async Task<string> ReadCacheData(string name)
        {
            string data = null;
            try
            {
                StorageFolder folder = ApplicationData.Current.LocalCacheFolder;
                StorageFile file = await folder.GetFileAsync(name);
                var stream = await file.OpenAsync(FileAccessMode.ReadWrite);
                StreamReader streamReader = new StreamReader(stream.AsStreamForRead());
                data = await streamReader.ReadToEndAsync();
                streamReader.Dispose();
                stream.Dispose();
            }
            catch (Exception ex) { } // TODO Gotta catch em all
            return data;
        }

        public static async Task<DateTime> GetCacheTimestamp(string name)
        {
            DateTime timestamp = DateTime.MinValue;
            try
            {
                StorageFolder folder = ApplicationData.Current.LocalCacheFolder;
                StorageFile file = await folder.GetFileAsync(name);
                var props = await file.GetBasicPropertiesAsync();
                timestamp = props.DateModified.LocalDateTime;
            }
            catch (Exception ex) { } // TODO Gotta catch em all
            return timestamp;
        }
    }
}