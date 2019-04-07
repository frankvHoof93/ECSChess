using ECSChess.Components.Input;
using Unity.Entities;

namespace ECSChess.Systems.Display
{
    public class ShowAvailableMovesSystem : ComponentSystem
    {
        private EntityQuery Selected;

        protected override void OnCreateManager()
        {
            Selected = GetEntityQuery(typeof(Selected));
            base.OnCreateManager();
        }

        protected override void OnUpdate()
        {

        }
    }
}