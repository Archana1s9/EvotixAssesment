using NUnit.Framework;
using OpenQA.Selenium;
using SSD.UI.Tests.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SSD.UI.Tests.PageObjectModels
{
    class UserDefinedPage
    {
        IWebDriver driver;
        protected MeridianHelpers Helper { get; }
       
        public IWebElement Btn_newRecord => driver.FindElement(By.XPath("//a[text()=' New Record ']"));
       
        public UserDefinedPage(IWebDriver driver)
        {
            this.driver = driver;
            Helper = new MeridianHelpers(driver);

        }
        public void ClickOnNewRecordButton()
        {
             Helper.WaitForTime(1);
            Helper.ClickElement(Btn_newRecord);
          
        }
       
    }
}
