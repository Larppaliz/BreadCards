using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LarrysCards.Patches
{
    [HarmonyPatch(typeof(ProjectileCollision))]
    class LarrysCards_ProjectileCollisionPatch
    {
        [HarmonyPatch("HitSurface")]
        [HarmonyPrefix]
        public static bool hitSurface(ProjectileCollision __instance, ref ProjectileHitSurface.HasToStop __result, GameObject projectile, HitInfo hit)
        {
            if (projectile.GetComponent<ProjectileHit>().ownPlayer == __instance.GetComponentInParent<ProjectileHit>().ownPlayer)
            {
                return false;
            }

            return true;
        }
    }
}
