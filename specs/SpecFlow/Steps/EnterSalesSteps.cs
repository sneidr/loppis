using FluentAssertions;
using loppis.ViewModels;
using TechTalk.SpecFlow;

namespace LoppisSpecs.Steps
{
    [Binding]
    public class EnterSalesSteps
    {
        private readonly ScenarioContext m_scenarioContext;
        private SalesViewModel m_vm = new SalesViewModel();

        public EnterSalesSteps(ScenarioContext scenarioContext)
        {
            m_scenarioContext = scenarioContext;
        }

        [Given(@"the seller id is (.*)")]
        public void GivenTheSellerIdIs(int id)
        {
            m_vm.CurrentEntry.SellerId = id;
        }

        [Given(@"the price is (.*)")]
        public void GivenThePriceIs(int price)
        {
            m_vm.CurrentEntry.Price = price;
        }

        [When(@"the sale is completed")]
        public void WhenTheSaleIsCompleted()
        {
            m_vm.EnterSale();
        }

        [Then(@"an item with id (.*) and price (.*) should be added to the list")]
        public void ThenAnItemWithIdAndPriceShouldBeAddedToTheList(int id, int price)
        {
            m_vm.ItemList.Count.Should().Be(1);
            m_vm.ItemList[0].SellerId.Should().Be(id);
            m_vm.ItemList[0].Price.Should().Be(price);
        }

        [Then(@"the sum should be (.*)")]
        public void ThenTheSumShouldBe(int sum)
        {
            m_vm.labelTotal.Should().Be(sum);
        }
    }
}
