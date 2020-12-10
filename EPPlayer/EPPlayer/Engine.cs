using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Data;

/*
 * This module contains a game system agnostic framework to model
 *     an entity in a game system and its mechanics
 * 
 * Concepts
 
 * Entity: An entity in the world (typically representing a creature, PC or NPC,
 *     but the model won't judge you if you use it to model a toaster oven or blueberry bush)
 * Attribute: Any attribute usually describing a part or aspect of the entitiy
 *     ValueAttribute - Attribute holding an integer value 
 *         (for example dexterity or hit points)
 *     AttachableAttribute - Attribute which has significance merely by its presence
 *         (for example a sword, a beard, or a mortal enemy)
 *     AttributeFilter - a function with modifies a value attribute without changing it
 * Raw value: Value of a ValueAttribute with no filters applied
 * Cooked value: Value of a ValueAttribute with all filters applied
 * Color: Our term for "kind of" or "type of"
 * 
 * The model holds things that are (not things that could be). In other words, it currently
 * does not hold any AttachableAttributes which are not actually attached. All value attributes 
 * must be defined and initalized.
 */

namespace EPPlayer
{
    internal interface IAttachableAttribute
    {
        void OnAttach(Entity Entity);
        void OnDetach(Entity Entity);
    }

    internal interface IFilter
    {
        int FilteredValue(int PreviousValue);
    }

    abstract class Attribute
    {
        protected readonly string Name;
        protected readonly string Color;
        protected readonly string Description;

        /// <summary>
        /// Remember who created me
        /// </summary>
        internal Attribute Owner;
 
        /// <summary>
        /// Things I have created
        /// </summary>
        internal List<Attribute> Children = new List<Attribute>();

        internal Attribute(string Name, string Color, string Description = null)
        {
            this.Name = Name;
            this.Color = Color;
            this.Description = Description;
            this.Owner = null;
        }

        public string name
        {
            get { return this.Name; }
        }
        public string color
        {
            get { return this.Color; }
        }
        public string description
        {
            get { return this.Description; }
        }
    }

    internal class ValueAttribute : Attribute, INotifyPropertyChanged, IAttachableAttribute
    {
        private int RawValue;

        /// <summary>
        /// Things that change me
        /// </summary>
        protected List<IFilter> Filters = new List<IFilter>();
        /// <summary>
        /// Dependent attributes (who to notify if my value changes)
        /// </summary>
        internal List<ValueAttribute> DownstreamAttributes = new List<ValueAttribute>();

        internal ValueAttribute(string Name, string Color, int Value, string Description = null)
            : base(Name, Color, Description)
        {
            this.RawValue = Value;
        }
        public int rawValue
        {
            get { return this.RawValue; }
            set
            {
                if (this.RawValue != value)
                {
                    this.RawValue = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged("cookedValue");
                    foreach (Attribute Dependent in DownstreamAttributes)
                    {
                        // todo: work the dependency graph!
                    }
                }
            }
        }
        public int cookedValue
        {
            get
            {
                int Result = this.RawValue;
                foreach (AttributeFilter Filter in Filters)
                {
                    Result = Filter.FilteredValue(Result);
                }
                return Result;
            }
        }
        public List<string> cookedValueText
        {
            get
            {
                List<string> Log = new List<string>();
                int Result = this.RawValue;
                int Previous = Result;
                Log.Add(string.Format("Initial value of {0}: {1}", this.Name, Result.ToString()));
                
                foreach (AttributeFilter Filter in Filters)
                {
                    Result = Filter.FilteredValue(Result);
                    Log.Add(string.Format("Filter {0}/{1}: {2} ({3})", Filter.color, Filter.name, Result - Previous, Filter.description));
                    Previous = Result;
                }
                return Log;
            }
        }
        internal void AddFilter(IFilter Filter)
        {
            this.Filters.Add(Filter);
            NotifyPropertyChanged("cookedValue");
        }

        internal void RemoveFilter(IFilter Filter)
        {
            this.Filters.Remove(Filter);
            NotifyPropertyChanged("cookedValue");
        }

        internal void AddDependent(Attribute Dependent)
        {
            this.DownstreamAttributes.Add(Dependent as ValueAttribute);
        }

        internal void RemoveDependent(Attribute Dependent)
        {
            this.DownstreamAttributes.Remove(Dependent as ValueAttribute);
        }

