using Tanuki.Atlyss.API.Core.Commands;

namespace Tanuki.Atlyss.Core.Data.Commands.Callers;

public class Player(global::Player player) : ICaller
{
    public global::Player player = player;
}
