using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrainsCSharp.Repository.Interfaces;
using TrainsCSharp.Repository.Models;

namespace TrainsCSharp.Repository.Repositories
{
    public class RouteTextAdapterRepository : IRouteRepository
    {
        private string _connectionString;

        public RouteTextAdapterRepository(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new Exception("Connection string(file path) cannot be null or blank in the Route Text Adapter Repository");
            }

            _connectionString = connectionString;
        }

        public List<Route> GetAll()
        {
            List<Route> result = new List<Route>();

            string line;
            System.IO.StreamReader file = new System.IO.StreamReader(_connectionString);
            while ((line = file.ReadLine()) != null)
            {
                result.AddRange(ParseLineToGetRoutes(line));
            }

            return result;
        }

        private List<Route> ParseLineToGetRoutes(string lineOfText)
        {
            List<Route> result = new List<Route>();

            var splitLine = lineOfText.Split(',');

            if (splitLine != null && splitLine.Any())
            {
                foreach (var text in splitLine)
                {
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        List<char> chars = new List<char>(text.Trim());

                        if (chars.Count() == 3)
                        {
                            if (char.IsLetter(chars[0]) && char.IsLetter(chars[1]) && char.IsNumber(chars[2]))
                            {
                                Route route = new Route();
                                route.StartPoint = chars[0];
                                route.EndPoint = chars[1];
                                route.Distance = (int)char.GetNumericValue(chars[2]);
                                result.Add(route);
                            }
                        }
                    }
                }
            }

            return result;

        }
    }
}
