using UnityEngine;

public static class DUtil {

    /// <summary>
    /// Checks if two values are close enough of each other
    /// </summary>
    /// <param name="a">Value 1</param>
    /// <param name="b">Value 2</param>
    /// <param name="threshold">The threshold of how close the values should be</param>
    /// <returns>Returns true if the floats are threshold away of each other</returns>
    public static bool Approx(float a, float b, float threshold) {
        return a - b < 0 ? b - a <= threshold : a - b <= threshold;
    }

    /// <summary>
    /// Clamps a value between -1 and 1
    /// </summary>
    /// <param name="a">Value to be clamped</param>
    /// <returns>Returns the value clamped between -1 and 1</returns>
    public static float Clamp1Neg1(float a) {
        return a < -1 ? -1 : a > 1 ? 1 : a;
    }

    /// <summary>
    /// Used to check if a value is between two constants
    /// </summary>
    /// <param name="a">The queried float</param>
    /// <param name="min">Minimum</param>
    /// <param name="max">maximum</param>
    /// <returns>Returns wether or not given value is between min and max</returns>
    public static bool Between(float a, float min, float max) {
        return a > min ? a < max : false;
    }

    public static Vector3 Vec2ToVec3XZ(Vector2 vec2) {
        return new Vector3(vec2.x, 0, vec2.y);
    }

    public static Vector3 Vec3ToVec2XZ(Vector3 vec3) {
        return new Vector2(vec3.x, vec3.z);
    }
}
