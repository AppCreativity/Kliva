using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Kliva.Services
{
    public class IOService
    {
        private readonly StorageFolder _storage = ApplicationData.Current.LocalFolder;

        public async Task<FileRandomAccessStream> GetFile(string fileName)
        {
            FileRandomAccessStream stream = null;

            try
            {
                StorageFile destinationFile = await _storage.GetFileAsync(fileName).AsTask().ConfigureAwait(false);
                stream = (FileRandomAccessStream)await destinationFile.OpenAsync(FileAccessMode.Read);
            }
            catch (FileNotFoundException exception)
            {
                //File does not yet exist
            }

            return stream;
        }

        public async Task SaveFile(string fileName, byte[] content)
        {
            StorageFile destinationFile = await _storage.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteBytesAsync(destinationFile, content);
        }
    }
}
