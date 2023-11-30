using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SeleniumEntity.Models;
using SeleniumService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DenemeSelenium
{
    public class Program
    {
        static void Main(string[] args)
        {
            getUsers();
        }
        public static void getUsers()
        {
            IWebDriver driver = new ChromeDriver();
            driver.Navigate().GoToUrl("https://uludag.edu.tr/bm/konu/view?id=2219&title=akademik-kadro");
            IReadOnlyCollection<IWebElement> elements = driver.FindElements(By.TagName("tr"));
            List<UserProfiles> userList = new List<UserProfiles>();
            using (var db = new DbService())
            {
                foreach (var element in elements)
                {
                    var newProfile = new UserProfiles();
                    var text = element.Text.Split(new[] { "\r\n" }, StringSplitOptions.None);
                    text.All(item =>
                    {
                        var data = item.ToLower().Trim();
                        if (data.StartsWith("prof") || data.StartsWith("doç") || data.StartsWith("dr") || data.StartsWith("araş"))
                            newProfile.FullName = data;
                        else if (data.StartsWith("tel:"))
                            newProfile.TelephoneNumber = data.Split(new[] { "tel:" }, StringSplitOptions.None)[1];
                        else if (data.StartsWith("e-posta"))
                            newProfile.Email = data.Split(new[] { "e-posta:" }, StringSplitOptions.None)[1];
                        else if (data.StartsWith("adres"))
                            newProfile.Adress = data.Split(new[] { "adres:" }, StringSplitOptions.None)[1];
                        else if (data.StartsWith("avesis"))
                            newProfile.AvesisLink = data.Split(new[] { "avesis:" }, StringSplitOptions.None)[1];
                        return true;
                    });
                    var user = db.FirstOrDefault<UserProfiles>($"{nameof(UserProfiles.FullName)}='{newProfile.FullName}'");
                    if (user != null)
                    {
                        user.TelephoneNumber = newProfile.TelephoneNumber;
                        user.Email=newProfile.Email;
                        user.Adress = newProfile.Adress;
                        user.AvesisLink =newProfile.AvesisLink;
                        db.AddOrUpdateEntity(user);
                    }
                    else
                        db.AddOrUpdateEntity(newProfile);
                }
             }
        }
    }
}
