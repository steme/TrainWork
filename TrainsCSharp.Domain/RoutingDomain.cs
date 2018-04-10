using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using TrainsCSharp.Repository.Interfaces;
using TrainsCSharp.Repository.Models;
using TrainsCSharp.Core.Exceptions;
using System.Security.Cryptography;

namespace TrainsCSharp.Domain
{

    /// <summary>
    /// This is the routing domain, it contains business logic that is tightly coupled to a directed graph routing system for a passenger rail line
    /// </summary>
    public class RoutingDomain : IRoutingDomain
    {

        /// <summary>
        /// Private Data Access field for Route Repository
        /// </summary>
        private IRouteRepository _routeRepository;

        /// <summary>
        /// To construct this domain you must have an instance of IRouteRepostiory
        /// </summary>
        /// <param name="routeRepo">Data Access Dependency</param>
        public RoutingDomain(IRouteRepository routeRepo)
        {
            _routeRepository = routeRepo;
        }

        /// <summary>
        /// Get the shortest possible distance between any two train platforms
        /// </summary>
        /// <param name="start">Starting Point</param>
        /// <param name="end">Ending Point</param>
        /// <param name="maxDistance">Depth parameter for graph algorithm</param>
        /// <exception cref="RouteNotFoundException"
        /// <returns>distance in integrer form or a RouteNotFoundException exception</returns>
        public int GetShortestDistance(char start, char end, int maxDistance)
        {
            var routes = _routeRepository.GetAll();

            var routeDomainItems = GetRouteDomainItemsForData(routes);

            RouteScenarioContainer container = new RouteScenarioContainer(start, maxDistance);

            TraverseRouteScenarios(routeDomainItems, ref container, true);

            int distance = 0;

            if (container.Passengers.Any())
            {
                distance = container.Passengers
                    .Where(m => m.RouteMap.First().CompareTo(start) == 0 && m.RouteMap.Last().CompareTo(end) == 0)
                    .Select(m => m.DistanceTraveled)
                    .OrderBy(m => m)
                    .FirstOrDefault();
            }

            if (distance == 0)
            {
                throw new RouteNotFoundException();
            }

            return distance;
        }

        /// <summary>
        /// Get the distance between any number of linked platforms in the train system
        /// </summary>
        /// <param name="plannedRoute">A linked list of route points</param>
        /// <param name="maxDistance">Depth parameter for graph algorithm</param>
        /// <exception cref="RouteNotFoundException"
        /// <returns></returns>
        public int GetSpeicificRoutesDistance(LinkedList<char> plannedRoute, int maxDistance)
        {
            var routes = _routeRepository.GetAll();

            var routeDomainItems = GetRouteDomainItemsForData(routes);

            RouteScenarioContainer container = new RouteScenarioContainer(plannedRoute.First(), maxDistance);

            TraverseRouteScenarios(routeDomainItems, ref container, true);

            if (container.Passengers.Any())
            {
                foreach (var passenger in container.Passengers)
                {
                    if (plannedRoute.SequenceEqual(passenger.RouteMap))
                    {
                        return passenger.DistanceTraveled;
                    }
                }
            }

            throw new RouteNotFoundException();
        }

        /// <summary>
        /// Gets the count of routes that start and finish at the same train station
        /// </summary>
        /// <param name="start">Starting Platform</param>
        /// <param name="maxStop">Maximum number of platoforms on this route</param>
        /// <param name="maxDistance">Depth parameter for graph algorithm</param>
        /// <exception cref="RouteNotFoundException"
        /// <returns>number of routes that loop back to starting point</returns>
        public int GetCountOfRoutesThatLoop(char start, int maxStop, int maxDistance)
        {
            var routes = _routeRepository.GetAll();

            var routeDomainItems = GetRouteDomainItemsForData(routes);

            RouteScenarioContainer container = new RouteScenarioContainer(start, maxDistance);

            TraverseRouteScenarios(routeDomainItems, ref container, true);

            if (container.Passengers.Any())
            {
                if (maxStop > 0)
                {
                    return container.Passengers.Where(m => m.RouteMap.First().CompareTo(start) == 0 && m.RouteMap.Last().CompareTo(start) == 0 && m.Stops.Count() <= maxStop).Count();
                }
                else
                {
                    return container.Passengers.Count();
                }
            }

            throw new RouteNotFoundException();
        }

        /// <summary>
        /// Get the number of route variations with the same start and end point
        /// </summary>
        /// <param name="start">Starting Platform</param>
        /// <param name="end">Destination Platform</param>
        /// <param name="count">Stop Count</param>
        /// <exception cref="RouteNotFoundException"
        /// <param name="maxDistance">Depth parameter for graph algorithm</param>
        /// <returns>Number of stops with equal count between specified platforms</returns>
        public int GetNumberOfStopsWithEqualCount(char start, char end, int count, int maxDistance)
        {
            var routes = _routeRepository.GetAll();

            var routeDomainItems = GetRouteDomainItemsForData(routes);

            RouteScenarioContainer container = new RouteScenarioContainer(start, maxDistance);

            TraverseRouteScenarios(routeDomainItems, ref container, true);

            if (container.Passengers.Any())
            {
                return container.Passengers.Where(m => m.RouteMap.First().CompareTo(start) == 0 && m.RouteMap.Last().CompareTo(end) == 0 && m.Stops.Count() == count).Count();
            }

            throw new RouteNotFoundException();
        }


