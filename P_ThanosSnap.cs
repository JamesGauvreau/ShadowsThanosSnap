using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShadowsThanosSnap // Annotations from the DLL Modding Guide on SOFG Wiki. 
{
    public class P_EyesInShadow : Power // We are creating a new class, P_EyesInShadow, which is a subclass of the Power class. A class is a type of object that can store properties, or other data, and perform functions. "Public" means that this class can be accessed from other classes. Usually a class should be private, but in SOFG a private class will be reset every time that the game is reloaded, and we don't usually want that. 
    {
        public P_EyesInShadow(Map map) // This is a constructor, which means that this function is called when a new object (or instance) of this class is created. 
        // The "Map" in the parentheses is a parameter, which is a variable that is passed to the function when it is called. "Map map" refers to a Map (UP) called map (lc).
            : base(map)
        // ": base" means that the constructor will do things in the parent class (Power) before doing its own thing. Like other constructors, it needs a Map in order to do anything. If the first parens had been "(Map dolittle)" then this would have read ": base(dolittle)" instead
        {   // Because the curly brackets are empty, this constructor only does things that are done in the parent class (which is Power). 
        }

        public override string getName()
        {
            return "Eyes in the Shadows";
        }

        public override string getDesc()
        {
            return "Boosts the infiltration in selected location";
        }

        public override Sprite getIconFore()
        {
            return map.world.iconStore.enshadow;
        }

        public override bool validTarget(Unit unit)
        {
            return false;
        }

        public override bool validTarget(Location loc)
        {
            if (loc.settlement == null || loc.settlement.infiltration != 0.0 || loc.settlement.isInfiltrated)
            {
                return false;
            }

            foreach (Subsettlement sub in loc.settlement.subs)
            {
                if (sub.canBeInfiltrated() && !sub.infiltrated)
                {
                    return true;
                }
            }

            return false;
        }

        public override int getCost()
        {
            return 2;
        }

        public override void cast(Location loc)
        {
            base.cast(loc);
            Subsettlement subsettlement = null;
            foreach (Subsettlement sub in loc.settlement.subs)
            {
                if (sub.canBeInfiltrated() && !sub.infiltrated)
                {
                    subsettlement = sub;
                }
            }

            if (subsettlement != null)
            {
                subsettlement.infiltrated = true;
                if (subsettlement is Sub_Sewers && map.overmind.god is God_Tutorial2 && map.tutorialManager.state1 == ManagerTutorial.STATE_B1_INFILTRATE_SEWERS)
                {
                    map.tutorialManager.state1 = ManagerTutorial.STATE_B1_START_PLAGUE;
                }
            }
        }

        public override string getFlavour()
        {
            return "They're always watching us. Always.";
        }

        public override string getRestrictionText()
        {
            return "Must be cast on a settlement with no infiltration";
        }
    }
}
