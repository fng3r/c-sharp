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


        public static PudgeSensorsData RotateTo(this PudgeClientLevel2 client, PudgeSensorsData data, double dx, double dy)
        {
            var angle = Math.Atan2(dy, dx) * 180 / Math.PI;
            var rAngle = (angle - data.SelfLocation.Angle) % 360;
            if (Math.Abs(rAngle) > 180)
                rAngle -= Math.Sign(rAngle) * 360;
            return client.Rotate(rAngle); 
        }

        public static PudgeSensorsData GoTo(this PudgeClientLevel2 client, PudgeSensorsData data, Point2D end, RuneHashSet visited)
        {
            var old = data.SelfLocation;
            var dx = end.X - data.SelfLocation.X;
            var dy = end.Y - data.SelfLocation.Y;
            var distance = Math.Sqrt(dx * dx + dy * dy);
            data = client.RotateTo(data, dx, dy);            
            data = MoveByLine(client, data, distance, visited);
            if (!data.IsDead)
            {
                if (Movement.ApproximatelyEqual(old, data.SelfLocation, 2))
                {
                    data = client.Rotate(90);
                    data = client.MoveByLine(data, 1, visited);
                    data = client.Rotate(90);
                    data = client.MoveByLine(data, 1, visited);

                    visited.Check(data.WorldTime);
                }
                if (!Movement.ApproximatelyEqual(data.SelfLocation, end, 7))
                    return client.GoTo(data, end, visited);
            }
            return data;
        }

        public static PudgeSensorsData MoveByLine(this PudgeClientLevel2 client, PudgeSensorsData data, double distance, RuneHashSet visited)
        {
            var step = distance / 5.0;

            for (var i = 0; i < 5; i++)
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

    }
}
