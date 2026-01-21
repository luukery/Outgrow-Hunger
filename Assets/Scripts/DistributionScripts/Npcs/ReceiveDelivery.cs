using System.Collections.Generic;

using System.Diagnostics;

public class DeliveryResultService

{
    public DeliveryResult Transaction(List<Request> order, List<Request> needs, List<Request> given, NpcDialogue reactions)
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

        for (int i = 0; i < given.Count; i++)

        {

            bool foundInOrder = false;

            bool foundInNeeds = false;

            for (int j = 0; j < order.Count; j++)

            {

                if (order[j].FoodType == given[i].FoodType)

                {

                    foundInOrder = true;

                }

            }

            for (int j = 0; j < needs.Count; j++)

            {

                if (needs[j].FoodType == given[i].FoodType)

                {

                    foundInNeeds = true;

                }

            }

            if (!foundInOrder && !foundInNeeds)

            {

                result.WrongItems.Add(given[i]);

            }

        }



        result.reaction = Reaction(result, reactions);

        //

        return result;

    }

    public string Reaction(DeliveryResult result, NpcDialogue reactions)

    {
        // Niets geleverd

        if (result.TotalDelivered.Count == 0)

            return reactions.giveNothing;

        // Alles verkeerd geleverd

        if (result.Shortages.Count == result.TotalOrder.Count)

            return reactions.onlyWrongItems;

        // Tekorten in order

        if (result.Shortages.Count > 0)

            return reactions.littleShortage;

        // Te veel geleverd

        if (result.Excesses.Count > 0)

        {

            if (result.Excesses.Count > result.TotalOrder.Count / 2)

                return reactions.bigExcess;

            return reactions.littleExcess;

        }

        // Alles correct + needs gevuld

        if (result.NeedsShortage.Count == 0)

            return reactions.allNeedsFilled;

        // Alles correct volgens order

        return reactions.delivered;

    }

}

