using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSD.UI.Tests.Core
{
    public interface IAssureWeb
    {
        static bool IsOpen { get; }
        string GetWindowTitle();
      
        string GetPageTitle();
        void DoLogin(string username, string password);
        void DoLogout();
        void WaitForPageLoad();
        void WaitForPopup();
        void NavigateMainMenu(params string[] links);
       
        List<String> DetermineMenuHierarchy();
        
       
        void ClickFooterButton(string label);
        
        void Open(string url);
        void Close();
        void Quit();
       
    }
}
