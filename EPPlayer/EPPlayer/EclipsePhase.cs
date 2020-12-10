using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Specialized;

/* This module covers domain specific stuff for Eclipse Phase
 * 
 * There are two main components, one is the ResourceManager, dealing with reading the
 * XML definition file into game element classes (Skills, Aptitudes, Morphs, Traits etc.)
 * The other is the EPCharacter, a superset of the Entity class defined in the model.
 */

namespace EPPlayer
{
    /// <summary>
    /// Keep Skill, Stat and Aptitude modifiers in unparsed XML to avoid
    /// translating them into temporary classes; Modifiers get applied in
    /// ResourceManager.AttachModifiers()
    /// </summary>
    class Modifiers
    {
        public readonly IEnumerable<XElement> Aptitude;
        public readonly IEnumerable<XElement> AptitudeMax;
        public readonly IEnumerable<XElement> Skill;
        public readonly IEnumerable<XElement> Stat;
        private Modifiers(
            IEnumerable<XElement> Aptitude,
            IEnumerable<XElement> AptitudeMax,
            IEnumerable<XElement> Skill,
            IEnumerable<XElement> Stat)
        {
            this.Aptitude = Aptitude;
            this.AptitudeMax = AptitudeMax;
            this.Skill = Skill;
            this.Stat = Stat;
        }
        internal static Modifiers Parse(XElement Xml)
        {
            IEnumerable<XElement> Xml1 = Xml.Descendants("aptmod");
            IEnumerable<XElement> Xml2 = Xml.Descendants("aptitudeMax");
            IEnumerable<XElement> Xml3 = Xml.Descendants("skillmod");
            IEnumerable<XElement> Xml4 = Xml.Descendants("statmod");
            return new Modifiers(Xml1, Xml2, Xml3, Xml4);
        }
    }

    /// <summary>
    /// GoverningAptitude is used to create AttributeFilters
    /// </summary>
    class Skill : ValueAttribute, IAttachableAttribute 
    {
        protected readonly string GoverningAptitude;
        protected readonly bool AllowsDefaulting;
        /// <summary>
        /// Active or Knowledge
        /// </summary>
        protected readonly string Category;
        protected readonly string Group;
        public const string AttributeColor = "Skill";
        private bool IsField;

        internal Skill(string Name, string Apt, bool Default, string Category, string Group, string Description = null)
            :base (Name, Skill.AttributeColor, 0, Description)
        {
            this.GoverningAptitude = Apt;
            this.AllowsDefaulting = Default;
            this.Category = Category;
            this.Group = Group;
            this.IsField = Name.Contains(":");
        }
        public string category
        {
            get { return this.Category; }
        }
        public string governingAptitude
        {
            get { return this.GoverningAptitude; }
        }
        public bool allowsDefaulting
        {
            get { return this.AllowsDefaulting; }
        }
        public string group
        {
            get { return this.Group; }
        }
        public bool isFieldSkill
        {
            get { return this.IsField; }
        }

        public new void OnAttach(Entity Entity)
        {
            base.OnAttach(Entity);

            AttributeFilter Af = new AttributeFilter(this.Name, this.Color, this, this,
                att => att + (Entity[this.governingAptitude] as ValueAttribute).cookedValue);
            Af.Owner = this;
            Entity.Add(Af);
            (Entity[this.governingAptitude] as ValueAttribute).AddDependent(Af);
        }
    }

    class Aptitude : ValueAttribute
    {
        public readonly string ShortName;
        public const string AttributeColor = "Aptitude";

        internal Aptitude(string Name, string ShortName, string Description = null)
            :base (Name, Aptitude.AttributeColor, 0, Description)
        {
            this.ShortName = ShortName;
            this.rawValue = 15;
        }
        public string shortName
        {
            get { return this.ShortName; }
        }
    }

    /// <summary>
    /// Reputation - This doesn't do a lot other than being enumerated
    /// </summary>
    class Reputation : ValueAttribute
    {
        public const string AttributeColor = "Reputation";

        internal Reputation(string Name, string Description = null)
            :base (Name, Reputation.AttributeColor, 0, Description)
        {
        }
    }

    /// <summary>
    /// Stat - This doesn't do a lot other than being enumerated
    /// </summary>
    class Stat : ValueAttribute
    {
        public const string AttributeColor = "Stat";

        internal Stat(string Name, string Description = null)
            : base(Name, Reputation.AttributeColor, 0, Description)
        {
        }
    }

