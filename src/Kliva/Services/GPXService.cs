using System;
using System.Globalization;
using System.IO;
using System.Linq;
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
        private XNamespace _nsTopografix = XNamespace.Get("http://www.topografix.com/GPX/1/1");

        public Task InitGPXDocument()
        {
            _gpxDocument = new XDocument(
                new XDeclaration("1.0", "UTF-8", null),
                    new XElement(_nsTopografix + "gpx",
                        new XAttribute("creator", "Kliva"), //TODO: Glenn - Add some version number? Check other GPX files to see what is given?
                        new XAttribute("version", "1.1")));

            WriteMetaData();
            WriteBeginTrack();
            WriteBeginTrackSegment();

            return Task.CompletedTask;
        }

        public async Task<string> EndGPXDocument()
        {
            //TODO: Glenn - What folder do we use? LocalFolder or TempFolder?
            //TODO: Glenn - What file name?
            //TODO: Glenn - What with file cleanup? Or retry?

            StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync("Text.gpx", CreationCollisionOption.ReplaceExisting);
            using (IRandomAccessStream writeStream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                Stream writerStream = writeStream.AsStreamForWrite();
                _gpxDocument.Save(writerStream);
                await writerStream.FlushAsync();
            }

            return file.Path;
        }

        public async Task WriteGPXLocation(double latitude, double longitude)
        {
            var gpxTrack = _gpxDocument.Root.Elements(_nsTopografix + "trk").FirstOrDefault();
            var gpxTrackSegment = gpxTrack.Elements(_nsTopografix + "trkseg").FirstOrDefault();
            gpxTrackSegment.Add(
                new XElement(_nsTopografix + "trkpt",
                    new XAttribute("lat", latitude.ToString(CultureInfo.InvariantCulture)),
                    new XAttribute("lon", longitude.ToString(CultureInfo.InvariantCulture)),
                        new XElement(_nsTopografix + "time", DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'", CultureInfo.InvariantCulture))));            
        }

        private void WriteMetaData()
        {
            _gpxDocument.Root.Add(
                new XElement(_nsTopografix + "metadata",
                    new XElement(_nsTopografix + "time", DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'", CultureInfo.InvariantCulture))));
        }

        private void WriteBeginTrack()
        {
            _gpxDocument.Root.Add(
                new XElement(_nsTopografix + "trk",
                    new XElement(_nsTopografix + "name", "Kliva recorded track"))); //TODO: Glenn - Come up with a good name! Location reference, hour reference?
        }

        private void WriteBeginTrackSegment()
        {
            var gpxTrack = _gpxDocument.Root.Elements(_nsTopografix + "trk").FirstOrDefault();
            gpxTrack.Add(new XElement(_nsTopografix + "trkseg"));
        }
    }   
}