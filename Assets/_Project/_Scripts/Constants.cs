using UnityEngine;

public static class Constants {

    #region Saves
    public const string ShredderSaveKey = "shredder"; 
    public const string CurrencySaveKey = "currency";
    public const string LevelsSaveKey = "levels";
    public const string CurrentLvlIDSaveKey = "currentLvlId";
    #endregion

    #region Upgrades
    private const float IncomeBaseMultiplier = 1.0f; 
    private const float LevelBonusStep = 0.05f;
    
    public const int UpgradeDefaultPrice = 100;
    public const float DefaultAcceleration = 1f;
    public const float DefaultPower = 0.5f;
    public const int DefaultFuel = 100;
    
    public const float UpgradePriceMultiplier = 1.15f;
    public const float AccelerationUpgradeStep = 0.5f;
    public const float PowerUpgradeStep = 0.5f;
    public const int FuelUpgradeStep = 5;

    public const int StepsToNewLevel = 3;
    #endregion
    
    public const int CoinsToDiamondsRate = 150;
    public const float UpgradeInDiamondsDiscount = 0.75f;

    public static int GetPriceForUpgrade(int level) {
        float rawPrice = UpgradeDefaultPrice * Mathf.Pow(UpgradePriceMultiplier, level - 1);
    
        int snapping = 10;
        if (rawPrice > 1000) 
            snapping = 50;

        return Mathf.RoundToInt(rawPrice / snapping) * snapping;
    }
    
    public static int GetPriceForUpgradeInDiamonds(int level) {
        float rawPriceInCoins = UpgradeDefaultPrice * Mathf.Pow(UpgradePriceMultiplier, level - 1);
        float priceInDiamonds = (rawPriceInCoins / CoinsToDiamondsRate) * UpgradeInDiamondsDiscount;

        int finalPrice = Mathf.CeilToInt(priceInDiamonds);
        if (finalPrice >= 10 && finalPrice < 100) {
            finalPrice = Mathf.RoundToInt(finalPrice / 2f) * 2;
        }
        else if (finalPrice >= 100) {
            finalPrice = Mathf.RoundToInt(finalPrice / 5f) * 5;
        }

        return finalPrice;
    }
    
    private static float GetTotalIncomeMultiplier(int totalUpgradeLevels) => IncomeBaseMultiplier + totalUpgradeLevels * LevelBonusStep;
    public static int GetMoneyByDistance(int distance, int totalLevels) => Mathf.RoundToInt(distance * GetTotalIncomeMultiplier(totalLevels));
    public static int GetMoneyByObject(int objectValue, int totalLevels) => Mathf.RoundToInt(objectValue * GetTotalIncomeMultiplier(totalLevels));
}