    /// <summary>
    /// The AttachableAttribute carries Modifiers to be applied during Attach
    /// and to be removed during Detach. A default implementation of OnAttach
    /// is provided.
    /// </summary>
    abstract class AttachableAttribute : Attribute, IAttachableAttribute
    {
        private readonly Modifiers Modifiers;
        internal AttachableAttribute(
            string Name,
            string Color,
            Modifiers Modifiers,
            string Description = null)
            : base(Name, Color, Description)
        {
            this.Modifiers = Modifiers;
        }
        public virtual void OnAttach(Entity Entity)
        {
            ResourceManager.AttachModifiers(Entity, this, this.Modifiers);
        }
        public virtual void OnDetach(Entity Entity)
        {
        }
    }

    /// <summary>
    /// in EP, the Morph defines the Durability stat which is implemented here
    /// Morphs also come with free gear (included in the morph cost),
    /// so we add that as well.
    /// </summary>
    class Morph : AttachableAttribute
    {
        public readonly string Type;
        public readonly string Advantages, Disadvantages;
        public List<string> Implants = new List<string>();
        public readonly int Durability;
        public readonly int CPCost;
        public const string AttributeColor = "Morph";

        internal Morph(
            string Name, 
            string Type,
            string Advantages,
            string Disadvantages,
            int Durability,
            int CPCost,
            Modifiers Modifiers,
            string Description)
            : base(Name, AttributeColor, Modifiers, Description)
        {
            this.Type = Type;
            this.Advantages = Advantages;
            this.Disadvantages = Disadvantages;
            this.Durability = Durability;
            this.CPCost = CPCost;
        }

        public override void OnAttach(Entity Entity)
        {
            base.OnAttach(Entity);

            float DrMultiplier = this.type == "Biomorph" ? 1.5F : 2F;
            Entity.SetRawValue("Durability", this.Durability);
            Entity.SetRawValue("Wound Threshold", this.Durability / 5);
            Entity.SetRawValue("Death Rating", Convert.ToInt32(this.Durability * DrMultiplier));

            foreach (string ImplantName in Implants)
            {
                Gear Implant = ((EPCharacter)Entity).Resources.Gear.Find(El => El.name == ImplantName);
                Implant.Owner = this;
                Entity.Add(Implant);
            }
        }

        public override void OnDetach(Entity Entity)
        {
            base.OnDetach(Entity);

            Entity.SetRawValue("Durability", 0);
            Entity.SetRawValue("Wound Threshold", 0);
            Entity.SetRawValue("Death Rating", 0);
        }
        public string advantages
        {
            get { return this.Advantages; }
        }
        public string disadvantages
        {
            get { return this.Disadvantages; }
        }
        public string type
        {
            get { return this.Type; }
        }
        public int durability
        {
            get { return this.Durability; }
        }
        public int cpCost
        {
            get { return this.CPCost; }
        }
    }

    /// <summary>
    /// Really the only thing we need to do here is add/remove the Modifiers
    /// You can have only one Background per Character.
    /// </summary>
    class Background : AttachableAttribute
    {
        public readonly string Advantages, Disadvantages;
        public readonly string CommonMorphs;
        public const string AttributeColor = "Background";
        
        internal Background(
            string Name, 
            string Advantages,
            string Disadvantages,
            string CommonMorphs, 
            Modifiers Modifiers, 
            string Description = null)
            : base(Name, AttributeColor, Modifiers, Description)
        {
            this.Advantages = Advantages;
            this.Disadvantages = Disadvantages;
            this.CommonMorphs = CommonMorphs;
        }

        public string advantages
        {
            get { return this.Advantages; }
        }
        public string disadvantages
        {
            get { return this.Disadvantages; }
        }
        public string commonmorphs
        {
            get { return this.CommonMorphs; }
        }
    }

    /// <summary>
    /// Very similar to the Background (we could subclass this)
    /// You can have only one Faction per Character.
    /// </summary>
    class Faction : AttachableAttribute
    {
        public readonly string Advantages, Disadvantages;
        public const string AttributeColor = "Faction";

        internal Faction(
            string Name,
            string Advantages,
            string Disadvantages,
            Modifiers Modifiers,
            string Description = null)
            : base(Name, AttributeColor, Modifiers, Description)
        {
            this.Advantages = Advantages;
            this.Disadvantages = Disadvantages;
        }

        public string advantages
        {
            get { return this.Advantages; }
        }
        public string disadvantages
        {
            get { return this.Disadvantages; }
        }
    }

