using NUnit.Framework;
using OpenQA.Selenium;
using SSD.UI.Tests.Core;
using SSD.UI.Tests.Enums;
using SSD.UI.Tests.Tests;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSD.UI.Tests.PageObjectModels
{
    internal class Pages : BaseTest
    {
        public static String BROWSER = TestContext.Parameters["webAppBrowser"];
        public static LoginPage LoginPage;
        public static LandingPage LandingPage;
        public static UserDefinedPage UserDefinedPage;
        public static EnvironmentLandContaminationPage EnvironmentLandContaminationPage;
      


        public static void Init(IWebDriver driver)
        {
            LoginPage = new LoginPage(driver);
            LandingPage = new LandingPage(driver);
            UserDefinedPage = new UserDefinedPage(driver);
            EnvironmentLandContaminationPage = new EnvironmentLandContaminationPage(driver);
           

        }
        public static Browser TestBrowser()
        {
            return BROWSER == "CHROME" ? Browser.CHROME
                 : BROWSER == "CHROME_HEADLESS" ? Browser.CHROME_HEADLESS
                 : BROWSER == "EDGE" ? Browser.EDGE
                 : BROWSER == "FIREFOX" ? Browser.FIREFOX
                 : Browser.CHROME;
        }

    }
}
