Feature: US01 - Book Dedication
	As a potential customer
	I want to read the dedication
	So that I can inspire myself.

Scenario: Book's dedication
	When I open the dedication page for "Domain Driven Design"
	Then I read
		"""
		To Mom
		And Dad
		"""