Feature: PublicationsServiceTests

As a Developer
I want to add new Publication through API
In order to make it available for applications.
	Background:
		Given the Endpoint https://localhost:7207/api/v1/publications is available
	@publication-adding
	Scenario: Add Publication with unique Title
		When a Post Request is sent
		  | Description | UrlImage          | UserId |
		  | Sample      | A Sample UrlImage | 1          |
		Then A Response is received with Status 200
		And a Publication Resource is included in Response Body
		  | Id | Description | UrlImage          | UserId |
		  | 1  | Sample      | A Sample UrlImage | 1      |
	@publication-adding
	Scenario: Add Publication with existing Title
		Given A Publication is already stored
		  | Id | Description | UrlImage              | UserId |
		  | 1  | Ultimate    | The Ultimate UrlImage | 1      |
		When a Post Request is sent
		  | Description | UrlImage              | UserId |
		  | Ultimate    | The Ultimate UrlImage | 1      |
		Then A Response is received with Status 400
		And An Error Message is returned with value "Tutorial title already exists."