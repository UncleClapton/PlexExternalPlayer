using System.IO;
using System.Xml.Serialization;

namespace Clapton.Xml
{
    public static class IO
    {
		public static T ReadXml<T>(this string filePath) where T : new()
		{
			if (filePath == null || !File.Exists(filePath))
			{
				throw new FileNotFoundException("File not found.", filePath);
			}

			FileStream stream = null;
			T result = new T();
            var serializer = new XmlSerializer(typeof(T));

            try
			{
				stream = new FileStream(filePath, FileMode.Open);
                result = (T)serializer.Deserialize(stream);
			}
			finally
			{
				if (stream != null)
				{
					stream.Close();
					stream.Dispose();
				}
			}

			return result;
		}

        public static void WriteXml<T>(this T saveData, string savePath) where T : new()
        {
            var dir = Path.GetDirectoryName(Path.GetFullPath(savePath)) ?? "";
            Directory.CreateDirectory(dir);

            FileStream stream = null;
            var serializer = new XmlSerializer(typeof(T));

            try
            {
                stream = new FileStream(savePath, FileMode.Create);
                serializer.Serialize(stream, saveData);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                    stream.Dispose();
                }
            }
        }
    }
}
