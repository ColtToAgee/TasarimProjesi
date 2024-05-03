using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SeleniumEntity.Models;
using SeleniumService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HayatHospitalWorker
{
    public class Program
    {
        static void Main(string[] args)
        {
            getDoctors();
        }
        public static void getDoctors()
        {
            IWebDriver driver = new ChromeDriver();

            using (var db = new DbService())
            {
                var urlList = new List<string>();
                var doctorUrlList = new List<WebUrls>();
                driver.Navigate().GoToUrl("https://www.ozelhayathastanesi.com.tr/doktorlarimiz");
                var hospital = new Hospitals()
                {
                    HospitalName = "Özel Hayat Hastanesi",
                    HospitalAddress = @"Elmasbahçeler, Ankara Yolu Cd No: 44, 16230 Osmangazi/Bursa",
                    RowStateId = 1
                };
                db.AddOrUpdateEntity(hospital, $"{nameof(Hospitals.HospitalName)}='{hospital.HospitalName}'");
                var hospitalId = db.FirstOrDefault<Hospitals>($"{nameof(Hospitals.HospitalName)}='{hospital.HospitalName}'").Id;
                var doctorUrlStringList = driver.FindElements(By.ClassName("entry-title"));
                foreach(var doctorUrl in doctorUrlStringList)
                {
                    var doctorUrlString = doctorUrl.FindElement(By.TagName("a")).GetAttribute("href");
                    if (string.IsNullOrEmpty(doctorUrlString))
                        continue;
                    doctorUrlList.Add(new WebUrls()
                    {
                        UrlPath = doctorUrlString,
                    });
                }
                var policlinicDbList = db.GetList<Policlinics>();
                var titleDbList = db.GetList<Titles>();
                foreach(var doctorPage in doctorUrlList)
                {
                    try
                    {
                        driver.Navigate().GoToUrl(doctorPage.UrlPath);
                        var doctorInfo = driver.FindElement(By.ClassName("doctor-title")).Text;
                        var doctorName = doctorInfo.Split(new[] { "\r\n" }, StringSplitOptions.None)[1];
                        var doctorTitle = doctorInfo.Split(new[] { "\r\n" }, StringSplitOptions.None)[0];
                        var doctorPoliclinic = driver.FindElement(By.ClassName("doctor-category")).Text;
                        var newDoctorPoliclinic = new Policlinics()
                        {
                            PoliclinicName = doctorPoliclinic,
                            RowStateId = 1
                        };
                        db.AddOrUpdateEntity(newDoctorPoliclinic, $"{nameof(Policlinics.PoliclinicName)}='{doctorPoliclinic}'");
                        var newDoctor = new DoctorProfiles()
                        {
                            DoctorName = doctorName,
                            DoctorPoliclinic = db.FirstOrDefault<Policlinics>($"{nameof(Policlinics.PoliclinicName)}='{doctorPoliclinic}'").Id,
                            DoctorTitle = titleDbList.FirstOrDefault(a => a.TitleSubName == doctorTitle || a.TitleName == doctorTitle).Id,
                        };
                        db.AddOrUpdateEntity(newDoctor, $"{nameof(DoctorProfiles.DoctorName)}='{newDoctor.DoctorName}' and {nameof(DoctorProfiles.DoctorPoliclinic)}='{newDoctor.DoctorPoliclinic}'");
                    }
                    catch(Exception ex)
                    {
                        continue;
                    }
                }
            }
        }
    }
}
