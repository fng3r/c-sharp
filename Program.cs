using System;
using Pudge;
using Pudge.Player;
using Pudge.World;
using AIRLab.Mathematics;
using System.Collections.Generic;
using System.Linq;
using Pudge.Sensors;

namespace PudgeClient
{
    class Program
    {
        static List<Node> Empty = new List<Node>();
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
                args = new[] { "127.0.0.1", "14000" };
            //args = new[] { "87.224.245.130", "14001" };

            var ip = args[0];
            var port = int.Parse(args[1]);


            // Каждую неделю клиент будет новый. Соотетственно Level1, Level2 и Level3.
            var client = new PudgeClientLevel3();

            // У этого метода так же есть необязательные аргументы:
            // timeLimit -- время в секундах, сколько будет идти матч (по умолчанию 90)
            // operationalTimeLimit -- время в секундах, отображающее ваш лимит на операции в сумме за всю игру
            // По умолчанию -- 1000. На турнире будет использоваться значение 5. Подробнее про это можно прочитать в правилах.
            // isOnLeftSide -- предпочитаемая сторона. Принимается во внимание во время отладки. По умолчанию true.
            // seed -- источник энтропии для случайного появления рун. По умолчанию -- 0. 
            // При изменении руны будут появляться в другом порядке
            // speedUp -- ускорение отладки в два раза. Может вызывать снижение FPS на слабых машинах




            var data = client.Configurate(ip, port, CvarcTag, seed: 119, operationalTimeLimit: 1000);

            // Пудж узнает о всех событиях, происходящих в мире, с помощью сенсоров.
            // Для передачи и представления данных с сенсоров служат объекты класса PudgeSensoвс rsData.
            Print(data);

            // Каждое действие возвращает новые данные с сенсоров.


            // Для удобства, можно подписать свой метод на обработку всех входящих данных с сенсоров.
            // С этого момента любое действие приведет к отображению в консоли всех данных
            client.SensorDataReceived += Print;

            var rules = PudgeRules.Current;
            var points = PrepareForBattle.GetPoints();
            var graph = PrepareForBattle.MakeGraph(points);
            var excNodes = new int[] { 10, 11, 13, 20, 6, 7, 3, 15 };
            var excluded = graph.Nodes.Where(x => excNodes.Contains(x.NodeNumber)).ToList();
            var runes = PrepareForBattle.GetRunes();
            var specRunes = PrepareForBattle.GetSpecialRunes();
            var visited = new WorldInfo(rules.RuneRespawnTime);
            var killed = new WorldInfo(rules.SlardarRespawnTime);
            var slardarSpots = PrepareForBattle.GetSlardars();
            var central = new Point2D(0, 0);

            while (true)
            {
                var choices = new List<DijkstraAnswer>();
                choices.Add(InvestigateWorld(data, graph, new Point2D[] { central }, visited, Empty));
                var slPath = (InvestigateWorld(data, graph, slardarSpots, killed, Empty));
                var specPath = (InvestigateWorld(data, graph, specRunes, visited, Empty));
                EventData hookData;
                if (data.Events.Select(x => x.Event).Contains(PudgeEvent.HookCooldown))
                {
                    hookData = data.Events.Select(x => x).Where(x => x.Event == PudgeEvent.HookCooldown).First();
                    var time = hookData.Duration - (data.WorldTime - hookData.Start);
                    if (time < slPath.PathLength / rules.MovementVelocity)
                    {
                        choices.Add(slPath);
                        choices.Add(specPath);
                    }
                } else
                {
                    choices.Add(slPath);
                    choices.Add(specPath);
                }
                var chosen = InvestigateWorld(data, graph, runes, visited, excluded);
                if (choices.Any(x => x.PathLength != 0))
                {
                    var min = choices.Where(x => x.PathLength != 0).Min(x => x.PathLength);
                    chosen = choices.Where(x => x.PathLength == min).Single();
                }
                var slCD = killed.GetCooldown(data.WorldTime);
                if (slCD < 4.5)
                {
                    var nearest = slardarSpots.Where(x => Movement.ApproximatelyEqual(data.SelfLocation, x, 50)).SingleOrDefault();
                    var hookCD = double.Epsilon;
                    if (nearest != default(Point2D))
                    {
                        foreach (var eventData in data.Events)
                        {
                            if (eventData.Event == PudgeEvent.HookCooldown)
                            {
                                hookCD = eventData.Duration - (data.WorldTime - eventData.Start);
                                break;
                            }
                        }
                        if (hookCD < slCD)
                        {
                            client.GoTo(data, nearest, visited, killed);
                            var dist = Movement.GetDistance(nearest, data.SelfLocation);
                            client.Wait(slCD - dist / rules.MovementVelocity - 0.5);
                        }
                    }
                }

                var path = chosen.Path.Skip(1);
                if (path.Count() == 0)
                {
                    data = client.Wait(0.2);
                    continue;
                }
                data = PartWalking(data, client, path, visited, killed);
            }
        }

        public static PudgeSensorsData PartWalking(PudgeSensorsData data, PudgeClientLevel3 client, IEnumerable<Node> path, WorldInfo visited, WorldInfo killed)
        {            foreach (var node in path)
            {
                visited.Check(data.WorldTime);
                killed.Check(data.WorldTime);
                data = client.GoTo(data, node.Location, visited, killed);
                if (data.IsDead)
                {
                    for (int i = 0; i < 1; i++)
                    {
                        data = client.Wait(PudgeRules.Current.PudgeRespawnTime);
                        visited.Check(data.WorldTime);
                        killed.Check(data.WorldTime);
                    }
                    return data;
                }

            }
            var destination = path.Last().Location;
            visited.Add(destination);
            data = client.Wait(0.1);
            return data;
        }


        public static DijkstraAnswer InvestigateWorld(PudgeSensorsData data, Graph graph, IEnumerable<Point2D> runes, WorldInfo visited, List<Node> excluded)
        {
            var toGo = new List<DijkstraAnswer>();
            foreach (var rune in runes)
            {
                if (visited.HashSet.Contains(rune))
                    continue ;
                var loc = data.SelfLocation;
                var minimal = graph.Nodes.Select(x => Movement.GetDistance(x.Location, loc)).Min();
                var start = graph.Nodes.Where(x => Movement.GetDistance(x.Location, loc) == minimal).Single() ;
                var finish = graph.Nodes.Where(x => x.Location == rune).Single();
                if (start != finish)
                {
                    var path = PathFinder.DijkstraAlgo(graph, start, finish, excluded);
                    if (path != null)
                        toGo.Add(path);
                }
            }

            if (toGo.Count == 0) return new DijkstraAnswer(new List<Node>(), 0);
            var min = toGo.Select(x => x.PathLength).Min();
            var choice = toGo.Where(x => x.PathLength == min).First();
            return choice;
        }
    }
}
