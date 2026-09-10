using asuw.Content.Items.Accesories;
using Terraria.ModLoader;

namespace asuw.Content
{
    public partial class AsuPlayer : ModPlayer
    {
        public float CorruptionEyeStoredY = 0;
        public override void PreUpdateMovement()
        {
            if (CorruptionEyeZero)
            {
                if (Player.velocity.Y > 0.2f || Player.velocity.Y < -0.2f)
                {
                    CorruptionEyeStoredY = Player.velocity.Y;
                    Player.velocity.Y *= 1 + CorruptionEye.VerticalSpeedBoost;
                }
                else
                {
                    CorruptionEyeStoredY = 0;
                }
            }
        }
    }
}