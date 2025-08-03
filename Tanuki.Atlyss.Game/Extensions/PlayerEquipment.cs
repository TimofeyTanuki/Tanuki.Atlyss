namespace Tanuki.Atlyss.Game.Extensions;

public static class PlayerEquipment
{
    public static EquipData UsableWeapon(this global::PlayerEquipment PlayerEquipment) =>
        PlayerEquipment._equips[Player._mainPlayer._pCombat._isUsingAltWeapon ? "Alt Weapon" : "Weapon"];
}