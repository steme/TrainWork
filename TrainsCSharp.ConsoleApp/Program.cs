using System;
using System.Linq;
using System.Collections.Generic;
using TrainsCSharp.Domain;
using TrainsCSharp.Repository.Interfaces;
using TrainsCSharp.Repository.Repositories;

namespace TrainsCSharp.ConsoleApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            string fileName = "input.txt";
            string path = GetFilePath(fileName);

            //Instantiate Repository
            IRouteRepository repository = new RouteTextAdapterRepository(path);

            //Inject repository into domain
            IRoutingDomain domain = new RoutingDomain(repository);

            int defaultMaxDistance = 30;

            #region Output 1

            var outputOneRequest = new LinkedList<char>();
            outputOneRequest.AddLast('A');
            outputOneRequest.AddLast('B');
            outputOneRequest.AddLast('C');

            try
            {
                var abcRouteDistance = domain.GetSpeicificRoutesDistance(outputOneRequest, defaultMaxDistance);
                Console.WriteLine($"Output #01: {abcRouteDistance}");
            }
            catch (Core.Exceptions.RouteNotFoundException exc)
            {
                Console.WriteLine($"Output #01: {exc.StandardRouteNotFoundUserMessage}");
            }


            #endregion

            #region Output 2

            var outputTwoRequest = new LinkedList<char>();
            outputTwoRequest.AddLast('A');
            outputTwoRequest.AddLast('D');

            try
            {
                var adRouteDistance = domain.GetSpeicificRoutesDistance(outputTwoRequest, defaultMaxDistance);
                Console.WriteLine($"Output #02: {adRouteDistance}");
            }
            catch (Core.Exceptions.RouteNotFoundException exc)
            {
                Console.WriteLine($"Output #02: {exc.StandardRouteNotFoundUserMessage}");
            }

            #endregion

            #region Output 3

            var outputThreeRequest = new LinkedList<char>();
            outputThreeRequest.AddLast('A');
            outputThreeRequest.AddLast('D');
            outputThreeRequest.AddLast('C');

            try
            {
                var adcRouteDistance = domain.GetSpeicificRoutesDistance(outputThreeRequest, defaultMaxDistance);
                Console.WriteLine($"Output #03: {adcRouteDistance}");
            }
            catch (Core.Exceptions.RouteNotFoundException exc)
            {
                Console.WriteLine($"Output #03: {exc.StandardRouteNotFoundUserMessage}");
            }

            #endregion

            #region Output 4

            var outputFourRequest = new LinkedList<char>();
            outputFourRequest.AddLast('A');
            outputFourRequest.AddLast('E');
            outputFourRequest.AddLast('B');
            outputFourRequest.AddLast('C');
            outputFourRequest.AddLast('D');

            try
            {
                var aebcdRouteDistance = domain.GetSpeicificRoutesDistance(outputFourRequest, defaultMaxDistance);
                Console.WriteLine($"Output #04: {aebcdRouteDistance}");
            }
            catch (Core.Exceptions.RouteNotFoundException exc)
            {
                Console.WriteLine($"Output #04: {exc.StandardRouteNotFoundUserMessage}");
            }

            #endregion

            #region Output 5

            var outputFiveRequest = new LinkedList<char>();
            outputFiveRequest.AddLast('A');
            outputFiveRequest.AddLast('E');
            outputFiveRequest.AddLast('D');

            try
            {
                var aedcdRouteDistance = domain.GetSpeicificRoutesDistance(outputFiveRequest, defaultMaxDistance);
                Console.WriteLine($"Output #05: {aedcdRouteDistance}");
            }
            catch (Core.Exceptions.RouteNotFoundException exc)
            {
                Console.WriteLine($"Output #05: {exc.StandardRouteNotFoundUserMessage}");
            }

            #endregion

            #region Output 6

            try
            {
                var count = domain.GetCountOfRoutesThatLoop('C', 3, 50);
                Console.WriteLine($"Output #06: {count}");
            }
            catch (Core.Exceptions.RouteNotFoundException exc)
            {
                Console.WriteLine($"Output #06: {exc.StandardRouteNotFoundUserMessage}");
            }

            #endregion

            #region Output 7

            try
            {
                var count = domain.GetNumberOfStopsWithEqualCount('A', 'C', 4, 50);
                Console.WriteLine($"Output #07: {count}");
            }
            catch (Core.Exceptions.RouteNotFoundException exc)
            {
                Console.WriteLine($"Output #07: {exc.StandardRouteNotFoundUserMessage}");
            }


            #endregion

            #region Output 8

            try
            {
                var distance = domain.GetShortestDistance('A', 'C', 10);
                Console.WriteLine($"Output #08: {distance}");
            }
            catch (Core.Exceptions.RouteNotFoundException exc)
            {
                Console.WriteLine($"Output #08: {exc.StandardRouteNotFoundUserMessage}");
            }

            #endregion

            #region Output 9

            try
            {
                var distance = domain.GetShortestDistance('B', 'B', 10);
                Console.WriteLine($"Output #09: {distance}");
            }
            catch (Core.Exceptions.RouteNotFoundException exc)
            {
                Console.WriteLine($"Output #09: {exc.StandardRouteNotFoundUserMessage}");
            }

            #endregion

            #region Output 10

            try
            {
                var count = domain.GetCountOfRoutesThatLoop('C', 'C', 30);
                Console.WriteLine($"Output #10: {count}");
            }
            catch (Core.Exceptions.RouteNotFoundException exc)
            {
                Console.WriteLine($"Output #10: {exc.StandardRouteNotFoundUserMessage}");
            }

            #endregion

            Console.ReadLine();

        }

        private static string GetFilePath(string fileName)
        {
            var Documents = System.IO.Directory.GetFiles("../../TrainsCSharp/").ToList();
            string path = string.Empty;
            if (Documents.Any(m => m.Contains(fileName)))
            {
                path = Documents.FirstOrDefault(m => m.Contains(fileName));
            }

            if (string.IsNullOrWhiteSpace(path))
            {
                Console.WriteLine("Failed to find the requested input text file..");
                Console.ReadLine();
                Environment.Exit(Environment.ExitCode);
            }

            return path;
        }
    }





}
