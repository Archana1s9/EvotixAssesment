using OpenQA.Selenium;
using SSD.UI.Tests.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading;

namespace SSD.UI.Tests.PageObjectModels
{
    public class LoginPage: LandingPage
    {
        private static readonly string LOGIN_URL = ConfigurationManager.AppSettings["loginUrl"];
        private static readonly string USER_NAME = ConfigurationManager.AppSettings["userName"];
        private static readonly string USER_PASSWORD = ConfigurationManager.AppSettings["password"];
        IWebDriver driver;
        public LoginPage(IWebDriver driver): base(driver)
        {
            this.driver = driver;
        }

        public IWebElement _username => driver.FindElement(By.Id("username"));
        public IWebElement _password => driver.FindElement(By.Id("password"));
        public IWebElement _loginButton => driver.FindElement(By.Id("login"));

        public void Login()
        {
            EnterUserNameAndPassword(USER_NAME, USER_PASSWORD);
            ClickOnLoginBtn();
        }

        public void GoTo()
        {
            Helper.Open(LOGIN_URL);
        }

        public void EnterUserNameAndPassword(String username, String password)
        {
            _username.SendKeys(username);
            _password.SendKeys(password);
        }

        public void ClickOnLoginBtn()
        {
            _loginButton.Click();
            Helper.WaitForPageLoad();
        }
    }
}
