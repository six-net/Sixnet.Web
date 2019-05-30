using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Framework.ValueType
{
    /// <summary>
    /// Money
    /// </summary>
    [Serializable]
    public struct Money
    {
        #region constructor

        /// <summary>
        /// instance a money object
        /// </summary>
        /// <param name="amount">amount</param>
        public Money(decimal amount)
        {
            Amount = amount;
            Currency = defaultCurrency;
        }

        /// <summary>
        /// instance a money object
        /// </summary>
        /// <param name="amount">amount</param>
        /// <param name="currency">currency</param>
        public Money(decimal amount, Currency currency)
        {
            Amount = amount;
            Currency = currency;
        }

        #endregion
        #region fields

        /// <summary>
        /// default currency
        /// </summary>
        private static Currency defaultCurrency = Currency.CNY;

        #endregion

        #region Propertys

        /// <summary>
        /// get or set amount
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// get or set currency
        /// </summary>
        public Currency Currency { get; }

        /// <summary>
        /// get or set currencysign
        /// </summary>
        public string CurrencySign
        {
            get
            {
                return GetCurrencySign();
            }
        }

        #endregion

        #region static methods

        /// <summary>
        /// compare two MOney objects whether equal
        /// </summary>
        /// <param name="moneyOne">first money</param>
        /// <param name="moneyTwo">second money</param>
        /// <returns>whether equal</returns>
        public static bool Equals(Money moneyOne, Money moneyTwo)
        {
            return moneyOne.Currency == moneyTwo.Currency && moneyOne.Amount == moneyTwo.Amount;
        }

        /// <summary>
        /// verify whether can do calculate between two Money objects
        /// </summary>
        /// <param name="moneyOne">first money</param>
        /// <param name="moneyTwo">second money</param>
        private static void CalculateVerify(Money moneyOne, Money moneyTwo)
        {
            if (moneyOne.Currency != moneyTwo.Currency)
            {
                throw new Exception("both Money data don't hava the same Currency");
            }
        }

        /// <summary>
        /// set default currency
        /// </summary>
        /// <param name="defaultCurrency">default currency</param>
        public static void SetDefaultCurrency(Currency defaultCurrency)
        {
            Money.defaultCurrency = defaultCurrency;
        }

        #endregion

        #region methods

        /// <summary>
        /// get currency sign
        /// </summary>
        /// <returns></returns>
        string GetCurrencySign()
        {
            return string.Empty;
        }

        /// <summary>
        /// compare two Money objects whether equal
        /// </summary>
        /// <param name="obj">other Money object</param>
        /// <returns>whether equal</returns>
        public override bool Equals(object obj)
        {
            return Equals(this, (Money)obj);
        }
        public override int GetHashCode()
        {
            return string.Format("{0}_{1}", Currency, Amount).GetHashCode();
        }

        /// <summary>
        /// do add operation between two Money objects,must be the same currency can do this operation
        /// </summary>
        /// <param name="one">first money</param>
        /// <param name="two">second money</param>
        /// <returns>calculate result</returns>
        public static Money operator +(Money one, Money two)
        {
            CalculateVerify(one, two);
            decimal newAmount = one.Amount + two.Amount;
            return new Money(newAmount, one.Currency);
        }

        /// <summary>
        /// do subtraction operation between two Money objects,must be the same currency can do this operation
        /// </summary>
        /// <param name="one">first money</param>
        /// <param name="two">second money</param>
        /// <returns>calculate result</returns>
        public static Money operator -(Money one, Money two)
        {
            CalculateVerify(one, two);
            decimal newAmount = one.Amount - two.Amount;
            return new Money(newAmount, one.Currency);
        }

        /// <summary>
        /// do multiplication operation between two Money objects,must be the same currency can do this operation
        /// </summary>
        /// <param name="one">first money</param>
        /// <param name="two">second money</param>
        /// <returns>calculate result</returns>
        public static Money operator *(Money one, Money two)
        {
            CalculateVerify(one, two);
            decimal newAmount = one.Amount * two.Amount;
            return new Money(newAmount, one.Currency);
        }

        /// <summary>
        /// do division operation between two Money objects,must be the same currency can do this operation
        /// </summary>
        /// <param name="one">first money</param>
        /// <param name="two">second money</param>
        /// <returns>calculate result</returns>
        public static Money operator /(Money one, Money two)
        {
            CalculateVerify(one, two);
            decimal newAmount = one.Amount / two.Amount;
            return new Money(newAmount, one.Currency);
        }

        /// <summary>
        /// compare two Money objects whether equal
        /// </summary>
        /// <param name="one">first money</param>
        /// <param name="two">second money</param>
        /// <returns>whether equal</returns>
        public static bool operator ==(Money one, Money two)
        {
            return Equals(one, two);
        }

        /// <summary>
        /// compare two Money objects whether not equal
        /// </summary>
        /// <param name="one">first money</param>
        /// <param name="two">second money</param>
        /// <returns>whether not equal</returns>
        public static bool operator !=(Money one, Money two)
        {
            return !Equals(one, two);
        }

        /// <summary>
        /// determine whether the first money object less than second
        /// </summary>
        /// <param name="one">first money</param>
        /// <param name="two">second money</param>
        /// <returns>determined result</returns>
        public static bool operator <(Money one, Money two)
        {
            CalculateVerify(one, two);
            return one.Amount < two.Amount;
        }

        /// <summary>
        /// determine whether the first money object greater than second
        /// </summary>
        /// <param name="one">first money</param>
        /// <param name="two">second money</param>
        /// <returns>determined result</returns>
        public static bool operator >(Money one, Money two)
        {
            CalculateVerify(one, two);
            return one.Amount > two.Amount;
        }

        /// <summary>
        /// determine whether the first money object less than or equal to second
        /// </summary>
        /// <param name="one">first money</param>
        /// <param name="two">second money</param>
        /// <returns>determined result</returns>
        public static bool operator <=(Money one, Money two)
        {
            CalculateVerify(one, two);
            return one.Amount <= two.Amount;
        }

        /// <summary>
        /// determine whether the first money object greater than or equal to second
        /// </summary>
        /// <param name="one">first money</param>
        /// <param name="two">second money</param>
        /// <returns>determined result</returns>
        public static bool operator >=(Money one, Money two)
        {
            CalculateVerify(one, two);
            return one.Amount >= two.Amount;
        }

        /// <summary>
        /// add amount,minus amount if amount value is a negative number
        /// </summary>
        /// <param name="amount">amount value</param>
        /// <returns>calculated money</returns>
        public Money AddAmount(decimal amount)
        {
            Amount += amount;
            return this;
        }

        /// <summary>
        /// minus amount，add amount if amount value is a negative number
        /// </summary>
        /// <param name="amount">amount</param>
        /// <returns>calculated money</returns>
        public Money SubtractAmount(decimal amount)
        {
            Amount -= amount;
            return this;
        }

        #endregion
    }

    /// <summary>
    /// Currency
    /// </summary>
    [Serializable]
    public enum Currency
    {
        CNY = 110,
        USD = 120,
    }
}
