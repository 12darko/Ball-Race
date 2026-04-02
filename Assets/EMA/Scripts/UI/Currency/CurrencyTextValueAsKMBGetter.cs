using System;
using UnityEngine;
using UnityEngine.UI;

namespace EMA.Scripts.UI.Currency
{
    public static class CurrencyTextValueAsKMBGetter
    {
        public static String GetCurrencyText(int currency)
        {
            var _checker = currency.ToString().Length;
            var _resultTxt = "";

            if (_checker < 3) {
                _resultTxt = currency.ToString();
            }
            else if (_checker < 7) {
                _resultTxt = (currency / 1000f).ToString("N2") + "K";
            }
            else if (_checker < 11) {
                _resultTxt = (currency / 1000000f).ToString("N2") + "M";
            }
            else if (_checker < 15) {
                _resultTxt = (currency / 1000000000f).ToString("N2") + "B";
            }
            else if (_checker < 19) {
                _resultTxt = (currency / 1000000000000f).ToString("N2") + "T";
            }

            
            return _resultTxt;
        }
    }
}
