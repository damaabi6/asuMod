using asuw.Content.Buffs;
using asuw.Content.Dusts;
using asuw.Content.Projectiles;
using asuw.Content.Projectiles.InfoAndChargeBar;
using Microsoft.Xna.Framework;
using System;
using System.Security.Cryptography.X509Certificates;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Map;
using Terraria.ModLoader;

namespace asuw.Content.Items.Weapons.Melee
{
    //TODO total rework, move projectiles here, combine fast att and slow att
    public class IronLotus : ModItem
	{
        public double IronLotusCharge = 0f;
        public double IronLotusChargeNum = 0;
        public float IronLotusFlameCharge = 0;
        public int IronFlameTick = 0;
        public int IronFlameLevel = 6;
        public bool IronLotusFlame = false;

        public float IronFlameDMG = 100;
        public float IronFlameDMGLevel = 2;
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }
        public override void SetDefaults()
		{
			Item.width = 46;
			Item.height = 48;
			Item.value = Item.sellPrice(gold: 2, silver: 50);
			Item.rare = ItemRarityID.Green;	
			Item.useTime = 140;
			Item.useAnimation = 140;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 10;  
			Item.autoReuse = true; 
			Item.damage = 3800; 
			Item.DamageType = DamageClass.Melee; 
			Item.noMelee = true; 
			Item.noUseGraphic = true; 
			Item.channel = true;
			Item.shoot = ModContent.ProjectileType<IronLotusFire>(); 
			Item.shootSpeed = 5;
		}

		public override bool MeleePrefix()
		{
			return true;
		}
		public override bool AltFunctionUse(Player player)
		{
			return true;
		}
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			if (player.altFunctionUse == 2 && !player.mouseInterface && !Main.mapFullscreen && !Main.blockMouse)
			{
				if (IronLotusFlame && IronLotusFlameCharge >= 130)
				{
                    Projectile.NewProjectile(source, player.Center, Vector2.Zero, ModContent.ProjectileType<IronLotusFireMass>(), damage * 5, knockback, player.whoAmI);
                }
				else
				{ 
                    Projectile.NewProjectile(source, player.Center, Vector2.Zero, ModContent.ProjectileType<IronLotusPhys2>(), damage * 3, knockback, player.whoAmI);
                }

            }
			else
			{ 
				if (!IronLotusFlame)
				{ 
                    Projectile.NewProjectile(source, player.Center, Vector2.Zero, ModContent.ProjectileType<IronLotusPhys>(), damage, knockback, player.whoAmI);
                }
				else
				{
					Projectile.NewProjectile(source, position, velocity, type, damage / 3, knockback, player.whoAmI,player.direction);
				}
            }
			return false;
		}

        public override void UpdateInventory(Player player)
        {
           

            if (IronFlameTick == 1 && IronLotusFlame)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<IronFireBoomPlayer>(), player.statLife, 0, player.whoAmI);

                foreach (var npc in Main.ActiveNPCs)
                {
                    float sqrDistanceToPlayerowner = Vector2.DistanceSquared(player.Center, npc.Center);
                    if (npc.HasBuff(ModContent.BuffType<IronFire>()))
                    {
                    Projectile.NewProjectile(player.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<IronFireBoomNPC>(), (int)(IronFlameDMG * (IronFlameDMGLevel / 2)), 10f, player.whoAmI,npc.whoAmI);
                    }

                    if (sqrDistanceToPlayerowner <= (2000 * 2000))
                    {
                        
                    }

                }
            }


            if (IronLotusCharge > 131)
            {
                IronLotusCharge = 131;
            }
            if (IronLotusCharge >= 0 && IronLotusCharge <= 130)
            {
                IronLotusFlame = false;
                IronLotusChargeNum = Math.Floor(IronLotusCharge / 10);
                IronLotusFlameCharge = 0;


            }
            else
            {
                IronLotusChargeNum = Math.Floor(IronLotusFlameCharge / 10);
                IronLotusFlame = true;
            }

            if (IronLotusChargeNum >= 13)
            { IronLotusChargeNum = 13; }
            if (IronLotusFlameCharge >= 130)
            { IronLotusFlameCharge = 130; }

            if (IronLotusFlame)
            {

                IronFlameTick++;

                if (IronFlameTick % 480 == 0)
                { IronFlameLevel--; }

                if (IronFlameLevel < 0)
                { IronFlameLevel = 0; }

                if(IronFlameTick > 480)
                { IronFlameTick = 0;}

             
                Vector2 playerrandpos = player.Center + new Vector2(Main.rand.NextFloat(-player.width / 2, player.width / 2), Main.rand.NextFloat(-player.height / 2, player.height / 2));
                Dust dust2 = Dust.NewDustPerfect(playerrandpos, ModContent.DustType<SharpSparkDust>(), new Vector2(0, -5).RotatedByRandom(MathHelper.ToRadians(35f)) * Main.rand.NextFloat(0.1f, 1.9f));
                dust2.noGravity = true;
                dust2.scale = Main.rand.NextFloat(0.7f, 1.1f);
                dust2.color = Main.rand.NextBool() ? Color.OrangeRed : Color.Firebrick;

               
            }
            else
            {
                IronFlameLevel = 6;
                IronFlameTick = 0;
                IronFlameDMG = 100;
                IronFlameDMGLevel = 2;
            }

        

        }

        public override void HoldItem(Player player)
        {
            int type = ModContent.ProjectileType<IronLotusCharge>();
            if (Main.myPlayer == player.whoAmI)
                if (player.ownedProjectileCounts[type] < 1)
                    Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.MountedCenter, (Main.MouseWorld - player.MountedCenter).normalize() * 12, type, Item.damage, 0, player.whoAmI);
        }

       
    }
}