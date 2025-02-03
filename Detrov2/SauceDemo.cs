using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;


namespace Detrov2;


public class Tests
{
    private IPage page;
    [SetUp]
    public async Task Setup()
    {
        var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions{
        Headless = false
        });
        var incognito = await browser.NewContextAsync(new BrowserNewContextOptions{
        });
        page = await browser.NewPageAsync();
    }

    [Test]
    public async Task Saucedemo()
    {
        // 1. Navigate to the Sauce Labs Sample Application (https://www.saucedemo.com/) in Incognito mode

        await page.GotoAsync(url: "https://www.saucedemo.com/");
        // 2. Enter valid credentials to log in
        await page.FillAsync(selector:"#user-name", value:"standard_user");
        await page.FillAsync(selector:"#password", value:"secret_sauce");

        // 3.1 Verify that the login is successful.
        await page.ClickAsync(selector:"#login-button");
        var loginFailure = await page.Locator("#login-button").IsVisibleAsync();
        Console.WriteLine(loginFailure ? "Login unsuccessful" : "Login successful");
        Assert.That(loginFailure,Is.False,  "Login should be successful");

        // 3.2 User is redirected to the products page.
        string currentPageUrl = page.Url;
        string expectedPageUrl = "https://www.saucedemo.com/inventory.html";
        if (currentPageUrl == expectedPageUrl)
        {
            Console.WriteLine("User is redirected to the products page");
        }
        else
        {
            Console.WriteLine("User is NOT redirected to the products page");
        }
        Assert.That(currentPageUrl,Is.EqualTo(expectedPageUrl),  $"User should be redirected to: {expectedPageUrl}");

        // 4. Select a T-shirt by clicking on its image or name.
        // Selecting first T-shirt
        await page.Locator("img[alt*='T-Shirt']").Nth(0).ClickAsync();

        // 5. Verify that the T-shirt details page is displayed.
        var description = await page.Locator("[data-test='inventory-item']").IsVisibleAsync();
        Console.WriteLine(description ? "Description is visible" : "Description is NOT visible");
        Assert.That(description,Is.True, "T-Shirt description should be displayed");
        Assert.That(page.Url.Contains("https://www.saucedemo.com/inventory-item"));
        var itemName = await page.Locator("[data-test='inventory-item-name']").TextContentAsync();
        var itemDescription = await page.Locator("[data-test='inventory-item-desc']").TextContentAsync();
        var itemPrice = await page.Locator("[data-test='inventory-item-price']").TextContentAsync();

        // 6. Click the "Add to Cart" button.
        await page.Locator("#add-to-cart").ClickAsync();

        // 7. Verify that the T-shirt is added to the cart successfully.
        var cartBadge = await page.Locator("[data-test='shopping-cart-badge']").TextContentAsync();
        Assert.That(cartBadge,Is.Not.Empty, "T-Shirt has not been added to the cart");

        // 8. Navigate to the cart by clicking the cart icon or accessing the cart page directly.
        await page.Locator("[data-test='shopping-cart-link']").ClickAsync();

        // 9. Verify that the cart page is displayed.
        string currentCartPageUrl = page.Url;
        string expectedCartPageUrl = "https://www.saucedemo.com/cart.html";
        Assert.That(currentCartPageUrl,Is.EqualTo(expectedCartPageUrl),  $"User should be redirected to: {expectedCartPageUrl}");

        // 10. Review the items in the cart and ensure that the T-shirt is listed with the correct details (name, price,quantity, etc.).
        var itenNameInCart = await page.Locator("[data-test='inventory-item-name']").TextContentAsync();
        var itemDescriptionInCart = await page.Locator("[data-test='inventory-item-desc']").TextContentAsync();
        var itemPriceInCart = await page.Locator("[data-test='inventory-item-price']").TextContentAsync();
        Assert.That(itenNameInCart, Is.EqualTo(itemName));
        Assert.That(itemDescriptionInCart, Is.EqualTo(itemDescription));
        Assert.That(itemPriceInCart, Is.EqualTo(itemPrice));

        // 11. Click the "Checkout" button.
        await page.ClickAsync(selector:"#checkout");

        // 12. Verify that the checkout information page is displayed.
        string currentCheckoutPageUrl = page.Url;
        string expectedCheckoutPageUrl = "https://www.saucedemo.com/checkout-step-one.html";
        Assert.That(currentCheckoutPageUrl,Is.EqualTo(expectedCheckoutPageUrl),  $"User should be redirected to: {expectedCheckoutPageUrl}");

        // 13. Enter the required checkout information (e.g., name, shipping address, payment details).
        await page.FillAsync(selector:"#first-name", value:"ExampleFirstName");
        await page.FillAsync(selector:"#last-name", value:"ExampleLastName");
        await page.FillAsync(selector:"#postal-code", value:"LV-1055");

        // 14. Click the "Continue" button.
        await page.Locator("#continue").ClickAsync();

        // 15. Verify that the order summary page is displayed, showing the T-shirt details and the total amount.
        string currentSummaryUrl = page.Url;
        string expectedSummaryUrl = "https://www.saucedemo.com/checkout-step-two.html";
        Assert.That(currentSummaryUrl,Is.EqualTo(expectedSummaryUrl),  $"User should be redirected to: {expectedSummaryUrl}");
        Assert.That(await page.Locator("[data-test='inventory-item-name']").TextContentAsync(), Is.EqualTo(itemName));
        Assert.That(await page.Locator("[data-test='total-label']").TextContentAsync(), Is.Not.Empty);

        // 16. Click the "Finish" button.
        await page.Locator("#finish").ClickAsync();

        // 17. Verify that the order confirmation page is displayed, indicating a successful purchase.
        string currentConfirmationUrl = page.Url;
        string epectedConfirmationUrl = "https://www.saucedemo.com/checkout-complete.html";
        Assert.That(currentConfirmationUrl,Is.EqualTo(epectedConfirmationUrl),  $"User should be redirected to: {epectedConfirmationUrl}");

        // 18. Logout from the application.
        await page.Locator("#react-burger-menu-btn").ClickAsync();
        await page.Locator("[data-test='logout-sidebar-link']").ClickAsync();

        // 19. Verify that the user is successfully logged out and redirected to the login page.
        string currentLogOutUrl = page.Url;
        string expectedLogOutUrl = "https://www.saucedemo.com/";
        Assert.That(currentLogOutUrl,Is.EqualTo(expectedLogOutUrl),  $"User should be redirected to: {expectedLogOutUrl}");
        
    }
}
