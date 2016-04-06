using System;
using Pudge;
using Pudge.Player;
using AIRLab.Mathematics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PudgeClient
{
    public static class PudgeClientLevel1Extensions
    {
        public static PudgeSensorsData MoveTo(this PudgeClientLevel1 client, PudgeSensorsData data, double x, double y)
        {
            Console.WriteLine("Here: X: {0},    Y: {1}", data.SelfLocation.X, data.SelfLocation.Y);
            if (data.SelfLocation.Angle < 0) data.SelfLocation.Angle += 360;
            var dx = x - data.SelfLocation.X;
            var dy = y - data.SelfLocation.Y;
            Console.WriteLine("Going: X: {0}, Y: {1}", x, y);
            var angle = Math.Atan2(dy, dx) * 180 / Math.PI;
            Console.WriteLine("needed: {0}, self: {1}", angle, data.SelfLocation.Angle);
            var rAngle = (angle - data.SelfLocation.Angle) % 360;
            if (Math.Abs(rAngle) > 180)
                rAngle -= Math.Sign(rAngle) * 360;
            Console.WriteLine("!!" + rAngle);
            client.Rotate(rAngle);
            return client.Move(Math.Sqrt(dx * dx + dy * dy));
        }

    }
    class Program
    {
        const string CvarcTag = "Получи кварк-тэг на сайте";
        public static Point2D[] Runes = new Point2D[]
        {
            new Point2D(-70, -120),
            new Point2D(0, 0),
            new Point2D(-120, -70),
            new Point2D(70, 120),
            new Point2D(120, 70)
        };

        // Пример визуального отображения данных с сенсоров при отладке.
        // Если какая-то информация кажется вам лишней, можете закомментировать что-нибудь.
        
        static void Print(PudgeSensorsData data)
        {
            Console.WriteLine("---------------------------------");
            if (data.IsDead)
            {
                // Правильное обращение со смертью.
                Console.WriteLine("Ooops, i'm dead :(");
                return;
            }
            Console.WriteLine("I'm here: " + data.SelfLocation);
            Console.WriteLine("My score now: {0}", data.SelfScores);
            Console.WriteLine("Current time: {0:F}", data.WorldTime);
            foreach (var rune in data.Map.Runes)
                Console.WriteLine("Rune! Type: {0}, Size = {1}, Location: {2}", rune.Type, rune.Size, rune.Location);
            foreach (var heroData in data.Map.Heroes)
                Console.WriteLine("Enemy! Type: {0}, Location: {1}, Angle: {2:F}", heroData.Type, heroData.Location, heroData.Angle);
            foreach (var eventData in data.Events)
                Console.WriteLine("I'm under effect: {0}, Duration: {1}", eventData.Event,
                    eventData.Duration - (data.WorldTime - eventData.Start));
            Console.WriteLine("---------------------------------");
            Console.WriteLine();
        }

        static void Main(string[] args)
        {
            if (args.Length == 0)
                args = new[] { "127.0.0.1", "14000" };
            var ip = args[0];
            var port = int.Parse(args[1]);
            var client = new PudgeClientLevel1();

            var sensorData = client.Configurate(ip, port, CvarcTag);

            // Каждое действие возвращает данные с сенсоров.
            Print(sensorData);

            var points = new Point2D[]
            {
                new Point2D(-130, -130),
                new Point2D(-70, -120),
                new Point2D(0, -123),
                new Point2D(0, -90),
                new Point2D(-48, -38),
                new Point2D(48, -38),
                new Point2D(0, 0),
                new Point2D(-83, 0),
                new Point2D(-146, 0),
                new Point2D(83, 0),
                new Point2D(146, 0),
                new Point2D(-120, -70),
                new Point2D(120, 70),
                new Point2D(-48, 38),
                new Point2D(48, 38),
                new Point2D(0, 70),
                new Point2D(0, 123),
                new Point2D(70, 120)


            };

            var graph = new Graph(points);
            graph = graph.MakeBounds(
                0, 1,
                1, 2,
                2, 3,
                3, 4,
                3, 5,
                3, 6,
                4, 6,
                4, 5,
                5, 6,
                4, 7,
                7, 8,
                8, 11,
                5, 9,
                9, 10,
                10, 12,
                6, 13,
                6, 14,
                7, 13,
                9, 14,
                13, 15,
                14, 15,
                15, 16,
                16, 17
                );

            //client.SensorDataReceived += Print;
            var visited = new HashSet<Rune>();
            while(true)
            {
                var toGo = new List<DijkstraAnswer>();
                foreach (var rune in Runes)
                {
                    if (visited.Select(x => x.Location).Contains(rune))
                        continue;
                    var loc = sensorData.SelfLocation;
                    var start = graph.Nodes.Where(x => Math.Abs(x.Location.X - loc.X) < 1.5 && Math.Abs(x.Location.Y - loc.Y) < 1.5).Single();
                    var finish = graph.Nodes.Where(x => x.Location == rune).Single();
                    toGo.Add(DijkstraAlgo.Dijkstra(graph, start, finish));
                }
                if (toGo.Count == 0) break;
                var min = toGo.Select(x => x.PathLength).Min();
                var choice = toGo.Where(x => x.PathLength == min).Select(x => x.Path).First().Skip(1);
                foreach (var node in choice)
                    sensorData = client.MoveTo(sensorData, node.Location.X, node.Location.Y);
                visited.Add(
                    new Rune(choice.Last().Location, 25)
                        );
                //visited.RemoveWhere(x => x.ToDelete);
            }
            //sensorData = MoveTo(client, sensorData, 0, 0);
            //sensorData = MoveTo(client, sensorData, 0, 70);
            //sensorData = MoveTo(client, sensorData, -48, 38);
            //sensorData = MoveTo(client, sensorData, -83, 0);
            //sensorData = MoveTo(client, sensorData, -146, 0);
            //sensorData = MoveTo(client, sensorData, -120, -70);

            // Корректно завершаем работу
            client.Exit();
        }
    }
}
