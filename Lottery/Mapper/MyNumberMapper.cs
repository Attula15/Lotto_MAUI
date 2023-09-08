using Lottery.Domain.Database.Entity;
using Lottery.POCO;

public static class MyNumberMapper
{
    public static MyNumbersPOCO toPOCO(MyNumbersEntity entity)
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
}

