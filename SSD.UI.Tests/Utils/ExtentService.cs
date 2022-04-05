using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports.Reporter.Configuration;
using NUnit.Framework;
using System;

namespace PDMS.UI.Tests.Utils
{

	public class ExtentService
	{
		private static readonly Lazy<ExtentReports> _lazy =
			new Lazy<ExtentReports>(() => new ExtentReports());

		public static ExtentReports Instance { get { return _lazy.Value; } }

		static ExtentService()
		{
			var reporter = new ExtentHtmlReporter(System.AppDomain.CurrentDomain.BaseDirectory + $"HtmlReports\\index_{DateTime.Now.ToString("dd_MMM_yyyy_hhmmss")}.html");
			reporter.Config.Theme = Theme.Dark;
			Instance.AttachReporter(reporter);
		}

		private ExtentService()
		{
		}
	}

}