        public void OnAttach(Entity Entity)
        {
        }
        public void OnDetach(Entity Entity)
        {
            this.RawValue = 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    internal class AttributeFilter : Attribute, IFilter
    {
        internal readonly ValueAttribute Target;
        internal readonly Func<int, int> FilterFunction;

        internal AttributeFilter(string Name, string Color, 
            Attribute Owner, Attribute Target,
            Func<int, int> FilterFunction, string Description = null)
            : base(Name, Color, Description)
        {
            this.Owner = Owner;
            this.Target = Target as ValueAttribute;
            this.FilterFunction = FilterFunction;
        }

        public virtual int FilteredValue(int PreviousValue)
        {
            return FilterFunction(PreviousValue);
        }
    }

    class Entity : ObservableCollection<Attribute>
    {
        public Entity()
        {
        }

        // Overriding the Collection<T> default behaviour
        protected override void InsertItem(int index, Attribute newItem)
        {
            lock (this)
            {
                base.InsertItem(index, newItem);
            }
            if (newItem is IAttachableAttribute)
            {
                (newItem as IAttachableAttribute).OnAttach(this);
            }
            if (newItem is IFilter)
            {
                // todo: a good example of something you clearly CAN do, but should you? 
                ValueAttribute Target = ((newItem as AttributeFilter).Target as ValueAttribute);
                Target.AddFilter(newItem as IFilter);
                // System.Diagnostics.Debug.WriteLine("AF Target={0} Filter={1}", Target.name, newItem.description);
            }
            if (newItem.Owner != null)
            {
                newItem.Owner.Children.Add(newItem);
            }
        }

        protected override void SetItem(int index, Attribute newItem)
        {
            Attribute replaced = Items[index];
            lock (this)
            {
                base.SetItem(index, newItem);
            }

            throw new System.NotImplementedException();
        }

        protected override void RemoveItem(int index)
        {
            Attribute removedItem;
            lock (this)
            {
                removedItem = Items[index];
                base.RemoveItem(index);
            }
            if (removedItem is IAttachableAttribute)
            {
                (removedItem as IAttachableAttribute).OnDetach(this);
            }
            if (removedItem is IFilter)
            {
                ValueAttribute Target = ((removedItem as AttributeFilter).Target as ValueAttribute);
                Target.RemoveFilter(removedItem as IFilter);
                // System.Diagnostics.Debug.WriteLine("RF Target={0} Filter={1}", Target.name, removedItem.description);
            }
            foreach (Attribute Child in removedItem.Children)
            {
                this.Remove(Child);
            }
            removedItem.Children.Clear();
        }
        protected override void ClearItems()
        {
            lock (this)
            {
                base.ClearItems();
            }
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// helper function to add an additive filter ("bonus")
        /// Modifies (target name) by (Modifier)
        /// Note: does not cause a dependency (Modifier is fixed)
        /// </summary>
        /// <param name="Owner"></param>
        /// <param name="TargetName"></param>
        /// <param name="Modifier"></param>
        internal void AddAdditiveFilter(Attribute Owner, string TargetName, int Modifier)
        {
            Attribute TargetAttribute = this[TargetName];
            AttributeFilter Af = new AttributeFilter(Owner.name, Owner.color, Owner,
                TargetAttribute, i => i + Modifier, string.Format("+{0} from {1}", Modifier, Owner.name));
            Af.Owner = Owner;
            this.Add(Af);
        }

        internal int GetRawValue(string Name)
        {
            return this.OfType<ValueAttribute>().Where(Va => Va.name == Name).First().rawValue;
        }
        internal int GetCookedValue(string Name)
        {
            return this.OfType<ValueAttribute>().Where(Va => Va.name == Name).First().cookedValue;
        }
        internal List<string> GetCookedValueText(string Name)
        {
            return this.OfType<ValueAttribute>().Where(Va => Va.name == Name).First().cookedValueText;
        }
        internal void SetRawValue(string Name, int Value)
        {
            this.OfType<ValueAttribute>().Where(Va => Va.name == Name).First().rawValue = Value;
        }

        internal List<string> NonZeroAttributes
        {
            get
            {
                return this.OfType<ValueAttribute>().Where(Va => Va.rawValue > 0).Select(Va => Va.name).ToList();
            }
        }

        // Indexer
        internal Attribute this[string Name]
        {
            get
            {
                Attribute Att = this.Where(El => El.name == Name).First();
                return Att;
            }
        }

        public static bool operator== (Entity Left, Entity Right)
        {
            if (Left.Count != Right.Count)
            {
                return false;
            }
            // disjoint set = Except()
            var Disjoint = Left.Select(Aa => Aa.name).Except(Right.Select(Aa => Aa.name));
            if (Disjoint.Count() > 0)
            {
                return false;
            }
            foreach (ValueAttribute Va in Left.OfType<ValueAttribute>())
            {
                if ( Va.rawValue != Right.GetRawValue(Va.name))
                {
                    return false;
                }
            }
            return true;
        }
        public static bool operator !=(Entity Left, Entity Right)
        {
            return (!(Left == Right));
        }
    }
}