    /// <summary>
    /// Similar to Faction and Background, all we care about are the modifiers.
    /// Characters can have more than one Trait.
    /// todo: CP adjustment for Traits.
    /// </summary>
    class Trait : AttachableAttribute
    {
        private string Source;
        private bool IsPositive;
        private int CPCost;
        public const string AttributeColor = "Trait";

        internal Trait(
            string Name,
            string Source,
            bool IsPositive,
            int CPCost,
            Modifiers Modifiers,
            string Description = null)
            : base(Name, AttributeColor, Modifiers, Description)
        {
            this.Source = Source;
            this.IsPositive = IsPositive;
            this.CPCost = CPCost;
        }
        public string source
        {
            get { return this.Source; }
        }
        public bool isPositive
        {
            get { return this.IsPositive; }
        }
        public int cpCost
        {
            get { return this.CPCost; }
        }
    }

    /// <summary>
    /// All gear (Armor, ArmorMods, Enhancements, Cyberware) is jammed into once class
    /// for simplicity. 
    /// </summary>
    class Gear : AttachableAttribute
    {
        public readonly Int32 KineticArmor, EnergyArmor;
        public string Cost, Type;
        public const string AttributeColor = "Gear";

        internal Gear(
            string Name,
            string Type,
            string Cost,
            Int32 KineticArmor, 
            Int32 EnergyArmor,
            Modifiers Modifiers, 
            string Description = null)
            : base(Name, AttributeColor, Modifiers, Description)
        {
            this.KineticArmor = KineticArmor;
            this.EnergyArmor = EnergyArmor;
            this.Type = Type;
            this.Cost = Cost;
        }

        public override void OnAttach(Entity Entity)
        {
            base.OnAttach(Entity);
            if (KineticArmor > 0)
            {
                AttributeFilter Af = new AttributeFilter(this.Name, this.Color, this, 
                    Entity["Armor (Kinetic)"], 
                    value => value + this.KineticArmor);
                Af.Owner = this;
                Entity.Add(Af);
            }
            if (EnergyArmor > 0)
            {
                AttributeFilter Af = new AttributeFilter(this.Name, this.Color, this,
                    Entity["Armor (Energy)"],
                    value => value + this.KineticArmor);
                Af.Owner = this;
                Entity.Add(Af);
            }
        }

        public string type
        {
            get { return this.Type; }
        }
        public int kineticArmor
        {
            get { return this.KineticArmor; }
        }
        public int energyArmor
        {
            get { return this.EnergyArmor; }
        }
        public string creditCost
        {
            get { return this.Cost; }
        }
    }

    #region Resource Manager class

    /// <summary>
    /// The Resource Manager holds most of our data processing and collections
    /// of all the game world classes.
    /// Improvment: All the collections are r/w although are not meant to be written to.
    /// It also wound't be thread safe to do so. Only place writing should be our constructor
    /// (but this is not enforced currently)
    /// </summary>

    partial class ResourceManager
    {
        private static ResourceManager Self = null;
        private static object Lock = new object();

        public static Dictionary<string, string> LongNames = new Dictionary<string, string>
        {
            {"COG", "Cognition"},
            {"COO", "Coordination"},
            {"INT", "Intuition"},
            {"REF", "Reflexes"},
            {"SAV", "Savvy"},
            {"SOM", "Somatics"},
            {"WIL", "Willpower"},
            {"MOX", "Moxie"},
            {"CRED","Credits"},
            {"SPD", "Speed"},
            {"DUR", "Durability"}
        };

        public List<Aptitude> Aptitudes = new List<Aptitude>();
        public List<Stat> Stats = new List<Stat>();
        public List<Skill> Skills = new List<Skill>();
        public List<Reputation> Reputations = new List<Reputation>();
        public List<Background> Backgrounds = new List<Background>();
        public List<Faction> Factions = new List<Faction>();
        public List<Trait> Traits = new List<Trait>();
        public List<Morph> Morphs = new List<Morph>();
        public List<Gear> Gear = new List<Gear>();

