using System;
using System.Collections.Generic;
using System.Linq;
using GTANetworkAPI;
using RPServer.Controllers;
using RPServer.InternalAPI.Extensions;
using RPServer.Util;

namespace RPServer.Models.Inventory
{
    internal enum StackType
    {
        Singleton, // Only one instance of this ItemType per inventory and not stackable
        Multiton, // Multiple instances of this ItemType per inventory and not stackable
        Stackable // Only one instance of this ItemType per inventory but stackable
    }

    /// <summary>
    /// Basic Template to characterize the various items
    /// </summary>
    internal class ItemTemplate
    {
        private static HashSet<ItemTemplate> _itemTemplates;
        private static int _lastUsedID;

        public int ID { get; }
        public string ItemName { get; }
        public string ItemDesc { get; }
        public StackType StackType { get; }

        private ItemTemplate(string itemName, string itemDesc, StackType stackType)
        {
            ID = _lastUsedID++;
            ItemName = itemName;
            ItemDesc = itemDesc;
            StackType = stackType;
        }

        /// <summary>
        /// Searches for the corresponding ItemTemplate matching the specified template ID.
        /// </summary>
        /// <param name="id">The template ID</param>
        /// <returns>Either the ItemTemplate instance or null if not found</returns>
        public static ItemTemplate GetTemplate(uint id) => _itemTemplates.FirstOrDefault(it => it.ID == id);
        public static List<ItemTemplate> GetTemplates() => _itemTemplates.ToList();

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
                new ItemTemplate("Money", "The standard currency.", StackType.Stackable),
                new ItemTemplate("Dice", "Just an ordinary Dice", StackType.Multiton)
                {
                    _itemAction = args => // (Client client)
                    {
                        if (args == null || args.Length < 1) return;
                        var client = args[0] as Client;

                        if(client == null) return;
                        if (!client.IsLoggedIn() || !client.HasActiveChar()) return;
                        // TODO: Check whether the active character's inventory actually has the item they are trying to use.

                        ChatHandler.SendActionMessage(client, Shared.Data.Chat.NormalChatMaxDistance, $"rolls a dice landing on {RandomGenerator.GetInstance().Next(0, 7)}");
                    }
                },
                new ItemTemplate("Teddy Bear", "Just a Teddy Bear", StackType.Singleton)
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
