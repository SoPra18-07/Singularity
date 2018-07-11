using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using Singularity.Utils;

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
        /// Deserialize the object of the given xml-file.
        /// </summary>
        /// <param name="filepath">The xml-file I was talking about</param>
        /// <returns>A List containing all objects of the given file. Currently we should only have files with one object, though.</returns>
        private static List<object> Deserialize(string filepath)
        {
            var ser = new NetDataContractSerializer();
            var fs = new FileStream(filepath, FileMode.Open);
            var reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
            var deserializedObject = new List<object>();

            while (reader.Read())
            {
                if (ser.IsStartObject(reader))
                {
                    var o = ser.ReadObject(reader);
                    deserializedObject.Add(o);
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
        /// <param name="name">The name of the file. Don't forget to add .xml at the end!!!</param>
        /// /// <param name="isAchievement">True if you want to Save an Achievement class, false if its a GameSave</param>
        public static void Save(object toSave, string name, bool isAchievement)
        {
            string path;
            if (isAchievement)
            {
                path = @"%USERPROFILE%\Saved Games\Singularity\Achievements";
            }
            else
            {
                path = @"%USERPROFILE%\Saved Games\Singularity\Saves";
            }

            path = Environment.ExpandEnvironmentVariables(path);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            path = path + @"\" + name;
            Serialize(toSave, path);
        }

        /// <summary>
        /// Load the Xml-file containing whatever data. Remember that in our Saves there should only be one object, contact me if you want to change that.
        /// It may be, that a typecast is needed since this method returns an object.
        /// </summary>
        /// <param name="name">The Xml-file-name I was talking about</param>
        /// <param name="isAchievement">True if it is an Achievement you want to load, false if it is a GameSave</param>
        /// <returns>An optional containing the object that has been loaded, or null if the specified xml file does not exist.</returns>
        public static Optional<object> Load(string name, bool isAchievement)
        {
            string path;
            if (isAchievement)
            {
                path = @"%USERPROFILE%\Saved Games\Singularity\Achievements";
            }
            else
            {
                path = @"%USERPROFILE%\Saved Games\Singularity\Saves";
            }

            path = Environment.ExpandEnvironmentVariables(path);

            if (!Directory.Exists(path))
            {
                return Optional<object>.Of(null);
            }

            path += @"\" + name;
            if (!File.Exists(path))
            {
                return Optional<object>.Of(null);
            }

            var loadedObjects = Deserialize(path);
            if (loadedObjects.Count == 0)
            {
                throw new IOException("There are no deserialized Objects. Most likely your .xml file is empty.");
            }
            //One may implement additional logic here later
            return Optional<object>.Of(loadedObjects[0]);
        }

        /// <summary>
        /// Returns a list of the names of all GameSaves.
        /// </summary>
        /// <returns>An array containing the names of all Gamesaves.</returns>
        public static string[] GetSaveNames()
        {
            var path = @"%USERPROFILE%\Saved Games\Singularity\Saves";
            path = Environment.ExpandEnvironmentVariables(path);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                return new string[0];
            }
            else
            {
                var names = Directory.GetFiles(path);
                var index = 0;

                //Convert the paths of the files to their names
                foreach (var filepath in names)
                {
                    var name = filepath.Substring(filepath.LastIndexOf('\\') + 1);
                    names[index] = name;
                    index++;
                }

                return names;
            }
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
            var dummy1 = deserializedList[0];
            var dummy2 = deserializedList[1];
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

            //Check allegedly not Serializable Object Vector2
            Console.WriteLine("X and Y Coordinate of Vector2: " + dummyd.mVector.X + " " + dummyd.mVector.Y);
        }
    }
}
