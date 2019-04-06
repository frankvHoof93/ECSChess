using System;
using Unity.Entities;

namespace ECSChess.Components.Input
{
    /// <summary>
    /// Component used as a tag to describe an Entity as Selectable
    /// </summary>
    [Serializable]
    public struct Selectable : IComponentData
    {}
}