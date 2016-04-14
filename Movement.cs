﻿using AIRLab.Mathematics;
using CVARC.V2;
using Pudge;
using Pudge.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PudgeClient
{
    class Movement
    {

        public static double GetDistance(double dx, double dy)
        {
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public static double GetDistance(Point2D p, LocatorItem self)
        {
            var dx = p.X - self.X;
            var dy = p.Y - self.Y;
            return GetDistance(dx, dy);
        }

        public static bool ApproximatelyEqual(LocatorItem self, Point2D loc, double deviation)
        {
            return Math.Abs(self.X - loc.X) < deviation && Math.Abs(self.Y - loc.Y) < deviation;
        }

        public static bool ApproximatelyEqual(LocatorItem self, LocatorItem loc, double deviation)
        {
            return Math.Abs(self.X - loc.X) < deviation && Math.Abs(self.Y - loc.Y) < deviation;
        }
    }
}
