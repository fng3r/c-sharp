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

        public static IEnumerable<Point2D> Runes = PrepareForBattle.GetRunes();
        public static IEnumerable<Point2D> SpecRunes = PrepareForBattle.GetSpecialRunes();

        public static PudgeSensorsData GoTo(this PudgeClientLevel1 client, PudgeSensorsData data, Point2D end, RuneHashSet visited)
        {
            var old = data.SelfLocation;
            var dx = end.X - data.SelfLocation.X;
            var dy = end.Y - data.SelfLocation.Y;
            var distance = Math.Sqrt(dx * dx + dy * dy);
            var rAngle = Movement.FindAngle(data, dx, dy);
            if (Math.Abs(rAngle) > 7)
                data = client.Rotate(rAngle);
            data = MoveByLine(client, data, distance, visited);
            if (!data.IsDead)
            {
                if (Movement.ApproximatelyEqual(old, data.SelfLocation, 2))
                {
                    data = client.Rotate(180);
                    data = client.MoveByLine(data, 1, visited);
                    visited.Check(data.WorldTime);
                }
                if (!Movement.ApproximatelyEqual(data.SelfLocation, end, 5))
                    return client.GoTo(data, end, visited);
            }
            return data;
        }

        public static PudgeSensorsData MoveByLine(this PudgeClientLevel1 client, PudgeSensorsData data, double distance, RuneHashSet visited)
        {
            var step = distance / 3.0;

            for (var i = 0; i < 3; i++)
            {
                data = client.Move(step);
                visited.Check(data.WorldTime);
                if (data.IsDead)
                    break;
                //if (data.Map.Runes.Count() == 1)
                //{
                //    var target = data.Map.Runes.Single();
                //    var runes = PrepareForBattle.GetAllRunes();
                //    var goTo = runes.Where(x => x == target.Location).Single();
                //    return client.GoTo(data, goTo, visited);
                //}
            }
            return data;
        }

    }
}
