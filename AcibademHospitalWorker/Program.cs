using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SeleniumEntity.Models;
using SeleniumService.Services;
using System;
using System.Collections.Generic;
using System.Linq;


namespace AcibademHospitalWorker
{
    public class Program
    {
        static void Main(string[] args)
        {
            GetDoctors();
        }
        public static void GetDoctors()
        {
            IWebDriver driver = new ChromeDriver();

            using (var db = new DbService())
            {
                var urlList = db.GetList<WebUrls>();
                foreach (var url in urlList)
                {
                    driver.Navigate().GoToUrl(url.UrlPath);
                    var doctorName = driver.FindElement(By.CssSelector(".text.detail")).FindElement(By.TagName("h1")).Text.Split(new[] { "\r\n" }, StringSplitOptions.None)[1];
                    var doctorPoliclinic = driver.FindElement(By.CssSelector(".doctor.units.main.top")).Text;
                    var doctorEmail = driver.FindElement(By.CssSelector(".doctor.email")).Text;
                    var doctorTitle = driver.FindElement(By.CssSelector(".text.detail")).FindElement(By.TagName("h1")).FindElement(By.TagName("span")).Text;
                    var doctorPoliclinicId = db.FirstOrDefault<Policlinics>($"{nameof(Policlinics.PoliclinicName)}='{doctorPoliclinic}'") == null ? 0 : db.FirstOrDefault<Policlinics>($"{nameof(Policlinics.PoliclinicName)}='{doctorPoliclinic}'").Id;
                    var doctorTitleId = db.FirstOrDefault<Titles>($"{nameof(Titles.TitleName)}='{doctorTitle}'").Id;
                    var newDoctor = new DoctorProfiles()
                    {
                        DoctorName = doctorName,
                        DoctorEmail = doctorEmail,
                        DoctorPoliclinic = doctorPoliclinicId,
                        DoctorTitle = doctorTitleId,
                        DoctorHospital = 1,
                        RowStateId = 1,
                    };
                    var tempDoctor = db.FirstOrDefault<DoctorProfiles>($"{nameof(DoctorProfiles.DoctorName)}='{doctorName}'");
                    if (tempDoctor == null)
                    {
                        db.AddOrUpdateEntity(newDoctor);
                    }
                    else
                    {
                        tempDoctor.DoctorName = doctorName;
                        tempDoctor.DoctorEmail = doctorEmail;
                        tempDoctor.DoctorPoliclinic = doctorPoliclinicId;
                        tempDoctor.DoctorTitle = doctorTitleId;
                        db.AddOrUpdateEntity(tempDoctor);
                    }
                }
            }
        }
        public static void getDoctorImages()
        {
            IWebDriver driver = new ChromeDriver();
            driver.Navigate().GoToUrl("https://www.acibadem.com.tr/doktorlar/");
            using (var db = new DbService())
            {
                var doctorDbList = db.GetList<DoctorProfiles>();
                var doctorList = driver.FindElements(By.ClassName("doctor-lazy-load-img-cancelled"));
                foreach (var doctor in doctorList)
                {
                    var image = doctor.GetDomAttribute("src");
                    var name = doctor.GetDomAttribute("alt");
                    if (image != null && name != null)
                    {
                        var selectedDoctor = doctorDbList.FirstOrDefault(a => name.Contains(a.DoctorName));
                        if (selectedDoctor != null)
                        {
                            selectedDoctor.DoctorImageLink = "https://www.acibadem.com.tr" + image;
                            db.AddOrUpdateEntity<DoctorProfiles>(selectedDoctor);
                        }
                    }
                }
            }
        }
    }
}
