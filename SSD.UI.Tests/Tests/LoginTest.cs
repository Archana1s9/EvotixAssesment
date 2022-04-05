
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using SSD.UI.Tests.Core;
using SSD.UI.Tests.Enums;
using SSD.UI.Tests.PageObjectModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Text;

namespace SSD.UI.Tests.Tests
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
