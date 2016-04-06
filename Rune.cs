using AIRLab.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PudgeClient
{
    class Rune
    {
        public readonly Point2D Location;
        readonly double duration;
        readonly Stopwatch watch;
        double time { get { return watch.ElapsedMilliseconds * 1000; } }
        public bool ToDelete { get { return time > duration; } }

        public Rune(Point2D point, double durationTime)
        {
            Location = point;
            duration = durationTime;
            watch = new Stopwatch();
            watch.Start();
        }
    }
}
