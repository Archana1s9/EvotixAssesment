using AventStack.ExtentReports;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSD.UI.Tests.Utils
{
    public class GenericUtils
    {
        public static MediaEntityModelProvider CaptureScreenShot(IWebDriver driver, String screenShotName, bool IsFile = false)
        {
            ITakesScreenshot ts = (ITakesScreenshot)driver;


            if (IsFile)
            {
                ts.GetScreenshot().SaveAsFile(screenShotName);
                return null;
            }

            var screenshot = ts.GetScreenshot().AsBase64EncodedString;

            return MediaEntityBuilder.CreateScreenCaptureFromBase64String(screenshot, screenShotName).Build();
        }
    }
}
