using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace EPPlayer
{
    class UnitTests
    {
        private UnitTests()
        {
        }
        public static void RunUnitTests()
        {
            List<string> ExpectedColors = new List<string> { "Stat", "Aptitude", "Skill" };
            EPCharacter c;
            Int32 v;
            
            c = new EPCharacter();
            Debug.Assert(c["CP"] == 1000);
            Debug.Assert(c.Colors.Count() > 0);
            foreach (string s in ExpectedColors)
            {
                Debug.Assert(c.Colors.Contains(s));
            }
            Debug.Assert(c.ValueAttributesByColor("Aptitude").Count == 7);
            Debug.Assert(c["Coordination"] == 15);
            Debug.Assert(c["Beam Weapons"] == 15);
            c.SetRawValue("Beam Weapons", 42);
            Debug.Assert(c["Beam Weapons"] == 15 + 42);

            c.AttachAttribute("Background", "Isolate");
            // this gives 40 cp bonus (plus 1000 starting CP)
            Debug.Assert(c["CP"] == 1040);
            c.DetachAttribute("Background", "Isolate");
            Debug.Assert(c["CP"] == 1000);

            Debug.Assert(c["Pilot: Groundcraft"] == 15);
            c.AttachAttribute("Background", "Lunar Colonist");
            Debug.Assert(c["Pilot: Groundcraft"] == 25);
            c.DetachAttribute("Background", "Lunar Colonist");
            Debug.Assert(c["Pilot: Groundcraft"] == 15);

            Debug.Assert(c["Somatics"] == 15);
            c.SetRawValue("Somatics", 42);
            Debug.Assert(c["Somatics"] == 42);
            Debug.Assert(c["Durability"] == 0);
            c.AttachAttribute("Morph", "Fury");
            Debug.Assert(c["Durability"] == 50);
            Debug.Assert(c["Wound Threshold"] == 10);
            // now capped by the Fury morph
            Debug.Assert((v = c["Somatics"]) == 31);
            c.DetachAttribute("Morph", "Fury");
            Debug.Assert(c["Somatics"] == 42);
            c.SetRawValue("Somatics", 15);
            Debug.Assert(c["Somatics"] == 15);

            foreach (Background bg in c.Resources.Backgrounds)
            {
                c.AttachAttribute("Background", bg.Name);
                c.DetachAttribute("Background", bg.Name);
            }
            foreach (Morph m in c.Resources.Morphs)
            {
                c.AttachAttribute("Morph", m.Name);
                c.DetachAttribute("Morph", m.Name);
            }
        }
    }
}