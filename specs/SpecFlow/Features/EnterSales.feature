Feature: EnterSales
Entering sales into sale list

Link to a feature: https://trello.com/c/Hc0eCgqz
***Further read***: **[Learn more about how to generate Living Documentation](https://docs.specflow.org/projects/specflow-livingdoc/en/latest/LivingDocGenerator/Generating-Documentation.html)**

@mytag
Scenario: Entering a sale
	Given the seller id is 15
	And the price is 10
	When the sale is completed
	Then an item with id 15 and price 10 should be added to the list
	And the sum should be 10