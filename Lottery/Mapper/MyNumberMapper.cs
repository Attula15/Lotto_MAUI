using Lottery.Domain.Database.Entity;
using Lottery.Domain.Entity;
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

    public static MyNumbersPOCO toPOCOFromWinning(WinningNumbersEntity entity)
    {
        string[] listOfNumbersInString = entity.numbers.Split(";");
        List<int> extractedNumbers = new List<int>();

        foreach (string number in listOfNumbersInString)
        {
            Debug.WriteLine("Number: " + number);
            bool transformable = true;

            try
            {
                int.Parse(number.Replace(";", "").Trim());
            }
            catch(FormatException ex)
            {
                transformable = false;
            }

            if (transformable)
            {
                Debug.WriteLine("Parseable: " + number.Replace(";", "").Trim());
                extractedNumbers.Add(int.Parse(number.Replace(";", "").Trim()));
            }
        }
        Debug.WriteLine("Extracted Numbers: " + extractedNumbers);
        return new MyNumbersPOCO
            (
                entity.id,
                entity.date,
                entity.whichOne,
                extractedNumbers
            );
    }
}

