using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Windows.Storage;
using Windows.Storage.Streams;
using System.IO;

// This module covers domain specific stuff for EP

namespace EPPlayer
{
    /*
     * Failed experiment: Deserialize the EP xml into classes using DataContextReader
     * Code below to end of file is not in use.
     * Tried converting the dtd into xsd and have xsd.exe create classes =>
     *   The xsd created code cannot be consumed by store app (lacking the xml attribute annotation)
     * Worked with DataContract and DataContractSerializer
     *   Hit weird end-of-file exception or Serializer would just hang.
     * I suspect my data is not in the right format (it was expecting xml-schema-instance)
     * At the end of the day it was less elegant but cheaper just to hand code the classes.
     */

    [DataContract(Name = "CoreRules")]
    public sealed class fCoreRules
    {
        [DataMember]
        public List<fSkill> Skills;
    }

    [CollectionDataContract(ItemName = "Skill")]
    public sealed class fSkillList : List<fSkill>
    {
    }

    [DataContract(Name = "Skill")]
    public sealed class fSkill
    {
        [DataMember]
        public string Name;
        [DataMember]
        public string GoverningAptitude;
        [DataMember]
        public bool AllowsDefaulting;
        [DataMember]
        public string Category;
    }

    class Failed
    {
        Failed()
        {
            /*
            StorageFile sf = await ApplicationData.Current.LocalFolder.CreateFileAsync("ep.xml", CreationCollisionOption.OpenIfExists);
            Windows.Storage.Streams.IRandomAccessStream stream = await sf.OpenReadAsync();
            System.IO.Stream str = System.IO.WindowsRuntimeStreamExtensions.AsStreamForRead(stream);

            List<Type> KnownTypes = new List<Type> { typeof(CoreRules), typeof(Skill) };

            DataContractSerializer s2 = new DataContractSerializer(typeof(Skill));
            DataContractSerializer s3 = new DataContractSerializer(typeof(CoreRules), 
                "CoreRules", 
                @"http://www.eclipsephase.com/namespace/",
                KnownTypes);
            */

            DataContractSerializer s4 = new DataContractSerializer(typeof(fSkillList),
                "Skill",
                @"http://www.eclipsephase.com/namespace/");

            fSkillList SL = new fSkillList();
            fSkill newSkill = new fSkill();
            newSkill.AllowsDefaulting = false;
            newSkill.Category = "Knowledge";
            newSkill.GoverningAptitude = "SOM";
            newSkill.Name = "Hacking";
            SL.Add(newSkill);
            SL.Add(newSkill);

            MemoryStream Buffer = new MemoryStream();
            StringWriter SW = new StringWriter();
            s4.WriteObject(Buffer, SL);
            Buffer.Seek(0, SeekOrigin.Begin);
            StreamReader reader = new StreamReader(Buffer);
            var contents = reader.ReadToEnd();

            // Skill[] whoah = (Skill[]) s4.ReadObject(str);
            // List<Skill> whoah = (List<Skill>)s2.ReadObject(str);
        }
    }
}