using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace Singularity.Serialization
{
    /// <summary>
    /// A Xml-Serializer that uses the NetDataContractSerializer for serialization. Besides serializing and deserializing it also has a
    /// testmethod, a method to load a Savegame (in xml) and a method to save the current Gamestate.
    /// </summary>
    internal static class XSerializer
    {
        /// <summary>
        /// Serializes a object that has the  DataContract() attribute and all of its Fields that have been marked by the DataMember() attribute.
        /// </summary>
        /// <param name="toSerialize"> The object to be serialized</param>
        /// <param name="filepath"> The path where the xml-file should be created. Note that the name of the xml-file has to be part of the path.
        /// Example: ..\config\Gamesave_to_be_created.xml </param>
        private static void Serialize(object toSerialize, string filepath)
        {
            var ser = new NetDataContractSerializer();
            var fs = new FileStream(path: filepath, mode: FileMode.Create);
            var xdwriter = XmlDictionaryWriter.CreateTextWriter(stream: fs);

            //Write xml-header
            xdwriter.WriteStartDocument(standalone: true);

            //Write the xml-data of the object to the file
            ser.WriteObject(writer: xdwriter, graph: toSerialize);

            //Write xml-tail
            xdwriter.WriteEndDocument();

            xdwriter.Flush();
            fs.Flush();
            fs.Close();
        }

        /// <summary>
        /// Deserialize the object of the given xml-file.
        /// </summary>
        /// <param name="filepath">The xml-file I was talking about</param>
        /// <returns>A List containing all objects of the given file. Currently we should only have files with one object, though.</returns>
        private static List<object> Deserialize(string filepath)
        {
            var ser = new NetDataContractSerializer();
            var fs = new FileStream(path: filepath, mode: FileMode.Open);
            var reader = XmlDictionaryReader.CreateTextReader(stream: fs, quotas: new XmlDictionaryReaderQuotas());
            var deserializedObject = new List<object>();

            while (reader.Read())
            {
                if (ser.IsStartObject(reader: reader))
                {
                    var o = ser.ReadObject(reader: reader);
                    deserializedObject.Add(item: o);
                }
                break;
            }
            fs.Flush();
            fs.Close();

            return deserializedObject;
        }

        /// <summary>
        /// Save the given object in a xml file with the given name. Already existing files with that name will be overridden.
        /// The destination will be  %USERPROFILE%\Saved Games\Singularity (the directory will be created if it doesnt exist).
        /// </summary>
        /// <param name="toSave">The object that should be saved</param>
        /// /// <param name="name">The name of the file. Don't forget to add .xml at the end!!!</param>
        public static void Save(object toSave, string name)
        {
            var path = @"%USERPROFILE%\Saved Games\Singularity";
            path = Environment.ExpandEnvironmentVariables(name: path);
            if (!Directory.Exists(path: path))
            {
                Directory.CreateDirectory(path: path);
            }
            path = path + @"\" + name;
            Serialize(toSerialize: toSave, filepath: path);
        }

        /// <summary>
        /// Load the Xml-file containing whatever data. Remember that in our Saves there should only be one object, contact me if you want to change that.
        /// It may be, that a typecast is needed since this method returns an object.
        /// </summary>
        /// <param name="path">The Xml-file I was talking about</param>
        /// <returns>The Object that has been loaded</returns>
        public static object Load(string path)
        {
            var loadedObjects = Deserialize(filepath: path);
            if (loadedObjects.Count == 0)
            {
                throw new IOException(message: "There are no deserialized Objects. Most likely your .xml file is empty.");
            }
            //One may implement additional logic here later
            return loadedObjects[index: 0];
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
            commonList.Add(item: new SerializationDummy(randomvalue: 100, list: new List<SerializationDummy>()));
            list.Add(item: new SerializationDummy(randomvalue: 1, list: commonList));
            list.Add(item: new SerializationDummy(randomvalue: 2, list: commonList));
            var dummy = new SerializationDummy(randomvalue: 20, list: list);

            Console.WriteLine(value: "Before Serialization: ");
            Console.WriteLine(value: "=====================");
            dummy.PrintFields();
            dummy.Increment();

            //Serialize
            var path = @"%USERPROFILE%\Saved Games";
            path = Environment.ExpandEnvironmentVariables(name: path);
            path = path + @"\Gamesave.xml";
            Serialize(toSerialize: dummy, filepath: path);

            //Deserialize
            var deserialized = Deserialize(filepath: path);
            var dummyd = (SerializationDummy) deserialized[index: 0];
            Console.WriteLine(value: "After Deserialization: ");
            Console.WriteLine(value: "======================");

            //Check fields
            dummyd.PrintFields();

            //Check shared references
            var deserializedList = dummyd.GetList();
            var dummy1 = deserializedList[index: 0];
            var dummy2 = deserializedList[index: 1];
            dummyd.Increment();
            if (ReferenceEquals(objA: dummy1.GetList(), objB: dummy2.GetList()))
            {
                Console.WriteLine(value: "SHARED REFERENCES HAVE BEEN PRESERVED CORRECTLY.");
            }
            else
            {
                Console.WriteLine(value: "Shared references have not been preserved correctly...");
            }

            //Check Cyclic reference
            if (ReferenceEquals(objA: dummyd.GetDummy().GetCyclicReference(), objB: dummyd))
            {
                Console.WriteLine(value: "CYCLIC REFERNCE HAS BEEN PRESERVED CORRECTLY.");
            }
            else
            {
                Console.WriteLine(value: "Cyclic reference has not been preserved...");
            }

            //Check allegedly not Serializable Object Vector2
            Console.WriteLine(value: "X and Y Coordinate of Vector2: " + dummyd.mVector.X + " " + dummyd.mVector.Y);
        }
    }
}
