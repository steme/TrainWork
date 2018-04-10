using System.Collections.Generic;
using TrainsCSharp.Domain;
using Xunit;
using Moq;
using TrainsCSharp.Repository.Interfaces;
using TrainsCSharp.Repository.Models;

namespace TrainsCSharp.Test
{
    public class RoutingTests
    {

        private Mock<IRouteRepository> _routeRepositoryMock;

        public RoutingTests()
        {
            _routeRepositoryMock = new Mock<IRouteRepository>();

            List<Route> fakeRouteData = new List<Route>()
            {
                new Route(){StartPoint =  'A', EndPoint = 'B', Distance = 5},
                new Route(){StartPoint =  'B', EndPoint = 'C', Distance = 4},
                new Route(){StartPoint =  'C', EndPoint = 'D', Distance = 8},
                new Route(){StartPoint =  'D', EndPoint = 'C', Distance = 8},
                new Route(){StartPoint =  'D', EndPoint = 'E', Distance = 6},
                new Route(){StartPoint =  'A', EndPoint = 'D', Distance = 5},
                new Route(){StartPoint =  'C', EndPoint = 'E', Distance = 2},
                new Route(){StartPoint =  'E', EndPoint = 'B', Distance = 3},
                new Route(){StartPoint =  'A', EndPoint = 'E', Distance = 7}
            };

            _routeRepositoryMock.Setup(m => m.GetAll()).Returns(fakeRouteData);
        }


        [Fact]
        public void ChallengeOneProof()
        {
            IRoutingDomain domain = new RoutingDomain(_routeRepositoryMock.Object);

            var request = new LinkedList<char>();

            request.AddLast('A');
            request.AddLast('B');
            request.AddLast('C');

            int distance = domain.GetSpeicificRoutesDistance(request, 20);

            Assert.Equal(9, distance);

        }

        [Fact]
        public void ChallengeTwoProof()
        {

            IRoutingDomain domain = new RoutingDomain(_routeRepositoryMock.Object);

            var request = new LinkedList<char>();

            request.AddLast('A');
            request.AddLast('D');

            int distance = domain.GetSpeicificRoutesDistance(request, 20);

            Assert.Equal(5, distance);

        }

        [Fact]
        public void ChallengeThreeProof()
        {
            IRoutingDomain domain = new RoutingDomain(_routeRepositoryMock.Object);

            var request = new LinkedList<char>();

            request.AddLast('A');
            request.AddLast('D');
            request.AddLast('C');

            int distance = domain.GetSpeicificRoutesDistance(request, 20);

            Assert.Equal(13, distance);
        }

        [Fact]
        public void ChallengeFourProof()
        {

            IRoutingDomain domain = new RoutingDomain(_routeRepositoryMock.Object);

            var request = new LinkedList<char>();

            request.AddLast('A');
            request.AddLast('E');
            request.AddLast('B');
            request.AddLast('C');
            request.AddLast('D');

            int distance = domain.GetSpeicificRoutesDistance(request, 50);

            Assert.Equal(22, distance);

        }

        [Fact]
        public void ChallengeFiveProof()
        {

            IRoutingDomain domain = new RoutingDomain(_routeRepositoryMock.Object);

            var request = new LinkedList<char>();

            request.AddLast('A');
            request.AddLast('E');
            request.AddLast('D');

            Assert.Throws<Core.Exceptions.RouteNotFoundException>(() => domain.GetSpeicificRoutesDistance(request, 50));

        }

        [Fact]
        public void ChallengeSixProof()
        {

            IRoutingDomain domain = new RoutingDomain(_routeRepositoryMock.Object);

            var count = domain.GetCountOfRoutesThatLoop('C', 3, 50);

            Assert.Equal(2, count);

        }

        [Fact]
        public void ChallengeSevenProof()
        {
            IRoutingDomain domain = new RoutingDomain(_routeRepositoryMock.Object);

            var count = domain.GetNumberOfStopsWithEqualCount('A', 'C', 4, 50);

            Assert.Equal(3, count);
        }

        [Fact]
        public void ChallengeEightProof()
        {
            IRoutingDomain domain = new RoutingDomain(_routeRepositoryMock.Object);

            var distance = domain.GetShortestDistance('A', 'C', 10);

            Assert.Equal(9, distance);

        }

        [Fact]
        public void ChallengeNineProof()
        {
            IRoutingDomain domain = new RoutingDomain(_routeRepositoryMock.Object);

            var distance = domain.GetShortestDistance('B', 'B', 10);

            Assert.Equal(9, distance);

        }

        [Fact]
        public void ChallengeTenProof()
        {
            IRoutingDomain domain = new RoutingDomain(_routeRepositoryMock.Object);

            var count = domain.GetCountOfRoutesThatLoop('C', 'C', 30);

            Assert.Equal(7, count);

        }



    }
}
