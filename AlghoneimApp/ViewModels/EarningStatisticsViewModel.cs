using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.ViewModels
{

    public class EarningStatisticsViewModel
    {

        public int Id { get; set; }

        public double Total { get; set; }

        public string Name { get; set; }

        public DateTime OrderDateTime { get; set; }

        public int PaymentMethod { get; set; }
        
    }
    
    public class EarningListViewModel
    {
        public EarningListViewModel()
        {
            Statistics = new List<EarningStatisticsViewModel>();
        }
        public List<EarningStatisticsViewModel> Statistics { get; set; }
    }

}