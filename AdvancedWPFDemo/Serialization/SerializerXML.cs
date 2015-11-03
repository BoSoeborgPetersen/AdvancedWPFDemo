using _02350AdvancedDemo.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace _02350AdvancedDemo.Serialization
{
    public class SerializerXML
    {
        public static SerializerXML Instance { get; } = new SerializerXML();

        private SerializerXML() { }

        public async void AsyncSerializeToFile(Diagram diagram, string path)
        {
            await Task.Run(() => SerializeToFile(diagram, path));
        }

        private void SerializeToFile(Diagram diagram, string path)
        {
            using(FileStream stream = File.Create(path))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Diagram));
                serializer.Serialize(stream, diagram);
            }
        }

        public Task<Diagram> AsyncDeserializeFromFile(string path)
        {
            return Task.Run(() => DeserializeFromFile(path));
        }

        private Diagram DeserializeFromFile(string path)
        {
            using (FileStream stream = File.OpenRead(path))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Diagram));
                Diagram diagram = serializer.Deserialize(stream) as Diagram;

                return diagram;
            }
        }

        public Task<string> AsyncSerializeToString(Diagram diagram)
        {
            return Task.Run(() => SerializeToString(diagram));
        }

        private string SerializeToString(Diagram diagram)
        {
            var stringBuilder = new StringBuilder();

            using (TextWriter stream = new StringWriter(stringBuilder))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Diagram));
                serializer.Serialize(stream, diagram);
            }

            return stringBuilder.ToString();
        }

        public Task<Diagram> AsyncDeserializeFromString(string xml)
        {
            return Task.Run(() => DeserializeFromString(xml));
        }

        private Diagram DeserializeFromString(string xml)
        {
            using (TextReader stream = new StringReader(xml))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Diagram));
                Diagram diagram = serializer.Deserialize(stream) as Diagram;

                return diagram;
            }
        }
    }
}
