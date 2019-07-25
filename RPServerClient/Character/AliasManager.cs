using System.Collections.Generic;
using System.Security;
using RAGE;
using RAGE.Elements;
using Shared.Enums;
using Events = RAGE.Events;

namespace RPServerClient.Character
{
    internal class Alias
    {
        public Player player { set; get; }
        public string aliasText { set; get; }

        public Alias(Player player, string aliasText)
        {
            this.player = player;
            this.aliasText = aliasText;
        }
    }

    internal class AliasManager : Events.Script
    {
        private readonly List<Alias> _clientAlises;

        public AliasManager()
        {
            _clientAlises = new List<Alias>();
            
            Events.OnEntityStreamIn += OnPlayerStreamIn;
            Events.OnEntityStreamOut += OnPlayerStreamOut;
            Events.Tick += Tick;

            Events.Add(Shared.Events.ServerToClient.Character.SetAliasInfo, SetAliasInfo);
        }

        private void OnPlayerStreamIn(Entity entity)
        {
            if (entity.Type != Type.Player) return;
            var p = (Player) entity;

            if(!Client.Globals.IsAccountLoggedIn || !Client.Globals.HasActiveChar) return;

            Events.CallRemote(Shared.Events.ClientToServer.Character.RequestAliasInfo, p.RemoteId);
        }

        private void SetAliasInfo(object[] args)
        {
            if(args == null || args.Length < 2) return;

            var aliasTxt = args[0] as string;
            RAGE.Chat.Output(aliasTxt);
            var remoteID = int.Parse(args[1].ToString());
            RAGE.Chat.Output(remoteID.ToString());
            //var other = Entities.Players.Streamed.Find(p => p.RemoteId == remoteID);
            var other = Entities.Players.All.Find(p => p.RemoteId == remoteID);
            if (other == null) return;

            var al = _clientAlises.Find(p => p.player == other);

            if (al == null)
            {
                _clientAlises.Add(new Alias(other, aliasTxt));
            }
            else
            {
                al.aliasText = aliasTxt;
            }
        }

        private void OnPlayerStreamOut(Entity entity)
        {
            if (entity.Type != Type.Player) return;
            var p = (Player)entity;

            if (!Client.Globals.IsAccountLoggedIn || !Client.Globals.HasActiveChar) return;


        }


        private void Tick(List<Events.TickNametagData> nametags)
        {
            foreach (var alias in _clientAlises)
            {
                RAGE.Game.Graphics.SetDrawOrigin(alias.player.Position.X, alias.player.Position.Y, alias.player.Position.Z + 1f, 0);
                RAGE.NUI.UIResText.Draw(alias.aliasText, 0, 0, RAGE.Game.Font.ChaletLondon, 0.3f, System.Drawing.Color.White, RAGE.NUI.UIResText.Alignment.Centered, false, false, 0);
                RAGE.Game.Graphics.ClearDrawOrigin();
            }

        }

    }
}
