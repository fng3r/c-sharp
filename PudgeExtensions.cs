using AIRLab.Mathematics;
using Pudge;
using Pudge.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PudgeClient
{
    static class PudgeClientLevel1Extensions
    {
        public static PudgeSensorsData GoTo(this PudgeClientLevel1 client, PudgeSensorsData data, Point2D end, RuneHashSet visited)
        {
            var old = data.SelfLocation;
            var dx = end.X - data.SelfLocation.X;
            var dy = end.Y - data.SelfLocation.Y;
            var rAngle = Movement.FindAngle(data, dx, dy);
            var distance = Math.Sqrt(dx * dx + dy * dy);
            if (Math.Abs(rAngle) > 5)
                data = client.Rotate(rAngle);
            data = MoveByLine(client, data, distance, visited);
            if (!data.IsDead)
                if (Movement.ApproximatelyEqual(old, data.SelfLocation, 1))
                {
                    client.Rotate(180);
                    data = client.Move(3);
                    visited.Check(data.WorldTime);
                }
                if (!Movement.ApproximatelyEqual(data.SelfLocation, end, 3))
                    return client.GoTo(data, end, visited);
            return data;
        }

        public static PudgeSensorsData MoveByLine(this PudgeClientLevel1 client, PudgeSensorsData data, double distance, RuneHashSet visited)
        {
            var step = distance / 10.0;

            for (var i = 0; i < 10; i++)
            {
                data = client.Move(step);
                visited.Check(data.WorldTime);
                if (data.IsDead)
                    break;
                checkSomething();
            }
            return data;
        }

        public static void checkSomething()
        {

        }
    }
}
