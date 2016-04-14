using AIRLab.Mathematics;
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
<<<<<<< HEAD
        public static PudgeSensorsData GoTo(PudgeSensorsData data, PudgeClientLevel1 client, Point2D end)
        {
            var dx = end.X - data.SelfLocation.X;
            var dy = end.Y - data.SelfLocation.Y;
            var rAngle = FindAngle(data, dx, dy);
            var distance = Math.Sqrt(dx * dx + dy * dy);
            if (Math.Abs(rAngle) > 5)
                data = client.Rotate(rAngle);
            data = MoveByLine(client, data, distance);
            if (!data.IsDead)
                if (!ApproximatelyEqual(data.SelfLocation, end, 3))
                    return GoTo(data, client, end);
            return data;
        }

        public static double FindAngle(PudgeSensorsData data, double dx, double dy)
        {
            var angle = Math.Atan2(dy, dx) * 180 / Math.PI;
            var rAngle = (angle - data.SelfLocation.Angle) % 360;
            if (Math.Abs(rAngle) > 180)
                rAngle += -Math.Sign(rAngle) * 360;
            return rAngle;
        }

        public static PudgeSensorsData MoveByLine(PudgeClientLevel1 client, PudgeSensorsData data, double distance)
        {
            var step = distance / 10.0;

            for (var i = 0; i < 10; i++)
            {
                data = client.Move(step);
                if (data.IsDead)
                    break;
                checkSomething();
            }
            return data;
        }

        public static void checkSomething()
        {

        }
=======
        
        
>>>>>>> refs/remotes/origin/master

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
