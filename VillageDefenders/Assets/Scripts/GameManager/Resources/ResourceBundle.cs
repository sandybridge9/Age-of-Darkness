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

    #endregion

    public ResourceBundle()
    {
        gold = 0;
        wood = 0;
        stone = 0;
        iron = 0;
    }

    public ResourceBundle(double gold, double wood, double stone, double iron)
    {
        this.gold = gold;
        this.wood = wood;
        this.stone = stone;
        this.iron = iron;
    }

    public bool CheckIfThereAreEnoughResources(ResourceBundle bundle)
    {
        return CheckIfThereAreEnoughResources(bundle.Gold, bundle.Wood, bundle.Stone, bundle.Iron);
    }

    private bool CheckIfThereAreEnoughResources(double gold, double wood, double stone, double iron)
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
        return true;
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
    }

    public void ReturnResources(ResourceBundle maximumCapacity, ResourceBundle resources, int percentage)
    {
        var goldAmount = (resources.Gold * percentage) / 100;
        var woodAmount = (resources.Wood * percentage) / 100;
        var stoneAmount = (resources.Stone * percentage) / 100;
        var ironAmount = (resources.Iron * percentage) / 100;
        if (gold + goldAmount >= maximumCapacity.gold)
        {
            gold = maximumCapacity.gold;
        }
        else
        {
            gold += goldAmount;
        }
        if (wood + woodAmount >= maximumCapacity.wood)
        {
            wood = maximumCapacity.wood;
        }
        else
        {
            wood += woodAmount;
        }
        if (stone + stoneAmount >= maximumCapacity.stone)
        {
            stone = maximumCapacity.stone;
        }
        else
        {
            stone += stoneAmount;
        }
        if (iron + ironAmount >= maximumCapacity.iron)
        {
            iron = maximumCapacity.iron;
        }
        else
        {
            iron += ironAmount;
        }
    }

    public void AddResources(ResourceBundle resources)
    {
        gold += resources.Gold;
        wood += resources.Wood;
        stone += resources.Stone;
        iron += resources.Iron;
    }
}
