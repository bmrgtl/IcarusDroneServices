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
        // 6.1: Private Attributes
        private int _serviceTag;
        private string _clientName;
        private string _droneModel;
        private string _serviceProblem;
        private double _serviceCost;

        // 6.1: Getters and Setters

        // get set service tag
        public int ServiceTag
        {
            get { return _serviceTag; }
            set
            {
                _serviceTag = value;
            }
        }

        // get set client name
        public string ClientName
        {
            get { return _clientName; }
            set
            {
                // 6.1: Format client name to title case
                TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                _clientName = textInfo.ToTitleCase(value.ToLower()); 
            }
        }

        // get set drone model
        public string DroneModel
        {
            get { return _droneModel; }
            set
            {
                _droneModel = value;
            }
        }

        // get set service problem
        public string ServiceProblem
        {
            get { return _serviceProblem; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    _serviceProblem = ""; // extra-safety measure
                    return;
                }

                // 6.1: Format service problem to sentence case
                string problem = value.ToLower();
                _serviceProblem = char.ToUpper(problem[0]) + problem.Substring(1);
            }
        }

        // get set service cost
        public double ServiceCost
        {
            get { return _serviceCost; }
            set
            {
                _serviceCost = Math.Round(value,2);
            }
        }

        // 6.1: Public accessor methods
        public Drone(int serviceTag, string clientName, string droneModel, string serviceProblem, double serviceCost)
        {
            ServiceTag = serviceTag;
            ClientName = clientName;
            DroneModel = droneModel;
            ServiceProblem = serviceProblem;
            ServiceCost = serviceCost;
        }

        // 6.1: Display Method that returns client name and service cost
        public string display()
        {
            return $"Client Name: {_clientName} | Service Cost: ${_serviceCost:F2}";
        }

        // for ui listbox display
        public override string ToString()
        {
            return display();
        }

    }
}
