using System;
using System.Collections.Generic;
using System.Linq;
using GTANetworkAPI;
using RPServer.Controllers;
using RPServer.InternalAPI.Extensions;
using RPServer.Util;

namespace RPServer.Models.Inventory
{
    /// <summary>
    /// Basic Template to characterize the various items
    /// </summary>
    internal abstract class ItemTemplate
    {
        private static int _lastUsedID;

        public int ID { get; }
        public string ItemName { get; }
        public string ItemDesc { get; }

        private protected ItemTemplate(string itemName, string itemDesc)
        {
            ID = _lastUsedID++;
            ItemName = itemName;
            ItemDesc = itemDesc;
        }

        /// <summary>
        /// Searches for the corresponding ItemTemplate matching the specified template ID.
        /// </summary>
        /// <param name="id">The template ID</param>
        /// <returns>Either the ItemTemplate instance or null if not found</returns>
        public static ItemTemplate GetTemplate(uint id) => ItemTemplates.FirstOrDefault(it => it.ID == id);
        public static List<ItemTemplate> GetTemplates() => ItemTemplates.ToList();

        #region ItemAction
        protected Action<object[]> _itemAction;
        public bool IsUsable() => _itemAction != null;
        public void InvokeAction(object[] args)
        {
            if(IsUsable()) _itemAction.Invoke(args);
        }
        #endregion

        #region LoadingTemplates
        protected static HashSet<ItemTemplate> ItemTemplates = new HashSet<ItemTemplate>();
        protected static bool Loaded;
        internal static void LoadItemTemplates()
        {
            if (Loaded)
            {
                Logger.GetInstance().ServerError("Item Templates were already loaded.");
                return;
            }

            Logger.GetInstance().ServerInfo("Loading Item Templates...");
            StackableItemTemplate.LoadTemplates();
            SingletonItemTemplate.LoadTemplates();
            MultitionItemTemplate.LoadTemplates();
            Logger.GetInstance().ServerInfo($"\tLoaded {ItemTemplates.Count} total item templates.");

            Loaded = true;
        }
        #endregion

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

    internal class StackableItemTemplate : ItemTemplate
    {
        private StackableItemTemplate(string itemName, string itemDesc) : base(itemName, itemDesc)
        {
        }

        internal static void LoadTemplates()
        {
            if(Loaded) return;
            var count = ItemTemplates.Count;

            ItemTemplates.Add(new StackableItemTemplate("Money", "The standard currency."));
            ItemTemplates.Add(new StackableItemTemplate("Note", "Some note."));

            Logger.GetInstance().ServerInfo($"\t\tLoaded {ItemTemplates.Count - count} Stackable item Templates...");
        }
    }

    internal class SingletonItemTemplate : ItemTemplate
    {
        private SingletonItemTemplate(string itemName, string itemDesc) : base(itemName, itemDesc)
        {
        }

        internal static void LoadTemplates()
        {
            if (Loaded) return;
            var count = ItemTemplates.Count;

            ItemTemplates.Add(new SingletonItemTemplate("Teddy Bear", "Just a Teddy Bear"));
            ItemTemplates.Add(new SingletonItemTemplate("Water Bottle", "A water bottle"));

            Logger.GetInstance().ServerInfo($"\t\tLoaded {ItemTemplates.Count - count} Singleton item Templates...");
        }
    }

    internal class MultitionItemTemplate : ItemTemplate
    {
        private MultitionItemTemplate(string itemName, string itemDesc) : base(itemName, itemDesc)
        {
        }

        internal static void LoadTemplates()
        {
            if (Loaded) return;
            var count = ItemTemplates.Count;

            ItemTemplates.Add(new MultitionItemTemplate("Dice", "Just an ordinary Dice")
            {
                _itemAction = args => // (Client client)
                {
                    if (args == null || args.Length < 1) return;
                    var client = args[0] as Client;

                    if (client == null) return;
                    if (!client.IsLoggedIn() || !client.HasActiveChar()) return;
                    // TODO: Check whether the active character's inventory actually has the item they are trying to use.

                    ChatHandler.SendActionMessage(client, Shared.Data.Chat.NormalChatMaxDistance, $"rolls a dice landing on {RandomGenerator.GetInstance().Next(0, 7)}");
                }
            });

            Logger.GetInstance().ServerInfo($"\t\tLoaded {ItemTemplates.Count - count} Multition item Templates...");

        }
    }
}
