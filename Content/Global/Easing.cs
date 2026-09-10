using System;


namespace asuw.Content
{
    //this is vibe coded lol
    public static partial class AsuUtils
    {
        // Constants used by Back and Elastic families
        private const float c1 = 1.70158f;
        private const float c2 = c1 * 1.525f;
        private const float c3 = c1 + 1f;
        private const float c4 = (2f * MathF.PI) / 3f;
        private const float c5 = (2f * MathF.PI) / 4.5f;

        // ── Linear ──────────────────────────────────────────────────────────────
        public static float Linear(float t) => t;

        // ── Sine ────────────────────────────────────────────────────────────────
        public static float SineIn(float t) => 1f - MathF.Cos(t * MathF.PI / 2f);
        public static float SineOut(float t) => MathF.Sin(t * MathF.PI / 2f);
        public static float SineInOut(float t) => -(MathF.Cos(MathF.PI * t) - 1f) / 2f;

        // ── Quad ────────────────────────────────────────────────────────────────
        public static float QuadIn(float t) => t * t;
        public static float QuadOut(float t) => 1f - (1f - t) * (1f - t);
        public static float QuadInOut(float t) => t < 0.5f
            ? 2f * t * t
            : 1f - MathF.Pow(-2f * t + 2f, 2f) / 2f;

        // ── Cubic ───────────────────────────────────────────────────────────────
        public static float CubicIn(float t) => t * t * t;
        public static float CubicOut(float t) => 1f - MathF.Pow(1f - t, 3f);
        public static float CubicInOut(float t) => t < 0.5f
            ? 4f * t * t * t
            : 1f - MathF.Pow(-2f * t + 2f, 3f) / 2f;

        // ── Quart ───────────────────────────────────────────────────────────────
        public static float QuartIn(float t) => t * t * t * t;
        public static float QuartOut(float t) => 1f - MathF.Pow(1f - t, 4f);
        public static float QuartInOut(float t) => t < 0.5f
            ? 8f * t * t * t * t
            : 1f - MathF.Pow(-2f * t + 2f, 4f) / 2f;

        // ── Quint ───────────────────────────────────────────────────────────────
        public static float QuintIn(float t) => t * t * t * t * t;
        public static float QuintOut(float t) => 1f - MathF.Pow(1f - t, 5f);
        public static float QuintInOut(float t) => t < 0.5f
            ? 16f * t * t * t * t * t
            : 1f - MathF.Pow(-2f * t + 2f, 5f) / 2f;

        // ── Expo ────────────────────────────────────────────────────────────────
        public static float ExpoIn(float t) => t == 0f ? 0f : MathF.Pow(2f, 10f * t - 10f);
        public static float ExpoOut(float t) => t == 1f ? 1f : 1f - MathF.Pow(2f, -10f * t);
        public static float ExpoInOut(float t) => t == 0f ? 0f : t == 1f ? 1f : t < 0.5f
            ? MathF.Pow(2f, 20f * t - 10f) / 2f
            : (2f - MathF.Pow(2f, -20f * t + 10f)) / 2f;

        // ── Parabola ────────────────────────────────────────────────────────────
        public static float Parabola(float t, float height) => 4 * height * t * (1 - t);
      
        // ── Circ ────────────────────────────────────────────────────────────────
        public static float CircIn(float t) => 1f - MathF.Sqrt(1f - t * t);
        public static float CircOut(float t) => MathF.Sqrt(1f - (t - 1f) * (t - 1f));
        public static float CircInOut(float t) => t < 0.5f
            ? (1f - MathF.Sqrt(1f - 4f * t * t)) / 2f
            : (MathF.Sqrt(1f - (-2f * t + 2f) * (-2f * t + 2f)) + 1f) / 2f;

        // ── Back (overshoots slightly) ───────────────────────────────────────────
        public static float BackIn(float t) => c3 * t * t * t - c1 * t * t;
        public static float BackOut(float t) => 1f + c3 * MathF.Pow(t - 1f, 3f) + c1 * MathF.Pow(t - 1f, 2f);
        public static float BackInOut(float t) => t < 0.5f
            ? MathF.Pow(2f * t, 2f) * ((c2 + 1f) * 2f * t - c2) / 2f
            : (MathF.Pow(2f * t - 2f, 2f) * ((c2 + 1f) * (2f * t - 2f) + c2) + 2f) / 2f;

        // ── Elastic (spring-like) ────────────────────────────────────────────────
        public static float ElasticIn(float t) => t == 0f ? 0f : t == 1f ? 1f
            : -MathF.Pow(2f, 10f * t - 10f) * MathF.Sin((t * 10f - 10.75f) * c4);
        public static float ElasticOut(float t) => t == 0f ? 0f : t == 1f ? 1f
            : MathF.Pow(2f, -10f * t) * MathF.Sin((t * 10f - 0.75f) * c4) + 1f;
        public static float ElasticInOut(float t) => t == 0f ? 0f : t == 1f ? 1f : t < 0.5f
            ? -(MathF.Pow(2f, 20f * t - 10f) * MathF.Sin((20f * t - 11.125f) * c5)) / 2f
            : (MathF.Pow(2f, -20f * t + 10f) * MathF.Sin((20f * t - 11.125f) * c5)) / 2f + 1f;

        // ── Bounce ──────────────────────────────────────────────────────────────
        public static float BounceIn(float t) => 1f - BounceOut(1f - t);
        public static float BounceInOut(float t) => t < 0.5f
            ? (1f - BounceOut(1f - 2f * t)) / 2f
            : (1f + BounceOut(2f * t - 1f)) / 2f;
        public static float BounceOut(float t)
        {
            const float n1 = 7.5625f, d1 = 2.75f;
            if (t < 1f / d1) return n1 * t * t;
            if (t < 2f / d1) return n1 * (t -= 1.5f / d1) * t + 0.75f;
            if (t < 2.5f / d1) return n1 * (t -= 2.25f / d1) * t + 0.9375f;
            return n1 * (t -= 2.625f / d1) * t + 0.984375f;
        }

        public static float OutIn(Func<float, float> easing, float t)
        {
            if (t < 0.5f) return easing(t * 2f) / 2f;
            return 1f - easing((1f - t) * 2f) / 2f;
        }

        // usage
       /* float v = OutIn(Easing.QuadOut, t);*/  // or CubicOut, ExpoOut, etc.
    }
}
