using UnityEngine;

namespace Tanuki.Atlyss.Game.Extensions;

public static class PlayerMove
{
    public static void Teleport(this global::PlayerMove PlayerMove, Vector3 Position, Quaternion Rotation)
    {
        PlayerMove._playerController.enabled = false;
        PlayerMove.transform.SetPositionAndRotation(Position, Rotation);
        PlayerMove._playerController.enabled = true;
    }
    public static void Teleport(this global::PlayerMove PlayerMove, Vector3 Position) =>
        Teleport(PlayerMove, Position, PlayerMove.transform.rotation);
}