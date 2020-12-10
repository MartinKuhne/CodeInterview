using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Windows.Storage;
using Windows.Storage.Streams;
using System.IO;
using System.Threading.Tasks;

/*
 * This module contains a simplified model which should be easier to serialize
 */

namespace EPPlayer
{
    [DataContract(Name = "EPCharacter")]
    class PersistentModel
    {
        [DataMember()]
        private Dictionary<string, int> KeyNumberPairs = new Dictionary<string, int>();
        [DataMember()]
        private List<KeyValuePair<string, string>> KeyStringPairs = new List<KeyValuePair<string, string>>();

        private PersistentModel()
        {
            KeyNumberPairs.Add("Version", 1);
        }

        public static async Task WriteObject(EPCharacter Character, string FileName)
        {
            DataContractSerializer ser = new DataContractSerializer(typeof(PersistentModel));
            PersistentModel Model = new PersistentModel();

            System.Diagnostics.Debug.WriteLine("Serialization begins");

            StorageFile sf = await ApplicationData.Current.LocalFolder.CreateFileAsync(FileName, CreationCollisionOption.ReplaceExisting);
            Windows.Storage.Streams.IRandomAccessStream WriteStream = await sf.OpenAsync(FileAccessMode.ReadWrite);
            System.IO.Stream Writer = System.IO.WindowsRuntimeStreamExtensions.AsStreamForWrite(WriteStream);

            // detach the morph so the free items and skill increases that might have come with it don't become permanent.
            Morph m = Character.OfType<Morph>().FirstOrDefault();
            if (m != null)
            {
                Character.Remove(m);
                Model.KeyStringPairs.Add(new KeyValuePair<string, string>(m.color, m.name));
            }

            foreach (ValueAttribute Va in Character.OfType<ValueAttribute>().Where(El => El.rawValue > 0))
            {
                Model.KeyNumberPairs.Add(Va.name, Va.rawValue);
            }
            foreach (Background Att in Character.OfType<Background>())
            {
                Model.KeyStringPairs.Add(new KeyValuePair<string, string>(Att.color, Att.name));
            }
            foreach (Faction Att in Character.OfType<Faction>())
            {
                Model.KeyStringPairs.Add(new KeyValuePair<string, string>(Att.color, Att.name));
            }
            foreach (Trait Att in Character.OfType<Trait>())
            {
                Model.KeyStringPairs.Add(new KeyValuePair<string, string>(Att.color, Att.name));
            }
            foreach (Gear Att in Character.OfType<Gear>())
            {
                Model.KeyStringPairs.Add(new KeyValuePair<string, string>(Att.color, Att.name));
            }

            ser.WriteObject(Writer, Model);
            Writer.Dispose();
            WriteStream.Dispose();

            if (m != null)
            {
                Character.DeprecatedAttachAttribute(Morph.AttributeColor, m.name);
            }

            System.Diagnostics.Debug.WriteLine("Serialization successful");
        }

        public static async Task<EPCharacter> ReadObject(string FileName)
        {
            PersistentModel DeserializedModel;
            EPCharacter DeserialziedCharacter = new EPCharacter();

            System.Diagnostics.Debug.WriteLine("Deserialization begins");

            StorageFile sf = await ApplicationData.Current.LocalFolder.CreateFileAsync(FileName, CreationCollisionOption.OpenIfExists);
            var Prop = await sf.GetBasicPropertiesAsync();
            if (Prop.Size == 0)
            {
                System.Diagnostics.Debug.WriteLine("Deserialization: 0 byte file");
                throw new InvalidDataException("0 byte file");
            }
            Windows.Storage.Streams.IRandomAccessStream ReadStream = await sf.OpenReadAsync();
            System.IO.Stream Reader = System.IO.WindowsRuntimeStreamExtensions.AsStreamForRead(ReadStream);

            DataContractSerializer ser = new DataContractSerializer(typeof(PersistentModel));

            DeserializedModel = (PersistentModel) ser.ReadObject(Reader);
            Reader.Dispose();
            ReadStream.Dispose();

            if (DeserializedModel.KeyNumberPairs["Version"] != 1)
            {
                throw new InvalidDataException("Wrong file version");
            }

            foreach (KeyValuePair<string, int> kvp in DeserializedModel.KeyNumberPairs.Where(El => El.Key != "Version"))
            {
                DeserialziedCharacter.SetRawValue(kvp.Key, kvp.Value);
            }
            foreach (KeyValuePair<string, string> kvp in DeserializedModel.KeyStringPairs)
            {
                if (DeserialziedCharacter.Where(Att => Att.name == kvp.Value).Count() > 0)
                {
                    System.Diagnostics.Debug.Assert(false);
                }
                else
                {
                    try
                    {
                        DeserialziedCharacter.DeprecatedAttachAttribute(kvp.Key, kvp.Value);
                    }
                    catch { }
                }
            }

            System.Diagnostics.Debug.WriteLine("Deserialization successful");
            return DeserialziedCharacter;
        }
    }
}