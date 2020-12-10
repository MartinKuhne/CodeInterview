using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.System.Threading;

using Windows.Graphics.Printing;
using Windows.UI.Xaml.Printing;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace EPPlayer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CreateCharacter : Page
    {
        /// <summary>
        /// This function needs to be called every time App.Character gets set to a new object.
        /// </summary>
        public void Reinit()
        {
            var Enum = App.Character.Where(El => El.GetType() == typeof(Stat) || El.GetType() == typeof(Aptitude));
            this.gvStats.ItemsSource = Enum;
            this.gvRep.ItemsSource = this.gvRepEdit.ItemsSource =
                App.Character.OfType<Reputation>();
            // funky: OrderBy was resulting in an empty set here. weird.
            this.gvAttachables.ItemsSource = App.Character.NonValueAttributes;
            this.fvCreate.SelectedIndex = 0;

            /*
            this.gvSkills1.ItemsSource = App.Character.CombatSkills;
            this.gvSkills2.ItemsSource = App.Character.ActionSkills;
            this.gvSkills3.ItemsSource = App.Character.MovementSkills;
            this.gvSkills4.ItemsSource = App.Character.InterpersonalSkills;
             */

            this.BilledCP.DataContext = this.BilledActiveSkills.DataContext = this.BilledKnowlegdeSkills.DataContext = App.Character;

            // force display of first page
            Faction f = App.Character.OfType<Faction>().FirstOrDefault();
            if (f != null)
            {
                lvSelectFaction.SelectedItem = f;
            }
        }

        public CreateCharacter()
        {
            this.InitializeComponent();

#if DEBUG
            EPPlayer.UnitTests.RunUnitTests();
#endif
            ResourceManager RM = ResourceManager.GetInstance();
            this.Reinit();

            this.gvTraits.ItemsSource = App.Character.Resources.Traits.Where(Tr => Tr.isPositive == true);
            this.gvTraits2.ItemsSource = App.Character.Resources.Traits.Where(Tr => Tr.isPositive == false);
            this.gvArmor.ItemsSource = App.Character.Resources.Armors;
            this.gvArmorMod.ItemsSource = App.Character.Resources.Gear.Where(G => G.type == "ArmorMod" || G.type == "ArmorAccessory");

            // need an ordered query for the grouping to work (!!)
            var Enum = App.Character.OfType<Skill>().GroupBy(Sk => Sk.group).OrderBy(Sk => Sk.Key);
            this.allSkills.Source = Enum; // this also sets the Zoomed In data source
            this.gvFieldSkillsZoomedOut.ItemsSource = allSkills.View.CollectionGroups;

            this.gvItems1.ItemsSource = App.Character.Resources.Gear.Where(G => G.type == "Augmentation");
            this.gvItems2.ItemsSource = App.Character.Resources.Gear.Where(G => G.type == "Gear");
            this.lvSelectMorph.ItemsSource = App.Character.Resources.Morphs;
            this.lvSelectFaction.ItemsSource = App.Character.Resources.Factions;
            this.lvSelectBackground.ItemsSource = App.Character.Resources.Backgrounds;

            this.lvSelectAptitudes.ItemsSource = App.Character.OfType<Aptitude>();
        }

        private void CommonNonValueAttributeChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (object o in e.AddedItems)
            {
                if (App.Character.Contains(o) == false)
                {
                    App.Character.Add(o as Attribute);
                }
            }
            foreach (object o in e.RemovedItems)
            {
                App.Character.Remove(o as Attribute);
            }
        }
        
        private void FactionChanged(object sender, SelectionChangedEventArgs e)
        {
            CommonNonValueAttributeChanged(sender, e);
            if (e.RemovedItems.Count() > 0 && e.AddedItems.Count == 0)
            {
                this.spFaction.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.spFaction.Visibility = Visibility.Visible;
            }
        }

        private void BackgroundChanged(object sender, SelectionChangedEventArgs e)
        {
            CommonNonValueAttributeChanged(sender, e);

            if (e.RemovedItems.Count() > 0 && e.AddedItems.Count == 0)
            {
                this.spBackground.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.spBackground.Visibility = Visibility.Visible;
            }
        }
        private void MorphChanged(object sender, SelectionChangedEventArgs e)
        {
            CommonNonValueAttributeChanged(sender, e);

            if (e.RemovedItems.Count() > 0 && e.AddedItems.Count == 0)
            {
                this.spMorph.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.spMorph.Visibility = Visibility.Visible;
            }
        }
        private void AptitdudeChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count() > 0 && e.AddedItems.Count == 0)
            {
                spAptitudeDetail.Visibility = Visibility.Collapsed;
            }
            else
            {
                spAptitudeDetail.Visibility = Visibility.Visible;
            }
            if (e.AddedItems.Count > 0)
            {
                Aptitude Apt = e.AddedItems.First() as Aptitude;
                IEnumerable<string> Skills = App.Character.Resources.Skills.Where(Sk => Sk.governingAptitude == Apt.name && (Sk.isFieldSkill == false)).Select(Sk => Sk.name);
                IEnumerable<string> FieldSkills = App.Character.Resources.Skills.Where(Sk => Sk.governingAptitude == Apt.name && Sk.isFieldSkill == false).Select(Sk => Sk.name.Substring(0, Sk.name.IndexOf(':'))).Distinct();
                tbSkillsForAptitude.Text = "Skills: " + string.Join(", ", Skills.Concat(FieldSkills));
            }
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            RegisterForPrinting();
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            UnregisterForPrinting();
        }

        private void fvCreate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object o = e.AddedItems.First();
            if (o != null && o.GetType() == typeof(Grid))
            {
                Grid SelectedGrid = o as Grid;
                switch (SelectedGrid.Name) 
                {
                    case "grPlay":
                        // need an ordered query for the grouping to work (!!)
                        var Enum = App.Character.OfType<Skill>().Where(Sk => Sk.allowsDefaulting || Sk.rawValue > 0).GroupBy(Sk => Sk.group).OrderBy(Sk => Sk.Key);
                        this.playSkills.Source = Enum; // this also sets the Zoomed In data source
                        this.gvPlayZoomedOut.ItemsSource = playSkills.View.CollectionGroups;
                        break;
                    case "grFaction":
                        // note faction is the first page and this code can run before the child listview is initialized.
                        // hence the code below is duplicated in the page constructor
                        if (lvSelectFaction != null)
                        {
                            Faction f = App.Character.OfType<Faction>().FirstOrDefault();
                            if (f != null)
                            {
                                lvSelectFaction.SelectedItem = f;
                            }
                        }
                        break;
                    case "grBackground":
                        Background b = App.Character.OfType<Background>().FirstOrDefault();
                        if (b != null)
                        {
                            lvSelectBackground.SelectedItem = b;
                        }
                        break;
                    case "grMorph":
                        Morph m = App.Character.OfType<Morph>().FirstOrDefault();
                        if (m != null)
                        {
                            lvSelectMorph.SelectedItem = m;
                        }
                        break;
                    case "grTraits1":
                        // todo: get this code to work
                        IEnumerable<Trait> Traits = App.Character.OfType<Trait>().Where(Tr => Tr.isPositive == true);
                        this.gvTraits.SelectedItems.Add(Traits);
                        break;
                }
            }

        }

        private void MenuSelected(object sender, RoutedEventArgs e)
        {
            Button Selection = sender as Button;
            switch (Selection.Name)
            {
                case "bDelete":
                    App.Character = new EPCharacter();
                    Reinit();
                    break;
                case "bHelp":
                    Windows.System.Launcher.LaunchUriAsync(new Uri(@"http://epplayer.codeplex.com"));
                    break;
            }
        }

        #region Print support
        // useful sample at http://mtaulty.com/CommunityServer/blogs/mike_taultys_blog/archive/2012/10/25/windows-store-apps-and-xaml-based-printing-rough-notes.aspx
        const int PrintFontSize = 14;
        static SolidColorBrush PrintFontColor = new SolidColorBrush(Colors.Black);

        void RegisterForPrinting()
        {
            PrintManager manager = PrintManager.GetForCurrentView();
            manager.PrintTaskRequested += OnPrintTaskRequested;
        }
        void UnregisterForPrinting()
        {
            PrintManager printMan = PrintManager.GetForCurrentView();
            printMan.PrintTaskRequested -= OnPrintTaskRequested;
        }

        void OnPrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs args)
        {
            var deferral = args.Request.GetDeferral();

            PrintTask printTask = args.Request.CreatePrintTask(
            "EPPlayer printout",
            OnPrintTaskSourceRequestedHandler);

            printTask.Completed += OnPrintTaskCompleted;

            deferral.Complete();
        }

        void OnPrintTaskCompleted(PrintTask sender, PrintTaskCompletedEventArgs args)
        {
            this._pageSize = null;
            this._imageableRect = null;
            this._document = null;
            this._pages = null;
        }

        async void OnPrintTaskSourceRequestedHandler(PrintTaskSourceRequestedArgs args)
        {
            var deferral = args.GetDeferral();

            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                this._document = new PrintDocument();

                this._document.Paginate += OnPaginate;
                this._document.GetPreviewPage += OnGetPreviewPage;
                this._document.AddPages += OnAddPages;

                // Tell the caller about it.
                args.SetSource(this._document.DocumentSource);
            }
            );
            deferral.Complete();
        }

        void OnAddPages(object sender, AddPagesEventArgs e)
        {
            this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                this._document.AddPage(this._pages[1]);
                this._document.AddPagesComplete();
            }
            );
        }

        void PrintFillColumn(String ColumnHeader, Grid Layout, int Column, int Row, IEnumerable<Attribute> Attributes)
        {
            StackPanel sp = new StackPanel();
            Grid.SetColumn(sp, Column);
            Grid.SetRow(sp, Row);
            sp.Margin = new Thickness(8);
            Layout.Children.Add(sp);

            TextBlock HeaderText = new TextBlock()
            {
                Text = ColumnHeader,
                FontWeight = Windows.UI.Text.FontWeights.SemiBold,
                FontSize = PrintFontSize,
                Foreground = PrintFontColor
            };
            sp.Children.Add(HeaderText);

            foreach (Attribute Att in Attributes)
            {
                TextBlock textBlock = new TextBlock()
                {
                    FontSize = PrintFontSize,
                    Foreground = PrintFontColor
                };
                if (Att is ValueAttribute)
                {
                    textBlock.Text = string.Format("{0}: {1}", Att.name, ((ValueAttribute) Att).cookedValue);
                }
                else
                {
                    textBlock.Text = string.Format("{0}", Att.name);
                }
                sp.Children.Add(textBlock);
            }
        }

        void OnGetPreviewPage(object sender, GetPreviewPageEventArgs e)
        {
            SolidColorBrush FontColor = new SolidColorBrush(Colors.Black);
            EPCharacter Character = App.Character;

            this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                // NB: assuming it's ok to keep all these pages in
                // memory. might not be the right thing to do
                // of course.
                if (this._pages == null)
                {
                    this._pages = new Dictionary<int, UIElement>();
                }
                if (!this._pages.ContainsKey(e.PageNumber))
                {
                    // TBD: Unsure about DPI here.
                    Canvas canvas = new Canvas();
                    canvas.Width = this._pageSize.Value.Width;
                    canvas.Height = this._pageSize.Value.Height;

                    Grid Layout = new Grid();
                    Layout.ColumnDefinitions.Add(new ColumnDefinition());
                    Layout.ColumnDefinitions.Add(new ColumnDefinition());
                    Layout.ColumnDefinitions.Add(new ColumnDefinition());
                    Layout.RowDefinitions.Add(new RowDefinition());
                    Layout.RowDefinitions.Add(new RowDefinition());
                    Layout.RowDefinitions.Add(new RowDefinition());
                    Layout.Margin = new Thickness(8);

                    PrintFillColumn("Aptitudes", Layout, 0, 0, Character.OfType<Aptitude>());
                    PrintFillColumn("Stats", Layout, 1, 0, Character.OfType<Stat>());
                    PrintFillColumn("Background and Traits", Layout, 2, 0, Character.Where(Att => 
                        Att.GetType() == typeof(Faction) ||
                        Att.GetType() == typeof(Background) ||
                        Att.GetType() == typeof(Morph) ||
                        Att.GetType() == typeof(Trait) ||
                        Att.GetType() == typeof(Gear)
                    ));
                    PrintFillColumn("Combat", Layout, 0, 1, Character.CombatSkills);
                    PrintFillColumn("Action", Layout, 1, 1, Character.ActionSkills);
                    PrintFillColumn("Social", Layout, 2, 1, Character.InterpersonalSkills);
                    PrintFillColumn("Moving", Layout, 0, 2, Character.MovementSkills);
                    PrintFillColumn("Field skills", Layout, 1, 2, Character.OfType<Skill>().Where(Sk => Sk.isFieldSkill == false && Sk.rawValue > 0));
                    PrintFillColumn("Reputation", Layout, 2, 2, Character.OfType<Reputation>());
                    
                    Canvas.SetLeft(Layout, this._imageableRect.Value.Left + 8);
                    Canvas.SetTop(Layout, this._imageableRect.Value.Top + 8);
                    canvas.Children.Add(Layout);

                    this._pages[e.PageNumber] = canvas;
                }
                this._document.SetPreviewPage(e.PageNumber,
                this._pages[e.PageNumber]);
            }
            );
        }
    
        void OnPaginate(object sender, PaginateEventArgs e)
        {
            this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                // I have one page and that's *FINAL* !
                this.GetPageSize(e);

                this._document.SetPreviewPageCount(1, PreviewPageCountType.Final);
            }
            );
        }

        void GetPageSize(PaginateEventArgs e)
        {
            if (this._pageSize == null)
            {
                PrintPageDescription description = e.PrintTaskOptions.GetPageDescription(
                    (uint)e.CurrentPreviewPageNumber);

                this._pageSize = description.PageSize;
                this._imageableRect = description.ImageableRect;
            }
        }
        Size? _pageSize;
        Rect? _imageableRect;
        Dictionary<int, UIElement> _pages;
        PrintDocument _document;
    }
    #endregion

    public class ConvertColorToBrush : IValueConverter
    {
        private static Dictionary<Type, Windows.UI.Color> MyColors = new Dictionary<Type, Windows.UI.Color>()
            {
                // left side
                { typeof(Aptitude),   Windows.UI.Colors.LightGoldenrodYellow },
                { typeof(Reputation), Windows.UI.Colors.LightBlue },
                { typeof(Stat),       Windows.UI.Colors.Ivory },
                { typeof(Skill),      Windows.UI.Colors.Ivory },
                // right side
                { typeof(Gear),       Windows.UI.Colors.LightBlue },
                { typeof(Morph),      Windows.UI.Colors.Lavender },
                { typeof(Background), Windows.UI.Colors.Ivory },
                { typeof(Faction),    Windows.UI.Colors.LightGoldenrodYellow },
                { typeof(Trait),      Windows.UI.Colors.IndianRed }
            };
        public object Convert(object Source, System.Type ConvertTo, object parameter, string language)
        {
            if (ConvertTo != typeof(Brush))
            {
                throw new ArgumentException("I only convert to brushes");
            }
            return new SolidColorBrush(MyColors[Source.GetType()]);
        }
        public object ConvertBack(object value, System.Type type, object parameter, string language)
        {
            throw new NotImplementedException(); //doing one-way binding so this is not required.
        }
    }

    class Dice
    {
        private static Random Rng = new Random();
        public static int SimpleSuccess()
        {
            return Rng.Next(1, 100);
        }
    }
