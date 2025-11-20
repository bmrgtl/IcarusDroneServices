using System.Collections.ObjectModel;
using System.Security.Policy;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IcarusDroneServices
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // 6.2 - 6.4: Data Collections
        List<Drone> FinishedList = new List<Drone>();
        Queue<Drone> RegularService = new Queue<Drone>();
        Queue<Drone> ExpressService = new Queue<Drone>();

        // UI Collections
        public ObservableCollection<Drone> RegularServiceCollection { get; set; } = new();
        public ObservableCollection<Drone> ExpressServiceCollection { get; set; } = new();
        public ObservableCollection<Drone> FinishedServiceCollection { get; set; } = new();

        // service tag tracker, start with 90 so first increment is 100
        private int currentServiceTag = 90;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        // exit application
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // 6.5: Add new drone to appropriate queue
        private void AddNewItem(object sender, RoutedEventArgs e)
        {
            // Validate service cost
            if (!ValidateServiceCost(out double cost))
            {
                return; // stop if invalid
            }

            // Validate other fields
            if (string.IsNullOrWhiteSpace(txtClientName.Text) ||
                string.IsNullOrWhiteSpace(txtDroneModel.Text) ||
                string.IsNullOrWhiteSpace(txtServiceProblem.Text))
            {
                txtServiceStatusDrone.Text = "Please fill in all fields.";
                return;
            }

            // Parse service tag safely
            if (!int.TryParse(txtServiceTag.Text, out int serviceTag))
            {
                txtServiceStatusDrone.Text = "Please ensure valid service tag.";
                return;
            }

            // Increment service tag
            IncrementServiceTag();

            // Validate service tag range
            if (serviceTag == -1)
            {
                txtServiceStatusDrone.Text = "Service tags must be at least 100 and at most 900.";
                return; // stop if invalid
            }

            // Create new drone object
            var drone = new Drone(
                serviceTag,
                txtClientName.Text,
                txtDroneModel.Text,
                txtServiceProblem.Text,
                cost);

            // Validate service tag
            if (drone.ServiceTag % 10 != 0)
            {
                txtServiceStatusDrone.Text = "Service tags must be of increments of 10.";
                return;
            }

            // Get service priority            
            string priority = GetServicePriority();


            // Add to Queue based on priority
            if (priority == "Regular")
            {
                RegularService.Enqueue(drone);
            }
            else if (priority == "Express")
            {
                // 6.6: Add 15% surcharge to all express items
                drone.ServiceCost *= 1.15;
                ExpressService.Enqueue(drone);
            }
            else
            {
                txtServiceStatusDrone.Text = "Please select a service priority.";
                return;
            }

            // Display updated queues
            displayRegularServices();
            displayExpressServices();

            // Clear input fields
            ClearFields();
            txtServiceStatusQueue.Text = "Drone added to " + priority + " service queue.";
        }

        

        // 6.7: Get service priority from radio buttons
        private string GetServicePriority()
        {
            if (btnRegular.IsChecked == true)
            {
                return "Regular";
            }
            else if (btnExpress.IsChecked == true)
            {
                return "Express";
            }
            else
            {
                return "No Service Selected";
            }
        }

        //6.8: Display elements in Regular Services Queue
        private void displayRegularServices()
        {
            RegularServiceCollection.Clear();
            foreach (var drone in RegularService)
            {
                RegularServiceCollection.Add(drone);
            }
        }

        // 6.9: Display elements in Express Services Queue
        private void displayExpressServices()
        {
            ExpressServiceCollection.Clear();
            foreach (var drone in ExpressService)
            {
                ExpressServiceCollection.Add(drone);
            }
        }

        // 6.10: Validate Service Cost
        private bool ValidateServiceCost(out double cost)
        {
            cost = 0;

            if (double.TryParse(txtServiceCost.Text, out cost))
            {
                // ensures two decimal places only
                cost = Math.Round(cost, 2);

                // ensures that cost is not 0 or negative
                if (cost <= 0)
                {
                    txtServiceStatusDrone.Text = "Service cost cannot be negative or 0.";
                    return false;
                }

                return true;
            }

            else
            {
                txtServiceStatusDrone.Text = "Please ensure valid service cost.";
                return false;
            }
        }

        // 6.11: Increment service tag by 10, reset to 100 if exceeds 900
        private int IncrementServiceTag()
        {
            int nextTag = currentServiceTag + 10;   

            if (nextTag < 100)
            {
                txtServiceStatusDrone.Text = "Service tag must be at least 100.";
                return -1;
            }

            if (nextTag > 900)
            {
                nextTag = 100;
            }

            currentServiceTag = nextTag;
            txtServiceTag.Value = nextTag;
            return currentServiceTag;
        }

        // 6.12: Display client name and problem on selection for Regular Service
        private void lvRegularServices_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (lvRegularServices.SelectedItem is Drone selectedDrone)
            {
                txtClientName.Text = selectedDrone.ClientName;
                txtServiceProblem.Text = selectedDrone.ServiceProblem;
                txtServiceStatusQueue.Text = "Drone with Service Tag " + selectedDrone.ServiceTag + "displayed in textbox.";
            }
        }

        // 6.13: Display client name and problem on selection for Express Services
        private void lvExpressServices_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (lvExpressServices.SelectedItem is Drone selectedDrone)
            {
                txtClientName.Text = selectedDrone.ClientName;
                txtServiceProblem.Text = selectedDrone.ServiceProblem;
                txtServiceStatusQueue.Text = "Drone with Service Tag " + selectedDrone.ServiceTag + "displayed in textbox.";
            }
        }

        // 6.14: Dequeue and remove from list for Regular Service and Place in Finished List and Listbox
        private void btnMarkCompleteReg_Click(object sender, RoutedEventArgs e)
        {
            if (lvRegularServices.SelectedItem is Drone selectedDrone)
            {
                if (selectedDrone != RegularService.Peek())
                {
                    // ensure that lected drone is at front of queue
                    txtServiceStatusQueue.Text = "Please select the drone at the front of the Regular Service queue.";
                    return;
                }

                if (RegularService.Count == 0)
                {
                    // ensure queue is not empty
                    txtServiceStatusQueue.Text = "No drones in Regular Service queue.";
                    return;
                }

                // Error Handling for regular dequeueing
                try
                {
                    // dequeue and remove from collection
                    RegularService.Dequeue();
                    RegularServiceCollection.Remove(selectedDrone);

                    // move to finished list and collection
                    FinishedList.Add(selectedDrone);
                    FinishedServiceCollection.Add(selectedDrone);

                    // update status
                    txtServiceStatusQueue.Text = "Drone with Service Tag " + selectedDrone.ServiceTag + " has been serviced and moved to Finished List.";
                }

                catch (Exception ex)
                {
                    txtServiceStatusQueue.Text = "Error processing drone: " + ex.Message;
                    return;
                }
            }
        }

        // 6.15: Dequeue and remove from list for Express Service and Place in Finished List and Listbox
        private void btnMarkCompleteExp_Click(object sender, RoutedEventArgs e)
        {
            if (lvExpressServices.SelectedItem is Drone selectedDrone)
            {
                if (selectedDrone != ExpressService.Peek())
                {
                    // ensure that selected drone is at front of queue
                    txtServiceStatusQueue.Text = "Please select the drone at the front of the Express Service queue.";
                    return;
                }

                if (ExpressService.Count == 0)
                {
                    // ensure queue is not empty
                    txtServiceStatusQueue.Text = "No drones in Express Service queue.";
                    return;
                }

                // Error Handling for express dequeueing
                try
                {
                    // remove from queue and collection
                    ExpressService.Dequeue();
                    ExpressServiceCollection.Remove(selectedDrone);

                    // move to finished list and collection
                    FinishedList.Add(selectedDrone);
                    FinishedServiceCollection.Add(selectedDrone);

                    // update status
                    txtServiceStatusQueue.Text = "Drone with Service Tag " + selectedDrone.ServiceTag + " has been serviced and moved to Finished List.";
                } 
                catch (Exception ex)
                {
                    txtServiceStatusQueue.Text = "Error processing drone: " + ex.Message;
                    return;
                }
            }
        }

        // 6.16: Remove item on double click from Finished Services listbox
        private void lbCompletedServices_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lbCompletedServices.SelectedItem == null)
            {
                txtServiceStatusQueue.Text = "Please select a drone from the Finished List to remove.";
                return;
            }

            if (lbCompletedServices.SelectedItem is Drone selectedDrone)
            {
                // Error Handling for removing finished drone
                try
                {
                    // remove from finished list and collection
                    FinishedList.Remove(selectedDrone);
                    FinishedServiceCollection.Remove(selectedDrone);

                    // update status
                    txtServiceStatusQueue.Text = "Drone with Service Tag " + selectedDrone.ServiceTag + " has been paid and removed from Finished List.";
                }
                catch (Exception ex)
                {
                    txtServiceStatusQueue.Text = "Error removing drone: " + ex.Message;
                    return;
                }

            }
        }

        // 6.17: Clear Input Fields
        private void ClearFields()
        {
            txtServiceTag.Value = IncrementServiceTag();
            txtClientName.Clear();
            txtDroneModel.Clear();
            txtServiceProblem.Clear();
            txtServiceCost.Clear();
            btnRegular.IsChecked = false;
            btnExpress.IsChecked = false;
        }
    }
}