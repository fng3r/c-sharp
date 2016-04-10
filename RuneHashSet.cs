using AIRLab.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PudgeClient
{
    class RuneHashSet
    {
        public HashSet<Point2D> HashSet { get; private set; }
        double clearTime;

        public RuneHashSet()
        {
            HashSet = new HashSet<Point2D>();
        }

        public bool Contains(Point2D point)
        {
            return HashSet.Contains(point);
        }

        public void Add(Point2D point)
        {
            HashSet.Add(point);
        }
        
        bool NeedToClear(double currentTime)
        {
            return currentTime - clearTime > 5;
        }

        public void Check(double currentTime)
        {
            if (NeedToClear(currentTime))
                if (25 - (currentTime % 25) < 1 || currentTime % 25 < 1)
                {
                    HashSet = new HashSet<Point2D>();
                    clearTime = currentTime;
                }
        }
    }
}
