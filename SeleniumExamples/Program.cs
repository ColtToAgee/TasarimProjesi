using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumExamples
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IWebDriver driver = new ChromeDriver();
            driver.Navigate().GoToUrl("https://uludag.edu.tr/bm/konu/view?id=2219&title=akademik-kadro");
            IReadOnlyCollection<IWebElement> element = driver.FindElements(By.ClassName("td"));
        }
    }
}
