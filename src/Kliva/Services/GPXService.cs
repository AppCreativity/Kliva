using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Windows.Storage;
using Windows.Storage.Streams;
using Kliva.Services.Interfaces;

namespace Kliva.Services
{
    public class GPXService : IGPXService
    {
        private XmlWriter _xmlWriter;
        private readonly XmlWriterSettings _xmlWriterSettings;        

        public GPXService()
        {
            _xmlWriterSettings = new XmlWriterSettings() { Indent = true, Async = true };
        }

        public async Task InitGPXDocument()
        {
            //TODO: Glenn - What folder do we use? LocalFolder or TempFolder?
            //TODO: Glenn - What file name?
            //TODO: Glenn - What with file cleanup? Or retry?
            StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync("Text.gpx");
            using (IRandomAccessStream writeStream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                Stream writerStream = writeStream.AsStreamForWrite();

                _xmlWriter = XmlWriter.Create(writerStream, _xmlWriterSettings);
                _xmlWriter.WriteStartDocument();
                _xmlWriter.WriteStartElement("gpx", "http://www.topografix.com/GPX/1/1");
                _xmlWriter.WriteAttributeString("version", "1.1");
                _xmlWriter.WriteAttributeString("creator", "Kliva"); //TODO: Glenn - Add some version number? Check other GPX files to see what is given?

                //_xmlWriter.Flush();
                await _xmlWriter.FlushAsync();
            }
        }

        public async Task WriteGeoposition()
        {
        }

        public async Task EndGPXDocument()
        {
        }
    }
}