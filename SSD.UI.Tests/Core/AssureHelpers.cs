using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.Events;
using OpenQA.Selenium.Support.UI;
using PDMS.UI.Tests.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;

namespace PDMS.UI.Tests.Core
{
    public class MeridianHelpers : IAssureWeb
    {
        private IWebDriver Driver { get; }
        private static EventFiringWebDriver driver = null;
        private string baseUrl = "";
        private const string scriptGetMVVMList = @"let result=[];for(let key of Object.keys(window)){if(key.startsWith('mvvm') && window[key].KendoValidator){result.push(key);}}return result;";
        private const string scriptSetChangeFlag = @"setTimeout(function() {{ _GENERATED_ }}, 10);";
        private List<string> mvvmList = new List<string>();

        public static bool IsOpen { get; private set; } = false;

        public MeridianHelpers(IWebDriver _driver)
        {
            Driver = _driver;
        }

        public void ClickElement(IWebElement ele)
        {
            ele.Click();
        }

        public void EnterText(IWebElement ele, String input)
        {
            ele.Clear();
            WaitForTime(1);
            ele.SendKeys(input);

        }


        public string GetWindowTitle()
        {
            if (IsOpen)
            {
                return driver.Title;
            }
            else
            {
                return "";
            }
        }

        public string GetPageTitle()
        {
            if (IsOpen)
            {
                var pageMenuElement = driver.FindElement(By.Id("LAYOUT_PAGE_MENU"));
                try
                {
                    var titleTextElement = pageMenuElement.FindElement(By.XPath($"//div[@class='pdms-title-text']"));
                    return titleTextElement.Text;
                }
                catch (NoSuchElementException)
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }
        public void Close()
        {
            try
            {
                driver.ElementClicked -= Driver_ElementClicked;
                driver.Close();
            }
            finally
            {
                IsOpen = false;
            }
        }
        public void Quit()
        {
            try
            {
                if (IsOpen)
                {
                    Close();
                };
                driver.Quit();
            }
            finally
            {
                driver = null;
            }
        }
        private void Driver_ElementClicked(object sender, WebElementEventArgs e)
        {
            if (mvvmList.Count > 0)
            {
                var scriptEngine = (IJavaScriptExecutor)driver;
                try
                {
                    scriptEngine.ExecuteScript(getSetChangeFlagScript());
                }
                catch (OpenQA.Selenium.UnhandledAlertException)
                {
                    scriptEngine.ExecuteScript(getSetChangeFlagScript());
                    e.Element.Click();
                }
            }
        }

        public void WaitForPopup()
        {
            var counter = 0;
            ensureIsOpen();
            int popupCount = 0;
            do
            {
                counter++;
                if (counter == 5) // Waiting for 15 secs
                {
                    throw new TimeoutException("Waiting for a popup, but it did not open.");
                }
                Thread.Sleep(3 * 1000);
                popupCount = driver.FindElements(By.CssSelector("div.k-window")).Where(x => x.Displayed).Count();
            }
            while (popupCount == 0);
        }

        public void WaitForPageLoad()
        {
            var scriptEngine = (IJavaScriptExecutor)driver;
            var readyState = "";

            do
            {
                Thread.Sleep(1 * 1000);
                readyState = scriptEngine.ExecuteScript("return document.readyState") as string;
            }
            while (readyState != "complete");

            mvvmList.Clear();
            var scriptResult = scriptEngine.ExecuteScript(scriptGetMVVMList);
            if (scriptResult != null && scriptResult.GetType() == typeof(ReadOnlyCollection<object>))
            {
                var modelNames = (scriptResult as ReadOnlyCollection<object>);
                for (int index = 0; index < modelNames.Count; index++)
                {
                    mvvmList.Add($"{modelNames[index]}");
                }
            }
        }

        private string getSetChangeFlagScript()
        {
            var generatedScript = "";
            foreach (var modelName in mvvmList)
            {
                generatedScript += $"window.{modelName}.HasChanges=false;";
            }
            return scriptSetChangeFlag.Replace("_GENERATED_", generatedScript);
        }

        private void ensureIsOpen()
        {
            if (!IsOpen)
            {
                throw new InvalidOperationException("Assure application is not running.");
            }
        }

        private string getBaseUrl(string url)
        {
            Uri uri = new Uri(url);
            return uri.Scheme + "://" + uri.Authority;
        }

        public void Open(string url)
        {

            if (string.IsNullOrEmpty(url)) throw new ArgumentNullException(nameof(url));
            baseUrl = getBaseUrl(url);

            try
            {
                if (!IsOpen)
                {
                    if (driver == null)
                    {
                        driver = new EventFiringWebDriver(Driver);
                        driver.Manage().Timeouts().PageLoad = TimeSpan.FromMinutes(3.0);
                        driver.Manage().Window.Maximize();
                        //driver.ElementClicked += Driver_ElementClicked;
                    }
                    IsOpen = true;
                }

                driver.Url = url;
                WaitForPageLoad();
            }
            catch
            {
                IsOpen = false;
            }
        }
        public void NavigateMainMenu(params string[] links)
        {
            ensureIsOpen();
            var linkList = links.ToList();

          //  driver.Navigate().GoToUrl(baseUrl);
            WaitForPageLoad();

            if (links != null && links.Length > 0)
            {
                var menuTriggerElement = driver.FindElement(By.XPath("//a[text()='Modules']"));
                menuTriggerElement.Click();
                  WaitForTime(2);

                var menuElement = driver.FindElement(By.XPath("//a[text()='Modules']"));
                var modules = menuElement.FindElement(By.XPath($"..//ul"));
                NavigateSubMenu(modules, linkList);

                WaitForPageLoad();
            }
        }
        
        public void NavigateSubMenu(IWebElement menu, List<string> links)
        {
            if (links.Count > 0)
            {
                var linkText = links[0];
                var linkList = new List<string>();
                linkList.AddRange(links);
                linkList.RemoveAt(0);

                var subMenuItems = menu.FindElements(By.XPath($"./li"));
                foreach (var item in subMenuItems)
                {
                    var link = item.FindElement(By.XPath($"./a"));
                    var path = link.Text;
                    if (path == linkText)
                    {
                        link.Click();
                          WaitForTime(2);

                       
                        break;
                    }
                }
            }
        }
      

        public List<String> DetermineMenuHierarchy()
        {
            var callableMenus = new List<string>();

            ensureIsOpen();
            driver.Navigate().GoToUrl(baseUrl);
            WaitForPageLoad();

            var menuTriggerElement = driver.FindElement(By.Id("LAYOUT_PULLMENU_TRIGGER"));
            menuTriggerElement.Click();
            WaitForTime(1);

            var menuElement = driver.FindElement(By.Id("LAYOUT_PULLMENU"));
            var mainMenu = menuElement.FindElement(By.XPath($".//ul"));
            DetermineMenuHierarchy(mainMenu, callableMenus, "");

            return callableMenus;
        }
        public void DetermineMenuHierarchy(IWebElement menu, List<String> callableMenus, string MenuPath)
        {
            var subMenuItems = menu.FindElements(By.XPath($"./li"));
            foreach (var item in subMenuItems)
            {
                var link = item.FindElement(By.XPath($"./a"));
                var path = link.Text;
                var newPath = MenuPath;
                if (newPath != "")
                    newPath += ".";
                newPath += path;
                var lowerMenu = item.FindElements(By.XPath($"./div/ul"));
                if (lowerMenu.Count == 0)
                {
                    callableMenus.Add(newPath);
                }
                else
                {
                    link.Click();
                    WaitForTime(1);

                    var nextMenu = lowerMenu[0];
                    DetermineMenuHierarchy(nextMenu, callableMenus, newPath);

                    var backLinks = item.FindElements(By.XPath($"./div/div/a"));
                    if (backLinks.Count != 0)
                    {
                        var back = backLinks[0];
                        back.Click();
                        WaitForTime(1);
                    }
                }
            }
        }
        public void DoLogin(string username, string password)
        {
            ensureIsOpen();
            var usernameInput = driver.FindElement(By.Id("LOGIN_USER_NAME"));
            var passwordInput = driver.FindElement(By.Id("LOGIN_PASSWORD"));
            var loginButton = driver.FindElement(By.CssSelector("input[type=submit][value=Login]"));

            usernameInput.SendKeys(username);
            passwordInput.SendKeys(password);
            loginButton.Click();

            WaitForPageLoad();
        }

        private string getBindingMemberByLabel(string label, out string changeHandler)
        {
            if (string.IsNullOrEmpty(label))
            {
                throw new ArgumentNullException(nameof(label));
            }

            var controlWrapper = driver.FindElement(By.XPath($"//label[contains(text(), '{label}')]//ancestor::div[contains(@class, 'pdms-page-control')]"));
            var targetControl = controlWrapper.FindElement(By.XPath(".//*[contains(@data-bind, 'value') or contains(@data-bind, 'checked')]"));
            var binding = targetControl.GetAttribute("data-bind");

            binding = binding.Replace("value :", "value:").Replace("checked :", "checked:");
            if (string.IsNullOrEmpty(binding) && (binding.Contains("value:")))
            {
                throw new InvalidOperationException("Target control missing binding information.");
            }

            changeHandler = "";
            var eventsBinding = binding.Split(',').Where(x => x.Contains("events:")).FirstOrDefault();
            if (!string.IsNullOrEmpty(eventsBinding))
            {
                eventsBinding = eventsBinding.Replace("events:", "").Replace("{", "").Replace("}", "").Replace("change :", "change:").Trim();
                changeHandler = eventsBinding.Split(',').Where(x => x.Contains("change:")).DefaultIfEmpty("change:").FirstOrDefault().Split(':')[1].Trim();
            }

            return binding.Split(',').Where(x => x.Contains("value:") || x.Contains("checked:")).FirstOrDefault().Split(':')[1].Trim();

        }

        private string getBindingMemberByFieldName(string fieldName, out string changeHandler)
        {
            if (string.IsNullOrEmpty(fieldName))
            {
                throw new ArgumentNullException(nameof(fieldName));
            }

            var targetControl = driver.FindElement(By.XPath($"//*[contains(@data-bind, '{fieldName}')]"));
            var binding = targetControl.GetAttribute("data-bind");

            binding = binding.Replace("value :", "value:").Replace("checked :", "checked:").Replace("events :", "events:");
            if (string.IsNullOrEmpty(binding))
            {
                throw new InvalidOperationException("Target control missing binding information.");
            }

            changeHandler = "";
            var eventsBinding = binding.Split(',').Where(x => x.Contains("events:")).FirstOrDefault();
            if (!string.IsNullOrEmpty(eventsBinding))
            {
                eventsBinding = eventsBinding.Replace("events:", "").Replace("{", "").Replace("}", "").Replace("change :", "change:").Trim();
                changeHandler = eventsBinding.Split(',').Where(x => x.Contains("change:")).DefaultIfEmpty("change:").FirstOrDefault().Split(':')[1].Trim();
            }

            return binding.Split(',').Where(x => x.Contains("value:") || x.Contains("checked:")).FirstOrDefault().Split(':')[1].Trim();

        }

        private string getBindingMemberBySourceName(string sourceName)
        {
            if (string.IsNullOrEmpty(sourceName))
            {
                throw new ArgumentNullException(nameof(sourceName));
            }

            var targetControl = driver.FindElement(By.XPath($"//*[contains(@data-bind, '{sourceName}')]"));
            var binding = targetControl.GetAttribute("data-bind");

            binding = binding.Replace("source :", "source:");
            if (string.IsNullOrEmpty(binding))
            {
                throw new InvalidOperationException("Target control missing binding information.");
            }

            return binding.Split(',').Where(x => x.Contains("source:")).FirstOrDefault().Split(':')[1].Trim();

        }

        private string getValueString(object value)
        {
            string result = "null";
            if (value != null)
            {
                var type = value.GetType();
                switch (type.Name)
                {
                    case "Boolean":
                        result = (bool)value ? "true" : "false";
                        break;
                    case "DateTime":
                        var dateValue = (DateTime)value;
                        result = $"PDMS.Utils.Date({dateValue.Year},{dateValue.Month - 1},{dateValue.Day})";
                        break;
                    case "String":
                    case "Char":
                        result = $"'{value}'";
                        break;
                    case "Byte":
                    case "Decimal":
                    case "Double":
                    case "Int16":
                    case "Int32":
                    case "Int64":
                    case "SByte":
                    case "Single":
                    case "UInt16":
                    case "UInt32":
                    case "UInt64":
                        result = $"{value}";
                        break;
                }
            }
            return result;
        }

       
       
        public void EnterDateFieldByElement(IWebElement datePicker, DateTime newDate)
        {
            ClickElement(datePicker);

            var genericCalendarElement = driver.FindElement(By.XPath("(//div[@data-role='calendar'])[2]"));
            var calendarId = genericCalendarElement.GetAttribute("id");
            var calendarElement = driver.FindElement(By.Id(calendarId));
            var calendarElementHeader = calendarElement.FindElement(By.XPath($"(//div[@Id='{calendarId}']//a[@data-action='nav-up'])"));
            var calendarPreviousBtn = calendarElement.FindElement(By.XPath($"(//div[@Id='{calendarId}']//a[@data-action='prev'])"));
            var calendarNextBtn = calendarElement.FindElement(By.XPath($"(//div[@Id='{calendarId}']//a[@data-action='next'])"));
                        

            //Select Year
            ClickElement(calendarElementHeader);
            WaitForTime(1);
            ClickElement(calendarElementHeader);

            var inputYear = newDate.Year;
            var yearPanelRows = calendarElement.FindElements(By.XPath($"//div[@Id='{calendarId}']//tr"));

            
            var firstYearInPanel = int.Parse(yearPanelRows[0].FindElement(By.TagName("td")).Text);
            var lastYearInPanel = int.Parse((yearPanelRows[2].FindElements(By.TagName("td")))[3].Text);

            var isYearAvailableOnScreen = false;

            
            
            while (!isYearAvailableOnScreen)
            {
                if (inputYear < firstYearInPanel)
                {
                    calendarPreviousBtn.Click();
                    //WaitForTime(1);
                    //ClickElement(datePicker);
                    yearPanelRows = calendarElement.FindElements(By.XPath($"//div[@Id='{calendarId}']//tr"));

                }
                else if (inputYear > lastYearInPanel)
                {
                    calendarNextBtn.Click();
                    //WaitForTime(1);
                    //ClickElement(datePicker);
                    yearPanelRows = calendarElement.FindElements(By.XPath($"//div[@Id='{calendarId}']//tr"));
                }
                else
                {
                    break;
                }

                firstYearInPanel = int.Parse(yearPanelRows[0].FindElement(By.TagName("td")).Text);
                lastYearInPanel = int.Parse((yearPanelRows[2].FindElements(By.TagName("td")))[3].Text);
            }

            bool isClicked = false;

            foreach (IWebElement yearRow in yearPanelRows)
            {
                var yearColumns = yearRow.FindElements(By.XPath($"//div[@Id='{calendarId}']//td/a"));
                foreach (IWebElement yearColumn in yearColumns)
                {
                    if (yearColumn.Text == inputYear.ToString())
                    {
                        yearColumn.Click();
                        isClicked = true;
                        break;
                    }
                }
                if (isClicked)
                {
                    break;
                }
            }

            //Select Month
            isClicked = false;
            var inputMonth = newDate.ToString("MMM", null);
            var monthPanelRows = calendarElement.FindElements(By.XPath($"//div[@Id='{calendarId}']//tr"));
            foreach (IWebElement monthRow in monthPanelRows)
            {
                var monthColumns = monthRow.FindElements(By.TagName("td"));
                foreach (IWebElement monthColumn in monthColumns)
                {
                    if(monthColumn.Text == inputMonth)
                    {
                        monthColumn.Click();
                        //ClickElement(datePicker);
                        isClicked = true;
                        break;
                    }
                }
                if (isClicked)
                {
                    break;
                }
            }

            if (!calendarElement.Displayed)
            {
                ClickElement(datePicker);
            }
            //Select Day
            isClicked = false;
            var inputDay = newDate.ToString("dd", null).TrimStart('0');
            var dayPanelRows = calendarElement.FindElements(By.XPath($"//div[@Id='{calendarId}']//tr"));
            foreach (IWebElement dayRow in dayPanelRows)
            {
                var dayColumns = dayRow.FindElements(By.XPath($"//div[@Id='{calendarId}']//td[not(contains(@class,'k-other-month'))]"));
                foreach (IWebElement dayColumn in dayColumns)
                {
                    if (dayColumn.Text == inputDay)
                    {
                        WaitForTime(1);
                        dayColumn.Click();
                        isClicked = true;
                        break;
                    }
                }
                if (isClicked)
                {
                    break;
                }
            }
        }

        public void EnterDateFieldByLabel(string label, DateTime newDate)
        {
            var controlWrapper = driver.FindElement(By.XPath($"//label[starts-with(text(),'{label}')]//ancestor::div[contains(@class, 'pdms-page-control')]"));
            var inputControl = controlWrapper.FindElement(By.XPath($".//descendant::input"));
            inputControl.Clear();
            inputControl.Click();
            var today = DateTime.Today;
            var diff = newDate.Day - today.Day;
            if (diff < 0)
            {
                for (int i = 0; i > diff; i--)
                {
                    inputControl.SendKeys(Keys.Down);
                }
            }
            else if (diff > 0)
            {
                for (int i = 0; i < diff; i++)
                {
                    inputControl.SendKeys(Keys.Up);
                }
            }
            else
            {
                inputControl.SendKeys(Keys.Up);
                inputControl.SendKeys(Keys.Down);
            }
            inputControl.SendKeys(Keys.Right);
            diff = newDate.Month - today.Month;
            if (diff < 0)
            {
                for (int i = 0; i > diff; i--)
                {
                    inputControl.SendKeys(Keys.Down);
                }
            }
            else if (diff > 0)
            {
                for (int i = 0; i < diff; i++)
                {
                    inputControl.SendKeys(Keys.Up);
                }
            }
            /*else
            {
                inputControl.SendKeys(Keys.Up);
                inputControl.SendKeys(Keys.Down);
            }*/
            inputControl.SendKeys(Keys.Right);
            diff = newDate.Year - today.Year;
            if (diff < 0)
            {
                for (int i = 0; i > diff; i--)
                {
                    inputControl.SendKeys(Keys.Down);
                }
            }
            else if (diff > 0)
            {
                for (int i = 0; i < diff; i++)
                {
                    inputControl.SendKeys(Keys.Up);
                }
            }
            /*else
            {
                inputControl.SendKeys(Keys.Up);
                inputControl.SendKeys(Keys.Down);
            }*/
            inputControl.SendKeys(Keys.Enter);
        }
       

        /*public void ClickFooterButton(string label)
        {
            if (string.IsNullOrEmpty(label))
            {
                throw new ArgumentNullException(nameof(label));
            }

            ensureIsOpen();
            var footerElement = driver.FindElement(By.XPath("//div[@class='buttons_line_wrapper']"));
            var buttonElement = footerElement.FindElement(By.XPath($"//button[contains(text(),'{label}')]"));
            buttonElement.Click();
        }*/
        public void ClickFooterButton(string label)
        {

            By by = By.XPath($"//button[contains(text(),'{label}')]");
            if (string.IsNullOrEmpty(label))
            {
                throw new ArgumentNullException(nameof(label));
            }

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementIsVisible(by));

            ensureIsOpen();
           // var footerElement = driver.FindElement(By.Id("LAYOUT_PAGE_FOOTER"));
            var buttonElement = driver.FindElement(by);
            buttonElement.Click();
        }

