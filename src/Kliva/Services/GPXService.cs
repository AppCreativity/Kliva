using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Windows.Storage;
using Windows.Storage.Streams;
using Kliva.Services.Interfaces;

namespace Kliva.Services
{
    public class GPXService : IGPXService
    {
        private XDocument _gpxDocument;        

        public Task InitGPXDocument()
        {
            var namespaceTopografix = XNamespace.Get("http://www.topografix.com/GPX/1/1");

            _gpxDocument = new XDocument(
                new XDeclaration("1.0", "UTF-8", null),
                    new XElement(namespaceTopografix + "gpx",
                        new XAttribute("creator", "kliva"),
                        new XAttribute("version", "1.1")));

            return Task.CompletedTask;
        }

        public async Task EndGPXDocument()
        {
            StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync("Text.gpx", CreationCollisionOption.ReplaceExisting);
            using (IRandomAccessStream writeStream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                Stream writerStream = writeStream.AsStreamForWrite();
                _gpxDocument.Save(writerStream);
                await writerStream.FlushAsync();
            }
        }
    }

    public class GPXService2 : IGPXService
    {
        private XmlWriter _xmlWriter;
        private readonly XmlWriterSettings _xmlWriterSettings;        

        public GPXService2()
        {
            _xmlWriterSettings = new XmlWriterSettings() { Indent = true, Async = true };
        }

        public async Task InitGPXDocument()
        {
            //TODO: Glenn - What folder do we use? LocalFolder or TempFolder?
            //TODO: Glenn - What file name?
            //TODO: Glenn - What with file cleanup? Or retry?
            StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync("Text.gpx", CreationCollisionOption.ReplaceExisting);
            using (IRandomAccessStream writeStream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                Stream writerStream = writeStream.AsStreamForWrite();

                _xmlWriter = XmlWriter.Create(writerStream, _xmlWriterSettings);
                _xmlWriter.WriteStartDocument();
                _xmlWriter.WriteStartElement("gpx", "http://www.topografix.com/GPX/1/1");
                _xmlWriter.WriteAttributeString("version", "1.1");
                _xmlWriter.WriteAttributeString("creator", "Kliva"); //TODO: Glenn - Add some version number? Check other GPX files to see what is given?

                WriteMetaData();
                WriteBeginTrack();
                WriteBeginTrackSegment();

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

        private void WriteMetaData()
        {
            _xmlWriter.WriteStartElement("metadata");
            _xmlWriter.WriteElementString("time", DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'", CultureInfo.InvariantCulture));
            _xmlWriter.WriteEndElement();
        }

        private void WriteBeginTrack()
        {
            _xmlWriter.WriteStartElement("trk");
            //TODO: Glenn - Come up with a good name! Location reference, hour reference?
            _xmlWriter.WriteElementString("name", "Kliva recorded track");
        }

        private void WriteBeginTrackSegment()
        {
            _xmlWriter.WriteStartElement("trkseg"); 
        }

        private void WriteEndTrackSegment()
        {
            _xmlWriter.WriteEndElement();
        }

        private void WriteEndTrack()
        {
            _xmlWriter.WriteEndElement();
        }
    }
}