        /// <summary>
        /// This is the algorithm we use the run scenarios through our rail system. We take a supplied list of routes and traverse all possible scenarios with the specific starting point.
        /// </summary>
        /// <param name="routes">List if routes in the train system</param>
        /// <param name="container">A reference object that we store the scenario data in</param>
        /// <param name="outerLoop">A boolean that helps us determine what layer of the method we are in</param>
        private void TraverseRouteScenarios(List<RouteDomainItem> routes, ref RouteScenarioContainer container, bool outerLoop)
        {
            foreach (var route in routes)
            {
                //if we are in the outer loop and the starting point provided matches the starting point of the route then we need to create a new passenger instance
                if (outerLoop && route.StartingLocation.CompareTo(container.StartingLocation) == 0)
                {
                    RoutePassenger passenger = new RoutePassenger()
                    {
                        TravelingToward = route.Destination,
                        DistanceTraveled = route.Distance,
                        Stops = new LinkedList<char>(),
                        RouteMap = new LinkedList<char>(),
                    };
                    //we use the route map to keep track of everywhere the passenger has been
                    passenger.RouteMap.AddLast(route.StartingLocation);
                    passenger.RouteMap.AddLast(route.Destination);
                    //we use the stops to map everywhere the passenger has arrived
                    passenger.Stops.AddLast(route.Destination);
                    //we create a hash to have an easy way to identify this passenger
                    passenger.SetHash();

                    container.Passengers.Add(passenger);
                    //We created our passenger, now lets recursively determine how many places he can possibly go in the specified distance 
                    TraverseRouteScenarios(routes, ref container, false);

                }//If we are in a inner loop we can examine our passengers and see if they have divergent options
                else
                {
                    for (var i = 0; i < container.Passengers.Count(); i++)
                    {
                        //if the upcoming route is where this passenger is traveling to
                        if (container.Passengers[i].TravelingToward.CompareTo(route.StartingLocation) == 0)
                        {
                            //since we need to preserve scenario data we clone our passenger inorder to create a new instance for tracking purposes
                            var clone = container.Passengers[i].Clone();
                            clone.Stops.AddLast(route.Destination);
                            clone.RouteMap.AddLast(route.Destination);
                            //since this passenger is divergent from the passenger that may have stopped we will create a new unique identifier
                            clone.SetHash();
                            clone.DistanceTraveled += route.Distance;
                            clone.TravelingToward = route.Destination;

                            //if we dont have a max distance traveled this passenger would travel forever and probably starve to death
                            //also we need to make sure this clone does not already exist in this universe otherwise we end up with duplicate data
                            //this is why we generated unique identifier with the hash function
                            if (clone.DistanceTraveled < container.MaxDistance && !container.Passengers.Any(m => string.Compare(m.Hash, clone.Hash, true) == 0))
                            {
                                container.Passengers.Add(clone);
                                //lets traverse all the new possibilities with this new clone
                                TraverseRouteScenarios(routes, ref container, false);
                            }

                        }
                    }
                }
            }
        }


        /// <summary>
        /// A little helper method to transform the route data model into a route domain model
        /// </summary>
        /// <param name="routes"></param>
        /// <returns></returns>
        private List<RouteDomainItem> GetRouteDomainItemsForData(List<Route> routes)
        {
            List<RouteDomainItem> result = new List<RouteDomainItem>();

            foreach (var route in routes)
            {
                result.Add(new RouteDomainItem(route.StartPoint, route.EndPoint, route.Distance));
            }

            return result;
        }

    }

    /// <summary>
    /// Our container for scenario recursion, a starting point and max distance is required
    /// </summary>
    class RouteScenarioContainer
    {

        public RouteScenarioContainer(char start, int maxDistance)
        {
            StartingLocation = start;
            Passengers = new List<RoutePassenger>();
            MaxDistance = maxDistance;
        }

        public char StartingLocation { get; set; }

        public int MaxDistance { get; set; }

        public List<RoutePassenger> Passengers { get; set; }

    }

    class RoutePassenger
    {
        public RoutePassenger()
        {
            Stops = new LinkedList<char>();
            RouteMap = new LinkedList<char>();
            Passangers = new List<RoutePassenger>();
        }

        public char TravelingToward { get; set; }

        public int DistanceTraveled { get; set; }

        public LinkedList<char> Stops { get; set; }

        public LinkedList<char> RouteMap { get; set; }

        public List<RoutePassenger> Passangers { get; set; }

        public string Hash { get; set; }

        /// <summary>
        /// we this to generate a unique id for our passenger
        /// </summary>
        public void SetHash()
        {

            using (MD5 md5Hash = MD5.Create())
            {

                StringBuilder builder = new StringBuilder();

                foreach (var item in RouteMap)
                {
                    builder.Append(item);
                }

                Hash = GetMd5Hash(md5Hash, builder.ToString());
            }

        }

        /// <summary>
        /// simple method that takes any string and returns an md5 hash of it
        /// </summary>
        /// <param name="md5Hash"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        private string GetMd5Hash(MD5 md5Hash, string input)
        {

            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

        /// <summary>
        /// We have to do some funky stuff to clone the passenger with new in memory data points to prevent issues with referenced datapoints
        /// </summary>
        /// <returns></returns>
        public RoutePassenger Clone()
        {
            var clone = new RoutePassenger()
            {
                DistanceTraveled = DistanceTraveled,
                TravelingToward = TravelingToward,
            };

            clone.Stops = new LinkedList<char>();
            foreach (var stop in Stops)
            {
                clone.Stops.AddLast(stop);
            }

            clone.RouteMap = new LinkedList<char>();
            foreach (var stop in RouteMap)
            {
                clone.RouteMap.AddLast(stop);
            }


            return clone;
        }
    }

    class RouteDomainItem
    {
        public RouteDomainItem(char start, char finish, int distance)
        {
            StartingLocation = start;
            Destination = finish;
            Distance = distance;
        }

        public char StartingLocation { get; set; }

        public char Destination { get; set; }

        public int Distance { get; set; }
    }
}
