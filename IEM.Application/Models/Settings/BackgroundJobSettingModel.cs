using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEM.Application.Models.Settings
{
    public class BackgroundJobSettingModel
    {
        public int RetryAttempts { get; set; }
        public int SlidingInvisibilityTimeoutInMinutes { get; set; }
        public bool DashboardEnabled { get; set; }
        public JobDashboardSettingModel Dashboard { get; set; }
        public BackgroundJobSettingModel()
        {
            Dashboard = new JobDashboardSettingModel();
        }

        public class JobDashboardSettingModel
        {
            public bool IsReadOnly { get; set; }
            public string? Path { get; set; }
            public string? AdministrationAccessNames { get; set; }

        }
    }
}
