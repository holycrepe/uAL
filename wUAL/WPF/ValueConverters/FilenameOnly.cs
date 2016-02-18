using System;
using System.Globalization;
using System.IO;


namespace wUAL
{
    public class FilenameOnlyConverter : BaseConverter
    {
        public override object Convert(object value, Type targetType,
                                       object parameter, CultureInfo culture)
        {
            if (value != null) {
                var filename = value.ToString();
                return Path.GetFileNameWithoutExtension(filename);
            }
            return null;
            //            var i = filename.LastIndexOfAny(new [] { '\\', '/' });
            //            if (i > -1)
            //            {
            //                filename = filename.Substring(i + 1);
            //            }
            //            i = filename.LastIndexOf(".");
            //            if (i > -1)
            //            {
            //                filename = filename.Substring(0, i);
            //            }
            //            return filename;
        }
    }
}
