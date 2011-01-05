using System;
using System.Diagnostics;

namespace ShootNBTEdit
{
    [DebuggerDisplay("({X}, {Y}, {Z})")]
    public struct Position
    {
        private readonly bool notEmpty;

        public Position(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.notEmpty = true;
        }

        public readonly double X;
        public readonly double Y;
        public readonly double Z;

        public bool IsEmpty { get { return !notEmpty; } }

        public override bool Equals(object obj)
        {
            if (!(obj is Position))
                return false;

            Position comparison = (Position)obj;

            return (this.IsEmpty && comparison.IsEmpty) || (comparison.X.Equals(X) && comparison.Y.Equals(Y) && comparison.Z.Equals(Z));
        }

        public override int GetHashCode()
        {
            return this.X.GetHashCode() ^ this.Y.GetHashCode() ^ this.Z.GetHashCode();
        }

        public ChunkPosition ToChunkCoordinates()
        {
            int x = (int)Math.Floor(this.X);
            x = this.X > 0 ? (x / 16) : ((x - 15) / 16);
            int z = (int)Math.Floor(this.Z);
            z = this.Z > 0 ? (z / 16) : ((z - 15) / 16);
            return new ChunkPosition(x, z);
        }
    }
}
