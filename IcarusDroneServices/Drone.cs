using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IcarusDroneServices
{
    public class Drone
    {
        private int _serviceTag;
        private string _clientName;
        private string _droneModel;
        private string _serviceProblem;
        private double _serviceCost;

        //get set client name
        public string ClientName
        {
            get { return _clientName; }
            set
            {
                TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                _clientName = textInfo.ToTitleCase(value.ToLower()); 
            }
        }

        //get set drone model
        public string DroneModel
        {
            get { return _droneModel; }
            set
            {
                _droneModel = value;
            }
        }

        //get set service problem
        public string ServiceProblem
        {
            get { return _serviceProblem; }
            set
            {
                string problem = value.ToLower();
                _serviceProblem = char.ToUpper(problem[0]) + problem.Substring(1);
            }
        }

        //get set service cost
        public double ServiceCost
        {
            get { return _serviceCost; }
            set
            {
                _serviceCost = Math.Round(value,2);
            }
        }

        //get set service tag
        public int ServiceTag
        {
            get { return _serviceTag; }
            set
            {
                _serviceTag = value;
            }
        }

        public string display()
        {
            return $"Client Name: {_clientName}\nService Cost: {_serviceCost}";

        }

    }
}
