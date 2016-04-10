using AIRLab.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PudgeClient
{
    class PrepareForBattle
    {
        public static IEnumerable<Point2D> GetAllRunes()
        {
            var first = GetRunes();
            var second = GetSpecialRunes();
            return first.Concat(second);
        }

        public static IEnumerable<Point2D> GetRunes()
        {
            return new Point2D[]
            {
                new Point2D(-70, -120),
                new Point2D(0, 0),
                new Point2D(-120, -70),
                new Point2D(70, 120),
                new Point2D(120, 70)
            };
        }

        public static IEnumerable<Point2D> GetSpecialRunes()
        {
            return new Point2D[]
            {
                new Point2D(130, -130),
                new Point2D(-130, 130)
            };
        }

        public static Point2D[] GetPoints()
        {
            return new Point2D[]
            {
                new Point2D(-130, -130),        //0
                new Point2D(-70, -120),         //1
                new Point2D(0, -110),           //2
                new Point2D(0, -80),            //3
                new Point2D(-55, -28),          //4
                new Point2D(55, -28),           //5
                new Point2D(0, 0),              //6
                new Point2D(-75, 0),            //7
                new Point2D(-130, -5),           //8
                new Point2D(75, 0),             //9
                new Point2D(130, -5),            //10
                new Point2D(-120, -70),         //11
                new Point2D(120, 70),           //12
                new Point2D(-58, 38),           //13
                new Point2D(58, 38),            //14
                new Point2D(0, 80),             //15
                new Point2D(0, 110),            //16
                new Point2D(70, 120),           //17
                new Point2D(130, 130),          //18
                new Point2D(100, -80),          //19
                new Point2D(130, -130),         //20
                new Point2D(-100, 80),          //21
                new Point2D(-130, 130),         //22
                new Point2D(-115, 50),          //23
                new Point2D(115, -50),          //24
                new Point2D(-50, 85),           //25
                new Point2D(50, -85),            // 26
                new Point2D(70, -100),             //27
                new Point2D(-70, 100),            //28
            };

        }

        public static Graph MakeGraph(Point2D[] points)
        {
            var graph = new Graph(points);
            return graph.MakeBounds(
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
                10, 19,
                8, 21,
                7, 23,
                21, 23,
                19, 24,
                9, 24,
                16, 27,
                22, 28,
                20, 27,
                26, 27,
                25, 28,
                24, 10,
                10, 5,
                8, 4,
                26, 2,
                26, 3,
                25, 16,
                25, 15,
                2, 27,
                16, 28
                );
        }
    }
}
