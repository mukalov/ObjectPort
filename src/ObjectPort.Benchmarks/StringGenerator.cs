namespace ObjectPort.Benchmarks
{
    using System;
    using System.Text;

    public class StringGenerator
    {
        private char[] characterArray;
        Random randNum = new Random();

        public StringGenerator()
        {
            characterArray = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();
        }

        private char GetRandomCharacter()
        {
            return this.characterArray[(int)((this.characterArray.GetUpperBound(0) + 1) * randNum.NextDouble())];
        }

        public string Generate(int passwordLength)
        {
            StringBuilder sb = new StringBuilder();
            sb.Capacity = passwordLength;
            for (int count = 0; count <= passwordLength - 1; count++)
            {
                sb.Append(GetRandomCharacter());
            }
            if ((sb != null))
            {
                return sb.ToString();
            }
            return string.Empty;
        }

    }
}
