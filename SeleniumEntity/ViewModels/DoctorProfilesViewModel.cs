using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumEntity.ViewModels
{
    public class DoctorProfilesViewModel:BaseViewModel
    {
        public string DoctorName { get; set; }
        public string DoctorEmail { get; set; }
        public int DoctorTitle { get; set; }
        public string DoctorTitleName { get; set; }
        public int DoctorHospital { get; set; }
        public string DoctorHospitalName { get; set; }
        public string DoctorImageLink { get; set; }
        public int DoctorPoliclinic { get; set; }
        public string DoctorPoliclinicName { get; set; }

    }
}
