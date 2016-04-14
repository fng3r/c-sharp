using AIRLab.Mathematics;
using Pudge;
using Pudge.Player;
using Pudge.Sensors.Map;
using Pudge.World;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PudgeClient
{

    static class PudgeClientLevel2Extensions
    {
        public static bool OnMove = false;
        public static IEnumerable<Point2D> SlardarSpots = PrepareForBattle.GetSlardars();
        public static IEnumerable<Point2D> Points = PrepareForBattle.GetPoints();
        
        public static PudgeSensorsData RotateTo(this PudgeClientLevel2 client, PudgeSensorsData data, double dx, double dy)
        {
            var angle = Math.Atan2(dy, dx) * 180 / Math.PI;
            var rAngle = (angle - data.SelfLocation.Angle) % 360;
            if (Math.Abs(rAngle) > 180)
                rAngle -= Math.Sign(rAngle) * 360;
            return client.Rotate(rAngle); ;
        }
        public static PudgeSensorsData GoTo(this PudgeClientLevel2 client, PudgeSensorsData data, Point2D end, RuneHashSet visited, RuneHashSet killed)
        {
            var old = data.SelfLocation;
            var dx = end.X - data.SelfLocation.X;
            var dy = end.Y - data.SelfLocation.Y;
            var distance = Movement.GetDistance(dx, dy);
            data = client.RotateTo(data, dx, dy);
            data = MoveByLine(client, data, distance, visited, killed);
            if (!data.IsDead)
            {
                if (Movement.ApproximatelyEqual(old, data.SelfLocation, 2))
                {
                    for (int i = 0; i < 1; i++)
                    {
                        data = client.Rotate(180);
                        data = client.MoveByLine(data, 3, visited, killed);
                    }
                    visited.Check(data.WorldTime);
                    killed.Check(data.WorldTime);
                }
                if (!Movement.ApproximatelyEqual(data.SelfLocation, end, 7))
                    return client.GoTo(data, end, visited, killed);
            }
            return data;
        }

        public static PudgeSensorsData MoveByLine(this PudgeClientLevel2 client, PudgeSensorsData data, double distance, RuneHashSet visited, RuneHashSet killed)
        {
            var step = distance / 10;

            for (var i = 0; i < 10; i++)
            {
                if (data.IsDead)
                    break;
                if (CheckEnemy(data))
                    if (!data.Events.Select(x => x.Event).Contains(PudgeEvent.HookCooldown))
                    {
                        killed.Add(SlardarSpots.Where(x => Movement.ApproximatelyEqual(data.SelfLocation, x, 100)).Single());
                        data = HookEnemy(client, data);
                        break;
                    }
#region MaybeNextTime
                //if (CheckRune(data))
                //    if (!OnMove)
                //    {
                //        var loc = data.Map.Runes.First().Location;
                //        var min = Points.Select(x => Movement.GetDistance(x, data.SelfLocation)).Min();
                //        var toGo = Points.Where(x => Movement.GetDistance(x, data.SelfLocation) == min).First();
                //        OnMove = !OnMove;
                //        data = client.GoTo(data, toGo, visited);
                //        data = client.GoTo(data, loc, visited);
                //        visited.Add(loc);
                //        OnMove = !OnMove;
                //        break;
                //    }
                #endregion
                data = client.Move(step);
                visited.Check(data.WorldTime);
                killed.Check(data.WorldTime);
            }

            return data;
        }

        public static bool CheckEnemy(PudgeSensorsData data)
        {
            return data.Map.Heroes.Select(x => x.Type).Contains(HeroType.Slardar);
        }

        public static bool CheckRune(PudgeSensorsData data)
        {
            return data.Map.Runes.Count != 0;
        }

        public static PudgeSensorsData HookEnemy(PudgeClientLevel2 client, PudgeSensorsData data)
        {
            var old = data.SelfLocation;
            var heroData = data.Map.Heroes.Where(x => x.Type == HeroType.Slardar).Single();
            var dx = heroData.Location.X - data.SelfLocation.X;
            var dy = heroData.Location.Y - data.SelfLocation.Y;
            data = client.RotateTo(data, dx, dy);
            data = client.Hook();
            while (data.Events.Select(x => x.Event).Contains(PudgeEvent.HookThrown))
                data = client.Wait(0.05);
            return data;
        }

    }
}
