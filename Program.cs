using System;
using Pudge;
using Pudge.Player;
using AIRLab.Mathematics;
using System.Collections.Generic;
using System.Linq;

namespace PudgeClient
{
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

        public static Point2D[] SpecRunes = new Point2D[]
        {
            new Point2D(-130, 130),
            new Point2D(130, -130)
        };

        public static HashSet<Point2D> Visited = new HashSet<Point2D>();

        static void Print(PudgeSensorsData data)
        {
            Console.WriteLine("---------------------------------");
            if (data.IsDead)
            {
                Console.WriteLine("Ooops, i'm dead :(");
                return;
            }
            Console.WriteLine("I'm here: " + data.SelfLocation);
            Console.WriteLine("My score now: {0}", data.SelfScores);
            Console.WriteLine("Current time: {0:F}", data.WorldTime);
            foreach (var rune in data.Map.Runes)
                Console.WriteLine("Point2D! Type: {0}, Size = {1}, Location: {2}", rune.Type, rune.Size, rune.Location);
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
                args = new[] {"127.0.0.1", "14000"};
            var ip = args[0];
            var port = int.Parse(args[1]);

            // Каждую неделю клиент будет новый. Соотетственно Level1, Level2 и Level3.
            var client = new PudgeClientLevel1();

            // У этого метода так же есть необязательные аргументы:
            // timeLimit -- время в секундах, сколько будет идти матч (по умолчанию 90)
            // operationalTimeLimit -- время в секундах, отображающее ваш лимит на операции в сумме за всю игру
            // По умолчанию -- 1000. На турнире будет использоваться значение 5. Подробнее про это можно прочитать в правилах.
            // isOnLeftSide -- предпочитаемая сторона. Принимается во внимание во время отладки. По умолчанию true.
            // seed -- источник энтропии для случайного появления рун. По умолчанию -- 0. 
            // При изменении руны будут появляться в другом порядке
            // speedUp -- ускорение отладки в два раза. Может вызывать снижение FPS на слабых машинах

            
           

            var sensorData = client.Configurate(ip, port, CvarcTag);

            // Пудж узнает о всех событиях, происходящих в мире, с помощью сенсоров.
            // Для передачи и представления данных с сенсоров служат объекты класса PudgeSensorsData.
            Print(sensorData);

            // Каждое действие возвращает новые данные с сенсоров.
            sensorData = client.Move();
            Print(sensorData);

            var points = new Point2D[]
            {
                new Point2D(-130, -130),        //0
                new Point2D(-70, -120),         //1
                new Point2D(0, -123),           //2
                new Point2D(0, -70),            //3
                new Point2D(-55, -28),          //4
                new Point2D(55, -28),           //5
                new Point2D(0, 0),              //6
                new Point2D(-83, 0),            //7
                new Point2D(-146, 0),           //8
                new Point2D(83, 0),             //9
                new Point2D(146, 0),            //10
                new Point2D(-120, -70),         //11
                new Point2D(120, 70),           //12
                new Point2D(-48, 38),           //13
                new Point2D(48, 38),            //14
                new Point2D(0, 70),             //15
                new Point2D(0, 123),            //16
                new Point2D(70, 120),           //17
                new Point2D(130, 130),          //18
                new Point2D(85, -80),           //19
                new Point2D(130, -130),         //20
                new Point2D(-100, 85),          //21
                new Point2D(-130, 130)          //22
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
                6, 15,
                7, 13,
                9, 14,
                13, 15,
                14, 15,
                15, 16,
                16, 17,
                17, 18,
                12, 18,
                0, 11,
                19, 20,
                21, 22,
                2, 19,
                10, 19,
                8, 21,
                16, 21
                );

            // Для удобства, можно подписать свой метод на обработку всех входящих данных с сенсоров.
            // С этого момента любое действие приведет к отображению в консоли всех данных
            client.SensorDataReceived += Print;

            while (true)
            {
                Visited = checkRunesList(Visited, sensorData.WorldTime);
                IEnumerable<Node> choice;
                //if (sensorData.Events.Select(x => x.Event).Contains(Pudge.World.PudgeEvent.Invisible))
                //    choice = InvestigateWorld(sensorData, graph, SpecRunes);
                choice = InvestigateWorld(sensorData, graph, Runes);
                if (choice.Count() == 0)
                {
                    sensorData = client.Wait(1);
                    continue;
                }
                foreach (var node in choice)
                    sensorData = Movement.GoTo(sensorData, client, node.Location);
                Visited.Add(choice.Last().Location);
            }


            // Корректно завершаем работу
            //client.Exit();
        }


