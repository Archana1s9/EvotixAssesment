using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using SSD.UI.Tests.Core;
using SSD.UI.Tests.PageObjectModels;
using SSD.UI.Tests.Utils;
using System;
using System.IO;
using System.Threading;

namespace SSD.UI.Tests.Tests
{
    public class BaseTest : ExtentTestManager
    {
        private IWebDriver Driver { get; set; }
        [ThreadStatic]
        protected static ExtentTest parentTest;
        [ThreadStatic]
        protected static ExtentTest childTest;

        public String pathString;



        public BaseTest()
        {
            parentTest = CreateTest(TestContext.CurrentContext.Test.ClassName);
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            TestContext.WriteLine("OneTimeSetUp");

            var factory = new WebDriverFactory();
            Driver = factory.GetDriver(Pages.TestBrowser());
            Pages.Init(Driver);

            String ScreenshotFolderNameDynamic = System.AppDomain.CurrentDomain.BaseDirectory + $"CaptureScreenShots\\";

            pathString = System.IO.Path.Combine(ScreenshotFolderNameDynamic, DateTime.Now.ToString("dd_MMM_yyyy_hhmmss"));

            System.IO.Directory.CreateDirectory(pathString);

            //fileName = pathString + $"Screenshot_{ DateTime.Now.ToString("hh_mm_ss")}_{testName}.png";

        }
        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            //RunLevel Teardowns to be done here
            TestContext.WriteLine("OneTimeTearDown");

            Pages.LandingPage.LogOut();

            Flush();
        }
        [SetUp]
        public void BeforeTest()
        {
            TestContext.WriteLine("BeforeTest");
            childTest = CreateMethod(parentTest.Model.Name, TestContext.CurrentContext.Test.Name);
        }

        [TearDown]
        public void AfterTest()
        {
            TestContext.WriteLine("AfterTest");

            try
            {
                //bool passed = TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Passed;
                var exec_status = TestContext.CurrentContext.Result.Outcome.Status;
                var stacktrace = string.IsNullOrEmpty(TestContext.CurrentContext.Result.StackTrace) ? ""
                : string.Format("{0}", TestContext.CurrentContext.Result.StackTrace);
                var errorMsg = TestContext.CurrentContext.Result.Message;
                MediaEntityModelProvider mediaEntity = null;

                Status logstatus = Status.Info;

                String fileName, testName;

                testName = TestContext.CurrentContext.Test.Name.Replace("(", "_").Replace("/", "_").Replace("\"", "").Replace(")", "");
                // parentFolder = System.AppDomain.CurrentDomain.BaseDirectory + $"\\CaptureScreenShots\\";
                //fileName = System.AppDomain.CurrentDomain.BaseDirectory + $"\\CaptureScreenShots\\Screenshot_{DateTime.Now.ToString("hh_mm_ss")}_{testName}.png";
                fileName = pathString + "\\" + $"Screenshot_{ DateTime.Now.ToString("hh_mm_ss")}_{testName}.png";


                switch (exec_status)
                {
                    case TestStatus.Failed:
                        logstatus = Status.Fail;
                        childTest.Log(logstatus, $"Test ended with {logstatus} - {stacktrace}");
                        //* Capturing Screenshots using built-in methods in ExtentReports 4 *//*
                        //mediaEntity = GenericUtils.CaptureScreenShot(Driver, fileName);
                        //Attach screenshot to the Azure Devops Attachment
                        GenericUtils.CaptureScreenShot(Driver, fileName, true);
                        TestContext.AddTestAttachment(fileName);
                        break;
                    case TestStatus.Passed:
                        logstatus = Status.Pass;
                        //* Capturing Screenshots using built-in methods in ExtentReports 4 *//*
                        //mediaEntity = GenericUtils.CaptureScreenShot(Driver, fileName);
                        //Attach screenshot to the Azure Devops Attachment
                        GenericUtils.CaptureScreenShot(Driver, fileName, true);
                        TestContext.AddTestAttachment(fileName);
                        break;
                    case TestStatus.Inconclusive:
                        logstatus = Status.Warning;
                        break;
                    case TestStatus.Skipped:
                        logstatus = Status.Skip;
                        break;
                    default:
                        break;
                }
                childTest.Log(logstatus, "Test: " + TestContext.CurrentContext.Test.Name + " Status:" + logstatus + stacktrace, mediaEntity);

            }
            catch (Exception e)
            {
                throw;
            }
            
            

        }
    }
}
