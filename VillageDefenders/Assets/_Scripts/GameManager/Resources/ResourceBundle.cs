using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceBundle
{
    #region Fields

    private double gold;
    private double wood;
    private double stone;
    private double iron;
    private double food;

    #endregion

    #region Properties

    public double Gold
    {
        get { return gold; }
    }

    public double Wood
    {
        get { return wood; }
    }

    public double Stone
    {
        get { return stone; }
    }

    public double Iron
    {
        get { return iron; }
    }

    public double Food
    {
        get { return food; }
    }

    #endregion

    public ResourceBundle()
    {
        gold = 0;
        wood = 0;
        stone = 0;
        iron = 0;
        food = 0;
    }

    public ResourceBundle(double gold, double wood, double stone, double iron, double food)
    {
        this.gold = gold;
        this.wood = wood;
        this.stone = stone;
        this.iron = iron;
        this.food = food;
    }

    public bool CheckIfThereAreEnoughResources(ResourceBundle bundle)
    {
        return CheckIfThereAreEnoughResources(bundle.Gold, bundle.Wood, bundle.Stone, bundle.Iron, bundle.Food);
    }

    private bool CheckIfThereAreEnoughResources(double gold, double wood, double stone, double iron, double food)
    {
        if (Gold < gold)
        {
            return false;
        }
        if (Wood < wood)
        {
            return false;
        }
        if (Stone < stone)
        {
            return false;
        }
        if (Iron < iron)
        {
            return false;
        }
        if (Food < food)
        {
            return false;
        }
        return true;
    }

    public bool HasReachedMaximumCapacity(ResourceBundle maximumCapacity)
    {
        bool hasReached = false;
        if (gold >= maximumCapacity.Gold)
        {
            gold = maximumCapacity.Gold;
            hasReached = true;
        }
        if (wood >= maximumCapacity.Wood)
        {
            wood = maximumCapacity.Wood;
            hasReached = true;
        }
        if (stone >= maximumCapacity.Stone)
        {
            stone = maximumCapacity.Stone;
            hasReached = true;
        }
        if (iron >= maximumCapacity.Iron)
        {
            iron = maximumCapacity.Iron;
            hasReached = true;
        }
        if (food >= maximumCapacity.Food)
        {
            food = maximumCapacity.Food;
            hasReached = true;
        }
        return hasReached;
    }

    public bool SubtractResources(ResourceBundle cost)
    {
        if (CheckIfThereAreEnoughResources(cost))
        {
            Subtract(cost);
            return true;
        }
        else
        {
            return false;
        }
    }

    private void Subtract(ResourceBundle cost)
    {
        gold -= cost.Gold;
        wood -= cost.Wood;
        stone -= cost.stone;
        iron -= cost.iron;
        food -= cost.food;
    }

    public void ReturnResources(ResourceBundle maximumCapacity, ResourceBundle resources, int percentage)
    {
        var goldAmount = (resources.Gold * percentage) / 100;
        var woodAmount = (resources.Wood * percentage) / 100;
        var stoneAmount = (resources.Stone * percentage) / 100;
        var ironAmount = (resources.Iron * percentage) / 100;
        var foodAmount = (resources.Food * percentage) / 100;
        if (gold + goldAmount >= maximumCapacity.gold)
        {
            gold = maximumCapacity.gold;
        }
        else
        {
            gold += goldAmount;
        }
        if (wood + woodAmount >= maximumCapacity.Wood)
        {
            wood = maximumCapacity.wood;
        }
        else
        {
            wood += woodAmount;
        }
        if (stone + stoneAmount >= maximumCapacity.Stone)
        {
            stone = maximumCapacity.stone;
        }
        else
        {
            stone += stoneAmount;
        }
        if (iron + ironAmount >= maximumCapacity.Iron)
        {
            iron = maximumCapacity.iron;
        }
        else
        {
            iron += ironAmount;
        }
        if (food + foodAmount >= maximumCapacity.Food)
        {
            food = maximumCapacity.food;
        }
        else
        {
            food += foodAmount;
        }
    }

    public void AddResources(ResourceBundle resources)
    {
        gold += resources.Gold;
        wood += resources.Wood;
        stone += resources.Stone;
        iron += resources.Iron;
        food += resources.Food;
    }

    public override string ToString()
    {
        return "Gold:" +gold +" Wood: " +wood +" Stone: " +stone +" Iron: " +iron +" Food:"  +food;
    }
}
