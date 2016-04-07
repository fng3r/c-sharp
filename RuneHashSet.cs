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
        
        bool NeedToClear(double currentTime)
        {
            return currentTime - clearTime > 2;
        }

        public void Check(double currentTime)
        {
            if (NeedToClear(currentTime))
                if (25 - (currentTime % 25) < 2 || currentTime % 25 < 2)
                {
                    HashSet = new HashSet<Point2D>();
                    clearTime = currentTime;
                }           
        }
    }
}
