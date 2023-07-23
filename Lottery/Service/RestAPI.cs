
using CommunityToolkit.Mvvm.Messaging.Messages;
using Lottery.Domain;
using Lottery.Domain.Entity;
using System;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Lottery.Service;
public class RestAPI : IRestAPI
{
    private const string baseURL = "http://89.132.218.168:8080/api";
    private const string user = "nepthys";
    private const string passwd = "Hernita5";
    private const string authenticationString = $"{user}:{passwd}";
    private string base64String = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(authenticationString));
    
    public RestAPI() 
    {}

    public async Task<WinningNumbersEntity> GetWinningnumbers(string whichOne)
    {
        using HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64String);


        if (whichOne.Equals("5"))
        {
            return await client.GetFromJsonAsync<WinningNumbersEntity>(baseURL + "/getWinning5");
        }
        else if (whichOne.Equals("6"))
        {
            return await client.GetFromJsonAsync<WinningNumbersEntity>(baseURL + "/getWinning6");
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    public async Task<PrizesHolderEntity> GetPrizes()
    {
        using HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64String);

        PrizesHolderEntity returnable = new PrizesHolderEntity();
        returnable.prizes = new List<PrizesEntity>
        {
            await client.GetFromJsonAsync<PrizesEntity>(baseURL + "/getPrize5"),
            await client.GetFromJsonAsync<PrizesEntity>(baseURL + "/getPrize6")
        };

        return returnable;
    }
}
