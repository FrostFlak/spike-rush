using Helpers;
using UnityEngine;

namespace Models {
    public class Currency {
        public Observable<int> Coins;
        public Observable<int> Diamonds;

        public void Add(CurrencyType type, int amount) {
            switch (type) {
                case CurrencyType.Coin:
                    Coins.Set(Coins.Value() + amount);
                    break;
                
                case CurrencyType.Diamond:
                    Diamonds.Set(Diamonds.Value() + amount);
                    break;
            }
        }
        
        public bool Subtract(CurrencyType type, int amount) {
            switch (type) {
                case CurrencyType.Coin:
                    if (amount > Coins.Value())
                        return false;
                    
                    Coins.Set(Mathf.Clamp(Coins.Value() - amount, 0, int.MaxValue));
                    break;
                
                case CurrencyType.Diamond:
                    if (amount > Diamonds.Value())
                        return false;
                    
                    Diamonds.Set(Mathf.Clamp(Diamonds.Value() - amount, 0, int.MaxValue));
                    break;
            }

            return true;
        }  
    }
}