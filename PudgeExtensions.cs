using AIRLab.Mathematics;
using Pudge;
using Pudge.Player;
using Pudge.Sensors.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PudgeClient
{
    static class PudgeClientLevel2Extensions
    {

        public static IEnumerable<Point2D> Runes = PrepareForBattle.GetRunes();
        public static IEnumerable<Point2D> SpecRunes = PrepareForBattle.GetSpecialRunes();
        public static bool isMoving;
        public static Point2D startPoint;

        public static PudgeSensorsData RotateTo(this PudgeClientLevel2 client, PudgeSensorsData data, double dx, double dy)
        {
            var angle = Math.Atan2(dy, dx) * 180 / Math.PI;
            var rAngle = (angle - data.SelfLocation.Angle) % 360;
            if (Math.Abs(rAngle) > 180)
                rAngle -= Math.Sign(rAngle) * 360;
            return client.Rotate(rAngle); 
        }

        public static PudgeSensorsData GoTo(this PudgeClientLevel2 client, PudgeSensorsData data, Point2D end, 
            RuneHashSet visited, Point2D targetPoint)
        {
            var old = data.SelfLocation;
            var dx = end.X - data.SelfLocation.X;
            var dy = end.Y - data.SelfLocation.Y;
            var distance = Math.Sqrt(dx * dx + dy * dy);
            data = client.RotateTo(data, dx, dy);
            //var startPoint = new Point2D();
            //double oldAngle;
            //var oldLocation = Tuple.Create(startPoint, 0.0); 
            if (!isMoving)
            {
                startPoint = new Point2D(data.SelfLocation.X, data.SelfLocation.Y);                
            }
            data = MoveByLine(client, data, distance, visited, targetPoint, startPoint);

            if (!data.IsDead)
            {
                if (Movement.ApproximatelyEqual(old, data.SelfLocation, 2))
                {
                    for (var i = 0; i < 2; i++)
                    {
                        data = client.Rotate(90);
                        data = client.MoveByLine(data, 1, visited, targetPoint, startPoint);
                    }

                    visited.Check(data.WorldTime);
                }

                if (!Movement.ApproximatelyEqual(data.SelfLocation, end, 2))
                    return client.GoTo(data, end, visited, targetPoint);
            }
            return data;
        }

        public static PudgeSensorsData MoveByLine(this PudgeClientLevel2 client, PudgeSensorsData data, double distance, 
            RuneHashSet visited, Point2D targetPoint, Point2D oldLocation)
        {
            var step = distance / 7.0;
            
            for (var i = 0; i < 7; i++)
            {
                if (data.IsDead)
                    break;

                if (CheckSlardar(data))
                {
                    if (!data.Events.Select(x => x.Event).Contains(Pudge.World.PudgeEvent.HookCooldown))
                    {
                        data = HookSmb(client, data);
                        break;
                    }
                }

                 
                data = client.Move(step);

                var runeLocation = new Point2D(); 
                if (data.Map.Runes.Count != 0)
                    runeLocation = data.Map.Runes.First().Location;
                if (CheckRune(data) && !isMoving)
                    {
                        if (runeLocation != targetPoint)
                            {
                                isMoving = !isMoving;
                                data = client.GoTo(data, runeLocation, visited, targetPoint);
                                visited.Add(runeLocation);
                                
                                data = client.GoTo(data, oldLocation, visited, targetPoint);
                                isMoving = !isMoving;
                                break;
                            }
                    }
                
                visited.Check(data.WorldTime);                               
            }
            return data;
        }

        public static PudgeSensorsData HookSmb(PudgeClientLevel2 client, PudgeSensorsData data)
        {
            //if (data.Events.Select(x => x.Event).Contains(Pudge.World.PudgeEvent.HookCooldown))
            //    return data; 
            var old = data.SelfLocation;
            var heroData = data.Map.Heroes.Where(x => x.Type == HeroType.Slardar).Single(); 
            var dx = heroData.Location.X - data.SelfLocation.X;
            var dy = heroData.Location.Y - data.SelfLocation.Y;
            data = client.RotateTo(data, dx, dy); 
            data = client.Hook();
            //data = client.Wait(1); 
            while(data.Events.Select(x => x.Event).Contains(Pudge.World.PudgeEvent.HookThrown))
            {
                data = client.Wait(0.1); 
            }
            return data; 
        }


        public static bool CheckSlardar(PudgeSensorsData data)
        {
            return data.Map.Heroes.Select(x => x.Type).Contains(Pudge.Sensors.Map.HeroType.Slardar); 
        }

        public static bool CheckRune(PudgeSensorsData data)
        {
            return data.Map.Runes.Count != 0; 
        }
    }
}
