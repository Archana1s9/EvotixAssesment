using NUnit.Framework;
using PDMS.UI.Tests.Models;
using PDMS.UI.Tests.PageObjectModels;
using PDMS.UI.Tests.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using static PDMS.UI.Tests.Models.EnvironmentLandContamination;

namespace PDMS.UI.Tests.Tests
{
    [TestFixture, Order(1)]
    public class EnvironmentLandContaminationTest : BaseTest
    {
        public static int recCount = 0;
        public static List<string> recordForDeletion = new List<string>();
        public static List<string> recordForValidation = new List<string>();

        [Test, TestCaseSource(typeof(InputReader), nameof(InputReader.GetLandContaminationData))]
        public void CreateLandContaminationRecord(EnvironmentLandContamination landContamination)
        {
           

            if (!Pages.LandingPage.IsUserLoggedIn())
            {
                Pages.LoginPage.GoTo();
                Pages.LoginPage.Login();
                Pages.LandingPage.NavigateEnvironment();
                Pages.LandingPage.NavigateLandContamination();
            }

            TestContext.WriteLine("Create New Record");
            Pages.UserDefinedPage.ClickOnNewRecordButton();
            var desc = Pages.EnvironmentLandContaminationPage.EnterLandContaminationDetails(landContamination);
            if (landContamination.Delete.ToUpper() == "YES")
            {
                recordForDeletion.Add(desc);
            }
            else
            {
                recordForValidation.Add(desc);
            }
                

            recCount--;
            if (recCount == 0)
            {
                //Initiate Deletion
                recordForDeletion.ForEach(desc =>
                {
                    Pages.EnvironmentLandContaminationPage.DeleteLandContaminationRecord(desc);
                });

                recordForValidation.ForEach(desc =>
                {
                    //Check Validation
                    Pages.EnvironmentLandContaminationPage.ValidateRecord(desc);
                });
            }

            
        }
    
    }
}
