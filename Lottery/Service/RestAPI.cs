
using Lottery.Domain;
using Lottery.Domain.Entity;
using Lottery.Domain.RequestBody;
using Lottery.Domain.ResponseBody;
using Lottery.POCO;
using System;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Lottery.Service;
public class RestAPI : IRestAPI
{
    private const string baseURL = "http://osiris.myddns.me:8080/api";
    private const string logoutURL = "http://osiris.myddns.me:8015/realms/LotteryKeycloak/protocol/openid-connect/logout";

    public RestAPI() 
    {}

    public async Task<MyNumbersPOCO> GetWinningnumbers(string whichOne)
    {
        using HttpClient client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(5);
        string token = await DatabaseService.GetLatestToken();
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
            Debug.WriteLine("\nException: " + ex.Message);
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
            Debug.WriteLine("\nException: " + ex.Message);
            return returnable;
        }

        returnable.prizes = new List<PrizesEntity>
        {
            prize5, prize6
        };

        return returnable;
    }

    public async Task<bool> uploadNumbers(List<int> numbers, int whichOne)
    {
        using HttpClient client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(5);
        string token = await DatabaseService.GetLatestToken();
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

    public async Task<bool> logOut()
    {
        using HttpClient client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(5);
        try
        {
            string refreshToken = await DatabaseService.GetLatestRefreshToken();
            List<KeyValuePair<string, string>> bodyData = new List<KeyValuePair<string, string>>();

            bodyData.Add(new KeyValuePair<string, string>("client_id", "login-app"));
            bodyData.Add(new KeyValuePair<string, string>("refresh_token", refreshToken));

            HttpContent content = new FormUrlEncodedContent(bodyData);
            HttpResponseMessage response = await client.PostAsync(logoutURL, content);
            if(!response.IsSuccessStatusCode)
            {
                return false;
            }
        }
        catch(Exception ex)
        {
            Debug.WriteLine("\nException: " + ex.Message);
            return false;
        }

        return true;
    }

    public async Task<SavedNumbersPOCO> getSavedNumbersFromAPI(int whichOne)
    {
        using HttpClient client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(5);
        try
        {
            string token = await DatabaseService.GetLatestToken();
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
            string token = await DatabaseService.GetLatestToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return await client.GetFromJsonAsync<List<PrizesEntity>>(baseURL + "/getLastYearPrize?whichOne=" + whichOne);
        }
        catch (Exception ex)
        {
            Debug.WriteLine("\nException: " + ex.Message);
            return null;
        }
    }
}
