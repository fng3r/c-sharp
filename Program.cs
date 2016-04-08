using System;
using Pudge;
using Pudge.Player;
using Pudge.World;
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
            new Point2D(-70, -130),
            new Point2D(0, 0),
            new Point2D(-130, -70),
            new Point2D(70, 130),
            new Point2D(130, 70)
        };

        public static Point2D[] SpecRunes = new Point2D[]
        {
            new Point2D(-130, 130),
            new Point2D(130, -130)
        };

        public static RuneHashSet Visited = new RuneHashSet();

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


            var points = new Point2D[]
            {
                new Point2D(-130, -130),        //0
                new Point2D(-70, -130),         //1
                new Point2D(0, -130),           //2
                new Point2D(0, -70),            //3
                new Point2D(-55, -28),          //4
                new Point2D(55, -28),           //5
                new Point2D(0, 0),              //6
                new Point2D(-83, 0),            //7
                new Point2D(-130, 0),           //8
                new Point2D(83, 0),             //9
                new Point2D(130, 0),            //10
                new Point2D(-130, -70),         //11
                new Point2D(130, 70),           //12
                new Point2D(-48, 38),           //13
                new Point2D(58, 38),            //14
                new Point2D(0, 70),             //15
                new Point2D(0, 130),            //16
                new Point2D(70, 130),           //17
                new Point2D(130, 130),          //18
                new Point2D(100, -80),           //19
                new Point2D(130, -130),         //20
                new Point2D(-100, 80),          //21
                new Point2D(-130, 130),         //22
                new Point2D(-100, 50),          //23
                new Point2D(100, -50),           //24
                new Point2D(0, 85),             //25
                new Point2D(0, -85),             //26
                new Point2D(70, -100),            //27
                new Point2D(-70, 100)             //28
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
                16, 21,
                7, 23,
                21, 23,
                9, 24,
                19, 24,
                21, 25,
                15, 25,
                16, 25,
                2, 26,
                3, 26,
                19, 26,
                2, 27,
                19, 27,
                20, 27,
                16, 28,
                21, 28,
                22, 28,
                26, 27,
                15, 25
                );

            // Для удобства, можно подписать свой метод на обработку всех входящих данных с сенсоров.
            // С этого момента любое действие приведет к отображению в консоли всех данных
            client.SensorDataReceived += Print;

            while (true)
            {
                Visited.Check(sensorData.WorldTime);
                var queue = new Queue<DijkstraAnswer>(); 
                var choice = InvestigateWorld(sensorData, graph, Runes);
                queue.Enqueue(choice); 
                //stack.Push(choice); 
                foreach (var dataEvent in sensorData.Events)
                {
                    if (dataEvent.Event == PudgeEvent.Invisible || dataEvent.Event == PudgeEvent.Hasted)
                    {
                        var choice1 = InvestigateWorld(sensorData, graph, SpecRunes);
                        
                        //if (choice1.PathLength == 0)
                        //    break;
                        var timeRemaining = dataEvent.Duration - (sensorData.WorldTime - dataEvent.Start);
                        if (choice1.PathLength / 40 < timeRemaining)
                            if (choice1.PathLength > choice.PathLength)
                            {
                                choice1 = PathFinder.DijkstraAlgo(graph, choice.Path.Last(), choice1.Path.Last());
                                queue.Enqueue(choice1);
                            }
                            else
                            {
                                queue.Dequeue();
                                queue.Enqueue(choice1);
                            }
                                
                    }
                }
                while (queue.Count != 0)
                {
                    
                    var path = queue.Dequeue().Path.Skip(1);
                    if (path.Count() == 0)
                    {
                        sensorData = client.Wait(0.01);
                        continue;
                    }
                    foreach (var node in path)
                    {
                        Visited.Check(sensorData.WorldTime);
                        sensorData = Movement.GoTo(sensorData, client, node.Location);
                        if (sensorData.IsDead)
                        {
                            client.Wait(5);
                            break;
                        }

                    }
                    Visited.HashSet.Add(path.Last().Location);
                    sensorData = client.Wait(0.05);
                    
                }
            }

            // Корректно завершаем работу
            //client.Exit();
        }


        public static DijkstraAnswer InvestigateWorld(PudgeSensorsData data, Graph graph, Point2D[] runes)
        {
            var toGo = new List<DijkstraAnswer>();
            foreach (var rune in runes)
            {
                if (Visited.HashSet.Contains(rune))
                    continue;
                Node begin = new Node(0, new Point2D(0, 0)); 
                var loc = data.SelfLocation;
                var start = graph.Nodes.Where(x => Movement.ApproximatelyEqual(loc, x.Location, 5));
                if (start.Count() == 0)
                {
                    var allRunes = runes.Concat(SpecRunes); 
                    foreach (var e in allRunes)
                    {
                        if (Movement.ApproximatelyEqual(loc, e, 15))
                            begin = graph.Nodes.Where(x => x.Location == e).Single(); 
                    }
                } else
                {
                    begin = graph.Nodes.Where(x => Movement.ApproximatelyEqual(loc, x.Location, 5)).Single();
                }
                    
                var finish = graph.Nodes.Where(x => x.Location == rune).Single();
                toGo.Add(PathFinder.DijkstraAlgo(graph, begin, finish));
            }

            if (toGo.Count == 0) return new DijkstraAnswer(new List<Node>(), 0);
            var min = toGo.Where(x => x.PathLength != 0).Select(x => x.PathLength).Min();
            var choice = toGo.Where(x => x.PathLength == min).First();
            return choice;
        }
    }
}
