using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SeleniumEntity.Models;
using SeleniumService.Services;
using System.Collections.Generic;
using System.Linq;
using System;
namespace MedicanaHospitalWorker
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
                driver.Navigate().GoToUrl("https://www.medicana.com.tr/hastanelerimiz");
                var hospitalList = driver.FindElements(By.ClassName("caption"));

                foreach (var hospital in hospitalList)
                {
                    var hospitalName = hospital.FindElement(By.TagName("a")).Text;
                    var hospitalUrl = "https://www.medicana.com.tr/" + hospital.FindElement(By.TagName("a")).GetDomAttribute("href");
                    var hospitalAdress = hospital.FindElement(By.TagName("p")).Text;
                    var newHospital = new Hospitals()
                    {
                        HospitalName = hospitalName,
                        HospitalAddress = hospitalAdress
                    };
                    urlList.Add(hospitalUrl);
                    db.AddOrUpdateEntity(newHospital, $"{nameof(Hospitals.HospitalName)}='{newHospital.HospitalName}'");
                }
                foreach (var url in urlList)
                {
                    driver.Navigate().GoToUrl(url + "/hekimlerimiz");
                    var units = driver.FindElements(By.ClassName("medical-unit"));
                    foreach (var unit in units)
                    {
                        doctorUrlList.AddRange(unit.FindElements(By.TagName("a")).Select(a => new WebUrls()
                        {
                            UrlPath = "https://www.medicana.com.tr/" + a.GetDomAttribute("href")
                        }));
                        var newPoliclinic = new Policlinics()
                        {
                            PoliclinicName = unit.FindElement(By.TagName("h2")).Text
                        };
                        db.AddOrUpdateEntity(newPoliclinic, $"{nameof(Policlinics.PoliclinicName)} = '{newPoliclinic.PoliclinicName}'");
                    }
                }
                var policlinicList = db.GetList<Policlinics>();
                var hospitalDbList = db.GetList<Hospitals>();
                var doctorTitleDbList = db.GetList<Titles>();
                foreach (var doctorUrl in doctorUrlList)
                {
                    driver.Navigate().GoToUrl(doctorUrl.UrlPath);
                    try
                    {
                        var doctorInfos = driver.FindElement(By.ClassName("doctorInfo"));
                        var doctorPoliclinic = doctorInfos.FindElements(By.TagName("a")).FirstOrDefault(a => a.GetDomAttribute("href").Contains("tibbi-birimler") || a.GetDomAttribute("href").Contains("/medicana-da/checkup")).Text;
                        var doctorHospital = doctorInfos.FindElement(By.TagName("p")).Text;
                        var doctorTitle = doctorInfos.FindElement(By.TagName("h1")).FindElement(By.TagName("span")).Text;
                        var newDoctor = new DoctorProfiles()
                        {
                            DoctorName = doctorInfos.FindElement(By.TagName("h1")).Text.Split(new[] { "\r\n" }, StringSplitOptions.None)[1],
                            DoctorTitle = doctorTitleDbList.FirstOrDefault(a => a.TitleSubName == doctorTitle) != null ? doctorTitleDbList.FirstOrDefault(a => a.TitleSubName == doctorTitle).Id : 0,
                            DoctorPoliclinic = policlinicList.FirstOrDefault(a => a.PoliclinicName == doctorPoliclinic) != null ? policlinicList.FirstOrDefault(a => a.PoliclinicName == doctorPoliclinic).Id : 0,
                            DoctorEmail = doctorInfos.FindElement(By.ClassName("fa-envelope-o")).Text,
                            DoctorHospital = hospitalDbList.FirstOrDefault(a => a.HospitalName == doctorHospital) != null ? hospitalDbList.FirstOrDefault(a => a.HospitalName == doctorHospital).Id : 0,
                            DoctorImageLink = driver.FindElement(By.ClassName("img-responsive")).GetDomAttribute("src")
                        };

                        db.AddOrUpdateEntity(newDoctor, $"{nameof(DoctorProfiles.DoctorName)}='{newDoctor.DoctorName}' and {nameof(DoctorProfiles.DoctorPoliclinic)}='{newDoctor.DoctorPoliclinic}'");

                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
            }
        }

    }
}
