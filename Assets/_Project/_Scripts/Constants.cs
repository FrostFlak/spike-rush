using UnityEngine;

public static class Constants {

    #region Saves
    public const string ShredderSaveKey = "shredder"; 
    public const string CurrencySaveKey = "currency";
    public const string LevelsSaveKey = "levels";
    public const string CurrentLvlIDSaveKey = "currentLvlId";
    public const string SettingsSaveKey = "settings";
    #endregion

    #region Income
    private const float IncomeBaseMultiplier = 1f;
    private const float LevelBonusStep = 0.1f;
    #endregion
    
    #region DefaultValues
    public const int UpgradeDefaultPrice = 75;
    public const float DefaultAcceleration = 1f;
    public const float DefaultPower = 1f;
    public const int DefaultFuel = 75;
    public const int DefaultMaxSpeed = 5;
    #endregion

    #region Multipliers
    public const float UpgradePriceMultiplier = 1.175f;
    public const float MaxSpeedMultiplier = 1.035f;
    public const float AccelerationUpgradeStep = 0.15f;
    public const float PowerUpgradeStep = 0.12f;
    public const int FuelUpgradeStep = 3;
    #endregion
    
    #region Limits
    public const float MaxSpeedLimit = 13.75f;
    public const int StepsToNewLevel = 3;
    #endregion
    
    public const int CoinsToDiamondsRate = 150;
    public const float UpgradeInDiamondsDiscount = 0.5f;

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
    public static int GetCoinsByDistance(int distance, int levelDistance, int totalLevels) {
        float baseReward = distance * 0.1f; 
    
        float progress = (float)distance / levelDistance;
        float progressBonus = progress * 50f; // Например, до +50 монет за полный уровень
    
        float multiplier = GetTotalIncomeMultiplier(totalLevels);
    
        return Mathf.RoundToInt((baseReward + progressBonus) * multiplier);
    }
    public static int GetCoinsByObject(int objectValue, int totalLevels) => Mathf.RoundToInt(objectValue * GetTotalIncomeMultiplier(totalLevels));

    public static int GetMaxSpeedByAccelerationLvl(int level) => Mathf.RoundToInt(Mathf.Clamp(DefaultMaxSpeed * Mathf.Pow(MaxSpeedMultiplier, level - 1),DefaultMaxSpeed, MaxSpeedLimit));
}
