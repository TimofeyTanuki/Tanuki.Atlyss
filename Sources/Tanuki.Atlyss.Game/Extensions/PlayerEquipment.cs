namespace Tanuki.Atlyss.Game.Extensions;

public static class PlayerEquipment
{
    extension(global::PlayerEquipment PlayerEquipment)
    {
        public EquipData UsableWeapon =>
            PlayerEquipment._equips[global::Player._mainPlayer._pCombat._isUsingAltWeapon ? "Alt Weapon" : "Weapon"];
    }
}
