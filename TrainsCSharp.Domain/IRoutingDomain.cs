using System;
using System.Collections.Generic;
using System.Text;
using TrainsCSharp.Core.Exceptions;

namespace TrainsCSharp.Domain
{
    public interface IRoutingDomain
    {

        int GetShortestDistance(char start, char end, int maxDistance);

        int GetSpeicificRoutesDistance(LinkedList<char> plannedRoute, int maxDistance);

        int GetCountOfRoutesThatLoop(char start, int maxStop, int maxDistance);

        int GetNumberOfStopsWithEqualCount(char start, char end, int match, int maxDistance);


    }
}
