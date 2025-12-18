using System.Collections.Generic;

public class DeliveryResultService
{
    public DeliveryResult Transaction(List<Request> order, List<Request> given)
    {
        DeliveryResult result = new DeliveryResult();

        List<Request> orderedTotals = new List<Request>();
        List<Request> deliveredTotals = new List<Request>();

        for (int i = 0; i < order.Count; i++)
        {
            Request r = order[i];
            Request existing = FindByType(orderedTotals, r.FoodType);

            if (existing == null)
            {
                orderedTotals.Add(
                    new Request(r.Amount, r.FoodType, r.Quality)
                );
            }
            else
            {
                existing.Amount += r.Amount;
            }

            result.TotalOrderedAmount += r.Amount;
        }

        for (int i = 0; i < given.Count; i++)
        {
            Request r = given[i];
            Request existing = FindByType(deliveredTotals, r.FoodType);

            if (existing == null)
            {
                deliveredTotals.Add(
                    new Request(r.Amount, r.FoodType, r.Quality)
                );
            }
            else
            {
                existing.Amount += r.Amount;
            }

            result.TotalDeliveredAmount += r.Amount;
        }

        for (int i = 0; i < orderedTotals.Count; i++)
        {
            Request ordered = orderedTotals[i];
            Request delivered = FindByType(deliveredTotals, ordered.FoodType);

            int deliveredAmount = delivered != null ? delivered.Amount : 0;

            if (deliveredAmount < ordered.Amount)
            {
                int shortage = ordered.Amount - deliveredAmount;
                result.TotalFoodShortage += shortage;
                result.Shortages.Add(
                    new Request(shortage, ordered.FoodType, ordered.Quality)
                );
            }
            else if (deliveredAmount > ordered.Amount)
            {
                int excess = deliveredAmount - ordered.Amount;
                result.TotalFoodExcess += excess;
                result.Excesses.Add(
                    new Request(excess, ordered.FoodType, ordered.Quality)
                );
            }
        }

        return result;
    }

    Request FindByType(List<Request> list, FoodType.Type type)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].FoodType == type)
            {
                return list[i];
            }
        }

        return null;
    }
}
