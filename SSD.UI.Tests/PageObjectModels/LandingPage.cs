using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using PDMS.UI.Tests.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace PDMS.UI.Tests.PageObjectModels
{
   
    public class LandingPage 
    {
        IWebDriver driver;
        protected MeridianHelpers Helper { get; }
        public IWebElement Menu_triggerElement => driver.FindElement(By.XPath("//a[text()='Modules']"));
        public IWebElement Menu_landContamination => driver.FindElement(By.XPath("//section[@role='main']//a[text()='Land Contamination']"));
        public IWebElement Btn_submit => driver.FindElement(By.XPath("//button[contains(text(),'Submit')]"));
        public IWebElement Dpdwn_user => driver.FindElement(By.XPath("//*[@class='she-nav-menu header-user-info she-user-info']"));
        public IWebElement Btn_logout => driver.FindElement(By.XPath("//a[text()='Log Out']"));





        public LandingPage(IWebDriver driver)
        {
            this.driver = driver;
            Helper = new MeridianHelpers(driver);

        }

        public bool IsUserLoggedIn()
        {
            var t = driver.Title;
            if (t != "Assure" && t !="")
            {
                return Helper.isElementExists(Menu_triggerElement);
            }
            else
            {
                return false;
            }
            
           

        }
        public void NavigateEnvironment()
        {
            Helper.NavigateMainMenu("Environment");

        }
        public void NavigateLandContamination()
        {
            Helper.ClickElement(Menu_landContamination);
            

        }
        

        public bool IsDialogPresent()
        {
            IAlert alert = ExpectedConditions.AlertIsPresent().Invoke(driver);
            return (alert != null);
        }
        public void LogOut()
        {
            Helper.ClickElement(Dpdwn_user);
            Helper.WaitForTime(1);
            Helper.ClickElement(Btn_logout);
            TestContext.WriteLine("User logout Successfully");
            Helper.WaitForTime(2);
            Helper.Close();

        }
       
        public void TestLogout()
        {
            Helper.DoLogout();
            Assert.AreEqual(Helper.GetWindowTitle(), "Home - Login");
        }
    }

}
