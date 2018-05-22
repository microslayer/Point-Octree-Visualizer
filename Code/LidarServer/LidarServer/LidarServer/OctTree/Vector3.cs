using System;

public class Vector3
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; } 

    public static Vector3 Zero = new Vector3(0, 0, 0); 

    public Vector3(double x, double y, double z)
    {
        this.X = x;
        this.Y = y;
        this.Z = z; 
    }

    public static Vector3 operator +(Vector3 v1, Vector3 v2)
    {
        return new Vector3(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
    }

    public static Vector3 operator -(Vector3 v1, Vector3 v2)
    {
        // returns v1 - v2 
        return new Vector3(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
    }

    public static Vector3 operator /(Vector3 v, float d)
    {
        // returns v1 / v2 
        return new Vector3(v.X / d, v.Y / d, v.Z / d);
    }
}
