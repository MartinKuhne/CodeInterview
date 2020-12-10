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
    class EPData
    {
    }
    class Skill
    {
        public readonly string Name;
        public readonly string GoverningAptitude;
        public readonly bool AllowsDefaulting;
        public readonly string Category;
        public Skill(string Name, string Apt, bool Default, string Category)
        {
            this.Name = Name;
            this.GoverningAptitude = Apt;
            this.AllowsDefaulting = Default;
            this.Category = Category;
        }
    }
    class Aptitude
    {
        public readonly string Name;
        public readonly string ShortName;
        public Aptitude(string Name, string ShortName)
        {
            this.Name = Name;
            this.ShortName = ShortName;
        }
    }
    /*
      <morph>
      <name>Fury</name>
      <type>Biomorph</type>
      <description>Furies are combat morphs. These transgenic human upgrades feature genetics tailored for endurance, strength, and reflexes, as well as behavioralmodifications for aggressiveness and cunning. To offset tendencies for unruliness and macho behavior patterns, furies feature gene sequences promoting pack mentalities and cooperation, and they tend to be biologically female.</description>
      <implants>
        <text>Basic Biomods, Basic Mesh Inserts, Bioweave Armor (Light), Cortical Stack, Enhanced, Vision, Neurachem (Level 1), Toxin Filters</text>
        <implant>Basic Biomods</implant>
        <implant>Basic Mesh Inserts</implant>
        <implant>Bioweave Armor (Light)</implant>
        <implant>Cortical Stack</implant>
        <implant>Enhanced Vision</implant>
        <implant>Neurachem (Level 1)</implant>
        <implant>Toxin Filters</implant>
      </implants>
      <aptitudeMax>
        <text>30</text>
        <COG>30</COG>
        <COO>30</COO>
        <INT>30</INT>
        <REF>30</REF>
        <SAV>30</SAV>
        <SOM>30</SOM>
        <WIL>30</WIL>
      </aptitudeMax>
      <durability>50</durability>
      <woundThreshold>10</woundThreshold>
      <advantages>
        <text>+5 COO, +5 REF, +10 SOM, +5 WIL, +5 to one aptitude of the player&apos;s choice</text>
        <aptmod>
          <name>COO</name>
          <amount>5</amount>
        </aptmod>
        <aptmod>
          <name>REF</name>
          <amount>5</amount>
        </aptmod>
        <aptmod>
          <name>SOM</name>
          <amount>10</amount>
        </aptmod>
        <aptmod>
          <name>WIL</name>
          <amount>5</amount>
        </aptmod>
        <aptmod>
          <name>Choice</name>
          <choice>Any</choice>
          <amount>5</amount>
        </aptmod>
      </advantages>
      <disadvantages>
        <text>None</text>
      </disadvantages>
      <CPCost>75</CPCost>
      <creditCost>Expensive</creditCost>
      <creditmin>40000</creditmin>
    </morph>
    */

    class Morph : Attribute, IAttachableAttribute
    {
        public readonly string Type;
        public readonly string Advantages, Disadvantages;
        public List<string> Implants = new List<string>();
        public readonly int Durability, WoundThreshold;
        public readonly IEnumerable<XElement> AptitudeMax;
        public readonly IEnumerable<XElement> AptitudeModifiers;

        const string MyColor = "Morph";

        public Morph(string Name, string Type, string Advantages, string Disadvantages,
            int Durability, int WoundThreshold,
            IEnumerable<XElement> AptitudeMax, IEnumerable<XElement> AptitudeModifiers,
            string Description)
            : base(Name, MyColor, Description)
        {
            this.Type = Type;
            this.Advantages = Advantages;
            this.Disadvantages = Disadvantages;
            this.Durability = Durability;
            this.WoundThreshold = WoundThreshold;
            this.AptitudeMax = AptitudeMax;
            this.AptitudeModifiers = AptitudeModifiers;
        }
        public string AAName
        {
            get { return this.Name; }
        }
        public string AAColor
        {
            get { return this.Color; }
        }
        public void OnAttach(Entity Entity)
        {
            Entity.VFilters.Add(new AttributeFilter("Wound Threshold", MyColor, 
                Entity.VAttributes["Wound Threshold"], value => value + this.WoundThreshold));
            Entity.VFilters.Add(new AttributeFilter("Durability", MyColor,
                Entity.VAttributes["Durability"], value => value + this.Durability));

            foreach (XElement xe in AptitudeModifiers.Where(el => el.Name == "aptmod"))
            {
                string name = xe.Element("name").Value;
                string amount = xe.Element("amount").Value;
                if ((!string.IsNullOrEmpty(name)) &&
                    (!string.IsNullOrEmpty(amount)))
                {
                    int Modifier = Convert.ToInt32(amount);
                    if (name == "Choice")
                    {
                        Entity.VFilters.Add(new AttributeFilter("CP Compensation", MyColor,
                            Entity.VAttributes["CP"], value => value + (10 * Modifier), this.Name));
                    }
                    else
                    {
                        Entity.VFilters.Add(new AttributeFilter(
                            name, MyColor, Entity.VAttributes[ResourceManager.AptitudeLongNames[name]], 
                            value => value + Modifier));
                    }
                }
            }
            // Length == 3 to just read the elements named for an aptitude shorthand
            foreach (XElement xe in AptitudeMax.Elements().Where(el => ResourceManager.AptitudeLongNames.ContainsKey(el.Name.ToString())))
            {
                System.Diagnostics.Debug.WriteLine(xe);
            }

            foreach (XElement xe in AptitudeMax.Elements().Where(el => ResourceManager.AptitudeLongNames.ContainsKey(el.Name.ToString())))
            {
                string name = (string) xe.Name.ToString();
                int Max = Convert.ToInt32(xe.Value);
                Entity.VFilters.Add(new AttributeFilter(
                    name, MyColor, Entity.VAttributes[ResourceManager.AptitudeLongNames[name]], value => Math.Max(value, Max)));
            }
        }
        public void OnDetach(Entity Entity)
        {
            Entity.VFilters.RemoveAll(att => att.Color == MyColor);
        }
    }

    class Background : Attribute, IAttachableAttribute
    {
        public readonly string Advantages, Disadvantages;
        public readonly string CommonMorphs;
        // keeping unprocessed XML is a bit of a layer violation; 
        // didn't want to parse it into an intermediate
        // class but don't hav access to filters from this class
        public readonly IEnumerable<XElement> SkillModAsXml;
        public Background(string Name, string Advantages, string Disadvantages,
            string CommonMorphs, IEnumerable<XElement> SkillModAsXml, string Description = null)
            : base(Name, "Background", Description)
        {
            this.Advantages = Advantages;
            this.Disadvantages = Disadvantages;
            this.CommonMorphs = CommonMorphs;
            this.SkillModAsXml = SkillModAsXml;
        }

        public string AAName
        {
            get { return this.Name; }
        }
        public string AAColor
        {
            get { return this.Color; }
        }
        public void OnAttach(Entity Entity)
        {
          /* <name>Networking</name>
           * <field>Hypercorps</field>
           * <amount>20</amount> */

            foreach (XElement xe in SkillModAsXml.Where(el=>el.Name == "skillmod"))
            {
                string name = xe.Element("name").Value;
                if (xe.Element("amount") != null)
                {
                    string amount = xe.Element("amount").Value;
                    if (xe.Element("field") != null)
                    {
                        name = string.Format("{0}: {1}", name, xe.Element("field").Value);
                    }
                    int Modifier = Convert.ToInt32(amount);
                    if (name == "Choice")
                    {
                        // sometimes the user gets to choose which skills to apply a bonus to.
                        // for now, we'll give extra CP to spend
                        int Quantity = 1;
                        if (xe.Element("quantity") != null)
                        {
                            Quantity = Convert.ToInt32(xe.Element("quantity").Value);
                        }
                        Entity.VFilters.Add(new AttributeFilter("CP Compensation", "Background",
                            Entity.VAttributes["CP"], value => value + (Quantity * Modifier), this.Name));
                    }
                    else
                    {
                        Entity.VFilters.Add(new AttributeFilter(
                            name, "Background", Entity.VAttributes[name], value => value + Modifier));
                    }
                }
                else
                {
                    // todo: skillmod with no amount
                }
            }
        }
        public void OnDetach(Entity Entity)
        {
            Entity.VFilters.RemoveAll(att => att.Color == "Background");
        }
    }

    class ResourceManager
    {
        private static ResourceManager Self = null;
        private static object Lock = new object();

        public static Dictionary<string, string> AptitudeLongNames = new Dictionary<string, string>
        {
            {"COG", "Cognition"},
            {"COO", "Coordination"},
            {"INT", "Intuition"},
            {"REF", "Reflexes"},
            {"SAV", "Savvy"},
            {"SOM", "Somatics"},
            {"WIL", "Willpower"}
        };

        // note while these collections are r/w they are not meant to be written to
        // and it is not thread safe to do so. Only place writing should be our constructor

        public List<Skill> Skills = new List<Skill>();
        public List<string> Reputations = new List<string>();
        public List<Aptitude> Aptitudes = new List<Aptitude>();
        public List<string> Stats = new List<string>();
        public List<Background> Backgrounds = new List<Background>();
        public List<Morph> Morphs = new List<Morph>();

        public static Dictionary<string, string> ShortNames = new Dictionary<string, string>();
        
        private ResourceManager()
        {
            XElement CoreRules = null;
            XElement AdditionalRules = null;

            CoreRules = XElement.Load("CoreRules-jsenek.xml");
            AdditionalRules = XElement.Load("Aptitudes.xml");

            // read skills taking into consideration some skills have Fields
            // this implementation makes each field it's own skill
            // inconsiderate side effect: only the first category gets read (needs to be Knowledge or Active)
            foreach (XElement xe in CoreRules.Descendants("skills").Descendants("skill"))
            {
                IEnumerable<XElement> Fields = xe.Descendants("field");
                if (Enumerable.Count(Fields) > 0)
                {
                    foreach (XElement xeField in Fields)
                    {
                        string SkillName = string.Format("{0}: {1}",
                            xe.Element("name").Value, xeField.Value);
                        Skills.Add(new Skill(
                            SkillName,
                            xe.Element("apt").Value,
                            xe.Element("defaulting").Value == "Yes" ? true : false,
                            xe.Element("category").Value));
                    }
                }
                else
                {
                    Skills.Add(new Skill(
                        xe.Element("name").Value,
                        xe.Element("apt").Value,
                        xe.Element("defaulting").Value == "Yes" ? true : false,
                        xe.Element("category").Value));
                }
            }

            /*  <Reputations>
                <Rep Name="@-rep"></Rep>
             */
            foreach (XElement xe in AdditionalRules.Descendants("Rep"))
            {
                Reputations.Add(xe.Attribute("Name").Value);
            }
 
            /* <Aptitudes>
                  <Aptitude Name="Cognition">
                    <Shorthand>COG</Shorthand>
                    <Description></Description>
                  </Aptitude>
             */
            foreach (XElement xe in AdditionalRules.Descendants("Aptitudes").Descendants("Aptitude"))
            {
                Aptitudes.Add(new Aptitude(
                    xe.Attribute("Name").Value,
                    xe.Element("Shorthand").Value));
                ShortNames.Add(xe.Element("Shorthand").Value, xe.Attribute("Name").Value);
            }

            // Stats
            foreach (XElement xe in AdditionalRules.Descendants("Stats").Descendants("Stat"))
            {
                Stats.Add(xe.Attribute("Name").Value);
                if (! string.IsNullOrEmpty(xe.Element("Shorthand").Value))
                {
                    ShortNames.Add(xe.Element("Shorthand").Value, xe.Attribute("Name").Value);
                }
            }

            // Backgrounds
            foreach (XElement xe in CoreRules.Descendants("backgrounds").Descendants("background"))
            {
                string Debug1 = xe.Element("name").Value;
                string Debug2 = xe.Element("description").Value;
                string Debug3 = xe.Element("advantages").Element("text").Value;
                string Debug4 = xe.Element("disadvantages").Element("text").Value;
                string Debug5 = xe.Element("commonmorphs").Element("text").Value;
                IEnumerable<XElement> Rest = xe.Descendants("skillmod");

                Backgrounds.Add(new Background(
                    xe.Element("name").Value,
                    xe.Element("advantages").Element("text").Value,
                    xe.Element("disadvantages").Element("text").Value,
                    xe.Element("commonmorphs").Element("text").Value,
                    xe.Descendants("skillmod"),
                    xe.Element("description").Value));
            }

            // Morphs
            foreach (XElement xe in CoreRules.Descendants("morphs").Descendants("morph"))
            {
                string Debug1 = xe.Element("name").Value;
                string Debug2 = xe.Element("description").Value;
                string Debug3 = xe.Element("advantages").Element("text").Value;
                string Debug4 = xe.Element("disadvantages").Element("text").Value;
                string Debug5 = xe.Element("durability").Value;
                string Debug6 = xe.Element("woundThreshold").Value;
                string Debug7 = xe.Element("aptitudeMax").Element("COO").Value;

                IEnumerable<XElement> Rest = xe.Descendants("aptmod");

                Morphs.Add(new Morph(
                    xe.Element("name").Value,
                    xe.Element("type").Value,
                    xe.Element("advantages").Element("text").Value,
                    xe.Element("disadvantages").Element("text").Value,
                    Convert.ToInt32(xe.Element("durability").Value),
                    Convert.ToInt32(xe.Element("woundThreshold").Value),
                    xe.Descendants("aptitudeMax"),
                    xe.Descendants("aptmod"),
                    xe.Element("description").Value));
                // missing implants
            }
        }

        public static ResourceManager GetInstance()
        {
            lock (Lock)
            {
                if (Self == null)
                {
                    Self = new ResourceManager();
                }
                return Self;
            }
        }
    }
    
    class EPCharacter : Entity
    {
        public readonly ResourceManager Resources = ResourceManager.GetInstance();

        public void AttachAttribute(string Type, string Name)
        {
            IAttachableAttribute NewAttribute = null;
            switch (Type)
            {
                case "Background":
                  NewAttribute = Resources.Backgrounds.Find(bg => bg.Name == Name);
                    break;
                case "Morph":
                    NewAttribute = Resources.Morphs.Find(bg => bg.Name == Name);
                break;
            }
            if (NewAttribute != null)
            {
                NewAttribute.OnAttach(this);
                this.AAttributes.Add(NewAttribute);
            }
            else
            {
                throw new ArgumentException("Attribute not found");
            }
        }

        public void DetachAttribute(string Type, string Name)
        {
            IAttachableAttribute ToRemove = AAttributes.Find(att => att.AAName == Name && att.AAColor == Type);
            if (ToRemove != null)
            {
                ToRemove.OnDetach(this);
                this.AAttributes.Remove(ToRemove);
            }
            else
            {
                throw new ArgumentException("Attribute not found");
            }
        }

        // This constructor creates a brand new character
        
        public EPCharacter()
        {
            foreach (Skill s in Resources.Skills)
            {
                ValueAttribute NewAttribute = new ValueAttribute(s.Name, "Skill", 0);
                this.VAttributes.Add(s.Name, NewAttribute);
                this.VFilters.Add(new AttributeFilter(s.Name, "Skill", 
                    NewAttribute, att => att + this[ResourceManager.ShortNames[s.GoverningAptitude]]));
            }
            foreach (string Rep in Resources.Reputations)
            {
                this.VAttributes.Add(Rep, new ValueAttribute(Rep, "Reputation", 0));
            }
            foreach (Aptitude Apt in Resources.Aptitudes)
            {
                this.VAttributes.Add(Apt.Name, new ValueAttribute(Apt.Name, "Aptitude", 15));
            }
            foreach (string Stat in Resources.Stats)
            {
               this.VAttributes.Add(Stat, new ValueAttribute(Stat, "Stat", 0));
            }

            this.VFilters.Add(new AttributeFilter("Initiative", "CoreRules", 
                VAttributes["Initiative"], a => a + (this["Intuition"] + this["Reflexes"] * 2) / 5));
            this.VFilters.Add(new AttributeFilter("Wound Threshold", "CoreRules",
                VAttributes["Wound Threshold"], a => a + this["Durability"] / 5));
            this.VFilters.Add(new AttributeFilter("Death Rating", "CoreRules", 
                VAttributes["Death Rating"], a => a + Convert.ToInt16(this["Durability"] * 1.5)));
            this.VFilters.Add(new AttributeFilter("Lucidity", "CoreRules", 
                VAttributes["Lucidity"], a => a + this["Willpower"] * 2));
            this.VFilters.Add(new AttributeFilter("Trauma Threshold", "CoreRules", 
                VAttributes["Trauma Threshold"], a => a + this["Lucidity"] / 5));
            this.VFilters.Add(new AttributeFilter("Insanity Rating", "CoreRules",
                VAttributes["Insanity Rating"], a => a + this["Lucidity"] * 2));
            this.VFilters.Add(new AttributeFilter("Damage Bonus", "CoreRules", 
                VAttributes["Damage Bonus"], a => a + this["Somatics"] / 10));

            this.SetRawValue("CP", 1000);
        }
    }

    /* Character creation and point costs:
        Define Character Concept (p. 130)
        Choose Background (p. 131)
        Choose Faction (p. 132)
        Spend Free Points (p. 134)
            105 aptitude points
            1 Moxie
            5,000 credit
            50 Rep
            Native tongue
        Spend Customization Points (p. 135)
            1,000 CP to spend
            15 CP = 1 Moxie
            10 CP = 1 aptitude point
            5 CP = 1 psi sleight
            5 CP = 1 specialization
            2 CP = 1 skill point (61-80)
            1 CP = 1 skill point (up to 60)
            1 CP = 1,000 credit
            1 CP = 10 rep
        Active skill minimum: 400 skill points
        Knowledge skill minimum: 300 skill points
        Choose Starting Morph (pp. 136 and 139)
        Choose Traits (pp. 136 and 145)
        Purchase Gear (p. 136)
        Choose Motivation (p. 137)
        Calculate Remaining Stats (p. 138)
        Detail the Character (p. 138)
    */

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