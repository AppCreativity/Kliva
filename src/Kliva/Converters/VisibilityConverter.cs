// ****************************************************************************
// <copyright company="AppCreativity"
//            file="VisibilityConverter.cs">
// Copyright © AppCreativity 2014
// </copyright>
// ****************************************************************************
// <project>AppCreativity.UniversalTools</project>
// <url>https://github.com/AppCreativity/UniversalTools</url>
// <license>
// See LICENSE in this solution or at 
// https://github.com/AppCreativity/UniversalTools/blob/master/LICENSE
// </license>
// ****************************************************************************

using System;
using System.Collections;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Kliva.Converters
{
    /// <summary>
    /// Converts the default value of any object to Collapsed and returns Visible for all other values.
    /// 
    /// For the use of properties: see unit tests for available combinations.
    /// </summary>
    public class VisibilityConverter : IValueConverter
    {
        public VisibilityConverter()
        {
            SupportIsNullOrEmpty = true;
            SupportCollectionEmpty = true;
        }

        /// <summary>
        /// Invert the Visibility result value
        /// </summary>
        public bool Inverse { get; set; }

        /// <summary>
        /// Check for IsNullOrEmpty on string input values.
        /// Returns Collapsed on an empty string
        /// 
        /// Default: true
        /// </summary>
        public bool SupportIsNullOrEmpty { get; set; }

        /// <summary>
        /// Check for an empty collection on input values.
        /// Returns Collapsed if no elements are found.
        /// 
        /// Default: true
        /// </summary>
        public bool SupportCollectionEmpty { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool visible;

            if (value is string && SupportIsNullOrEmpty)
            {
                visible = !string.IsNullOrEmpty(value.ToString());
            }
            else if (value is ICollection && SupportCollectionEmpty)
            {
                visible = ((ICollection)value).Count > 0;
            }
            else
            {
                var defaultValue = value != null ? GetDefaultValue(value.GetType()) : null;
                visible = !Equals(value, defaultValue);
            }

            if (Inverse)
                visible = !visible;

            return visible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }

        private static object GetDefaultValue(Type type)
        {
            return type.GetTypeInfo().IsValueType ? Activator.CreateInstance(type) : null;
        }
    }
}