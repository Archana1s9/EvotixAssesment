
using AventStack.ExtentReports;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading;

namespace SSD.UI.Tests.Utils
{

	public class ExtentTestManager
	{
		private static Dictionary<string, ExtentTest> _parentTestMap = new Dictionary<string, ExtentTest>();
		private static ThreadLocal<ExtentTest> _parentTest = new ThreadLocal<ExtentTest>();
		private static ThreadLocal<ExtentTest> _childTest = new ThreadLocal<ExtentTest>();


		private static readonly object _synclock = new object();



		// creates a parent test
		public static ExtentTest CreateTest(string testName, string description = null)
		{
			lock (_synclock)
			{
				_parentTest.Value = ExtentService.Instance.CreateTest(testName, description);
				return _parentTest.Value;
			}
		}

		// creates a node
		// node is added to the parent using the parentName
		// if the parent is not available, it will be created
		public static ExtentTest CreateMethod(string parentName, string testName, string description = null)
		{
			lock (_synclock)
			{
				ExtentTest parentTest = null;
				if (!_parentTestMap.ContainsKey(parentName))
				{
					parentTest = ExtentService.Instance.CreateTest(testName);
					_parentTestMap.Add(parentName, parentTest);
				}
				else
				{
					parentTest = _parentTestMap[parentName];
				}
				_parentTest.Value = parentTest;
				_childTest.Value = parentTest.CreateNode(testName, description);
				return _childTest.Value;
			}
		}

		public static ExtentTest CreateMethod(string testName)
		{
			lock (_synclock)
			{
				_childTest.Value = _parentTest.Value.CreateNode(testName);
				return _childTest.Value;
			}
		}

		public static ExtentTest GetMethod()
		{
			lock (_synclock)
			{
				return _childTest.Value;
			}
		}

		public static ExtentTest GetTest()
		{
			lock (_synclock)
			{
				return _parentTest.Value;
			}
		}

		public static void Flush()
		{
			ExtentService.Instance.Flush();
			
		}

		public static void SetStepStatusInfo(string stepDescription)
		{
			TestContext.WriteLine($"Test Step Info - {stepDescription}");
			_childTest.Value.Log(Status.Info, stepDescription);
		}

		public static void SetStepStatusPass(string stepDescription)
		{
			TestContext.WriteLine($"Test Step Passed - {stepDescription}");
			_childTest.Value.Log(Status.Pass, stepDescription);
		}
		public static void SetStepStatusFail(string stepDescription)
		{
			TestContext.WriteLine($"Test Step Failed - {stepDescription}");
			_childTest.Value.Log(Status.Fail, stepDescription);
		}
		public void SetStepStatusWarning(string stepDescription)
		{
			TestContext.WriteLine($"Test Step Warning - {stepDescription}");
			_childTest.Value.Log(Status.Warning, stepDescription);
		}
		public void SetTestStatusFail(string message = null)
		{

			TestContext.WriteLine($"Test failed - {message}");

			var printMessage = "<p><b>Test FAILED!</b></p>";
			if (!string.IsNullOrEmpty(message))
			{
				printMessage += $"Message: <br>{message}<br>";
			}
			_childTest.Value.Fail(printMessage);
		}
	}

}
