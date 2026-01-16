using UnityEngine;

namespace Tanuki.Atlyss.Game.Extensions;

public static class PlayerMove
{
    extension(global::PlayerMove instance)
    {
        public void Teleport(Vector3 position, Quaternion rotation)
        {
            instance._playerController.enabled = false;
            instance.transform.SetPositionAndRotation(position, rotation);
            instance._playerController.enabled = true;
        }

        public void Teleport(Vector3 Position) =>
            instance.Teleport(Position, instance.transform.rotation);
    }
}
