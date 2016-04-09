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
        const string CvarcTag = "85e664b5-7ce7-4a0c-88b8-82982d040b70";

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
                //args = new[] { "127.0.0.1", "14000" };
            args = new[] { "87.224.245.130", "14001" };

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


            // Для удобства, можно подписать свой метод на обработку всех входящих данных с сенсоров.
            // С этого момента любое действие приведет к отображению в консоли всех данных
            client.SensorDataReceived += Print;
            var points = PrepareForBattle.GetPoints();
            var graph = PrepareForBattle.MakeGraph(points);
            var runes = PrepareForBattle.GetRunes();
            var specRunes = PrepareForBattle.GetSpecialRunes();
            var visited = new RuneHashSet();

            while (true)
            {
                visited.Check(sensorData.WorldTime);
                var choice = InvestigateWorld(sensorData, graph, runes, visited);
                foreach (var dataEvent in sensorData.Events)
                {
                    if (dataEvent.Event == PudgeEvent.Invisible || dataEvent.Event == PudgeEvent.Hasted)
                    {
                        var choice1 = InvestigateWorld(sensorData, graph, specRunes, visited);
                        if (choice1.PathLength == 0)
                            break;
                        var timeRemaining = dataEvent.Duration - (sensorData.WorldTime - dataEvent.Start);
                        if (choice1.PathLength / 40 < timeRemaining)
                        {
                            choice = choice1;
                        }
                    }
                }
                var path = choice.Path.Skip(1);
                if (path.Count() == 0)
                {
                    sensorData = client.Wait(0.01);
                    continue;
                }
                foreach (var node in path)
                {
                    visited.Check(sensorData.WorldTime);
                    sensorData = client.GoTo(sensorData, node.Location, visited);
                    if (sensorData.IsDead)
                    {
                        client.Wait(5);
                        break;
                    }

                }
                visited.HashSet.Add(path.Last().Location);
                sensorData = client.Wait(0.05);
            }

            // Корректно завершаем работу
            //client.Exit();
        }


        public static DijkstraAnswer InvestigateWorld(PudgeSensorsData data, Graph graph, Point2D[] runes, RuneHashSet visited)
        {
            var toGo = new List<DijkstraAnswer>();
            foreach (var rune in runes)
            {
                if (visited.HashSet.Contains(rune))
                    continue ;
                var loc = data.SelfLocation;
                var start = graph.Nodes.Where(x => Movement.ApproximatelyEqual(loc, x.Location, 5)).Single();
                var finish = graph.Nodes.Where(x => x.Location == rune).Single();
                toGo.Add(PathFinder.DijkstraAlgo(graph, start, finish));
            }

            if (toGo.Count == 0) return new DijkstraAnswer(new List<Node>(), 0);
            var min = toGo.Where(x => x.PathLength != 0).Select(x => x.PathLength).Min();
            var choice = toGo.Where(x => x.PathLength == min).First();
            return choice;
        }
    }
}

#region Trees
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
#endregion