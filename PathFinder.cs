using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PudgeClient
{
    class DijkstraData
    {
        public Node Previous { get; set; }
        public double Price { get; set; }
    }

    public class DijkstraAnswer
    {
        public readonly List<Node> Path;
        public readonly double PathLength;

        public DijkstraAnswer(List<Node> path, double len)
        {
            Path = path;
            PathLength = len;
        }
    }

    public class PathFinder
    {
        public static DijkstraAnswer DijkstraAlgo(Graph graph, Node start, Node end)
        {
            var notVisited = graph.Nodes.ToList();
            var track = new Dictionary<Node, DijkstraData>();
            track[start] = new DijkstraData { Price = 0, Previous = null };

            while (true)
            {
                Node toOpen = null;
                var bestPrice = double.PositiveInfinity;
                foreach (var e in notVisited)
                {
                    if (track.ContainsKey(e) && track[e].Price < bestPrice)
                    {
                        bestPrice = track[e].Price;
                        toOpen = e;
                    }
                }

                if (toOpen == null) return null;
                if (toOpen == end) break;

                foreach (var e in toOpen.IncidentEdges)
                {
                    var currentPrice = track[toOpen].Price + e.Weight;
                    var nextNode = e.OtherNode(toOpen);
                    if (!track.ContainsKey(nextNode) || currentPrice < track[nextNode].Price)
                    {
                        track[nextNode] = new DijkstraData { Previous = toOpen, Price = currentPrice };
                    }
                }

                notVisited.Remove(toOpen);
            }

            var result = new List<Node>();
            var price = track[end].Price;
            while (end != null)
            {
                result.Add(end);
                end = track[end].Previous;
            }
            result.Reverse();
            return new DijkstraAnswer(result, price);
        }
    }
}