        private ResourceManager()
        {
            XElement CoreRules = null;
            XElement AdditionalRules = null;

            CoreRules = XElement.Load("CoreRules-jsenek.xml");
            AdditionalRules = XElement.Load("Aptitudes.xml");

            // read skills taking into consideration some skills have Fields
            // this implementation makes each field it's own skill

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
                            ResourceManager.LongNames[xe.Element("apt").Value],
                            xe.Element("defaulting").Value == "Yes" ? true : false,
                            xe.Element("category").Value,
                            xe.Element("name").Value,               // add the skill name proper as group
                            "<Skill description>"));
                    }
                }
                else
                {
                    Skills.Add(new Skill(
                        xe.Element("name").Value,
                        ResourceManager.LongNames[xe.Element("apt").Value],
                        xe.Element("defaulting").Value == "Yes" ? true : false,
                        xe.Element("category").Value,
                        xe.Element("group").Value,
                        "<Skill description>"));
                }
            }

            foreach (XElement xe in AdditionalRules.Descendants("Rep"))
            {
                Reputations.Add(new Reputation(
                    xe.Attribute("Name").Value,
                    xe.Element("Description").Value));
            }
 
            foreach (XElement xe in AdditionalRules.Descendants("Aptitudes").Descendants("Aptitude"))
            {
                Aptitudes.Add(new Aptitude(
                    xe.Attribute("Name").Value,
                    xe.Element("Shorthand").Value,
                    xe.Element("Description").Value));
            }

            // Stats
            foreach (XElement xe in AdditionalRules.Descendants("Stats").Descendants("Stat"))
            {
                Stats.Add(new Stat(
                    xe.Attribute("Name").Value,
                    xe.Element("Description").Value));
            }

            // Backgrounds
            foreach (XElement xe in CoreRules.Descendants("backgrounds").Descendants("background"))
            {
                string Name = xe.Element("name").Value;
                string Description = xe.Element("description").Value;
                Modifiers Mods = Modifiers.Parse(xe);

                Backgrounds.Add(new Background(
                    Name,
                    xe.Element("advantages").Element("text").Value,
                    xe.Element("disadvantages").Element("text").Value,
                    xe.Element("commonmorphs").Element("text").Value,
                    Mods,
                    Description));
            }

            // Traits
            List<IEnumerable<XElement>> ParseList = new List<IEnumerable<XElement>>();
            ParseList.Add(CoreRules.Descendants("positiveTraits").Descendants("trait"));
            ParseList.Add(CoreRules.Descendants("negativeTraits").Descendants("trait"));

            bool isPositive = true;
            foreach (IEnumerable<XElement> ParseChunk in ParseList)
            {
                foreach (XElement xe in ParseChunk)
                {
                    string Name = xe.Element("name").Value;
                    string Source = xe.Element("source").Value;
                    string Description = xe.Element("description").Value;
                    int Cost = Convert.ToInt32(xe.Element("CP").Value);
                    if (isPositive == false)
                    {
                        Cost = -Cost;
                    }
                    Modifiers Mods = Modifiers.Parse(xe);

                    Traits.Add(new Trait(
                        Name,
                        Source,
                        isPositive,
                        Cost,
                        Mods,
                        Description)); 
                }
                isPositive = false;
            }

            // Factions
            foreach (XElement xe in CoreRules.Descendants("factions").Descendants("faction"))
            {
                string Name = xe.Element("name").Value;
                string Description = xe.Element("description").Value;
                Modifiers Mods = Modifiers.Parse(xe);

                Factions.Add(new Faction(
                    Name,
                    xe.Element("advantages").Element("text").Value,
                    xe.Element("disadvantages").Element("text").Value,
                    Mods,
                    Description));
            }

            // Morphs
            foreach (XElement xe in CoreRules.Descendants("morphs").Descendants("morph"))
            {
                string Name = xe.Element("name").Value;
                string Description = xe.Element("description").Value;
                Modifiers Mods = Modifiers.Parse(xe);

                Morph Sleeve = new Morph(
                    Name,
                    xe.Element("type").Value,
                    xe.Element("advantages").Element("text").Value,
                    xe.Element("disadvantages").Element("text").Value,
                    Convert.ToInt32(xe.Element("durability").Value),
                    Convert.ToInt32(xe.Element("CPCost").Value),
                    Mods,
                    Description);
                foreach (XElement Implant in xe.Descendants("implants").Descendants("implant"))
                {
                    Sleeve.Implants.Add(Implant.Value.ToString());
                }
                Morphs.Add(Sleeve);
            }

            // parse all the different gear using a single function
            Dictionary<string, IEnumerable<XElement>> ParseList2 = new Dictionary<string, IEnumerable<XElement>>();
            ParseList2.Add("Augmentation", CoreRules.Descendants("gearAndEquipment").Descendants("augmentation"));
            ParseList2.Add("Armor", CoreRules.Descendants("otherGear").Descendants("armor"));
            ParseList2.Add("ArmorAccessory", CoreRules.Descendants("gearAndEquipment").Descendants("armorAccessory"));
            ParseList2.Add("ArmorMod", CoreRules.Descendants("gearAndEquipment").Descendants("armorMod"));
            ParseList2.Add("Gear", CoreRules.Descendants("gearAndEquipment").Descendants("gear"));
            foreach (KeyValuePair<string, IEnumerable<XElement>> ParseChunk in ParseList2)
            {
                foreach (XElement xe in ParseChunk.Value)
                {
                    string Name = xe.Element("name").Value;
                    string Description = xe.Element("description").Value;
                    string CreditCost = xe.Element("creditCost").Value;
                    int KineticArmor = 0, EnergyArmor = 0;
                    Modifiers Mods = Modifiers.Parse(xe);
                    if (xe.Element("energy") != null)
                    {
                        EnergyArmor = Convert.ToInt32(xe.Element("energy").Value);
                    }
                    if (xe.Element("kinetic") != null)
                    {
                        KineticArmor = Convert.ToInt32(xe.Element("kinetic").Value);
                    }
                    Gear.Add(new Gear(Name, ParseChunk.Key, CreditCost, KineticArmor, EnergyArmor,
                        Mods, Description));
                }
            }
        }

        /// <summary>
        /// Apply modifiers contained in the provided xml fragment on behalf of an Attribute
        /// Improvement: Aptitude limiters are applied last in this function, and they will only
        /// work correctly when they run last, but this is not enforced properly.
        /// </summary>

        public static void AttachModifiers(Entity Entity, Attribute Source, Modifiers Mods)
        {
            foreach (XElement xe in Mods.Aptitude.Where(el => el.Name == "aptmod"))
            {
                string name = xe.Element("name").Value;
                string amount = xe.Element("amount").Value;
                int Modifier = Convert.ToInt32(amount);
                if (name == "Choice")
                {
                    AttributeFilter Af = new AttributeFilter(Source.name, Source.color, Source,
                        Entity["CP"], value => value + (10 * Modifier), "CP Compensation");
                    Af.Owner = Source;
                    Entity.Add(Af);
                }
                else
                {
                    name = ResourceManager.LongNames[name];
                    Entity.AddAdditiveFilter(Source, name, Modifier);
                }
            }
            foreach (XElement xe in Mods.AptitudeMax.Elements().Where(el => ResourceManager.LongNames.ContainsKey(el.Name.ToString())))
            {
                string name = xe.Name.ToString();
                name = ResourceManager.LongNames[name];
                Int32 Max = Convert.ToInt32(xe.Value);
                // for some reason, math.max was NOT doing anything in this lambda
                AttributeFilter Af = new AttributeFilter(Source.name, Source.color, Source,
                    Entity[name], Num => Num > Max ? Max : Num,"Aptitude maximum");
                Af.Owner = Source;
                Entity.Add(Af);
            }
            foreach (XElement xe in Mods.Skill.Where(el => el.Name == "skillmod"))
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
                        AttributeFilter Af = new AttributeFilter(Source.name, Source.color, Source,
                            Entity["CP"], value => value + (Quantity * Modifier), "CP Compensation");
                        Af.Owner = Source;
                        Entity.Add(Af);
                    }
                    else
                    {
                        Entity.AddAdditiveFilter(Source, name, Modifier);
                    }
                }
                else
                {
                    // todo: skillmod with no amount
                }
            }
            foreach (XElement xe in Mods.Stat.Where(el => el.Name == "statmod"))
            {
                string name = xe.Element("name").Value;
                if (name == "Choice")
                {
                    // sometimes the user gets to choose which skills to apply a bonus to.
                    // todo: for stats, costs are variable, so this needs to be implemented with a callback query

                    throw new System.NotImplementedException("Sorry!");
                }
                else if (name == "REP")
                {
                    // todo: currently cannot process the Isolate -10 starting rep disadvantage
                    continue;
                }

                if (xe.Element("amount") != null)
                {
                    name = ResourceManager.LongNames[name];
                    int Modifier = Convert.ToInt32(xe.Element("amount").Value);
                    Entity.AddAdditiveFilter(Source, name, Modifier);
                }
                else
                {
                    // todo: statmod with no amount
                    throw new System.NotImplementedException("Sorry!");
                }
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

        #endregion

    /// <summary>
    /// The main event for the application layer, EPCharacter provides all the needed
    /// functionality to build and operate an EP character. Our main task is to supply
    /// the Model with all the known game rules.
    /// </summary>

    class EPCharacter : Entity, INotifyPropertyChanged
    {
        public readonly ResourceManager Resources = ResourceManager.GetInstance();
        /// <note>
        /// This has to be maintained manually, as there's no built-in way to 
        /// automatically maintain a subset of another ObserverableCollection
        /// </note>
        internal ObservableCollection<Attribute> NonValueAttributes = new ObservableCollection<Attribute>();

        // Overriding the Collection<T> default behaviour
        protected override void InsertItem(int index, Attribute newItem)
        {
            Type t = newItem.GetType();
            if (    t == typeof(Morph) ||
                    t == typeof(Background) ||
                    t == typeof(Faction) ||
                    t == typeof(Trait) ||
                    t == typeof(Gear) )
            {
                NonValueAttributes.Add(newItem);
            }
            base.InsertItem(index, newItem);
        }

        protected override void RemoveItem(int index)
        {
            Attribute removedItem = Items[index];
            NonValueAttributes.Remove(removedItem);
            base.RemoveItem(index);
        }

        public void DeprecatedAttachAttribute(string Color, string Name)
        {
            AttachableAttribute NewAttribute = null;
            Boolean isExclusive = true;
            switch (Color)
            {
                case Faction.AttributeColor:
                    NewAttribute = Resources.Factions.Find(bg => bg.name == Name);
                    break;
                case Background.AttributeColor:
                    NewAttribute = Resources.Backgrounds.Find(bg => bg.name == Name);
                    break;
                case Morph.AttributeColor:
                    NewAttribute = Resources.Morphs.Find(bg => bg.name == Name);
                    break;
                case Trait.AttributeColor:
                    NewAttribute = Resources.Traits.Find(bg => bg.name == Name);
                    isExclusive = false;
                    break;
                case Gear.AttributeColor:
                    NewAttribute = Resources.Gear.Find(bg => bg.name == Name);
                    isExclusive = false;
                    break;
            }
            if (isExclusive == true)
            {
                while (true)
                {
                    Attribute Att = this.Where(El => El.GetType() == NewAttribute.GetType()).FirstOrDefault();
                    if (Att != null)
                    {
                        this.Remove(Att);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            if (NewAttribute != null)
            {
                this.Add(NewAttribute);
            }
            else
            {
                throw new ArgumentException("Attribute not found");
            }
            NotifyPropertyChanged("BilledCPCost");
        }

        public void DeprecatedDetachAttribute(string Type, string Name)
        {
            Attribute Att = this.Where(El => El.name == Name && El.color == Type).FirstOrDefault();
            if (Att != null)
            {
                this.Remove(Att);
            }
            NotifyPropertyChanged("BilledCPCost");
        }

        public IEnumerable<Skill> Skills
        {
            get
            {
                IEnumerable<Skill> Set = this.OfType<Skill>();
                return Set;
            }
        }
        public IEnumerable<Skill> KnowledgeSkills
        {
            get
            {
                return this.Skills.Where(Sk => Sk.category == "Knowledge");
            }
        }
        public IEnumerable<Skill> ActiveSkills
        {
            get
            {
                return this.Skills.Where(Sk => Sk.category == "Active");
            }
        }
        public IEnumerable<Skill> CombatSkills
        {
            get
            {
                return this.Skills.Where(Sk => Sk.group == "Combat");
            }
        }
        public IEnumerable<Skill> ActionSkills
        {
            get
            {
                return this.Skills.Where(Sk => Sk.group == "Action");
            }
        }
        public IEnumerable<Skill> MovementSkills
        {
            get
            {
                return this.Skills.Where(Sk => Sk.group == "Movement");
            }
        }
        public IEnumerable<Skill> InterpersonalSkills
        {
            get
            {
                return this.Skills.Where(Sk => Sk.group == "Interpersonal");
            }
        }

        public Morph Morph
        {
            get
            {
                return this.OfType<Morph>().FirstOrDefault();
            }
        }

        /*
        Spend Free Points (p. 134)
            105 aptitude points (ok)
            1 Moxie             (ok)
            5,000 credit        (ok)
            50 Rep              (ok)
            Native tongue       (todo)
        Spend Customization Points (p. 135)
            1,000 CP to spend
            15 CP = 1 Moxie                  (ok)
            10 CP = 1 aptitude point         (ok)
            5 CP = 1 psi sleight             (todo)
            5 CP = 1 specialization          (todo)
            2 CP = 1 skill point (61-80)     (ok)
            1 CP = 1 skill point (up to 60)  (ok)
            1 CP = 1,000 credit              (ok)
            1 CP = 10 rep                    (ok)
        */

        public int PointsInAptitudes
        {
            get
            {
                return this.OfType<Aptitude>().Select(Va => Va.rawValue).Sum();
            }
        }

        private static int SkillPointCost(ValueAttribute Va)
        {
            // todo: Any skill or aptitude bonuses from gear are treated as modifications; 
            // they are applied after all CP are spent and do not affect the cost of buying skills or aptitudes during character creation.
            int Above60 = Math.Max(Va.cookedValue - 60, 0);
            int Below60 = Va.rawValue - Above60;
            return Below60 + (Above60 * 2);
        }

        public int PointsInKnowledgeSkills
        {
            get
            {
                IEnumerable<ValueAttribute> Skills = this.KnowledgeSkills;
                return Skills.Sum(Sk => SkillPointCost(Sk));
            }
        }

        public int PointsInActiveSkills
        {
            get
            {
                IEnumerable<ValueAttribute> Skills = this.ActiveSkills;
                return Skills.Sum(Sk => SkillPointCost(Sk));
            }
        }

        public int PointsInRep
        {
            get
            {
                return this.OfType<Reputation>().Select(Va => Va.rawValue).Sum();
            }
        }

        /// <summary>
        /// CP cost minus freebies
        /// </summary>
        public int BilledCPCost
        {
            get
            {
                int Sum, Cost = 0;

                Sum = this.PointsInRep;
                if (Sum > 50)
                {
                    Cost += (Sum - 50) / 10;
                }
                Sum = this.PointsInAptitudes;
                if (Sum > 105)
                {
                    Cost += 10 * (Sum - 105);
                }
                Cost += this.PointsInActiveSkills + this.PointsInKnowledgeSkills;

                Sum = this.GetCookedValue("Credits");
                Sum += this.CreditCost;
                if (Sum > 5000)
                {
                    Cost += (Sum - 5000) / 1000;
                }

                Sum = GetRawValue("Moxie");
                if (Sum > 1)
                {
                    Cost += (Sum - 1) * 15;
                }

                Cost += this.CommonCost;

                return Cost;
            }
        }

        public int CPCost
        {
            get
            {
                int Cost = 0;

                Cost += this.PointsInAptitudes * 10;
                Cost += this.PointsInRep / 10;
                Cost += this.PointsInActiveSkills + this.PointsInKnowledgeSkills;

                Cost += GetRawValue("Credits") / 1000;
                Cost += GetRawValue("Moxie") * 15;
                Cost += this.CommonCost;

                return Cost;
            }
        }

        private int CommonCost
        {
            get
            {
                int Cost = 0;

                // morph CP cost
                Morph M = this.Morph;
                if (M != null)
                {
                    Cost += M.CPCost;
                }

                Cost += this.OfType<Trait>().Select(Aa => Aa.cpCost).Sum();

                return Cost;
            }
        }

        /// <summary>
        /// translate cost classes (like "Moderate") using the average credit cost table
        /// </summary>
        public int CreditCost
        {
            get
            {
                IEnumerable<Gear> GearList = this.OfType<Gear>();
                return GearList.Select(Ge => ResourceManager.CostAverage[Ge.creditCost]).Sum();
            }
        }

        // This constructor creates a brand new character
        
        public EPCharacter()
            : base()
        {
            this.CollectionChanged += AAttributesChanged;

            foreach (Aptitude Apt in Resources.Aptitudes)
            {
                Aptitude Clone = new Aptitude(Apt.name, Apt.shortName, Apt.description);
                this.Add(Clone);
                Clone.PropertyChanged += VAttributeChanged;
            }
            foreach (Stat Stat in Resources.Stats)
            {
                Stat Clone = new Stat(Stat.name, Stat.description);
                this.Add(Clone);
                Clone.PropertyChanged += VAttributeChanged;
            }
            foreach (Skill s in Resources.Skills)
            {
                Skill Clone = new Skill(s.name, s.governingAptitude, s.allowsDefaulting, s.category, s.group, s.description);
                this.Add(Clone);
                Clone.PropertyChanged += VAttributeChanged;
            }
            foreach (Reputation Rep in Resources.Reputations)
            {
                Reputation Clone = new Reputation(Rep.name, Rep.description);
                this.Add(Clone);
                Clone.PropertyChanged += VAttributeChanged;
            }

            // Improvement: The relationships are being expressed twice here,
            // once in the Lambda and once in the AddDependent call.

            AttributeFilter Af = new AttributeFilter("Initiative", "CoreRules", null,
                this["Initiative"], a => a + (this.GetCookedValue("Intuition") + this.GetCookedValue("Reflexes") * 2) / 5);
            this.Add(Af);
            (this["Intuition"] as ValueAttribute).AddDependent(this["Initiative"]);
            (this["Reflexes"] as ValueAttribute).AddDependent(this["Initiative"]);

            Af = new AttributeFilter("Lucidity", "CoreRules", null,
                this["Lucidity"], a => a + this.GetCookedValue("Willpower") * 2);
            this.Add(Af);
            (this["Willpower"] as ValueAttribute).AddDependent(this["Lucidity"]);

            Af = new AttributeFilter("Trauma Threshold", "CoreRules", null,
                this["Trauma Threshold"], a => a + this.GetCookedValue("Lucidity") / 5);
            this.Add(Af);
            (this["Lucidity"] as ValueAttribute).AddDependent(this["Trauma Threshold"]);

            Af = new AttributeFilter("Insanity Rating", "CoreRules", null,
                this["Insanity Rating"], a => a +this.GetCookedValue("Lucidity") * 2);
            this.Add(Af);
            (this["Insanity Rating"] as ValueAttribute).AddDependent(this["Lucidity"]);

            Af = new AttributeFilter("Damage Bonus", "CoreRules", null,
                this["Damage Bonus"],  a => a + this.GetCookedValue("Somatics") / 10);
            this.Add(Af);
            (this["Somatics"] as ValueAttribute).AddDependent(this["Damage Bonus"]);

            this.SetRawValue("Moxie", 1);
            this.SetRawValue("Speed", 1);
            this.SetRawValue("CP", 1000);
            this.SetRawValue("Credits", 5000);
        }

        public new event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void AAttributesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

            bool NeedsUpdate = false;
            if (e.NewItems != null)
            {
                foreach (object o in e.NewItems)
                {
                    if (!(o.GetType() == typeof(Faction) ||
                          o.GetType() == typeof(Background)))
                    {
                        NeedsUpdate = true;
                    }
                }
            }
            if (NeedsUpdate == false && e.OldItems != null)
            {
                foreach (object o in e.OldItems)
                {
                    if (!(o.GetType() == typeof(Faction) ||
                          o.GetType() == typeof(Background)))
                    {
                        NeedsUpdate = true;
                    }
                }
            }
            if (NeedsUpdate == true)
            {
                NotifyPropertyChanged("BilledCPCost");
            }
        }
        public void VAttributeChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e == null)
            {
                System.Diagnostics.Debug.WriteLine("Skip");
            }
            else
            {
                NotifyPropertyChanged("BilledCPCost");
                NotifyPropertyChanged("PointsInActiveSkills");
                NotifyPropertyChanged("PointsInKnowledgeSkills");
            }
        }
    }

    /* Character creation and point costs:
        Active skill minimum: 400 skill points
        Knowledge skill minimum: 300 skill points
        Choose Starting Morph (pp. 136 and 139)
        Choose Traits (pp. 136 and 145)
        Purchase Gear (p. 136)
        Choose Motivation (p. 137)
        Calculate Remaining Stats (p. 138)
        Detail the Character (p. 138)
    */

    partial class ResourceManager
    {
        public static Dictionary<string, int> CostAverage = new Dictionary<string, int>()
        {
            { "Trivial", 50 },
            { "Low", 250 },
            { "Moderate", 1000 },
            { "High", 5000 },
            { "Expensive", 20000 }
        };

        public IEnumerable<Gear> Armors
        {
            get
            {
                return Gear.Where(g => g.Type == "Armor").OrderBy(g => g.name);
            }
        }

        public IEnumerable<string> Fields
        {
            get
            {
                return Skills.Where(Sk => Sk.isFieldSkill == true).Select(Va => Va.name.Split(':').First()).Distinct().OrderBy(s => s);
            }
        }

        // The accessor returns any attribute
        internal Attribute this[string Name]
        {
            get
            {
                Attribute Att;
                if ((Att = this.Factions.Find(bg => bg.name == Name)) != null) return Att;
                if ((Att = this.Backgrounds.Find(bg => bg.name == Name)) != null) return Att;
                if ((Att = this.Morphs.Find(bg => bg.name == Name)) != null) return Att;
                if ((Att = this.Traits.Find(bg => bg.name == Name)) != null) return Att;
                if ((Att = this.Gear.Find(bg => bg.name == Name)) != null) return Att;
                return null;
            }
        }
    }
}