using System;

namespace RayTracer {

	public struct Vector3 {
		public double x;
		public double y;
		public double z;

		public double Dot() {
			return ( x * x ) + ( y * y ) + ( z * z );
		}

		public double Dot( Vector3 other ) {
			return x * other.x + y * other.y + z * other.z;
		}

		public void Normalize() {
			double l = Math.Sqrt( Dot() );
			x = x / l;
			y = y / l;
			z = z / l;
		}

		public Vector3( double X, double Y, double Z ) {
			x = X;
			y = Y;
			z = Z;
		}

		public double DistanceTo( Vector3 other ) {
			double x2 = other.x - x;
			double y2 = other.y - y;
			double z2 = other.z - z;
			return Math.Sqrt( (double)( ( x2 * x2 ) + ( y2 * y2 ) + ( z2 * z2 ) ) );
		}

		public static Vector3 operator +( Vector3 left, Vector3 right ) {
			Vector3 sum = new Vector3();
			sum.x = left.x + right.x;
			sum.y = left.y + right.y;
			sum.z = left.z + right.z;
			return sum;
		}

		public static Vector3 operator -( Vector3 left, Vector3 right ) {
			Vector3 difference = new Vector3();
			difference.x = left.x - right.x;
			difference.y = left.y - right.y;
			difference.z = left.z - right.z;
			return difference;
		}

		public static Vector3 operator *( Vector3 left, double right ) {
			Vector3 difference = new Vector3();
			difference.x = left.x * right;
			difference.y = left.y * right;
			difference.z = left.z * right;
			return difference;
		}

		public static Vector3 operator -( Vector3 v ) {
			Vector3 negative = new Vector3();
			negative.x = 1 - v.x;
			negative.y = 1 - v.y;
			negative.z = 1 - v.z;
			return negative;
		}

		public static double operator *( Vector3 left, Vector3 right ) {
			return left.Dot( right );
		}
	}
}