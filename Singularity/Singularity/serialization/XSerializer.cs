using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.IO;
using System.Xml;

namespace Singularity.serialization
{
    /// <summary>
    /// A Xml-Serializer that uses the NetDataContractSerializer for serialization. Besides serializing and deserializing it also has a
    /// testmethod, a method to load a Savegame (in xml) and a method to save the current Gamestate.
    /// </summary>
    class XSerializer
    {
        /// <summary>
        /// Serializes a object that has the  DataContract() attribute and all of its Fields that have been marked by the DataMember() attribute.
        /// </summary>
        /// <param name="toSerialize"> The object to be serialized</param>
        /// <param name="filepath"> The path where the xml-file should be created. Note that the name of the xml-file has to be part of the path.
        /// Example: ..\config\Gamesave_to_be_created.xml </param>
        public static void Serialize(object toSerialize, string filepath)
        {
            var ser = new NetDataContractSerializer();
            var fs = new FileStream(filepath, FileMode.Create);
            var xdwriter = XmlDictionaryWriter.CreateTextWriter(fs);

            //Write xml-header
            xdwriter.WriteStartDocument(true);

            //Write the xml-data of the object to the file
            ser.WriteObject(xdwriter, toSerialize);

            //Write xml-tail
            xdwriter.WriteEndDocument();

            xdwriter.Flush();
            fs.Flush();
            fs.Close();
        }

        /// <summary>
        /// Deserialize the objects of the given xml-file to a List.
        /// </summary>
        /// <param name="filepath">The xml-file I was talking about</param>
        /// <returns>A List containing all the Xml-objects of the file.</returns>
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

        /// <summary>
        /// Save the current Gamestate in a xml file. The destination will be  %USERPROFILE%\Saved Games and the name of the file will
        /// depend on the level and the ingame time.
        /// </summary>
        public static void SaveGamestate()
        {
            throw new NotImplementedException();
            var path = @"%USERPROFILE%\Saved Games";
            path = Environment.ExpandEnvironmentVariables(path);

            //Will be implemented further when the explicit workflow is clear.
            path = path + @"\"; //+ Game.GetLevelName() + "-" + Game.GetIngameTime() + ".xml"
            //Serialize(GameScreen, path);
        }

        /// <summary>
        /// Load the Xml file containing the Gamestate.
        /// </summary>
        /// /// <param name="path">The xml-file I was talking about</param>
        public static void LoadGame(string path)
        {
            throw new NotImplementedException();
            //Will be implemented further when the explicit workflow is clear.
            var deserializedObjects = Deserialize(path);
            //Something.Initialize() 
        }

        /// <summary>
        /// A Testmethod for Serialization. It serializes Dummys with private fields,
        /// shared references, cyclic references static variables and a Vector2 (what was supposed
        /// to not be serializable but that seemed to work, well I dont mind) into a xml-file at %USERPROFILE%\Saved Games, with the filename
        /// Gamesave.xml. After that it deserializes the file again and tests if all referenes etc. are valid (output on Console).
        /// </summary>
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
            if (dummy1.GetList().GetHashCode() == dummy2.GetList().GetHashCode())
            {
                Console.WriteLine("SHARED REFERENCES HAVE BEEN PRESERVED CORRECTLY.");
            }
            else
            {
                Console.WriteLine("Shared references have not been preserved correctly...");
            }

            //Check Cyclic reference
            if (dummyd.GetDummy().GetCyclicReference().GetHashCode() == dummyd.GetHashCode())
            {
                Console.WriteLine("CYCLIC REFERNCE HAS BEEN PRESERVED CORRECTLY.");
            }
            else
            {
                Console.WriteLine("Cyclic reference has not been preserved...");
            }

            //Check allegedly not Serializable Object Vector2
            Console.WriteLine("X and Y Coordinate of Vector2: " + dummyd.mVector.X + " " + dummyd.mVector.Y);
        }
    }
}
