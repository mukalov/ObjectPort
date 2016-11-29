namespace ObjectPort.Benchmarks
{
    using System;
    using System.Text;

    public class StringGenerator
    {
        private char[] _characterArray;
        private Random _rnd;

        public StringGenerator()
        {
            _characterArray = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();
            _rnd = new Random();
        }

        private char GetRandomCharacter()
        {
            return this._characterArray[(int)((this._characterArray.GetUpperBound(0) + 1) * _rnd.NextDouble())];
        }

        public string Generate(int lengthMin, int lengthMax)
        {
            var sb = new StringBuilder();
            var length = _rnd.Next(lengthMin, lengthMax);
            sb.Capacity = length;
            for (int count = 0; count <= length - 1; count++)
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
