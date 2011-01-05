using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace ShootNBTEdit
{
    [DebuggerDisplay("({X},{Z}) - {ChunkPath}")]
    public struct ChunkPosition
    {
        public ChunkPosition(int x, int z)
        {
            this.X = x;
            this.Z = z;
        }

        public readonly int X;
        public readonly int Z;

        public string ChunkPath
        {
            get
            {
                var xPath = GetPositionFolderName(this.X);
                var zPath = GetPositionFolderName(this.Z);
                return Path.Combine(Path.Combine(xPath, zPath), string.Format("c.{0}.{1}.dat", ToBase36(this.X), ToBase36(this.Z)));
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ChunkPosition))
                return false;

            ChunkPosition comparison = (ChunkPosition)obj;

            return comparison.X.Equals(X) && comparison.Z.Equals(Z);
        }

        public override int GetHashCode()
        {
            return this.X.GetHashCode() ^ this.Z.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("{0},{1}", X, Z);
        }

        private static string GetPositionFolderName(int i)
        {
            int num = i;
            while (num < 0)
                num += 64;

            return ToBase36(num % 64);
        }

        private static long FromBase36(string base36String)
        {
            string chars = "0123456789abcdefghijklmnopqrstuvwxyz";

            bool negative = base36String.StartsWith("-");

            string inputChars = base36String.ToLowerInvariant();

            long output = 0;
            int position = 0;
            for (int i = inputChars.Length - 1; i >= (negative ? 1 : 0); i--)
            {
                char c = inputChars[i];
                output += chars.IndexOf(c) * (long)Math.Pow(36, position);
                position++;
            }

            return (negative ? output * -1 : output);
        }

        private static string ToBase36(long i)
        {
            if (0 <= i && i < 10)
                return i.ToString();

            List<char> result = new List<char>();

            var remainder = Math.Abs(i);

            while (remainder != 0)
            {
                var digit = remainder % 36;
                remainder /= 36;

                if (digit < 10)
                    result.Add(digit.ToString()[0]);
                else
                    result.Add((char)(digit - 10 + 'A'));
            }

            if (i < 0)
                result.Add('-');

            result.Reverse();

            return new string(result.ToArray());
        }
    }
}
