using System.Collections.Generic;
using RAGE;
using RAGE.Elements;

namespace RPServerClient.Character
{
    internal class AliasManager : Events.Script
    {

        public AliasManager()
        {
            Events.OnEntityStreamIn += OnPlayerStreamIn;
            Events.OnEntityStreamOut += OnPlayerStreamOut;
            Events.Tick += Tick;
        }

        private void OnPlayerStreamIn(Entity entity)
        {
            if (entity.Type != Type.Player) return;
            var p = (Player) entity;

            
        }

        private void OnPlayerStreamOut(Entity entity)
        {
            if (entity.Type != Type.Player) return;
            var p = (Player)entity;

        }


        private void Tick(List<Events.TickNametagData> nametags)
        {
            throw new System.NotImplementedException();
        }
    }
}
