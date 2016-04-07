using AIRLab.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PudgeClient
{
    public static class NodeExtensions
    {
        public static IEnumerable<Node> DepthSearch(this Node startNode)
        {
            var visited = new HashSet<Node>();
            var stack = new Stack<Node>();
            stack.Push(startNode);
            while (stack.Count != 0)
            {
                var node = stack.Pop();
                if (visited.Contains(node)) continue;
                visited.Add(node);
                yield return node;
                foreach (var incidentNode in node.IncidentNodes)
                    stack.Push(incidentNode);
            }
        }

        public static IEnumerable<Node> BreadthSearch(this Node startNode)
        {
            var visited = new HashSet<Node>();
            var queue = new Queue<Node>();
            queue.Enqueue(startNode);
            visited.Add(startNode);
            while (queue.Count != 0)
            {
                var node = queue.Dequeue();
                yield return node;
                foreach (var incidentNode in node.IncidentNodes)
                {
                    if (visited.Contains(incidentNode))
                    {
                        Console.WriteLine("!");
                    }
                    else
                    {
                        visited.Add(incidentNode);
                        queue.Enqueue(incidentNode);
                    }
                }
            }
        }
    }

    public class Edge
    {
        public readonly Node From;
        public readonly Node To;
        public readonly double Weight;
        public Edge(Node first, Node second)
        {
            this.From = first;
            this.To = second;
            var dx = first.Location.X - second.Location.X;
            var dy = first.Location.Y - second.Location.Y;
            Weight = Math.Sqrt(dx * dx + dy * dy);
        }
        public bool IsIncident(Node node)
        {
            return From == node || To == node;
        }
        public Node OtherNode(Node node)
        {
            if (!IsIncident(node)) throw new ArgumentException();
            if (From == node) return To;
            return From;
        }
    }

    public class Node
    {
        readonly List<Edge> edges = new List<Edge>();
        public readonly int NodeNumber;
        public readonly Point2D Location;

        public Node(int number, Point2D p)
        {
            NodeNumber = number;
            Location = p;
        }

        public IEnumerable<Node> IncidentNodes
        {
            get
            {
                return edges.Select(z => z.OtherNode(this));
            }
        }
        public IEnumerable<Edge> IncidentEdges
        {
            get
            {
                foreach (var e in edges) yield return e;
            }
        }
        public static Edge Connect(Node node1, Node node2, Graph graph)
        {
            if (!graph.Nodes.Contains(node1) || !graph.Nodes.Contains(node2)) throw new ArgumentException();
            var edge = new Edge(node1, node2);
            node1.edges.Add(edge);
            node2.edges.Add(edge);
            return edge;
        }
        public static void Disconnect(Edge edge)
        {
            edge.From.edges.Remove(edge);
            edge.To.edges.Remove(edge);
        }
    }

    public class Graph
    {
        private Node[] nodes;

        public Graph(Point2D[] p)
        {
            nodes = new Node[p.Length];
            for (int i = 0; i < p.Length; i++)
                nodes[i] = new Node(i, p[i]);
        }

        public int Length { get { return nodes.Length; } }

        public Node this[int index] { get { return nodes[index]; } }


        public IEnumerable<Node> Nodes
        {
            get
            {
                foreach (var node in nodes) yield return node;
            }
        }

        public void Connect(int index1, int index2)
        {
            Node.Connect(nodes[index1], nodes[index2], this);
        }

        public void Delete(Edge edge)
        {
            Node.Disconnect(edge);
        }

        public IEnumerable<Edge> Edges
        {
            get
            {
                return nodes.SelectMany(z => z.IncidentEdges).Distinct();
            }
        }

        public Graph MakeBounds(params int[] incidentNodes)
        {
            for (int i = 0; i < incidentNodes.Length - 1; i += 2)
                Connect(incidentNodes[i], incidentNodes[i + 1]);
            return this;
        }

        public List<List<Node>> FindConnectedComponents()
        {
            var result = new List<List<Node>>();
            var markedNodes = new HashSet<Node>();
            while (true)
            {
                var nextNode = Nodes.Where(node => !markedNodes.Contains(node)).FirstOrDefault();
                if (nextNode == null) break;
                var breadthSearch = nextNode.BreadthSearch().ToList(); ;
                result.Add(breadthSearch.ToList());
                foreach (var node in breadthSearch)
                    markedNodes.Add(node);
            }
            return result;
        }
    }
}