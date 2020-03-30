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

    public void ReturnResources(ResourceBundle resources, int percentage)
    {
        gold += (resources.Gold * percentage) / 100;
        wood += (resources.Wood * percentage) / 100;
        stone += (resources.Stone * percentage) / 100;
        iron += (resources.Iron * percentage) / 100;
    }
}