        public void DoLogout()
        {
            ensureIsOpen();
            driver.Navigate().GoToUrl($"{ baseUrl }/main-content/Log out");
            WaitForPageLoad();
        }

        public void GoToUrl(string url)
        {
            Driver.Navigate().GoToUrl(url);
        }


        public bool isElementExists(IWebElement ele)
        {
            bool isElementExists = false;

            try
            {
                if (ele.Displayed)
                {
                    isElementExists = true;
                }
            }
            catch (Exception e)
            {

            }
            return isElementExists;

        }
        public void ClickConfirmPopUpButton()
        {
            ClickFooterButton("Confirm");
        }
        public void RefreshPage()
        {
            driver.Navigate().Refresh();
            HandleAlert();

        }

        public bool IsDialogPresent()
        {
            IAlert alert = ExpectedConditions.AlertIsPresent().Invoke(driver);
            return (alert != null);
        }

        public void HandleAlert()
        {
            if (IsDialogPresent())
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                var alert = wait.Until(ExpectedConditions.AlertIsPresent());
                alert.Accept();

            }
        }
        public void ClickSaveAndCloseButton()
        {
            ClickFooterButton("Save & Close");
        }
       

        public void WaitForTime(int seconds)
        {
            Thread.Sleep(seconds * 1000);
        }

        public string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            return builder.ToString();
        }


    }
}
