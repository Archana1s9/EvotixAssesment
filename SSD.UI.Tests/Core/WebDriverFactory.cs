using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using PDMS.UI.Tests.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PDMS.UI.Tests.Core
{
    public class WebDriverFactory
    {
		private static IWebDriver driver;

		public IWebDriver GetDriver(Browser browser)
		{
			switch (browser)
			{
				case Browser.CHROME:
					var chromeOptions = new ChromeOptions();
					driver = new ChromeDriver("Drivers", chromeOptions);
					break;

				case Browser.CHROME_HEADLESS:
					chromeOptions = new ChromeOptions();
					chromeOptions.AddArguments("--headless");
					driver = new ChromeDriver("Drivers", chromeOptions);
					break;

				case Browser.EDGE:
					var edgeOptions = new EdgeOptions();
					driver = new EdgeDriver("Drivers", edgeOptions);
					break;

				case Browser.FIREFOX:
					var firefoxOptions = new FirefoxOptions();
					driver = new FirefoxDriver("Drivers", firefoxOptions);
					break;

				default:
					throw new NotSupportedException($"Unhandled browser! {browser}");
			}

			return driver;
		}		
	}
}
