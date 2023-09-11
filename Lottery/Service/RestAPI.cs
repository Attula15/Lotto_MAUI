
using Lottery.Domain;
using Lottery.Domain.Entity;
using Lottery.POCO;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Lottery.Service;
public class RestAPI : IRestAPI
{
    private const string baseURL = "http://osiris.myddns.me:8080/api";
    
    public RestAPI() 
    {}

    public async Task<MyNumbersPOCO> GetWinningnumbers(string whichOne)
    {
        using HttpClient client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(5);
        string token = await DatabaseService.GetLatestToken();
        Debug.WriteLine("Used token: " + token);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        WinningNumbersEntity winningNumbers = new WinningNumbersEntity();

        try
        {
            if (whichOne.Equals("5"))
            {
                winningNumbers = await client.GetFromJsonAsync<WinningNumbersEntity>(baseURL + "/getWinning5");
                Debug.WriteLine(winningNumbers.numbers);
                return MyNumberMapper.toPOCOFromWinning(winningNumbers);
            }
            else if (whichOne.Equals("6"))
            {
                winningNumbers = await client.GetFromJsonAsync<WinningNumbersEntity>(baseURL + "/getWinning6");
                return MyNumberMapper.toPOCOFromWinning(winningNumbers);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        catch(Exception ex)
        {
            return new MyNumbersPOCO();
        }
    }

    public async Task<PrizesHolderPOCO> GetPrizes()
    {
        using HttpClient client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(5);
        string token = await DatabaseService.GetLatestToken();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        PrizesEntity prize5;
        PrizesEntity prize6;
        PrizesHolderPOCO returnable = new PrizesHolderPOCO();

        try
        {
            prize5 = await client.GetFromJsonAsync<PrizesEntity>(baseURL + "/getPrize5");
            prize6 = await client.GetFromJsonAsync<PrizesEntity>(baseURL + "/getPrize6");
        }
        catch(Exception ex)
        {
            return returnable;
        }

        returnable.prizes = new List<PrizesEntity>
        {
            prize5, prize6
        };

        return returnable;
    }
}
