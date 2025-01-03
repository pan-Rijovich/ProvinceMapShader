namespace UnityEngine
{
    public static class ColorExtension
    {
        public static Color SetRBG(this Color color, float r = -1f, float b = -1f, float g = -1f, float a = -1f)
        {
            color.r = r == -1f ? color.r : Mathf.Clamp01(r);
            color.g = g == -1f ? color.g : Mathf.Clamp01(g);
            color.b = b == -1f ? color.b : Mathf.Clamp01(b);
            color.a = a == -1f ? color.a : Mathf.Clamp01(a);

            return color;
        }

        public static Color SetHSV(this Color color, float h = 1f, float s = 1f, float v = 1f, float a = 1f)
        {
            h = Mathf.Clamp01(h);
            s = Mathf.Clamp01(s); 
            v = Mathf.Clamp01(v); 

            float c = v * s;
            float x = c * (1 - Mathf.Abs((h * 6) % 2 - 1));
            float m = v - c;

            float r = 0, g = 0, b = 0;

            if (h >= 0 && h < 1f / 6f)
            {
                r = c; g = x; b = 0;
            }
            else if (h >= 1f / 6f && h < 2f / 6f)
            {
                r = x; g = c; b = 0;
            }
            else if (h >= 2f / 6f && h < 3f / 6f)
            {
                r = 0; g = c; b = x;
            }
            else if (h >= 3f / 6f && h < 4f / 6f)
            {
                r = 0; g = x; b = c;
            }
            else if (h >= 4f / 6f && h < 5f / 6f)
            {
                r = x; g = 0; b = c;
            }
            else if (h >= 5f / 6f && h <= 1)
            {
                r = c; g = 0; b = x;
            }

            color = new Color(r + m, g + m, b + m);
            return color;
        }

        public static string ToRGBAString(this Color color)
        {
            var result = "#";
            int number = (int)(color.r * 255);
            result += number.ToString("x2");

            number = (int)(color.g * 255);
            result += number.ToString("x2");

            number = (int)(color.b * 255);
            result += number.ToString("x2");

            number = (int)(color.a * 255);
            result += number.ToString("x2");

            return result;
        }

        public static string ToRGBString(this Color color)
        {
            var result = "#";
            int number = (int)(color.r * 255);
            result += number.ToString("x2");

            number = (int)(color.g * 255);
            result += number.ToString("x2");

            number = (int)(color.b * 255);
            result += number.ToString("x2");

            return result;
        }

        public static Color Parse(string colorString)
        {
            if (colorString.StartsWith("#"))
            {
                colorString = colorString.Substring(1);
            }

            if (colorString.Length != 6 && colorString.Length != 8)
            {
                Debug.LogError("Incorrect color format! Use the format #RRGGBB or #RRGGBBAA.");
                return Color.white;
            }

            float r = int.Parse(colorString.Substring(0, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
            float g = int.Parse(colorString.Substring(2, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
            float b = int.Parse(colorString.Substring(4, 2), System.Globalization.NumberStyles.HexNumber) / 255f;

            float a = 1f;
            if (colorString.Length == 8)
            {
                a = int.Parse(colorString.Substring(6, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
            }

            return new Color(r, g, b, a);
        }
    }
}
