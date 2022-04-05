
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using PDMS.UI.Tests.Core;
using PDMS.UI.Tests.Enums;
using PDMS.UI.Tests.PageObjectModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Text;

namespace PDMS.UI.Tests.Tests
{
   
        /*public void LoginSuccessfully()
        {
            Pages.LoginPage.GoTo();
            Pages.LoginPage.Login();
            //Assert.True(Pages.LandingPage.IsDisplayed());
            //Complete Validation step in LoginPage
        }*/
        [TestFixture, Category("Login Tests")]
        public class LoginTest : BaseTest
        {
            [Test]
            public void LoginSuccessfully()
            {
                var t = TestContext.Parameters["webAppUrl"];
                Pages.LoginPage.GoTo();
                
            }
        }
}
