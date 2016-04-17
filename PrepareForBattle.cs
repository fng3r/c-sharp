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
                new Point2D(-129, 130)
            };
        }

        public static Point2D[] GetSlardars()
        {
            return new Point2D[]
            {
                new Point2D(91, -95),
                new Point2D(-91, 95)
                //new Point2D(-105, 80),
                //new Point2D(105, -80)
            };
        }

        public static Point2D[] GetPoints()
        {
            return new Point2D[]
            {
                new Point2D(-115, -115),     //0
                new Point2D(-70, -120),      //1
                new Point2D(-10, -110),      //2
                new Point2D(10, -80),     //3
                new Point2D(-50, -20),   //4
                new Point2D(50, -20),    //5
                new Point2D(0 , 0),     //6
                new Point2D(-90, 25),   //7 (23)
                new Point2D(-120, -15),   //8
                new Point2D(90, -25), // 9 (24)
                new Point2D(120, 15), //10
                new Point2D(-120, -70), //11
                new Point2D(120, 70), //12
                new Point2D(-45, 30),  //13
                new Point2D(45, 30), //14
                new Point2D(-10, 80), //15
                new Point2D(10, 110), //16
                new Point2D(70, 120), //17
                new Point2D(115, 115), //18
                new Point2D(105, -80), //19
                new Point2D(130, -130), //20
                new Point2D(-105, 80), //21
                new Point2D(-129, 130), //22
                new Point2D(70, -100), //23
                new Point2D(-70, 100), //24
                new Point2D(91, -95), //25
                new Point2D(-91, 95) //26
            };

        }

        public static Graph MakeGraph(Point2D[] points)
        {
            var graph = new Graph(points);
            return graph.MakeBounds(
                0, 1,
                1, 2,
                2, 3,
                2, 5,
                2, 4,
                2, 6,
                2, 23,
                3, 4,
                3, 5,
                3, 6,
                3, 19,
                3, 23,
                4, 6,
                4, 8,
                4, 7,
                5, 10,
                5, 9,
                5, 6,
                6, 13,
                6, 14,
                6, 16,
                6, 15,
                7, 13,
                7, 8,
                7, 21,
                7, 24,
                8, 11,
                8, 13,
                8, 21,
                9, 10,
                9, 14,
                9, 19,
                9, 23,
                10, 14,
                10, 12,
                10, 19,
                11, 0,
                12, 18,
                13, 15,
                14, 16,
                15, 16,
                15, 24,
                15, 21,
                16, 17,
                16, 24,
                17, 18,
                19, 20,
                20, 23,
                21, 22,
                22, 24,
                25, 23,
                25, 19,
                25, 20,
                26, 21,
                26, 24,
                26, 22
                );
        }
    }
}