#region UIDesignDemoItems
    public class DemoVA
    {
        public string name { get; set; }
        public string color { get; set; }
        public string description  { get; set; }
        public int rawValue { get; set; }
        public int cookedValue { get; set; }
        public DemoVA()
        {
            this.name = "Singing";
            this.rawValue = 30;
            this.cookedValue = 50;
            this.description = "The quick brown fox jumps over the lazy dog.";
        }
    }

    public class DemoValueAttribute
    {
        public string name { get; set; }
        public string color { get; set; }
        public string description { get { return "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua."; } }
        public string category { get; set; }
        public string governingAptitude { get { return "Coordination"; } }
        public bool allowsDefaulting {get; set;}
        public string group { get; set; }
        public int rawValue { get; set; }
        public int cookedValue { get { return rawValue + 30; } }
        public DemoValueAttribute(string name, string color, string category, string group, int value, bool allowsDefaulting)
        {
            this.name = name; this.color = color; this.category = category;
            this.group = group; this.rawValue = value; this.allowsDefaulting = allowsDefaulting;
        }
        public DemoValueAttribute()
        {
        }
    }
    public class DemoValueAttributes : ObservableCollection<DemoValueAttribute>
    {
        public DemoValueAttributes()
        {
            this.Add(new DemoValueAttribute("Beer",  "Beverage", "Active", "Essentials", 10, true));
            this.Add(new DemoValueAttribute("Water", "Beverage", "Active", "Essentials", 10, true));
        }
    }
    public class DemoNonValueAttribute
    {
        public string name { get; set; }
        public string description { get; set; }
//      public string color { get; set; }
        public DemoNonValueAttribute() { }
    }

#endregion UIDesignDemoItems
}