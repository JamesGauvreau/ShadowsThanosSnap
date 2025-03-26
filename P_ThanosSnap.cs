using Assets.code; // Make sure that this is always here. 
using System;
using System.Text;
using UnityEngine;

namespace ShadowsThanosSnap // Annotations from the DLL Modding Guide on SOFG Wiki. 
{
    public class P_ThanosSnap : Power // We are creating a new class, P_EyesInShadow, which is a subclass of the Power class. A class is a type of object that can store properties, or other data, and perform functions. "Public" means that this class can be accessed from other classes. Usually a class should be private, but in SOFG a private class will be reset every time that the game is reloaded, and we don't usually want that. 
    {
        public P_ThanosSnap(Map map) // This is a constructor, which means that this function is called when a new object (or instance) of this class is created. 
        // The "Map" in the parentheses is a parameter, which is a variable that is passed to the function when it is called. "Map map" refers to a Map (UP) called map (lc).
            : base(map)
        // ": base" means that the constructor will do things in the parent class (Power) before doing its own thing. Like other constructors, it needs a Map in order to do anything. If the first parens had been "(Map dolittle)" then this would have read ": base(dolittle)" instead
        {   // Because the curly brackets are empty, this constructor only does things that are done in the parent class (which is Power). 
        }

        public /* Can be used by things outside P_EyesInShadows or Power */ override /* Replaces the "return 'Default Name' " function from the Power Class */ string /* function will return a string */ getName() // Like JS, functions need parentheses even if there's nothing in them.
        {
            return "Thanos Snap"; // Make sure to always have semicolons at the end of lines in C#.
        }

        public override string getDesc()
        {
            return "Every named character not under your control has a 50% chance of dying instantly. Characters under your control will survive, but will gain ten <b>menace</b>.";
        }

        public override Sprite getIconFore()
        {
            return EventManager.getImg("thanos.power_snap.png") // This tells the game to look in the thanos mod for the image. Because EventManager.getImg() is a static function, it can be used without having a specific EventManager to work with. Consider using Static functions and properties when they aren't related to any specific instance of the class, but remember that they won't have their status saved and loaded with the game even if they are public.
            /* return map.world.iconStore.enshadow */; // "Eyes in the Shadows" uses the same icon as "Enshadow."
        }

        public override bool /* short for Boolean */ validTarget(Unit unit)
        {
            return false;
        }

        // Other variables —
        // int: "integer", from -2 billion to 2 billion, but no fractions. Character's gold is an int.
        // double: Store up to 15-17 digits, in fractions or whole numbers. A settlement's shadow is a double between 0 and 1, and a modifier's charge is a double between 0 and 300. 
        // bool: true or false.
        // string: text.
        // float: as double, but shorter and smaller. Not used much in SoFG. 

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
            return 1;
        }

        public override void /* "void" means there's no return type and it isn't necessary to end every chain of the function with "return." It does what it does without passing anything back to whatever called the function. */ cast(Location loc)
        {
            base.cast(loc); // This will run Power's cast(Location loc) function, playing a sound effect and spending the power cost.
            foreach (Person person in map.persons) // "persons" is a data structure called a List, which contains a number of whatever it's a list of. In this case, it's a list of Person objects, every Person that is alive or dead. "Person person" is a variable that will be used to refer to each Person in the list, one at a time. This runs the loop for each item in the list, once. 
                // On the first runthrough, "person" will be map.persons[0], on the second runthrough it will be map.persons[1], and so on until it reaches the end of the list.
            {
                if (person.unit == null || person.unit.isCommandable() == false) // An "if" statement. If the null check isn't present, the game might crash once it tries to access a ruler or other person without a unit connected to them, because person.unit.isCommandable() can't be true or false if there's no person.unit. 
                    // "Don’t check an instance's property or call one of its functions until you’re absolutely certain the instance won’t be null."
                    // C# seems to use the same operators as JS, but there are a few exceptions.
                    // – "==" is used for comparison, "=" is used for assignment, or setting the thing on the left to the thing on the right.
                    // "is" resolves to true if the object on the left is an instance of the class on the right, e.g. "if (person.unit is UM)" resolves to true if person.-unit is a pure UM, a UM_HumanArmy, a UM_RavenousDead, etc. 
                    // Remember += and -=. 
                    // ++ is shorthand for +=1. Mut. mut. --. 
                {
                    if (person.isDead == false && Eleven.random.Next(2) == 0) // "Eleven" is a class comprised entirely of static functions and properties. It stores the random number generator for the current map's seed. "Random" is a class that takes a seed and uses it to spit out various random numbers. Using random.Next is the simplest way to generate a random number. Assign it a number in order to determine how many possible outcomes there are, e.g. random.Next(3) can output 0, 1, or 2; random.Next(2) can output 0 or 1.
                    {
                        person.die("Killed by Thanos Snap", false /* if this were true, you'd get a pop-up about the dead */ , null /* this is the killer, which you can leave as "null" because neither a Person nor a Unit has done the killing */ );
                    }
                }
                else // This only runs if the previous statement was false.
                {
                    person.unit.addMenace(10); // No need to null-check because the if statement above already did that; else could not have run if person.unit was null.
                    // We use addMenace instead of changing person.unit.inner_menace so that minimum menace can be re-caculated. 
                }
            }

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
            return "Goodbye world.";
        }

        public override string getRestrictionText()
        {
            return "Can target any location.";
        }
    }
}
