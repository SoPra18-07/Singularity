using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.IO;
using System.Xml;

namespace Singularity.serialization
{
    class XSerializer
    {
        public static void Serialize(object toSerialize, string filepath)
        {
            var ser = new NetDataContractSerializer();
            var fs = new FileStream(filepath, FileMode.Create);
            var xdwriter = XmlDictionaryWriter.CreateTextWriter(fs);

            //Write xml-header
            xdwriter.WriteStartDocument(true);

            ser.WriteObject(xdwriter, toSerialize);

            xdwriter.WriteEndDocument();

            xdwriter.Flush();
            fs.Flush();
            fs.Close();
        }

        public static List<object> Deserialize(string filepath)
        {
            var ser = new NetDataContractSerializer();
            var fs = new FileStream(filepath, FileMode.Open);
            var reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
            var deserializedObjects = new List<object>();

            while (reader.Read())
            {
                if (ser.IsStartObject(reader))
                {
                    var o = ser.ReadObject(reader);
                    var otype = o.GetType();
                    deserializedObjects.Add(Convert.ChangeType(o, otype));
                }
                break;
            }
            fs.Flush();
            fs.Close();
            return deserializedObjects;
        }

        public static void TestSerialization()
        {
            //Initialize Dummys
            var list = new List<SerializationDummy>();
            var commonList = new List<SerializationDummy>();
            commonList.Add(new SerializationDummy(100, new List<SerializationDummy>()));
            list.Add(new SerializationDummy(1, commonList));
            list.Add(new SerializationDummy(2, commonList));
            var dummy = new SerializationDummy(20, list);

            Console.WriteLine("Before Serialization: ");
            Console.WriteLine("=====================");
            dummy.PrintFields();
            dummy.Increment();

            //Serialize
            var path = @"%USERPROFILE%\Saved Games";
            path = Environment.ExpandEnvironmentVariables(path);
            path = path + @"\Gamesave.xml";
            Serialize(dummy, path);

            //Deserialize
            var deserialized = Deserialize(path);
            var dummyd = (SerializationDummy) deserialized[0];
            Console.WriteLine("After Deserialization: ");
            Console.WriteLine("======================");

            //Check fields
            dummyd.PrintFields();

            //Check shared references
            var deserializedList = dummyd.GetList();
            var dummy1 = (SerializationDummy) deserializedList[0];
            var dummy2 = (SerializationDummy) deserializedList[1];
            dummyd.Increment();
            if (ReferenceEquals(dummy1.GetList(), dummy2.GetList()))
            {
                Console.WriteLine("SHARED REFERENCES HAVE BEEN PRESERVED CORRECTLY.");
            }
            else
            {
                Console.WriteLine("Shared references have not been preserved correctly...");
            }

            //Check Cyclic reference
            if (ReferenceEquals(dummyd.GetDummy().GetCyclicReference(), dummyd))
            {
                Console.WriteLine("CYCLIC REFERNCE HAS BEEN PRESERVED CORRECTLY.");
            }
            else
            {
                Console.WriteLine("Cyclic reference has not been preserved...");
            }
        }
    }
}
