using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using CsvHelper;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        IWebDriver driver = new ChromeDriver();
        ExtentReports extent = new ExtentReports();

        string dateTime = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string reportFilePath = Path.Combine(Directory.GetCurrentDirectory(), $"Report_Sinthia.html");
        ExtentSparkReporter htmlreporter = new ExtentSparkReporter(reportFilePath);

        extent.AttachReporter(htmlreporter);
        ExtentTest test = extent.CreateTest("Parabank Registration & Login Automation", "Register, logout, login test");

        try
        {
            OpenUrl(driver, test, "https://parabank.parasoft.com/parabank/index.htm");

           using (var reader = new StreamReader(@"userdata\userdata.csv"))
           using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
           {
            var records = csv.GetRecords<UserData>().ToList();
            foreach (var record in records)
            {
                RegisterUser(driver, test, record);
                Logout(driver, test);
                PerformLogin(driver, test, record.Username, record.Password);
                ValidateLogin(driver, test);
                Logout(driver, test); // Logout after login validation
                Thread.Sleep(1000); // Wait for logout to complete
            }

           }

        }
        catch (Exception ex)
        {
            test.Log(Status.Fail, $"Test failed: {ex.Message}");
        }
        finally
        {
            extent.Flush();
            driver.Quit();
        }
    }

    static void OpenUrl(IWebDriver driver, ExtentTest test, string url)
    {
        test.Log(Status.Info, "Kazi Nusrat Sinthia \nDept of CSE, JnU\nAutomation Testing\nParabank Registration & Login Automation");
        driver.Navigate().GoToUrl(url);
        Thread.Sleep(1000);
        driver.Manage().Window.Maximize();
        test.Log(Status.Info, $"Navigated to {url}");
    }

    static void RegisterUser(IWebDriver driver, ExtentTest test, UserData data)
    {
        driver.FindElement(By.LinkText("Register")).Click();
        Thread.Sleep(1000);

        driver.FindElement(By.Id("customer.firstName")).SendKeys(data.FirstName);
        driver.FindElement(By.Id("customer.lastName")).SendKeys(data.LastName);
        driver.FindElement(By.Id("customer.address.street")).SendKeys(data.Address);
        driver.FindElement(By.Id("customer.address.city")).SendKeys(data.City);
        driver.FindElement(By.Id("customer.address.state")).SendKeys(data.State);
        driver.FindElement(By.Id("customer.address.zipCode")).SendKeys(data.ZipCode);
        driver.FindElement(By.Id("customer.phoneNumber")).SendKeys(data.Phone);
        driver.FindElement(By.Id("customer.ssn")).SendKeys(data.SSN);
        driver.FindElement(By.Id("customer.username")).SendKeys(data.Username);
        driver.FindElement(By.Id("customer.password")).SendKeys(data.Password);
        driver.FindElement(By.Id("repeatedPassword")).SendKeys(data.Password);

        driver.FindElement(By.CssSelector("input[value='Register']")).Click();
        Thread.Sleep(1000);
        test.Log(Status.Pass, $"User '{data.Username}' registered successfully.");
    }

    static void Logout(IWebDriver driver, ExtentTest test)
    {
        driver.FindElement(By.LinkText("Log Out")).Click();
        Thread.Sleep(1000);
        test.Log(Status.Info, "Logged out successfully.");
    }

    static void PerformLogin(IWebDriver driver, ExtentTest test, string username, string password)
    {
        driver.FindElement(By.Name("username")).SendKeys(username);
        driver.FindElement(By.Name("password")).SendKeys(password);
        driver.FindElement(By.CssSelector("input[value='Log In']")).Click();
        Thread.Sleep(1000);
        test.Log(Status.Info, $"Attempted login for user: {username}");
    }

    static void ValidateLogin(IWebDriver driver, ExtentTest test)
    {
        try
        {
            driver.FindElement(By.LinkText("Log Out"));
            test.Log(Status.Pass, "Login successful ✅");
        }
        catch
        {
            test.Log(Status.Fail, "Login failed ❌");
        }
    }
}

public class UserData
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string ZipCode { get; set; }
    public string Phone { get; set; }
    public string SSN { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}
