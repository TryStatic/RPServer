using System.Collections.Generic;
using RAGE.Elements;
using RPServerClient.Character.Util;
using Events = RAGE.Events;

namespace RPServerClient.Character
{
    internal class AliasManager : Events.Script
    {
        public static readonly List<Alias> ClientAlises = new List<Alias>();

        public AliasManager()
        {
            Events.OnEntityStreamIn += OnPlayerStreamIn;
            Events.OnEntityStreamOut += OnPlayerStreamOut;
            Events.Tick += Tick;

            Events.Add(Shared.Events.ServerToClient.Character.SetAliasInfo, OnSetAliasInfo);
        }

        private void OnPlayerStreamIn(Entity entity)
        {
            if (entity.Type != Type.Player) return;
            var p = (Player) entity;

            if(!Client.Globals.IsAccountLoggedIn || !Client.Globals.HasActiveChar) return;

            Events.CallRemote(Shared.Events.ClientToServer.Character.RequestAliasInfo, p.RemoteId);
        }

        private void OnPlayerStreamOut(Entity entity)
        {
            if (entity.Type != Type.Player) return;
            var p = (Player)entity;

            if (!Client.Globals.IsAccountLoggedIn || !Client.Globals.HasActiveChar) return;

            ClientAlises.RemoveAll(al => al.Player == p);
        }

        private void OnSetAliasInfo(object[] args)
        {
            if (args == null || args.Length < 2) return;

            var aliasTxt = args[0] as string;
            var remoteID = int.Parse(args[1].ToString());
            //var other = Entities.Players.Streamed.Find(p => p.RemoteId == remoteID);
            var other = Entities.Players.All.Find(p => p.RemoteId == remoteID);
            if (other == null) return;

            var al = ClientAlises.Find(p => p.Player == other);

            if (al == null) ClientAlises.Add(new Alias(other, aliasTxt));
            else al.AliasText = aliasTxt;
        }

        private void Tick(List<Events.TickNametagData> nametags)
        {
            foreach (var alias in ClientAlises)
            {
                RAGE.Game.Graphics.SetDrawOrigin(alias.Player.Position.X, alias.Player.Position.Y, alias.Player.Position.Z + 1f, 0);
                RAGE.NUI.UIResText.Draw(alias.AliasText, 0, 0, RAGE.Game.Font.ChaletLondon, 0.3f, System.Drawing.Color.White, RAGE.NUI.UIResText.Alignment.Centered, false, false, 0);
                RAGE.Game.Graphics.ClearDrawOrigin();
            }
        }
    }
}