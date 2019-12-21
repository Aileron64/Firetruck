using System;
using System.Collections.Generic;

namespace FireTruck
{
    class Street : IEquatable<Street>
    {
        public List<Street> ConnectedStreets = new List<Street>();
        public int Num { get; set; }

        public Street(string num)
        {
            try
            {
                Num = int.Parse(num);
            }
            catch
            {
                throw new ArgumentException(
                    "\n*** Please use integer street values ***");
            }
        }

        public void NewConnection(Street other)
        {
            if (!ConnectedStreets.Contains(other))
                ConnectedStreets.Add(other);
        }

        public bool Equals(Street other)
        {
            return Num == other.Num;
        }
    }
}
