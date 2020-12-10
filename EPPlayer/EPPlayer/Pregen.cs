using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Threading.Tasks;
using System.Runtime.Serialization;

// This module covers domain specific stuff for EP

namespace EPPlayer
{
    class Pregen
    {
        static Dictionary<string, UInt16> Pregen1 = new Dictionary<string, UInt16>
        {
            {"Academics: Astronomy", 25 },
            {"Academics: Engineering",  20 },
            {"Art: Writing",  15  },
            {"Beam Weapons",  30  },
            {"Blades",  25  },
            {"Climbing",  25  },
            {"Demolitions",  20  },
            {"Disguise",  20  },
            {"Fray",  35  },
            {"Free Fall",  45  },
            {"Freerunning",  25  },
            {"Hardware: Aerospace", 35  },
            {"Hardware: Robotics",  35 },
            {"Infiltration",  10  },
//            {"Interest: Brinker Groups",  45  },
//            {"Interests: Esoteric Muslim Traditions",  40  },
//            {"Interest: Habitat Infrastructure",  35  },
//            {"Interest: Outer System Habitats",  35  },
//            {"Interest: Scum Black Markets",  30  },
            {"Interfacing",  10  },
            {"Kinesics",  30  },
            {"Kinetic Weapons",  35  },
            {"Language: Arabic",  70  },
            {"Language: English",  20  },
            {"Language: Spanish",  15  },
            {"Medicine: Paramedic",  30  },
            {"Navigation",  40  },
            {"Perception",  35  },
            {"Persuasion",  15  },
            {"Pilot: Spacecraft",  35  },
            {"Profession: Security Ops",  45  },
            {"Protocol",  15  },
            {"Research",  20  },
            {"Scrounging",  30  },
            {"Unarmed Combat",  45 },
            {"@-rep",  40 },
            {"c-rep",  40 },
            {"i-rep",  20 }
        };
        public static void ApplyPregen(string Choice, EPCharacter c)
        {
            foreach (KeyValuePair<string, UInt16> kvp in Pregen.Pregen1)
            {
                c.SetRawValue(kvp.Key, kvp.Value);
            }
            c.Add(c.Resources.Backgrounds.Find(El => El.name == "Original Space Colonist"));
            c.Add(c.Resources.Morphs.Find(El => El.name == "Bouncer"));
            c.Add(c.Resources.Factions.Find(El => El.name == "Brinker"));
        }
    }
}