        public static IEnumerable<Node> InvestigateWorld(PudgeSensorsData data, Graph graph, Point2D[] runes)
        {
            var toGo = new List<DijkstraAnswer>();
            foreach (var rune in runes)
            {
                if (Visited.Contains(rune))
                    continue;
                var loc = data.SelfLocation;
                var start = graph.Nodes.Where(x => Math.Abs(x.Location.X - loc.X) < 30 && Math.Abs(x.Location.Y - loc.Y) < 30).Single();
                var finish = graph.Nodes.Where(x => x.Location == rune).Single();
                toGo.Add(DijkstraAlgo.Dijkstra(graph, start, finish));
            }

            if (toGo.Count == 0) return new List<Node>();
            var min = toGo.Where(x => x.PathLength != 0).Select(x => x.PathLength).Min();
            var choice = toGo.Where(x => x.PathLength == min).Select(x => x.Path).First().Skip(1);
            return choice;
        }

        public static HashSet<Point2D> checkRunesList(HashSet<Point2D> runesList, double currentTime)
        {
            //var list = runesList.Where(x => x.checkTime(currentTime)).ToArray();
            //var newSet = new HashSet<Point2D>();
            //foreach(var x in list)
            //{
            //    newSet.Add(x); 
            //}
            //return newSet; 
            if (25 - (currentTime % 25) < 2 || currentTime % 25 < 2)
            {
                return new HashSet<Point2D>(); 
            }
            return runesList; 
        }


    }
}



//var dx = Math.Abs(Math.Abs(start.X) - Math.Abs(end.X));
//var dy = Math.Abs(Math.Abs(start.Y) - Math.Abs(end.Y));
//var angle = Math.Atan(dy / dx) * -90;

//if (Math.Round(start.X) == Math.Round(end.X))
//{
//    var ang = 90 * Math.Sign(dy);
//    x = client.Rotate(ang);
//}
//else if (Math.Round(start.Y) == Math.Round(end.Y))
//{
//    x = client.Rotate(90 * Math.Sign(-dx));
//}
//else if (dy > 0 && dx < 0)
//{
//    x = client.Rotate(angle);
//}
//else if ()

//var distance = Math.Sqrt(dx * dx + dy * dy);
//return client.Move(distance); 



//var enumerator = graph.Edges.GetEnumerator();
//var x = client.Move(0);
//while (enumerator.MoveNext())
//{
//    var start = enumerator.Current.From.Location;
//    var end = enumerator.Current.To.Location;
//    x = Movement.GoTo(x, client, start, end);
//}


//var point1 = new Point2D(-130, -130); 
//var point2 = new Point2D(-130, 0);
//var point3 = new Point2D(-83, 0);
//var point4 = new Point2D(0, -130);
//var point5 = new Point2D(0, -100);
//var point6 = new Point2D(-40, -50);
//var mid = new Point2D(0, 0);

//x = Movement.GoTo(x, client, point1, point4);
//x = Movement.GoTo(x, client, point4, point5);
//x = Movement.GoTo(x, client, point5, point6); 
//x = Movement.GoTo(x, client, point1, point2);
//x = Movement.GoTo(x, client, point2, point3);
////x = MoveTo(x, client, point1, point4);
////x = MoveTo(x, client, point4, point5);
//x = Movement.GoTo(x, client, point3, point6);
//x = Movement.GoTo(x, client, point6, mid);



