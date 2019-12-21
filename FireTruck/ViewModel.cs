using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace FireTruck
{
    class ViewModel
    {
        public BindingList<string> Input { get; set; } = new BindingList<string>();
        public BindingList<string> Output { get; set; } = new BindingList<string>();
        public BindingList<string> InputStrings { get; set; } = new BindingList<string>();
        public List<string[]> InputArrays { get; set; } = new List<string[]>();
        public int FileIndex { get; set; }
        public int InputIndex { get; set; }

        private Street targetStreet;
        readonly List<Street> streets = new List<Street>();
        readonly List<List<Street>> routes = new List<List<Street>>();
        private int caseCount;

        const string INPUT_DIR = "InputFiles\\";

        public ViewModel()
        {
            foreach (string file in Directory.GetFiles(INPUT_DIR))
            {
                InputArrays.Add(File.ReadAllLines(file));
                InputStrings.Add(TrimFilename(file));
            }
        }

        //Converts InputFiles\$Name.txt to $Name
        public string TrimFilename(string name)
        {
            name = name.Substring(0, name.IndexOf('.'));
            return name.Substring(name.LastIndexOf('\\') + 1);
        }

        public void AddFile(string file)
        {
            InputArrays.Add(File.ReadAllLines(file));
            ParseInput(File.ReadAllLines(file));

            //Checks if filename is taken
            int i = 0;
            string name = TrimFilename(file), baseName = name;
            while (InputStrings.Contains(name))
            {
                i++;
                name = baseName + "(" + i + ")";
            }
            //Creates text file
            File.WriteAllLines(INPUT_DIR + name + ".txt", File.ReadAllLines(file));

            InputStrings.Add(name);
        }

        public void ParseInput(string[] input)
        {
            Input.Clear();
            Output.Clear();

            //Resets for next case
            routes.Clear();
            streets.Clear();
            targetStreet = null;
            caseCount = 0;

            //Makes sure first street is 1
            streets.Add(new Street("1"));

            try
            {
                foreach (string value in input)
                {
                    Input.Add(value);

                    if (value == "0 0")
                    {
                        FindRoutes();
                    }
                    else if (value.Length >= 3)
                    {
                        //Creates list of streets, 
                        //each with an list of connected streets
                        string[] values = value.Split(null);

                        Street a = new Street(values[0]);
                        Street b = new Street(values[1]);

                        if (!streets.Contains(a))
                            streets.Add(a);
                        else
                            a = streets[streets.IndexOf(a)];

                        if (!streets.Contains(b))
                            streets.Add(b);
                        else
                            b = streets[streets.IndexOf(b)];

                        a.NewConnection(b);
                        b.NewConnection(a);
                    }
                    else if (value.Length > 0)
                    {
                        //Sets target street
                        targetStreet = new Street(value);
                        streets.Add(targetStreet);
                    }
                }
            }
            catch (ArgumentException e)
            {
                Output.Add(e.Message);
            }
        }

        void FindRoutes()
        {
            caseCount++;
            Output.Add("\nCASE " + caseCount + ":");

            foreach (Street street in streets)
            {
                street.ConnectedStreets = street.ConnectedStreets.OrderBy(Street => Street.Num).ToList();
            }

            //This is where everthing happens
            routes.Add(new List<Street>());
            routes[0].Add(streets[0]);
            NextStreet(streets[0]);

            //Theres is one empty list 
            routes.Remove(routes.Last());
            PrintOutput();

            //Clear For Next
            routes.Clear();
            streets.Clear();
            targetStreet = null;
            streets.Add(new Street("1"));
        }

        void NextStreet(Street street)
        {
            if (street != targetStreet)
            {
                foreach (Street next in street.ConnectedStreets)
                {
                    //Check connected street to see if its repeated
                    if (!routes.Last().Contains(next))
                    {
                        routes.Last().Add(next);
                        NextStreet(next);
                    }
                }
                //Dead end
                routes.Last().Remove(street);
            }
            else
            {
                //Completes the current route
                routes.Add(new List<Street>(routes.Last()));
                routes.Last().Remove(street);
            }
        }

        void PrintOutput()
        {
            foreach (List<Street> path in routes)
            {
                string pathString = "";

                foreach (Street node in path)
                {
                    pathString += node.Num + "\t";
                }

                Output.Add(pathString);
            }

            Output.Add(routes.Count != 1 ? "There are " + routes.Count +
                " routes from the firestation to streetcorner " + targetStreet.Num + "."
                : "There is 1 route from the firestation to streetcorner " + targetStreet.Num + ".");
        }
    }
}
