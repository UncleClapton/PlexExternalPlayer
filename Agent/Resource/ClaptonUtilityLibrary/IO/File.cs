using System;
using System.IO;

namespace Clapton.IO
{
    public static class File
    {
        /// <summary>
        /// Creates a <see cref="StreamWriter"/> that appends text to the file represented by the given path string.
        /// </summary>
        /// <param name="path">Absolute or relative path of the file.</param>
        /// <param name="text">Text to be written to the file.</param>
        /// <returns>True if write was sucsuessful; False if an error while writing occurs.</returns>
        public static bool TryWriteToFile(string path, string text)
        {
            return TryWriteToFile(path, text, false);
        }
        /// <summary>
        /// Creates a <see cref="StreamWriter"/> that appends text to the file represented by the given path string.
        /// </summary>
        /// <param name="path">Absolute or relative path of the file.</param>
        /// <param name="text">Text to be written to the file.</param>
        /// <param name="append">True to append data to the file; False to overwrite the file.</param>
        /// <returns>True if write was sucsuessful; False if an error while writing occurs.</returns>
        public static bool TryWriteToFile(string path, string text, bool append)
        {
            try
            {
                FileInfo file = new FileInfo(path);

                if (!Directory.Exists(new FileInfo(path).Directory.FullName))
                {
                    Directory.CreateDirectory(file.Directory.FullName);
                }

                using (StreamWriter stream = new StreamWriter(file.FullName, append))
                {
                    stream.Write(text);
                    stream.Dispose();
                    stream.Close();
                }
                
                return true;
            }
            catch (Exception ex)
            {
                Exceptions.ExceptionHandling.ReportException(new object(), ex);
                return false;
            }
        }
    }
}
