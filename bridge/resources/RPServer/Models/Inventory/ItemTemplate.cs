using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using GTANetworkAPI;
using RPServer.Controllers;
using RPServer.InternalAPI.Extensions;
using RPServer.Util;

namespace RPServer.Models.Inventory
{
    /// <summary>
    /// Basic Template to characterize the various items
    /// </summary>
    internal class ItemTemplate
    {
        private static HashSet<ItemTemplate> _itemTemplates;
        private static int _lastUsedID;

        public int ID { get; }
        public string ItemName { get; set; }
        public string ItemDesc { get; set; }


        public ItemTemplate(string itemName, string itemDesc)
        {
            ID = _lastUsedID++;
            ItemName = itemName;
            ItemDesc = itemDesc;
        }

        #region ItemAction
        private Action<object[]> _itemAction;
        public bool IsUsable() => _itemAction != null;
        public void InvokeAction(object[] args)
        {
            if(IsUsable()) _itemAction.Invoke(args);
        }

        #endregion

        public static void LoadItemTemplates()
        {
            if (_itemTemplates != null)
            {
                Logger.GetInstance().ServerError("Item Tempaltes have been already loaded.");
                return;
            }

            Logger.GetInstance().ServerInfo("Loading Item Templates...");

            _itemTemplates = new HashSet<ItemTemplate>
            {
                new ItemTemplate("Money", "The standard currency."),
                new ItemTemplate("Dice", "Just an ordinary Dice")
                {
                    _itemAction = args => // (client)
                    {
                        if (args == null || args.Length < 1) return;
                        var client = args[0] as Client;

                        if(client == null) return;
                        if (!client.IsLoggedIn() || !client.HasActiveChar()) return;
                        // TODO: Check whether the active character's inventory actually has the item they are trying to use.

                        ChatHandler.SendActionMessage(client, Shared.Data.Chat.NormalChatMaxDistance, $"rolls a dice landing on {RandomGenerator.GetInstance().Next(0, 7)}");
                    }
                }
            };

            Logger.GetInstance().ServerInfo($"\tLoaded {_itemTemplates.Count} item templates.");

        }


        #region HashcodeEquals
        protected bool Equals(ItemTemplate other)
        {
            return ID == other.ID;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ItemTemplate) obj);
        }
        public override int GetHashCode()
        {
            return ID;
        }
        public static bool operator ==(ItemTemplate left, ItemTemplate right)
        {
            return Equals(left, right);
        }
        public static bool operator !=(ItemTemplate left, ItemTemplate right)
        {
            return !Equals(left, right);
        }
        #endregion
    }
}
