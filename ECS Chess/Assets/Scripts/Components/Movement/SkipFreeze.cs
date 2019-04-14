using Unity.Entities;

namespace ECSChess.Components.Movement
{
    /// <summary>
    /// Component used as a Tag to prevent an Entity from being processed by the FreezeStaticObjectsSystem
    /// </summary>
    public struct SkipFreeze : IComponentData
    {}
}