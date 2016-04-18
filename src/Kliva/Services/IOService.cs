using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;
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

        public async Task<IReadOnlyList<StorageFile>> GetFiles(List<string> fileTypes)
        {
            QueryOptions queryOptions = new QueryOptions(CommonFileQuery.DefaultQuery, fileTypes);
            StorageFileQueryResult queryResult = _storage.CreateFileQueryWithOptions(queryOptions);
            IReadOnlyList<StorageFile> files = await queryResult.GetFilesAsync();

            return files;
        }

        public async Task SaveFile(string fileName, byte[] content)
        {
            StorageFile destinationFile = await _storage.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteBytesAsync(destinationFile, content);
        }
    }
}
