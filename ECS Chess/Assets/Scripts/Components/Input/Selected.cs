using System;
using Unity.Entities;

namespace ECSChess.Components.Input
{
    /// <summary>
    /// Component used as a tag to describe an Entity as Selected by the User
    /// </summary>
    [Serializable]
    public struct Selected : IComponentData
    { }
}