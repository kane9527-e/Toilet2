// ReSharper disable once CheckNamespace

using System;
using System.Collections;
using System.Globalization;

// ReSharper disable once CheckNamespace
namespace TextVariable.Variables
{
    public class TimeTextVariable : TextVariable
    {
        private readonly string[] variableText =
        {
            "NOW",
            "UTCNOW",
            "YEAR",
            "MONTH",
            "DAY",
            "HOUR",
            "MINUTE",
            "SECOND"
        };

        internal override bool Detect(string originText)
        {
            return ((IList)variableText).Contains(originText.ToUpper());
        }

        internal override string Process(string originText)
        {
            switch (originText.ToUpper())
            {
                case "NOW":
                    originText = DateTime.Now.ToString(CultureInfo.InvariantCulture);
                    break;
                case "UTCNOW":
                    originText = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
                    break;
                case "YEAR":
                    originText = DateTime.Now.Year.ToString(CultureInfo.InvariantCulture);
                    break;
                case "MONTH":
                    originText = DateTime.Now.Month.ToString(CultureInfo.InvariantCulture);
                    break;
                case "DAY":
                    originText = DateTime.Now.Day.ToString(CultureInfo.InvariantCulture);
                    break;
                case "HOUR":
                    originText = DateTime.Now.Hour.ToString(CultureInfo.InvariantCulture);
                    break;
                case "MINUTE":
                    originText = DateTime.Now.Minute.ToString(CultureInfo.InvariantCulture);
                    break;
                case "SECOND":
                    originText = DateTime.Now.Second.ToString(CultureInfo.InvariantCulture);
                    break;
            }

            return originText;
        }
    }
}