//#region Trees
//double[][] Trees = new double[][] {
//                new double[3] {-160.0, 160.0, 0.0 },
//                new double[3] { -150.0, 160.0, 0.0 },
//                new double[3] { -140.0, 160.0, 0.0 },
//                new double[3] { -130.0, 160.0, 0.0 },
//                new double[3] { -120.0, 160.0, 0.0 },
//                new double[3] { -110.0, 160.0, 0.0 },
//                new double[3] { -100.0, 160.0, 0.0 },
//                new double[3] { -90.0, 160.0, 0.0 },
//                new double[3] { -80.0, 160.0, 0.0 },
//                new double[3] { -70.0, 160.0, 0.0 },
//                new double[3] { -60.0, 160.0, 0.0 },
//                new double[3] { -50.0, 160.0, 0.0 },
//                new double[3] { -40.0, 160.0, 0.0 },
//                new double[3] { -30.0, 160.0, 0.0 },
//                new double[3] { -20.0, 160.0, 0.0 },
//                new double[3] { -10.0, 160.0, 0.0 },
//                new double[3] { 0.0, 160.0, 0.0 },
//                new double[3] { 10.0, 160.0, 0.0 },
//                new double[3] { 20.0, 160.0, 0.0 },
//                new double[3] { 30.0, 160.0, 0.0 },
//                new double[3] { 40.0, 160.0, 0.0 },
//                new double[3] { 50.0, 160.0, 0.0 },
//                new double[3] { 60.0, 160.0, 0.0 },
//                new double[3] { 70.0, 160.0, 0.0 },
//                new double[3] { 80.0, 160.0, 0.0 },
//                new double[3] { 90.0, 160.0, 0.0 },
//                new double[3] { 100.0, 160.0, 0.0 },
//                new double[3] { 110.0, 160.0, 0.0 },
//                new double[3] { 120.0, 160.0, 0.0 },
//                new double[3] { 130.0, 160.0, 0.0 },
//                new double[3] { 140.0, 160.0, 0.0 },
//                new double[3] { 150.0, 160.0, 0.0 },
//                new double[3] { -160.0, 160.0, 0.0 },
//                new double[3] { -160.0, 150.0, 0.0 },
//                new double[3] { -160.0, 140.0, 0.0 },
//                new double[3] { -160.0, 130.0, 0.0 },
//                new double[3] { -160.0, 120.0, 0.0 },
//                new double[3] { -160.0, 110.0, 0.0 },
//                new double[3] { -160.0, 100.0, 0.0 },
//                new double[3] { -160.0, 90.0, 0.0 },
//                new double[3] { -160.0, 80.0, 0.0 },
//                new double[3] { -160.0, 70.0, 0.0 },
//                new double[3] { -160.0, 60.0, 0.0 },
//                new double[3] { -160.0, 50.0, 0.0 },
//                new double[3] { -160.0, 40.0, 0.0 },
//                new double[3] { -160.0, 30.0, 0.0 },
//                new double[3] { -160.0, 20.0, 0.0 },
//                new double[3] { -160.0, 10.0, 0.0 },
//                new double[3] { -160.0, 0.0, 0.0 },
//                new double[3] { -160.0, -10.0, 0.0 },
//                new double[3] { -160.0, -20.0, 0.0 },
//                new double[3] { -160.0, -30.0, 0.0 },
//                new double[3] { -160.0, -40.0, 0.0 },
//                new double[3] { -160.0, -50.0, 0.0 },
//                new double[3] { -160.0, -60.0, 0.0 },
//                new double[3] { -160.0, -70.0, 0.0 },
//                new double[3] { -160.0, -80.0, 0.0 },
//                new double[3] { -160.0, -90.0, 0.0 },
//                new double[3] { -160.0, -100.0, 0.0 },
//                new double[3] { -160.0, -110.0, 0.0 },
//                new double[3] { -160.0, -120.0, 0.0 },
//                new double[3] { -160.0, -130.0, 0.0 },
//                new double[3] { -160.0, -140.0, 0.0 },
//                new double[3] { -160.0, -150.0, 0.0 },
//                new double[3] { 160.0, -160.0, 0.0 },
//                new double[3] { 150.0, -160.0, 0.0 },
//                new double[3] { 140.0, -160.0, 0.0 },
//                new double[3] { 130.0, -160.0, 0.0 },
//                new double[3] { 120.0, -160.0, 0.0 },
//                new double[3] { 110.0, -160.0, 0.0 },
//                new double[3] { 100.0, -160.0, 0.0 },
//                new double[3] { 90.0, -160.0, 0.0 },
//                new double[3] { 80.0, -160.0, 0.0 },
//                new double[3] { 70.0, -160.0, 0.0 },
//                new double[3] { 60.0, -160.0, 0.0 },
//                new double[3] { 50.0, -160.0, 0.0 },
//                new double[3] { 40.0, -160.0, 0.0 },
//                new double[3] { 30.0, -160.0, 0.0 },
//                new double[3] { 20.0, -160.0, 0.0 },
//                new double[3] { 10.0, -160.0, 0.0 },
//                new double[3] { 0.0, -160.0, 0.0 },
//                new double[3] { -10.0, -160.0, 0.0 },
//                new double[3] { -20.0, -160.0, 0.0 },
//                new double[3] { -30.0, -160.0, 0.0 },
//                new double[3] { -40.0, -160.0, 0.0 },
//                new double[3] { -50.0, -160.0, 0.0 },
//                new double[3] { -60.0, -160.0, 0.0 },
//                new double[3] { -70.0, -160.0, 0.0 },
//                new double[3] { -80.0, -160.0, 0.0 },
//                new double[3] { -90.0, -160.0, 0.0 },
//                new double[3] { -100.0, -160.0, 0.0 },
//                new double[3] { -110.0, -160.0, 0.0 },
//                new double[3] { -120.0, -160.0, 0.0 },
//                new double[3] { -130.0, -160.0, 0.0 },
//                new double[3] { -140.0, -160.0, 0.0 },
//                new double[3] { -150.0, -160.0, 0.0 },
//                new double[3] { 160.0, -160.0, 0.0 },
//                new double[3] { 160.0, -150.0, 0.0 },
//                new double[3] { 160.0, -140.0, 0.0 },
//                new double[3] { 160.0, -130.0, 0.0 },
//                new double[3] { 160.0, -120.0, 0.0 },
//                new double[3] { 160.0, -110.0, 0.0 },
//                new double[3] { 160.0, -100.0, 0.0 },
//                new double[3] { 160.0, -90.0, 0.0 },
//                new double[3] { 160.0, -80.0, 0.0 },
//                new double[3] { 160.0, -70.0, 0.0 },
//                new double[3] { 160.0, -60.0, 0.0 },
//                new double[3] { 160.0, -50.0, 0.0 },
//                new double[3] { 160.0, -40.0, 0.0 },
//                new double[3] { 160.0, -30.0, 0.0 },
//                new double[3] { 160.0, -20.0, 0.0 },
//                new double[3] { 160.0, -10.0, 0.0 },
//                new double[3] { 160.0, 0.0, 0.0 },
//                new double[3] { 160.0, 10.0, 0.0 },
//                new double[3] { 160.0, 20.0, 0.0 },
//                new double[3] { 160.0, 30.0, 0.0 },
//                new double[3] { 160.0, 40.0, 0.0 },
//                new double[3] { 160.0, 50.0, 0.0 },
//                new double[3] { 160.0, 60.0, 0.0 },
//                new double[3] { 160.0, 70.0, 0.0 },
//                new double[3] { 160.0, 80.0, 0.0 },
//                new double[3] { 160.0, 90.0, 0.0 },
//                new double[3] { 160.0, 100.0, 0.0 },
//                new double[3] { 160.0, 110.0, 0.0 },
//                new double[3] { 160.0, 120.0, 0.0 },
//                new double[3] { 160.0, 130.0, 0.0 },
//                new double[3] { 160.0, 140.0, 0.0 },
//                new double[3] { 160.0, 150.0, 0.0 },
//                new double[3] { 140.0, -80.0, 0.0 },
//                new double[3] { 130.0, -80.0, 0.0 },
//                new double[3] { 120.0, -80.0, 0.0 },
//                new double[3] { 80.0, -140.0, 0.0 },
//                new double[3] { 80.0, -130.0, 0.0 },
//                new double[3] { 80.0, -120.0, 0.0 },
//                new double[3] { -140.0, 80.0, 0.0 },
//                new double[3] { -130.0, 80.0, 0.0 },
//                new double[3] { -120.0, 80.0, 0.0 },
//                new double[3] { -80.0, 140.0, 0.0 },
//                new double[3] { -80.0, 130.0, 0.0 },
//                new double[3] { -80.0, 120.0, 0.0 },
//                new double[3] { -100.0, -100.0, 0.0 },
//                new double[3] { -90.0, -90.0, 0.0 },
//                new double[3] { -80.0, -80.0, 0.0 },
//                new double[3] { -100.0, -30.0, 0.0 },
//                new double[3] { -90.0, -40.0, 0.0 },
//                new double[3] { -80.0, -50.0, 0.0 },
//                new double[3] { -70.0, -60.0, 0.0 },
//                new double[3] { -60.0, -70.0, 0.0 },
//                new double[3] { -50.0, -80.0, 0.0 },
//                new double[3] { -40.0, -90.0, 0.0 },
//                new double[3] { -30.0, -100.0, 0.0 },
//                new double[3] { 100.0, 100.0, 0.0 },
//                new double[3] { 90.0, 90.0, 0.0 },
//                new double[3] { 80.0, 80.0, 0.0 },
//                new double[3] { 100.0, 30.0, 0.0 },
//                new double[3] { 90.0, 40.0, 0.0 },
//                new double[3] { 80.0, 50.0, 0.0 },
//                new double[3] { 70.0, 60.0, 0.0 },
//                new double[3] { 60.0, 70.0, 0.0 },
//                new double[3] { 50.0, 80.0, 0.0 },
//                new double[3] { 40.0, 90.0, 0.0 },
//                new double[3] { 30.0, 100.0, 0.0 },
//                new double[3] { 80.0, -40.0, 0.0 },
//                new double[3] { 70.0, -50.0, 0.0 },
//                new double[3] { 60.0, -50.0, 0.0 },
//                new double[3] { 50.0, -60.0, 0.0 },
//                new double[3] { 40.0, -60.0, 0.0 },
//                new double[3] { 30.0, -70.0, 0.0 },
//                new double[3] { -80.0, 40.0, 0.0 },
//                new double[3] { -70.0, 50.0, 0.0 },
//                new double[3] { -60.0, 50.0, 0.0 },
//                new double[3] { -50.0, 60.0, 0.0 },
//                new double[3] { -40.0, 60.0, 0.0 },
//                new double[3] { -30.0, 70.0, 0.0 },
//                new double[3] {  -40.0, 0.0, 0.0 },
//                new double[3] { 40.0, 0.0, 0.0 }};
//#endregion