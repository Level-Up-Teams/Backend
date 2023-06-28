using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using LevelUpCenter.LookUrClimb.Resources;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using SpecFlow.Internal.Json;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using Xunit;

namespace LevelUpCenter.Test.Steps;

[Binding]
public sealed class PublicationsServiceStepDefinition : WebApplicationFactory<Program>
{
    private readonly WebApplicationFactory<Program> _factory;

    public PublicationsServiceStepDefinition(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }
    
    private HttpClient Client { get; set; }
    private Uri BaseUri { get; set; }
    private Task<HttpResponseMessage> Response { get; set; }
    
    [Given(@"the Endpoint https://localhost:(.*)/api/v(.*)/publications is available")]
    public void GivenTheEndpointHttpsLocalhostApiVPublicationsIsAvailable(int port, int version)
    {
        BaseUri = new Uri($"https://localhost:{port}/api/v{version}/publications");
        Client = _factory.CreateClient(new WebApplicationFactoryClientOptions {
            BaseAddress = BaseUri });
    }

    [When(@"a Post Request is sent")]
    public void WhenAPostRequestIsSent(Table savePublicationResource)
    {
        var resource = savePublicationResource.CreateSet<SavePublicationResource>().First();
        var content = new StringContent(resource.ToJson(), Encoding.UTF8, MediaTypeNames.Application.Json);
        Response = Client.PostAsync(BaseUri, content);
    }    

    [Then(@"A Response is received with Status (.*)")]
    public void ThenAResponseIsReceivedWithStatus(int expectedStatus)
    {
        var expectedStatusCode = ((HttpStatusCode)expectedStatus).ToString();
        var actualStatusCode = Response.Result.StatusCode.ToString();

        Assert.Equal(expectedStatusCode, actualStatusCode);
    }
    
    [Then(@"a Publication Resource is included in Response Body")]
    public async Task ThenAPublicationResourceIsIncludedInResponseBody(Table expectedPublicationResource)
    {
        var expectedResource = expectedPublicationResource.CreateSet<PublicationResource>().First();
        var responseData = await Response.Result.Content.ReadAsStringAsync();
        var resource = JsonConvert.DeserializeObject<PublicationResource>(responseData);
        Assert.Equal(expectedResource.Title, resource.Title);
    }

    [Given(@"A Publication is already stored")]
    public async void GivenAPublicationIsAlreadyStored(Table savePublicationResource)
    {
        var resource = savePublicationResource.CreateSet<SavePublicationResource>().First();
        var content = new StringContent(resource.ToJson(), Encoding.UTF8, MediaTypeNames.Application.Json);
        Response = Client.PostAsync(BaseUri, content);
        var responseData = await Response.Result.Content.ReadAsStringAsync();
        var responseResource = JsonConvert.DeserializeObject<PublicationResource>(responseData);
        Assert.Equal(resource.Title, responseResource.Title);
    }
    
    public void ThenAnErrorMessageIsReturnedWithValue(string expectedMessage)
    {
        var message = Response.Result.Content.ReadAsStringAsync().Result;
        Assert.Equal(expectedMessage, message);
    }

}