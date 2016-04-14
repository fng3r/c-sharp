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
<<<<<<< HEAD
        public static Point2D[] GetRunes()
        {
            return new Point2D[]
            {
                new Point2D(-70, -130),
                new Point2D(0, 0),
                new Point2D(-130, -70),
                new Point2D(70, 130),
                new Point2D(130, 70)
            };
        }

        public static Point2D[] GetSpecialRunes()
=======
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
>>>>>>> refs/remotes/origin/master
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
<<<<<<< HEAD
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
                new Point2D(-58, 38),           //13
                new Point2D(58, 38),            //14
                new Point2D(0, 70),             //15
                new Point2D(0, 130),            //16
                new Point2D(70, 130),           //17
                new Point2D(130, 130),          //18
                new Point2D(100, -80),          //19
                new Point2D(130, -130),         //20
                new Point2D(-100, 80),          //21
                new Point2D(-130, 130),         //22
                new Point2D(-110, 50),          //23
                new Point2D(110, -50),          //24
                new Point2D(-50, 85),           //25
                new Point2D(50, -85),            // 26
                new Point2D(70, -100),             //27
                new Point2D(-70, 100),            //28
=======
                new Point2D(-115, -115),     //0
                new Point2D(-70, -120),      //1
                new Point2D(-10, -110),      //2
                new Point2D(10, -80),     //3
                new Point2D(-50, -20),   //4
                new Point2D(50, -20),    //5
                new Point2D(0 , 0),     //6
                new Point2D(-90, 25),   //7 (23)
                new Point2D(-115, -15),   //8
                new Point2D(90, -25), // 9 (24)
                new Point2D(115, 15), //10
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
                new Point2D(-130, 130), //22
                new Point2D(70, -100), //23
                new Point2D(-70, 100), //24

>>>>>>> refs/remotes/origin/master
            };

        }

        public static Graph MakeGraph(Point2D[] points)
        {
            var graph = new Graph(points);
            return graph.MakeBounds(
                0, 1,
                1, 2,
                2, 3,
<<<<<<< HEAD
                3, 4,
                3, 5,
                3, 6,
                4, 6,
                //4, 5,
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
                2, 28,
                16, 27,
                20, 28,
                22, 27,
                26, 27,
                25, 28
=======
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
                22, 24
>>>>>>> refs/remotes/origin/master
                );
        }
    }
}
