using Lottery.Domain.Database.Entity;
using Lottery.Domain.Entity;
using Lottery.Domain.ResponseBody;
using Lottery.POCO;
using System.Diagnostics;

public static class MyNumberMapper
{
    public static MyNumbersPOCO toPOCOFromDBEntity(MyNumbersEntity entity)
    {
        string[] listOfNumbersInString = entity.numbers.Split(";");
        List<int> extractedNumbers = new List<int>();

        foreach (string number in listOfNumbersInString)
        {
            if (number != "")
            {
                extractedNumbers.Add(int.Parse(number.Replace(";", "").Trim()));
            }
        }

        return new MyNumbersPOCO
            (
                entity.id,
                entity.date,
                entity.numberType,
                extractedNumbers
            );
    }

    public static MyNumbersPOCO toPOCOFromWinning(WinningNumbersPOCO entity)
    {
        string[] listOfNumbersInString = entity.numbers.Split(";");
        List<int> extractedNumbers = new List<int>();

        foreach (string number in listOfNumbersInString)
        {
            bool transformable = true;

            try
            {
                int.Parse(number.Replace(";", "").Trim());
            }
            catch(FormatException ex)
            {
                Debug.WriteLine(ex.Message);
                transformable = false;
            }

            if (transformable)
            {
                extractedNumbers.Add(int.Parse(number.Replace(";", "").Trim()));
            }
        }

        return new MyNumbersPOCO
            (
                entity.id,
                entity.date,
                entity.whichOne,
                extractedNumbers
            );
    }

    public static MyNumbersPOCO toPOCOFromSavedNumbersPOCO(SavedNumbersPOCO saved, int whichOne)
    {
        if(saved == null)
        {
            return null;
        }

        MyNumbersPOCO returnable = new MyNumbersPOCO();
        string[] listOfNumbersInString = saved.numbers.Split(";");
        List<int> extractedNumbers = new List<int>();

        foreach (string number in listOfNumbersInString)
        {
            bool transformable = true;

            try
            {
                int.Parse(number.Replace(";", "").Trim());
            }
            catch (FormatException ex)
            {
                Debug.WriteLine(ex.Message);
                transformable = false;
            }

            if (transformable)
            {
                extractedNumbers.Add(int.Parse(number.Replace(";", "").Trim()));
            }
        }
        returnable.numbers = extractedNumbers;
        returnable.date = saved.date;
        returnable.numberType = whichOne;

        return returnable;
    }
}

