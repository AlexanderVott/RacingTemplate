using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace RedDev.Kernel.DB
{
	public class BaseDBXML
	{
		public static T Deserialize<T>(string content) where T: new()
		{
			var serializer = new XmlSerializer(typeof(T));
            using var reader = new StringReader(content);
            return (T) serializer.Deserialize(reader);
        }

		public static string Serialize<T>(T target)
		{
			var serializer = new XmlSerializer(typeof(T));
			var builder = new StringBuilder();
            using var writer = new StringWriter(builder);
            serializer.Serialize(writer, target);
            return builder.ToString();
		}

	}
}