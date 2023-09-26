
using Lottery.Domain;
using Lottery.Domain.Entity;
using Lottery.Domain.RequestBody;
using Lottery.Domain.ResponseBody;
using Lottery.POCO;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Lottery.Service;
public class RestAPI : IRestAPI
{
    private const string baseURL = "http://osiris.myddns.me:8080/api";

    private IKeyCloakService keyCloakService;

    public RestAPI(IKeyCloakService keyCloak)  
    {
        keyCloakService = keyCloak;
    }

    public async Task<MyNumbersPOCO> GetWinningnumbers(int whichOne)
    {
        using HttpClient client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(5);
        string token = keyCloakService.GetSessionToken();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        WinningNumbersPOCO winningNumbersApi = new WinningNumbersPOCO();

        try
        {
            if (whichOne.Equals(5))
            {
                winningNumbersApi = await client.GetFromJsonAsync<WinningNumbersPOCO>(baseURL + "/getWinning5");
                Debug.WriteLine(winningNumbersApi.numbers);
                return MyNumberMapper.toPOCOFromWinning(winningNumbersApi);
            }
            else if (whichOne.Equals(6))
            {
                winningNumbersApi = await client.GetFromJsonAsync<WinningNumbersPOCO>(baseURL + "/getWinning6");
                return MyNumberMapper.toPOCOFromWinning(winningNumbersApi);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        catch(Exception ex)
        {
            Debug.WriteLine("\nException: " + ex.Message);
            return new MyNumbersPOCO();
        }
    }

    public async Task<PrizesHolderPOCO> GetPrizes()
    {
        using HttpClient client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(5);
        string token = keyCloakService.GetSessionToken();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        PrizesEntity prize5;
        PrizesEntity prize6;
        PrizesHolderPOCO returnable = new PrizesHolderPOCO();

        try
        {
            prize5 = await client.GetFromJsonAsync<PrizesEntity>(baseURL + "/getPrize5");
            prize6 = await client.GetFromJsonAsync<PrizesEntity>(baseURL + "/getPrize6");
            Debug.WriteLine("Prize5 that is from the API: " + prize5.prize + ";" + prize5.whichOne);
        }
        catch(Exception ex)
        {
            Debug.WriteLine("\nException: " + ex.Message);
            return returnable;
        }

        returnable.prizes = new List<PrizesEntity>
        {
            prize5, prize6
        };
        Debug.WriteLine("The list that is being returned: "+returnable.prizes.ToString());
        return returnable;
    }

    public async Task<bool> uploadNumbers(List<int> numbers, int whichOne)
    {
        using HttpClient client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(5);
        string token = keyCloakService.GetSessionToken();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        try
        {
            string numbersInList = "";

            for (int i = 0; i < numbers.Count; i++)
            {
                numbersInList = numbersInList + numbers[i] + ";";
            }

            var body = new DrawnLotteryNumbersPOCO(numbersInList, whichOne);
            var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(baseURL + "/upload", content);
            if(response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("\nException: " + ex.Message);
            return false;
        }
    }

    public async Task<SavedNumbersPOCO> getSavedNumbersFromAPI(int whichOne)
    {
        using HttpClient client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(5);
        try
        {
            string token = keyCloakService.GetSessionToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return await client.GetFromJsonAsync<SavedNumbersPOCO>(baseURL + "/downloadNumbers/" + whichOne.ToString());
        }
        catch (Exception ex)
        {
            Debug.WriteLine("\nException: " + ex.Message);
            return null;
        }
    }

    public async Task<List<PrizesEntity>> getLastYearPrizes(string whichOne)
    {
        using HttpClient client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(5);
        try
        {
            string token = keyCloakService.GetSessionToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return await client.GetFromJsonAsync<List<PrizesEntity>>(baseURL + "/getLastYearPrize?whichOne=" + whichOne);
        }
        catch (Exception ex)
        {
            Debug.WriteLine("\nException: " + ex.Message);
            return null;
        }
    }

    public async Task<List<LotteryWinnersDataPOCO>> getLatestWinnersData(int whichOne)
    {
        using HttpClient client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(5);
        try
        {
            string token = keyCloakService.GetSessionToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return await client.GetFromJsonAsync<List<LotteryWinnersDataPOCO>>(baseURL + "/getLatestWinnersData?whichOne=" + whichOne);
        }
        catch (Exception ex)
        {
            Debug.WriteLine("\nException: " + ex.Message);
            return null;
        }
    }
}
