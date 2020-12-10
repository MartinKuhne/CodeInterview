using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;

// Note: The Assert methods are not available for Windows Store apps. Dang. Have to look at debug output.

namespace EPPlayer
{
    class UnitTests
    {
        private UnitTests()
        {
        }

        private static bool AssertValue(EPCharacter Character, string AttributeName, int ExpectedValue, string text = null)
        {
            int value = Character.GetCookedValue(AttributeName);
            ValueAttribute Att = Character.OfType<ValueAttribute>().Where(Va => Va.name == AttributeName).FirstOrDefault();
            Debug.Assert(value == ExpectedValue, text);
            if (value != ExpectedValue)
            {
                List<string> History = Character.GetCookedValueText(AttributeName);
                return false;
            }
            return true;
        }

        public static async void RunUnitTests()
        {
            List<string> ExpectedColors = new List<string> { "Stat", "Aptitude", "Skill" };
            EPCharacter Blank = new EPCharacter();
            EPCharacter c;
            Int32 Value;

            Debug.WriteLine("Unit tests start.");
            c = new EPCharacter();

            await PersistentModel.WriteObject(c, "test.xml");
            EPCharacter c21 = await PersistentModel.ReadObject("test.xml");
            Debug.Assert(c21 == c, "serialize empty character");

            c.DeprecatedAttachAttribute("Morph", "Fury");
            await PersistentModel.WriteObject(c, "test.xml");
            EPCharacter c2 = await PersistentModel.ReadObject("test.xml");
            
            c = new EPCharacter();
            AssertValue(c,"CP", 1000);
            
            AssertValue(c, "Coordination", 15, "4");
            AssertValue(c, "Beam Weapons", 15, "5");

            Debug.Assert(c == Blank);
            c.SetRawValue("Beam Weapons", 42);
            Debug.Assert(c != Blank);
            AssertValue(c, "Beam Weapons", 15 + 42, "6");

            c.DeprecatedAttachAttribute("Background", "Isolate");
            // this gives 40 cp bonus (plus 1000 starting CP)
            AssertValue(c, "CP", 1040);
            //Log = c.DebugCookedValue("CP");
            c.DeprecatedDetachAttribute("Background", "Isolate");
            AssertValue(c, "CP", 1000);

            c.DeprecatedAttachAttribute("Background", "Re-Instantiated");
            AssertValue(c, "Moxie", 3);
            //Log = c.DebugCookedValue("Moxie");
            c.DeprecatedDetachAttribute("Background", "Re-Instantiated");
            AssertValue(c, "Moxie", 1);

            AssertValue(c, "Pilot: Groundcraft", 15);
            c.DeprecatedAttachAttribute("Background", "Lunar Colonist");
            AssertValue(c, "Pilot: Groundcraft", 15 + 10);

            c.DeprecatedDetachAttribute("Background", "Lunar Colonist");
            AssertValue(c, "Pilot: Groundcraft",15);

            AssertValue(c, "Somatics", 15);
            c.SetRawValue("Somatics", 42);
            AssertValue(c, "Somatics", 42);
            AssertValue(c, "Durability", 0);
            int PreCost = c.CPCost;
            c.DeprecatedAttachAttribute("Morph", "Fury");
            int PostCost = c.CPCost;
            Debug.Assert(PreCost + 75 == PostCost, (PostCost - PreCost).ToString());
            AssertValue(c, "Durability",50);
            AssertValue(c, "Wound Threshold",10, c["Wound Threshold"].ToString());
            // now capped by the Fury morph
            AssertValue(c, "Somatics",30);
            c.DeprecatedDetachAttribute("Morph", "Fury");
            //Log = c.DebugCookedValue("Somatics");
            AssertValue(c, "Somatics",42);
            c.SetRawValue("Somatics", 15);
            //Log = c.DebugCookedValue("Somatics");
            AssertValue(c, "Somatics",15);

            c = new EPCharacter();
            c.DeprecatedAttachAttribute("Trait", "Animal Empathy");
            AssertValue(c, "Animal Handling", 25);

            c = new EPCharacter();
            c.DeprecatedAttachAttribute("Gear", "Vacsuit (Light)");
            Value = c.CreditCost;
            Debug.Assert(Value == 250, Value.ToString());
            AssertValue(c, "Armor (Kinetic)", 5);
            c.DeprecatedAttachAttribute("Gear", "Ablative Patches");
            AssertValue(c, "Armor (Kinetic)", 7);
            c.DeprecatedDetachAttribute("Gear", "Ablative Patches");
            AssertValue(c, "Armor (Kinetic)", 5);

            Stopwatch Sw = Stopwatch.StartNew();
            foreach (Background bg in c.Resources.Backgrounds)
            {
                c = new EPCharacter();
                c.DeprecatedAttachAttribute("Background", bg.name);
                c.DeprecatedDetachAttribute("Background", bg.name);
                Debug.Assert(c == Blank, "Backgrounds");
            }
            Debug.WriteLine(Sw.ElapsedMilliseconds);
            foreach (Morph m in c.Resources.Morphs)
            {
                c = new EPCharacter();
                c.DeprecatedAttachAttribute("Morph", m.name);
                c.DeprecatedDetachAttribute("Morph", m.name);
                Debug.Assert(c == Blank, "Morphs");
            }
            Debug.WriteLine(Sw.ElapsedMilliseconds);
            foreach (Gear g in c.Resources.Gear)
            {
                c = new EPCharacter();
                c.DeprecatedAttachAttribute("Gear", g.name);
                c.DeprecatedDetachAttribute("Gear", g.name);
                if (c != Blank)
                {
                    Debug.WriteLine("Fixme");
                }
                Debug.Assert(c == Blank, "Gear");
            }
            Debug.WriteLine(Sw.ElapsedMilliseconds);
            foreach (Trait t in c.Resources.Traits)
            {
                c = new EPCharacter();
                c.DeprecatedAttachAttribute("Trait", t.name);
                Debug.Assert(c.BilledCPCost == t.cpCost, c.BilledCPCost.ToString());
                c.DeprecatedDetachAttribute("Trait", t.name);
                Debug.Assert(c == Blank);
                Debug.Assert(c.BilledCPCost == 0, c.BilledCPCost.ToString());
            }
            Debug.WriteLine(Sw.ElapsedMilliseconds);
            foreach (Faction f in c.Resources.Factions)
            {
                c = new EPCharacter();
                c.DeprecatedAttachAttribute("Faction", f.name);
                c.DeprecatedDetachAttribute("Faction", f.name);
                Debug.Assert(c == Blank, "Factions");
            }
            Debug.WriteLine(Sw.ElapsedMilliseconds);


            c = new EPCharacter();
            c.DeprecatedAttachAttribute("Background", c.Resources.Backgrounds[0].name);
            c.DeprecatedAttachAttribute("Morph", c.Resources.Morphs[0].name);
            c.DeprecatedAttachAttribute("Faction", c.Resources.Factions[0].name);
            foreach (Gear g in c.Resources.Gear)
            {
                c.DeprecatedAttachAttribute("Gear", g.name);
            }
            foreach (Trait t in c.Resources.Traits)
            {
                c.DeprecatedAttachAttribute("Trait", t.name);
            }
            foreach (Aptitude a in c.Resources.Aptitudes)
            {
                c.SetRawValue(a.name, 5);
            }
            foreach (Skill s in c.Resources.Skills)
            {
                c.SetRawValue(s.name, 10);
            }
            Debug.WriteLine(Sw.ElapsedMilliseconds);
            await PersistentModel.WriteObject(c, "test.xml");
            EPCharacter c3 = await PersistentModel.ReadObject("test.xml");
            Debug.WriteLine(Sw.ElapsedMilliseconds);
            if (c3 != c)
            {
                Debug.Assert(false, "Serialziation test found differences");
            }
            
            
            
            Debug.WriteLine("Unit tests done.");
        }
    }
}