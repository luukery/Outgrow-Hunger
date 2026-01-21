using System.Collections.Generic;

public class DeliveryResultService
{
    public DeliveryResult Transaction(List<Request> order, List<Request> needs, List<Request> given)
    {
        DeliveryResult result = new DeliveryResult();

       result.TotalOrder = order;
       result.TotalDelivered = given;



        for (int i = 0; i < order.Count; i++)
        {
            Request wanted = order[i];
            int givenAmount = 0;

            for (int j = 0; j < given.Count; j++)
            {
                if (given[j].FoodType == wanted.FoodType)
                {
                    givenAmount += given[j].Amount;
                }
            }

            int diff = givenAmount - wanted.Amount;

            if (diff < 0)
            {
                result.Shortages.Add(new Request(-diff, wanted.FoodType, wanted.Quality));
            }
            else if (diff > 0)
            {
                result.Excesses.Add(new Request(diff, wanted.FoodType, wanted.Quality));
            }
        }

        for (int i = 0; i < needs.Count; i++)
        {
            Request need = needs[i];
            int givenAmount = 0;

            for (int j = 0; j < given.Count; j++)
            {
                if (given[j].FoodType == need.FoodType)
                {
                    givenAmount += given[j].Amount;
                }
            }

            if (givenAmount < need.Amount)
            {
                int diff = need.Amount - givenAmount;
                result.NeedsShortage.Add(new Request(diff, need.FoodType, need.Quality));
            }
        }

        return result;
    }
}
