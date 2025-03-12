using Microsoft.Playwright;
using Xunit;

namespace UKParliament.CodeTest.Tests.UITests
{
    public class PersonUiTests : IAsyncLifetime
    {
        private IPlaywright _playwright;
        private IBrowser _browser;
        private IPage _page;
        private string _baseUrl;

        public async Task InitializeAsync()
        {
            _baseUrl = TestConfig.BaseUrl;
            _playwright = await Playwright.CreateAsync();

            bool headless = (bool.TryParse(TestConfig.Headless, out bool result)) ? result : false;
            
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = headless
            });

            var context = await _browser.NewContextAsync();
            _page = await context.NewPageAsync();
        }

        [Fact]
        public async Task Should_Add_Person_Successfully()
        {
            //Act
            string genName = Guid.NewGuid().ToString();
            await _page.GotoAsync(_baseUrl);

            await _page.ClickAsync("#addPersonButton");

            await _page.FillAsync("#firstName", "John");
            await _page.FillAsync("#lastName", genName);
            await _page.FillAsync("#dateOfBirth", "1990-01-01");
            await _page.SelectOptionAsync("#department", new SelectOptionValue { Label = "HR" });

            // Wait for the Save button to be enabled before clicking
            await _page.Locator("#saveButton").WaitForAsync(new() { State = WaitForSelectorState.Visible });

            await _page.ClickAsync("#saveButton");

            // Wait for the new person to appear in the list
            await _page.Locator($"text=John {genName}").WaitForAsync(new() { State = WaitForSelectorState.Visible });

            var personExists = await _page.Locator($"text={genName}").IsVisibleAsync();
            
            //Assert
            Assert.True(personExists, $"Person with LastName {genName} is not found in the list of people");
        }

        [Fact]
        public async Task Should_Display_Client_Validation_Fields_Error()
        {
           //Act
            await _page.GotoAsync(_baseUrl);

            await _page.ClickAsync("#addPersonButton");

            //Do not fill in First name
            //touch
            await _page.ClickAsync("#firstName");
            await _page.PressAsync("#firstName", "Tab"); // Move focus away

            await _page.FillAsync("#lastName", "Doe");
            await _page.FillAsync("#dateOfBirth", "1990-01-01");
            await _page.SelectOptionAsync("#department", new SelectOptionValue { Label = "HR" });

            //Assert
            var saveButton = _page.Locator("#saveButton");
            var isDisabled = await saveButton.GetAttributeAsync("disabled") != null;
            Assert.True(isDisabled, "Save button should be disabled.");
            
            await _page.Locator("em.error").WaitForAsync(new() { State = WaitForSelectorState.Visible });
            var errorMessageText = await _page.Locator("em.error").InnerTextAsync();
            Assert.Equal("First Name is required", errorMessageText.Trim());
        }

        [Fact]
        public async Task Should_Display_Server_Validation_Fields_Error()
        {
            await _page.GotoAsync(_baseUrl);

            await _page.ClickAsync("#addPersonButton");

            //Fill in spaces in First name            
            await _page.FillAsync("#firstName", "   ");
            await _page.FillAsync("#lastName", "Doe");
            await _page.FillAsync("#dateOfBirth", "1990-01-01");
            await _page.SelectOptionAsync("#department", new SelectOptionValue { Label = "HR" });
            
            // Wait for the Save button to be visible and enabled
            await _page.Locator("#saveButton").WaitForAsync(new() { State = WaitForSelectorState.Visible });
            await _page.ClickAsync("#saveButton");
            await _page.Locator("em.saveError").WaitForAsync(new() { State = WaitForSelectorState.Visible });

            var errorMessage = await _page.Locator("em.saveError").InnerTextAsync();
            Assert.Contains("First Name is required", errorMessage.Trim());
        }



        [Fact]
        public async Task Should_Not_Create_Duplicate_Person()
        {
            //Arrange
            string genName = Guid.NewGuid().ToString();
            await _page.GotoAsync(_baseUrl);

            //Create person
            await _page.ClickAsync("#addPersonButton");

            await _page.FillAsync("#firstName", "Joe");
            await _page.FillAsync("#lastName", genName);
            await _page.FillAsync("#dateOfBirth", "1990-01-01");
            await _page.SelectOptionAsync("#department", new SelectOptionValue { Label = "HR" });

            // Wait for the Save button to be visible and enabled
            await _page.Locator("#saveButton").WaitForAsync(new() { State = WaitForSelectorState.Visible });

            await _page.ClickAsync("#saveButton");

            // Validate that the person was successfully added
            await _page.Locator($"text=Joe {genName}").WaitForAsync(new() { State = WaitForSelectorState.Visible });

            var personExists = await _page.Locator($"text=Joe {genName}").IsVisibleAsync();
            Assert.True(personExists);

            //Act
            //Create same person
            await _page.ClickAsync("#addPersonButton");

            await _page.FillAsync("#firstName", "Joe");
            await _page.FillAsync("#lastName", genName);
            await _page.FillAsync("#dateOfBirth", "1990-01-01");
            await _page.SelectOptionAsync("#department", new SelectOptionValue { Label = "HR" });

            // Wait for the Save button to be visible and enabled
            await _page.Locator("#saveButton").WaitForAsync(new() { State = WaitForSelectorState.Visible });

            await _page.ClickAsync("#saveButton");

            await _page.Locator("em.saveError").WaitForAsync(new()
            {
                State = WaitForSelectorState.Visible,
                Timeout = 1000  // Adjust timeout as needed
            });

            //Assert
            //API exceptions are displayed in em.saveError. In this case - DuplicateException
            await _page.Locator("em.saveError").WaitForAsync(new() { State = WaitForSelectorState.Visible });
            var errorMessageText = await _page.Locator("em.saveError").InnerTextAsync();
            Assert.Equal("A person with the same first name, last name, and date of birth already exists.", errorMessageText.Trim());
        }
        public async Task DisposeAsync()
        {
            await _browser.CloseAsync();
            _playwright.Dispose();
        }
    }

}
