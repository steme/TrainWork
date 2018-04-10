using System;
using System.Collections.Generic;
using System.Text;
using TrainsCSharp.Repository.Models;

namespace TrainsCSharp.Repository.Interfaces
{
    public interface IRouteRepository
    {
        List<Route> GetAll();
    }
}
