using System;

namespace Tanuki.Atlyss.Patching;

public class Patcher
{
    public void Use(params Type[] Patches) =>
        Core.Instance.Use(this, Patches);

    public void Unuse(params Type[] Patches) =>
        Core.Instance.Unuse(this, Patches);

    public void UnuseAll() =>
        Core.Instance.UnuseAll(this);
}