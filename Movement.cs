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
    class Movement
    {
        public static PudgeSensorsData GoTo(PudgeSensorsData data, PudgeClientLevel1 client, Point2D end)
        {
            var dx = end.X - data.SelfLocation.X;
            var dy = end.Y - data.SelfLocation.Y;
            var rAngle = CountAngle(data, dx, dy);
            var distance = Math.Sqrt(dx * dx + dy * dy);
            var currentState = client.Rotate(rAngle);
            currentState = MoveByLine(client, distance);
            return currentState;
        }

        public static double CountAngle(PudgeSensorsData data, double dx, double dy)
        {
            var angle = Math.Atan2(dy, dx) * 180 / Math.PI;
            var rAngle = (angle - data.SelfLocation.Angle) % 360;
            if (Math.Abs(rAngle) > 180)
                rAngle += -Math.Sign(rAngle) * 360;
            return rAngle;
        }

        public static PudgeSensorsData MoveByLine(PudgeClientLevel1 client, double distance)
        {
            var step = distance / 10.0;
            var currentState = new PudgeSensorsData();

            for (var i = 0; i < 10; i++)
            {
                currentState = client.Move(step);
                checkSomething();
            }

            return currentState;
        }

        public static void checkSomething()
        {

        }
    }
}
