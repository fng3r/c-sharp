﻿using System;
using Pudge;
using Pudge.Player;

namespace PudgeClient
{
    class Program
    {
        const string CvarcTag = "Получи кварк-тэг на сайте";

        // Пример визуального отображения данных с сенсоров при отладке.
        // Если какая-то информация кажется вам лишней, можете закомментировать что-нибудь.
        static PudgeSensorsData MoveTo(PudgeClientLevel1 client, PudgeSensorsData data, double x, double y)
        {
            client.Wait(0.5);
            Console.WriteLine("current: X - {0},    Y - {1}", data.SelfLocation.X, data.SelfLocation.Y);
            var dx = x - data.SelfLocation.X;
            var dy = y - data.SelfLocation.Y;
            if (data.SelfLocation.Angle < 0) data.SelfLocation.Angle += 360;
            Console.WriteLine("DX: {0}, DY: {1}", dx, dy);
            var angle = Math.Atan2(dy, dx) * 180 / Math.PI;
            Console.WriteLine("needed: {0}, self: {1}", angle, data.SelfLocation.Angle);
            var rAngle = (angle - data.SelfLocation.Angle) % 360;
            if (Math.Abs(rAngle) > 180)
                rAngle += -Math.Sign(rAngle) * 360;
            Console.WriteLine("!!" + rAngle);
            client.Rotate(rAngle);
            return client.Move(Math.Sqrt(dx * dx + dy * dy));
        }

        static void Print(PudgeSensorsData data)
        {
            Console.WriteLine("---------------------------------");
            if (data.IsDead)
            {
                // Правильное обращение со смертью.
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
            var sensorData = client.Configurate(ip, port, CvarcTag);

            // Каждое действие возвращает данные с сенсоров.
            Print(sensorData);
            sensorData = MoveTo(client, sensorData, 0, -123);
            sensorData = MoveTo(client, sensorData, 0, 0);
            sensorData = MoveTo(client, sensorData, 0, 70);
            sensorData = MoveTo(client, sensorData, -48, 38);
            sensorData = MoveTo(client, sensorData, -83, 0);
            sensorData = MoveTo(client, sensorData, -146, 0);
            sensorData = MoveTo(client, sensorData, -120, -70);


            Console.WriteLine(Math.Atan2(1, 0) + " " +  Math.Atan2(0, 1));
            // Для удобства, можно подписать свой метод на обработку всех входящих данных с сенсоров.
            // С этого момента любое действие приведет к отображению в консоли всех данных
            client.SensorDataReceived += Print;
            
            // Угол поворота указывается в градусах, против часовой стрелки.
            // Для поворота по часовой стрелке используйте отрицательные значения.
            //client.Rotate(-45);

            //client.Move(60);
            //client.Wait(0.1);

            //// Так можно хукать, но на первом уровне эта команда будет игнорироваться.
            //client.Hook();
            //client.Rotate(-170);
            //client.Move();
            //client.Wait(4);
            //client.Rotate(351);
            //client.Move();
            //for (int i = 0; i < 10; i++)
            //{
            //    if (i == 1) client.Rotate(90);
            //    client.Move(50);
            //}

            //// Пример длинного движения. Move(100) лучше не писать. Мало ли что произойдет за это время ;) 
            //for (int i = 0; i < 50; i++)
            //{
            //    client.Move(3);
            //}
            //client.Wait(5);
            //// Корректно завершаем работу
            client.Exit();
        }
    }
}
