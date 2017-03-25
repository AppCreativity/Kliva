using Windows.ApplicationModel.Resources;

namespace Kliva.Helpers
{
    public static class TranslationHelper
    {
        private static readonly ResourceLoader _resourceLoader = new ResourceLoader();

        /// <summary>
        /// Request the translated value for a given string key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <remarks>
        /// Note that key values are written with a . notation in the resource file
        /// But are only accessible through dash notation /
        /// Example : LoginTitle.Text in resource file can be accessed with LoginTitle/Text
        /// </remarks>
        public static string GetTranslation(string key)
        {
            return _resourceLoader.GetString(key);
        }
    }
}
