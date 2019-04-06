using System;
using Unity.Entities;

namespace ECSChess.Components.Input
{
    /// <summary>
    /// Component used as a tag to describe an Entity as Hovered over by the Mouse
    /// </summary>
    [Serializable]
    public struct Hovered : IComponentData
    {}
}