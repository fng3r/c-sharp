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

    static class PudgeClientLevel3Extensions
    {
        static SlardarRules rules = SlardarRules.Current;
        public static bool AfterHook = false;
        public static IEnumerable<Point2D> SlardarSpots = PrepareForBattle.GetSlardars();
        public static IEnumerable<Point2D> Points = PrepareForBattle.GetPoints();
        
        public static PudgeSensorsData RotateTo(this PudgeClientLevel3 client, PudgeSensorsData data, double dx, double dy)
        {
            var angle = Movement.FindAngle(data, dx, dy);
            return client.Rotate(angle);
        }

        public static PudgeSensorsData GoTo(this PudgeClientLevel3 client, PudgeSensorsData data, Point2D end, WorldInfo visited, WorldInfo killed)
        {
            var old = data.SelfLocation;
            var dx = end.X - data.SelfLocation.X;
            var dy = end.Y - data.SelfLocation.Y;
            var distance = Movement.GetDistance(dx, dy);
            data = client.RotateTo(data, dx, dy);
            data = MoveByLine(client, data, distance, visited, killed);
            if (!data.IsDead)
            {
                if (!AfterHook && Movement.ApproximatelyEqual(old, data.SelfLocation, 2))
                {
                    
                    data = client.Rotate(180);
                    data = client.MoveByLine(data, 10, visited, killed);                       
                    visited.Check(data.WorldTime);
                    killed.Check(data.WorldTime);
                }
                AfterHook = false;
                if (!Movement.ApproximatelyEqual(data.SelfLocation, end, 7))
                    return client.GoTo(data, end, visited, killed);
            }
            return data;
        }

        public static PudgeSensorsData MoveByLine(this PudgeClientLevel3 client, PudgeSensorsData data, double distance, WorldInfo visited, WorldInfo killed) 
        {
            double speed;
            if (data.Events.Select(x => x.Event).Contains(PudgeEvent.Hasted)){
                var hasteDuration = data.Events.Select(x => x).Where(x => x.Event == PudgeEvent.Hasted).First().Duration;
                if (hasteDuration > distance / 80)
                    speed = 80;
                else
                    speed = distance / (hasteDuration + (distance - hasteDuration * 80) / 40);
            }
            else
            {
                speed = 40; 
            }

            var count = Math.Floor((distance / speed) * 4.5);
            var step = distance / count;

            for (var i = 0; i < count; i++)
            {
                if (data.IsDead)
                    break;
                var check = CheckEnemy(data);
                if (check != "")
                    if (!data.Events.Select(x => x.Event).Contains(PudgeEvent.HookCooldown))
                    {
                        if (check == "Slardar")
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

        public static string CheckEnemy(PudgeSensorsData data)
        {
            if (data.Map.Heroes.Select(x => x.Type).Contains(HeroType.Pudge))
            {
                return "Pudge";
            } else if (data.Map.Heroes.Select(x => x.Type).Contains(HeroType.Slardar) &&
                !data.Events.Select(x => x.Event).Contains(PudgeEvent.Invisible))
            {
                return "Slardar";
            }
            return "";
        }

        public static bool CheckRune(PudgeSensorsData data)
        {
            return data.Map.Runes.Count != 0;
        }

        public static PudgeSensorsData HookEnemy(PudgeClientLevel3 client, PudgeSensorsData data)
        {
            var old = data.SelfLocation;
            var enemy = data.Map.Heroes.Where(x => x.Type == HeroType.Slardar || x.Type == HeroType.Pudge).Single();
            var dx = enemy.Location.X - data.SelfLocation.X;
            var dy = enemy.Location.Y - data.SelfLocation.Y;
            var angle = Movement.FindAngle(data, dx, dy);
            data = client.Rotate(angle);
            data = client.Hook();
            AfterHook = !AfterHook;
            data = client.Rotate(-angle);
            while (data.Events.Select(x => x.Event).Contains(PudgeEvent.HookThrown))
                data = client.Wait(0.05);
            return data;
        }

    }
}
