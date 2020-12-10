/* Design goals for the second version of the Model
 *  o Better integration with data binding
 *    o Designed for properties
 *    o Single observable collection to enable consistent change notifications
 *  o More code sharing (through using Interfaces instead of subclasses)
 *  o Integrate the handling of potential attributes (Model v1 does not contain those)
 *  o better Performance (v1 is a bit sluggish on Surface)
 *  
 */

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FutureVersion
{
    internal interface IAttachable
    {
        void OnAttach(Entity Entity);
        void OnDetach(Entity Entity);
    }

    internal interface IPrice
    {
        int xpCost
        {
            get;
        }
        int currencyCost
        {
            get;
        }
    }

    internal interface IFilter<T>
    {
        T FilteredValue(T PreviousValue);
    }

    abstract class Attribute
    {
        internal readonly string Name;
        internal readonly string Color;
        internal readonly string Description;

        /// <summary>
        /// Remember who created me
        /// </summary>
        protected Attribute Owner = null;
        /// <summary>
        /// Things I have created
        /// </summary>
        protected List<Attribute> Children = new List<Attribute>();
        /// <summary>
        /// Things I am dependent on (I need to know if any of these things change)
        /// </summary>
        protected List<Attribute> DependentOn = new List<Attribute>();
        /// <summary>
        /// Things that change me
        /// </summary>
        protected List<Attribute> Filters = new List<Attribute>();

        internal Attribute(string Name, string Color, string Description = null)
        {
            this.Name = Name;
            this.Color = Color;
            this.Description = Description;
        }

        internal static void AddFilter(Attribute Target, Attribute Filter)
        {
            Target.Filters.Add(Filter);
        }

        internal static void RemoveFilter(Attribute Target, Attribute Filter)
        {
            Target.Filters.Remove(Filter);
        }

        internal static void AddVector(Attribute WhoIAm, Attribute WhoIAmDependentOn)
        {
            WhoIAm.DependentOn.Add(WhoIAmDependentOn);
        }

        internal static void RemoveVector(Attribute WhoIAm, Attribute WhoIAmNoLongerDependentOn)
        {
            WhoIAm.DependentOn.Remove(WhoIAmNoLongerDependentOn);
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

    class ValueAttribute<T> : Attribute
    {
        private T Value;

        internal ValueAttribute(string Name, string Color, T Value, string Description = null)
            : base(Name, Color, Description)
        {
            this.Value = Value;
        }

        public T rawValue
        {
            get { return this.Value; }
            set
            {
                this.Value = value;
                NotifyPropertyChanged("rawValue");
                NotifyPropertyChanged("cookedValue");
            }
        }
        public T cookedValue
        {
            get
            {
                T Result = this.Value;
                foreach (Attribute Filter in Filters)
                {
                    Result = (Filter as IFilter<T>).FilteredValue(Result);
                }
                return Result;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
    /// <summary>
    /// Here, we are trying to solve the problem of
    /// "What are my data structures, which of them should be public, and how does one interact with the Model?"
    /// </summary>
    abstract class Entity : ObservableCollection<Attribute>
    {
    }
}