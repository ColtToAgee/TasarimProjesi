using Newtonsoft.Json;
using SeleniumEntity.Models;
using SeleniumEntity.ViewModels;
using SeleniumService.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using System.Web.Cors;

namespace TasarimProjeWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetDoctors()
        {
            var userList = new List<DoctorProfilesViewModel>();
            using (var db = new DbService())
            {
                userList = db.Query<DoctorProfilesViewModel>(@"
                            Select
                            dp.Id,
                            dp.DoctorName,
                            dp.DoctorEmail,
                            dp.DoctorHospital,
                            hp.HospitalName as [DoctorHospitalName],
                            dp.DoctorImageLink,
                            dp.DoctorPoliclinic,
                            pc.PoliclinicName as [DoctorPoliclinicName],
                            dp.DoctorTitle,
                            tl.TitleName as [DoctorTitleName],
                            dp.RowStateId
                            from DoctorProfiles as dp
                            Inner Join Policlinics as pc 
                            on pc.Id=dp.DoctorPoliclinic
                            Inner Join Titles as tl
                            on tl.Id = dp.DoctorTitle
                            Inner Join Hospitals as hp
                            on hp.Id = dp.DoctorHospital

                            ");
            }

            var userListData = JsonConvert.SerializeObject(userList);

            return Json(userListData,JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetPoliclinics()
        {
            var policlinicList = new List<PoliclinicsViewModel>();
            using (var db = new DbService())
            {
                policlinicList = db.Query<PoliclinicsViewModel>(@"
                                    Select
                                    dp.Id,
                                    dp.PoliclinicName
                                    from Policlinics as dp
                                    ");
            }

            var policlinicListData = JsonConvert.SerializeObject(policlinicList);

            return Json(policlinicListData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetTitles()
        {
            var titleList = new List<TitlesViewModel>();
            using (var db = new DbService())
            {
                titleList = db.Query<TitlesViewModel>(@"
                                    Select
                                    dp.Id,
                                    dp.TitleName
                                    from Titles as dp
                                    ");
            }

            var titleListData = JsonConvert.SerializeObject(titleList);

            return Json(titleListData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetHospitals() 
        {
            var hospitalList = new List<HospitalsViewModel>();
            using (var db = new DbService())
            {
                hospitalList = db.Query<HospitalsViewModel>(@"
                                        Select
                                        dp.Id,
                                        dp.HospitalName
                                        from Hospitals as dp
                                        ");
            }

            var hospitalListData = JsonConvert.SerializeObject(hospitalList);

            return Json(hospitalListData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetDoctorsWithFilter(int hospitalId,int policlinicId, int titleId)
        {

            var doctorList = new List<DoctorProfilesViewModel>();
            using (var db = new DbService())
            {
                var hospitalFilter = hospitalId != 0 ? $" and dp.DoctorHospital = {hospitalId}" : "";
                var policlinicFilter = policlinicId != 0 ? $" and dp.DoctorPoliclinic = {policlinicId}" : "";
                var titleFilter = titleId != 0 ? $" and dp.DoctorTitle = {titleId}" : "";
                doctorList = db.Query<DoctorProfilesViewModel>($@"
                            Select
                            dp.Id,
                            dp.DoctorName,
                            dp.DoctorEmail,
                            dp.DoctorHospital,
                            dp.DoctorImageLink,
                            dp.DoctorPoliclinic,
                            pc.PoliclinicName as [DoctorPoliclinicName],
                            dp.DoctorTitle,
                            tl.TitleName as [DoctorTitleName]
                            from DoctorProfiles as dp
                            Inner Join Policlinics as pc 
                            on pc.Id=dp.DoctorPoliclinic
                            Inner Join Titles as tl
                            on tl.Id = dp.DoctorTitle
                            where 1=1
                            {hospitalFilter}
                            {policlinicFilter}
                            {titleFilter}"
                            );
            }

            var doctorListData = JsonConvert.SerializeObject(doctorList);

            return Json(doctorListData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public void RunConsoleApp()
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd";
            process.StartInfo.FileName = "cmd";
            process.StartInfo.WorkingDirectory = @"C:\Users\Cagat\source\repos\TasarimProjesi\DenemeSelenium\bin\Debug";

            process.StartInfo.Arguments = "/c \"" + "DenemeSelenium.exe" + "\"";
            process.Start();
        }
    }
}