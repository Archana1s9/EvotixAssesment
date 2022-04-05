using NUnit.Framework;
using OpenQA.Selenium;
using PDMS.UI.Tests.Core;
using PDMS.UI.Tests.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using static PDMS.UI.Tests.Models.EnvironmentLandContamination;

namespace PDMS.UI.Tests.PageObjectModels
{
    public class EnvironmentLandContaminationPage : LandingPage
    {
        IWebDriver driver;

        protected MeridianHelpers Helper { get; }

        public IWebElement Txt_description => driver.FindElement(By.XPath("//*[@id='SheLandContamination_Description']"));
        public IWebElement Txt_date => driver.FindElement(By.XPath("//*[@id='SheLandContamination_SampleDate']"));
        public IWebElement Button_newRecord => driver.FindElement(By.XPath("//a[text()=' New Record ']"));

        public String Descirption_record_delete = "";


        public EnvironmentLandContaminationPage(IWebDriver driver) : base(driver)
        {
            this.driver = driver;

            Helper = new MeridianHelpers(driver);

        }

        public string EnterLandContaminationDetails(EnvironmentLandContamination environmentLandContaminationDetails)
        {
           
            return CreateLandContaminationRecord(environmentLandContaminationDetails);
           

        }
        public string CreateLandContaminationRecord(EnvironmentLandContamination landContaminationDetails)
        {
            String Descirption_record = landContaminationDetails.Description + Helper.RandomString(5);

            /// to delete initial record -storing in public variable to identify the record and delete
            /* if (i == 0)
             {*/

            Descirption_record_delete = Descirption_record;
           
            //}


            Helper.EnterText(Txt_description, Descirption_record);
            TestContext.WriteLine(landContaminationDetails.Description);
            Helper.WaitForTime(1);
            Helper.EnterText(Txt_date, landContaminationDetails.SampleDate);
            Helper.WaitForTime(1);
            Helper.ClickSaveAndCloseButton();
            TestContext.WriteLine("Record created Succesfully");

            return Descirption_record;
            /* for (int i = 0; i < landContaminationDetails.Count; i++)
             {
                 *//*if (!Helper.IsEnabled(Btn_commitmentAdd))
                 {*/
            /* ClickNewRecordButton();
                 EnterLandContaminationRecordDetails(landContaminationDetails, i);*/
            //}



        }
        /*  public void EnterLandContaminationRecordDetails(List<EnvironmentLandContamination> landContaminationDetails, int trValue)
          {
              int i = trValue;

              String Descirption_record = landContaminationDetails[i].Description + Helper.RandomString(5);

              /// to delete initial record -storing in public variable to identify the record and delete
              if (i==0)
              {

                  Descirption_record_delete = Descirption_record;
              }


              Helper.EnterText(Txt_description, Descirption_record);
              TestContext.WriteLine(landContaminationDetails[i].Description);
              Helper.WaitForTime(1);
              Helper.EnterText(Txt_date, landContaminationDetails[i].SampleDate);
              Helper.WaitForTime(1);
              Helper.ClickSaveAndCloseButton();
              TestContext.WriteLine("Record created Succesfully");

          }
          public void ClickNewRecordButton()
          {
              Helper.ClickElement(Button_newRecord);
          }
  */

        public void DeleteLandContaminationRecord(string desc)
        {
           
             
              String Delete_Toggle = "//span[contains(text(),'Description:')]/following-sibling::a[text()='#']/../../../following-sibling::div//button[@title='Manage Record']".Replace("#", desc);
               //span[contains(text(),'Description:')]/following-sibling::a[text()='AutomationTest1SPNMI']/../../../following-sibling::div//button[@title='Manage Record']
              driver.FindElement(By.XPath(Delete_Toggle)).Click();
              Helper.WaitForTime(1);
              String Delete = "//span[contains(text(),'Description:')]/following-sibling::a[text()='#']/../../../following-sibling::div//button[@title='Manage Record']/following-sibling::ul//a[contains(@id,'Delete')]".Replace("#", desc);

             //span[contains(text(),'Description:')]/following-sibling::a[text()='AutomationTest1SPNMI']/../../../following-sibling::div//button[@title='Manage Record']/following-sibling::ul//a[contains(@id,'Delete')]
      
              driver.FindElement(By.XPath(Delete)).Click();
            //cpnfirm button
            Helper.ClickConfirmPopUpButton();


         

        }

        public void ValidateRecord(String desc)
        {
            String DescriptionValue_xpath = "//span[contains(text(),'Description:')]/following-sibling::a[text()='#']".Replace("#", desc);
            IWebElement ele = driver.FindElement(By.XPath(DescriptionValue_xpath));

            if (Helper.isElementExists(ele))
            {
                TestContext.WriteLine("Record 2: Is Available");

            }
            else
            {
                TestContext.WriteLine("Record 2: Not Available");

            }

        }
    }
}



