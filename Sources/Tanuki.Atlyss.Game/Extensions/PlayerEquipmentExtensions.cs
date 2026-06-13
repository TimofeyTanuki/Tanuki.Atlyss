namespace Tanuki.Atlyss.Game.Extensions;

public static class PlayerEquipmentExtensions
{
    extension(global::PlayerEquipment instance)
    {
        public EquipData UsableWeapon =>
            instance._equips[Player._mainPlayer._pCombat._isUsingAltWeapon ? "Alt Weapon" : "Weapon"];
    }
}
