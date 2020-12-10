using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

/*
 * Concepts:
 * Entity - An entity in the world (typically representing a creature or being)
 * Attribute - Any attribute (represented by value or otherwise) of an entity
 *   ValueAttribute - subclass of Attribute, holding an integer value
 *   AttachableAttribute - Attribute w/ hooks to attach/detach from an entity.
 * Filter - a modifier to an attribute
 * Raw value - a base attribute or value (usually defined at character creation, often bought with points)
 * Cooked value - final value used for game play
 * The model holds things that are (not things that could be) in other words,
 *   any properties actually assigned to an entity vs. a list of possible attributes (traits, gear etc)
 * Filters are useful for core game mechanics such as derived values. Filters are not serialized.
 */

namespace EPPlayer
{
    interface IAttachableAttribute
    {
        void OnAttach(Entity Entity);
        void OnDetach(Entity Entity);
        string AAName
        {
            get;
        }
        string AAColor
        {
            get;
        }
    }

    class Attribute
    {
        public readonly string Name;
        public readonly string Color;
        public readonly string Description;

        public Attribute(string Name, string Color, string Description = null)
        {
            this.Name = Name;
            this.Color = Color;
            this.Description = Description;
        }
    }

    class ValueAttribute : Attribute
    {
        public int Value;
        public ValueAttribute(string Name, string Color, int Value, string Description = null)
            : base(Name, Color, Description)
        {
            this.Value = Value;
        }
    }

    class AttributeFilter : Attribute
    {
        public readonly ValueAttribute TargetAttribute;
        private readonly Func<int, int> FilterFunction;

        public AttributeFilter(string Name, string Color, 
            ValueAttribute Target, Func<int, int> FilterFunction, string Description = null)
            : base(Name, Color, Description)
        {
            this.TargetAttribute = Target;
            this.FilterFunction = FilterFunction;
        }

        public int FilteredValue(int PreviousValue)
        {
            return FilterFunction(PreviousValue);
        }
    }

    class Entity
    {
        public Dictionary<String, ValueAttribute> VAttributes = new Dictionary<String, ValueAttribute>();
        public List<AttributeFilter> VFilters = new List<AttributeFilter>();
        public List<IAttachableAttribute> AAttributes = new List<IAttachableAttribute>();
        public void Load()
        {
            throw new NotImplementedException("Sorry!");
        }
        public void Save()
        {
            throw new NotImplementedException("Sorry!");
        }
        public List<string> ValueAttributes
        {
            get
            {
                return VAttributes.Values.Select(att => att.Name).ToList<string>();
            }
        }
        public List<string> ValueAttributesByColor(string Color)
        {
            return VAttributes.Values.Where(va => va.Color == Color).Select(att => att.Name).ToList<string>();
        }
        public List<string> Colors
        {
            get
            {
                return VAttributes.Values.Select(att => att.Color).Distinct().ToList<string>();
            }
        }
        public int GetRawValue(string Name)
        {
            return VAttributes[Name].Value;
        }
        public void SetRawValue(string Name, int Value)
        {
            VAttributes[Name].Value = Value;
        }
        public ValueAttribute GetRawValueAttributeByName(string Name) 
        {
            return VAttributes[Name];
        }
        public void SetRawValueAttributeByName(string Name, int Value)
        {
            if (!VAttributes.ContainsKey(Name))
            {
                throw new ArgumentException("Unknown ValueAttribute");
            }
            VAttributes[Name].Value = Value;
        }

        // The accessor returns cooked values
        public int this[string Name]
        {
            get
            {
                if (! VAttributes.ContainsKey(Name))
                {
                    throw (new System.ArgumentException());
                }
                int Value = VAttributes[Name].Value;
                IEnumerable<AttributeFilter> ApplicableFilters = VFilters.Where(f => f.TargetAttribute.Name == Name);
                foreach (AttributeFilter Filter in ApplicableFilters)
                {
                    Value = Filter.FilteredValue(Value);
                }
                return Value;
            }
        }
    }
}
