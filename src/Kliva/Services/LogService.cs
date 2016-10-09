using System.Text;
using Windows.ApplicationModel;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.System.Profile;
using Kliva.Services.Interfaces;

namespace Kliva.Services
{
    public class LogService : ILogService
    {
        private readonly StringBuilder _logMessage = new StringBuilder();

        public string SystemFamily { get; }
        public string SystemVersion { get; }
        public string SystemArchitecture { get; }
        public string ApplicationName { get; }
        public string ApplicationVersion { get; }
        public string DeviceManufacturer { get; }
        public string DeviceModel { get; }

        public LogService()
        {
            // get the system family name
            AnalyticsVersionInfo ai = AnalyticsInfo.VersionInfo;
            SystemFamily = ai.DeviceFamily;

            // get the system version number
            string sv = AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
            ulong v = ulong.Parse(sv);
            ulong v1 = (v & 0xFFFF000000000000L) >> 48;
            ulong v2 = (v & 0x0000FFFF00000000L) >> 32;
            ulong v3 = (v & 0x00000000FFFF0000L) >> 16;
            ulong v4 = (v & 0x000000000000FFFFL);
            SystemVersion = $"{v1}.{v2}.{v3}.{v4}";

            // get the package architecure
            Package package = Package.Current;
            SystemArchitecture = package.Id.Architecture.ToString();

            // get the user friendly app name
            ApplicationName = package.DisplayName;

            // get the app version
            PackageVersion pv = package.Id.Version;
            ApplicationVersion = $"{pv.Major}.{pv.Minor}.{pv.Build}.{pv.Revision}";

            // get the device manufacturer and model name
            EasClientDeviceInformation eas = new EasClientDeviceInformation();
            DeviceManufacturer = eas.SystemManufacturer;
            DeviceModel = eas.SystemProductName;
        }

        public string Log(string title, string body)
        {
            _logMessage.Clear();
            _logMessage.AppendLine(title);            
            AppendDeviceInfo();
            _logMessage.AppendLine(body);

            return _logMessage.ToString();
        }

        private void AppendDeviceInfo()
        {
            _logMessage.AppendLine("****");

            _logMessage.AppendLine($"{SystemFamily} - {SystemVersion}");
            _logMessage.AppendLine($"{ApplicationName} - {ApplicationVersion}");
            _logMessage.AppendLine($"{DeviceManufacturer} - {DeviceModel}");

            _logMessage.AppendLine("****");
        }
    }
}
