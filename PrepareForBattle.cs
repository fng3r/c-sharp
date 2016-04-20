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
                new Point2D(0, -100),
                new Point2D(0, 0),
                new Point2D(0, 100),
                new Point2D(100, 0),
                new Point2D(-100, 0)
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

        public static Point2D[] GetSlardars()
        {
            return new Point2D[]
            {
                new Point2D(90, -95),
                new Point2D(-90, 95)
                //new Point2D(-105, 80),
                //new Point2D(105, -80)
            };
        }

        public static Point2D[] GetPoints()
        {
            return new Point2D[]
            {
                new Point2D(0, 0),          //0
                new Point2D(-80, -105),     //1
                new Point2D(0, -100),       //2
                new Point2D(20, -100),      //3
                new Point2D(70, -100),      //4
                new Point2D(130, -130),     //5
                new Point2D(90, -95),       //6
                new Point2D(100, -70),      //7
                new Point2D(-105, -80),     //8
                new Point2D(-100, 0),       //9
                new Point2D(-90, 20),       //10
                new Point2D(-100, 70),      //11
                new Point2D(-130,130),      //12
                new Point2D(-90, 95),       //13
                new Point2D(-70, 100),      //14
                new Point2D(-20, 90),       //15
                new Point2D(0, 100),        //16
                new Point2D(80, 105),       //17
                new Point2D(105, 80),       //18
                new Point2D(100, 0),        //19
                new Point2D(90, -20),       //20
                new Point2D(-130, -130),    //21
                new Point2D(130, 130),      //22

                


            };

        }

        public static Graph MakeGraph(Point2D[] points)
        {
            var graph = new Graph(points);
            return graph.MakeBounds(
                1, 2,
                1, 0,
                1, 8,
                2, 3,
                2, 4,
                2, 6,
                3, 0,
                3, 20,
                4, 5,
                5, 7,
                5, 6,
                6, 20,
                6, 0,
                7, 19,
                8, 9,
                8, 0,
                9, 10,
                9, 11,
                10, 15,
                10, 0,
                11, 12,
                12, 14,
                12, 13,
                13, 0,
                14, 15,
                15, 0,
                15, 16,
                16, 17, 
                17, 18,
                17, 0,
                18, 0,
                18, 19,
                19, 20,
                20, 0,
                21, 1,
                21, 8,
                22, 17,
                22, 18,
                21, 0,
                22, 0




                
                );
        }
    }
}
