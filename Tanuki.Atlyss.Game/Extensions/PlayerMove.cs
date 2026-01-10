using UnityEngine;

namespace Tanuki.Atlyss.Game.Extensions;

public static class PlayerMove
{
    extension(global::PlayerMove PlayerMove)
    {
        public void Teleport(Vector3 Position, Quaternion Rotation)
        {
            PlayerMove._playerController.enabled = false;
            PlayerMove.transform.SetPositionAndRotation(Position, Rotation);
            PlayerMove._playerController.enabled = true;
        }

        public void Teleport(Vector3 Position) =>
            PlayerMove.Teleport(Position, PlayerMove.transform.rotation);
    }
}