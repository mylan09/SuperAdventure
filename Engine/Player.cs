using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.Remoting.Messaging;
using System.Drawing;

namespace Engine
{
    public class Player : LivingCreature
    {

        private int _gold;

        private int _experiencePoints;

        private Location _currentLocation;

        public int Gold
        {
            get { return _gold; }
            set
            {
                _gold = value;
                OnPropertyChanged("Gold");
            }
        }
        public int ExperiencePoints
        {
            get { return _experiencePoints; }
            set
            {
                _experiencePoints = value;
                OnPropertyChanged("ExperiencePoints");
                OnPropertyChanged("Level");
            }
        }

        public event EventHandler<MessageEventArgs> OnMessage;

        public int Level { get; set; }

        public Location CurrentLocation
        {
            get { return _currentLocation; }
            set
            {
                _currentLocation = value;
                OnPropertyChanged("CurrentLocation");
            }
        }

        public Weapon CurrentWeapon { get; set; }

        public List<Weapon> Weapons
        {
            get { return Inventory.Where(x => x.Details is Weapon).Select(x => x.Details as Weapon).ToList(); }
        }

        public List<HealingPotion> Potions
        {
            get { return Inventory.Where(x => x.Details is HealingPotion).Select(x => x.Details as HealingPotion).Where(p => p != null).ToList(); }
        }

        public List <int> LocationsVisited { get; set; }
        public InventoryChest Chest { get; set; }

        public BindingList<InventoryItem> Inventory { get; set; }

        public BindingList<PlayerQuest> Quests { get; set; }

        public Monster CurrentMonster { get; set; }

        private Player(int currentHitPoints, int maximumHitPoints, int gold, int experiencePoints) : base(currentHitPoints, maximumHitPoints)
        {
            Gold = gold;
            ExperiencePoints = experiencePoints;

            Inventory = new BindingList<InventoryItem>();
            Quests = new BindingList<PlayerQuest>();
            Level = 1;
            LocationsVisited = new List<int>();
            Chest = new InventoryChest("Chest");
        }

        public Player() : base(0, 0)
        {
            Inventory = new BindingList<InventoryItem>();
            Quests = new BindingList<PlayerQuest>();
            LocationsVisited = new List<int>();
           
        }

        public static Player CreateDefaultPlayer()
        {
            Player player = new Player(10, 10, 20, 0);
            player.AddWeaponToInventory(World.ItemByID(World.ITEM_ID_RUSTY_SWORD), 1);
            player.CurrentLocation = World.LocationByID(World.LOCATION_ID_HOME);

            return player;

        }

        private void RaiseInventoryChangedEvent(Item item)
        {
            if (item is Weapon)
            {
                OnPropertyChanged("Weapons");
            }

            if (item is HealingPotion)
            {
                OnPropertyChanged("Potions");
            }


        }

        private void RaiseMessage(string message, bool addExtraNewLine = false, Color? textColor = null)
        {
            if (OnMessage != null)
            {
                if(textColor == null)
                {
                   textColor = System.Drawing.Color.Black;
                }   
                
                OnMessage?.Invoke(this, new MessageEventArgs(message, addExtraNewLine, textColor.Value));
            }
        }

        private void CompletelyHeal()
        {
            CurrentHitPoints = MaximumHitPoints;
        }

        public void MoveTo(Location location)
        {

            //Does the location have any required items
            if (!HasRequiredItemToEnterThisLocation(location))
            {
                RaiseMessage("You must have a " + location.ItemRequiredToEnter.Name + " to enter this location.", true, Color.Red);
                return;

            }

            // Update the player's current location
            //CurrentLocation = World.LocationByID(location.ID);
            CurrentLocation = location;

            if (!LocationsVisited.Contains(CurrentLocation.ID))
            {
                LocationsVisited.Add(CurrentLocation.ID);
            }

            // Completely heal the player
            CompletelyHeal();

            if (location.HasAQuest)
            {
                if (PlayerDoesNotHaveThisQuest(location.QuestAvailableHere))
                {
                    GiveQuestToPlayer(location.QuestAvailableHere);
                }
                else
                {
                    if(location == World.LocationByID(World.LOCATION_ID_DUNGEON) &&
                        PlayerHasNotCompleted(location.QuestAvailableHere))
                    {
                        CurrentLocation.CloseOffLocation = true;
                        OnPropertyChanged("CloseOffLocation");
                    }
                    if (PlayerHasNotCompleted(location.QuestAvailableHere) &&
                        PlayerHasAllQuestCompletionItemsFor(location.QuestAvailableHere))
                    {
                       
                        GivePlayerQuestRewards(location.QuestAvailableHere);
                    }
                }
            }
            

            SetTheCurrentMonsterForTheCurrentLocation(location);
            SortQuests();
        }

        private void SetTheCurrentMonsterForTheCurrentLocation(Location location)
        {
            // Überprüfen, ob es an dieser Location eine Quest gibt
            if (location.HasAQuest)
            {
                // Prüfen, ob der Spieler die Quest an dieser Location abgeschlossen hat
                PlayerQuest playerQuest = Quests.SingleOrDefault(pq => pq.Details.ID == location.QuestAvailableHere.ID);

                // Wenn die Quest abgeschlossen ist, wird kein Monster mehr generiert
                //if (playerQuest != null && playerQuest.IsCompleted)
                //{
                //    CurrentMonster = null;
                //    RaiseMessage("No monsters remain here, you've completed the quest.", true, Color.Gray);
                //    return;
                //}
            }

            // Populate the current monster with this location's monster (or null, if there is no monster here)
            CurrentMonster = location.NewInstanceOfMonsterLivingHere();
           
            if (CurrentMonster != null)
            {
                RaiseMessage("You see a " + CurrentMonster.Name, true, textColor: Color.Blue);
            }
        }

        public void UseWeapon(Weapon weapon)
        {

            // Überprüfen, ob die aktuelle Waffe der Bogen ist
            if (weapon.ID == World.ITEM_ID_BOW)
            {
                // Überprüfen, ob Pfeile im Inventar vorhanden sind
                var arrow = Inventory.SingleOrDefault(item => item.Details.ID == World.ITEM_ID_ARROW);

                if (arrow == null || arrow.Quantity <= 0)
                {
                    RaiseMessage("You have no arrows left to use the Bow.", true, Color.Red);
                    return;  // Kein Angriff, wenn keine Pfeile da sind
                }
                else
                {
                    // Entferne einen Pfeil aus dem Inventar
                    RemoveItemFromInventory(arrow.Details, 1);
                }
            }
            
            int damage = RandomNumberGenerator.NumberBetween(weapon.MinimumDamage, weapon.MaximumDamage);

            if (damage == 0)
            {
                RaiseMessage("You missed the " + CurrentMonster.Name, false, textColor: Color.Red);
            }
            else
            {
                CurrentMonster.CurrentHitPoints -= damage;
                RaiseMessage("You hit the " + CurrentMonster.Name + " for " + damage + " points.", true, Color.Green);
            }

            if (CurrentMonster != null && CurrentMonster.IsDead)
            {
                LootTheCurrentMonster();

                // "Move" to the current location, to refresh the current monster
                MoveTo(CurrentLocation);
            }
            else
            {
                LetTheMonsterAttack();
            }

        }

        private void LootTheCurrentMonster()
        {
            RaiseMessage("You defeated the " + CurrentMonster.Name, false, Color.Red);
            RaiseMessage("You receive " + CurrentMonster.RewardExperiencePoints + " experience points", false, Color.DarkSalmon);
            RaiseMessage("You receive " + CurrentMonster.RewardGold + " gold", false, Color.DarkSalmon);

            AddExperiencePoints(CurrentMonster.RewardExperiencePoints);
            Gold += CurrentMonster.RewardGold;

            // Give monster's loot items to the player

            foreach (InventoryItem inventoryItem in CurrentMonster.LootItems)
            {
                AddItemToInventory(inventoryItem.Details);

                RaiseMessage(string.Format("You loot {0} {1}", inventoryItem.Quantity, inventoryItem.Description), false, Color.Black);
            }
            RaiseMessage("");
        }

        private void LetTheMonsterAttack()
        {
            int damageToPlayer = RandomNumberGenerator.NumberBetween(0, CurrentMonster.MaximumDamage);

            RaiseMessage("The " + CurrentMonster.Name + " did " + damageToPlayer + " points of damage.");

            CurrentHitPoints -= damageToPlayer;

            if (IsDead)
            {
                RaiseMessage("The " + CurrentMonster.Name + " killed you.", true, Color.Red);

                MoveHome();
            }
        }

        public void UsePotion(HealingPotion potion)
        {
            RaiseMessage("You drink a " + potion.Name, true, Color.Green);

            HealPlayer(potion.AmountToHeal);

            RemoveItemFromInventory(potion);

            // The player used their turn to drink the potion, so let the monster attack now
            LetTheMonsterAttack();
        }

        private void HealPlayer(int hitPointsToHeal)
        {
            CurrentHitPoints = Math.Min(CurrentHitPoints + hitPointsToHeal, MaximumHitPoints);
        }

        public void AddExperiencePoints(int xpGain)
        {

            ExperiencePoints += xpGain; // add the earned XP to the players current ExperiencePoints property

            if (Level <= 2)

            {
                MaximumHitPoints = 10;
            }

            var levelUp = PlayerLevelUp();
            if (levelUp)
            {
                RaiseMessage("Congratulations! You're now level " + Level + " Your HitPoints increased by " + (Level * 5) + ".", false, Color.Pink);
            }
        }

        public bool HasRequiredItemToEnterThisLocation(Location location)
        {

            if (location.DoesNotHaveAnItemRequiredToEnter)
            {
                return true;
            }

            // See if the player has the required item in their inventory
            return Inventory.Any(ii => ii.Details.ID == location.ItemRequiredToEnter.ID);
       
        }

        public void RemoveQuestCompletionItems(Quest quest)
        {
            foreach (QuestCompletionItem qci in quest.QuestCompletionItems)
            {
                // Subtract the quantity from the player's inventory that was needed to complete the quest
                InventoryItem item = Inventory.SingleOrDefault(ii => ii.Details.ID == qci.Details.ID);
                if (item != null)
                {
                    RemoveItemFromInventory(item.Details, qci.Quantity);
                }
            }
        }

        public void AddItemToInventory(Item itemToAdd, int quantity = 1)
        {
            InventoryItem item = Inventory.SingleOrDefault(ii => ii.Details.ID == itemToAdd.ID);
            
            if(CurrentLocation != World.LocationByID(World.LOCATION_ID_TRAINING_ROOM))

            if (item == null)
            {
                // They didn't have the item, so add it to their inventory
                Inventory.Add(new InventoryItem(itemToAdd, quantity));
            }
            else
            {
                // They have the item in their inventory, so increase the quantity
                item.Quantity += quantity;
            }
            RaiseInventoryChangedEvent(itemToAdd);
        }

        public void AddWeaponToInventory(Item itemToAdd, int quantity = 1)
        {
            InventoryItem inventoryItem = Inventory.SingleOrDefault(ii => ii.Details.ID == itemToAdd.ID);

            if (inventoryItem == null)
            {
                Inventory.Add(new InventoryItem(itemToAdd, quantity));
            }
            else
            {
                inventoryItem.Quantity += quantity;
            }

            RaiseInventoryChangedEvent(itemToAdd);
        }

        public void SortQuests()
        {
            // Sortiere die Quests so, dass die nicht abgeschlossenen zuerst erscheinen
            var sortedQuests = Quests.OrderBy(q => q.IsCompleted).ToList();

            // Leere die ursprüngliche BindingList und füge die sortierten Quests wieder hinzu
            Quests.Clear();
            foreach (var quest in sortedQuests)
            {
                Quests.Add(quest);
            }
        }

        private void GiveQuestToPlayer(Quest quest)
        {
            
            RaiseMessage("You receive the " + quest.Name + " quest.", false, Color.Green);
            RaiseMessage(quest.Description, true, Color.Black);
            RaiseMessage("To complete it, return with:", false, Color.Red);

            foreach (QuestCompletionItem qci in quest.QuestCompletionItems)
            {
                RaiseMessage(string.Format("{0} {1}", qci.Quantity,
                                           qci.Quantity == 1 ? qci.Details.Name : qci.Details.NamePlural), false, Color.Green);
            }

            RaiseMessage("");

            if (CurrentLocation == World.LocationByID(World.LOCATION_ID_DUNGEON))
            {
                CurrentLocation.CloseOffLocation = true;
                OnPropertyChanged("CurrentLocation");
            }

            Quests.Add(new PlayerQuest(quest));

            SortQuests();
        }

        private bool PlayerDoesNotHaveThisQuest(Quest quest)
        {
            return Quests.All(pq => pq.Details.ID != quest.ID);
        }

        private bool PlayerHasNotCompleted(Quest quest)
        {
            return Quests.Any(pq => pq.Details.ID == quest.ID && !pq.IsCompleted);
        }

        private bool PlayerHasAllQuestCompletionItemsFor(Quest quest)
        {
            // See if the player has all the items needed to complete the quest here
            foreach (QuestCompletionItem qci in quest.QuestCompletionItems)
            {
                // Check each item in the player's inventory, to see if they have it, and enough of it
                if (!Inventory.Any(ii => ii.Details.ID == qci.Details.ID && ii.Quantity >= qci.Quantity))
                {
                    return false;
                }
            }

            // If we got here, then the player must have all the required items, and enough of them, to complete the quest.
            return true;
        }

        public void RemoveItemFromInventory(Item itemToRemove, int quantity = 1)
        {
            InventoryItem item = Inventory.SingleOrDefault(ii => ii.Details.ID == itemToRemove.ID);

            if (item == null)
            {
                // Das Item ist nicht im Inventar vorhanden, ignoriere
            }
            else
            {
                // Reduziere die Anzahl des Items im Inventar
                item.Quantity -= quantity;

                // Verhindere negative Mengen
                if (item.Quantity < 0)
                {
                    item.Quantity = 0;
                }

                // Entferne das Item, wenn die Menge 0 ist
                if (item.Quantity == 0)
                {
                    Inventory.Remove(item);
                }

                // Benachrichtige das UI, dass sich das Inventar geändert hat
                RaiseInventoryChangedEvent(itemToRemove);
            }
        }

        private void GivePlayerQuestRewards(Quest quest)
        {
            RaiseMessage("");
            RaiseMessage("You complete the '" + quest.Name + "' quest.", false, Color.Green);
            RaiseMessage("");
            RaiseMessage("You gave: ", false);

   
            
            if(CurrentLocation == World.LocationByID(World.LOCATION_ID_GUARD_POST))
            {
                foreach (QuestCompletionItem qci in quest.QuestCompletionItems)

                {
                    RaiseMessage(string.Format("{0} {1}", qci.Quantity,
                                               qci.Quantity == 1 ? qci.Details.Name : qci.Details.NamePlural) + " to the guard.", false);
                }

            }

            if (CurrentLocation == World.LocationByID(World.LOCATION_ID_SWAMP))
            {
                foreach (QuestCompletionItem qci in quest.QuestCompletionItems)

                {
                    RaiseMessage(string.Format("{0} {1}", qci.Quantity,
                                               qci.Quantity == 1 ? qci.Details.Name : qci.Details.NamePlural) + " to the Frog King.", false);
                }

            }

            else
            {
                foreach (QuestCompletionItem qci in quest.QuestCompletionItems)

                {
                    RaiseMessage(string.Format("{0} {1}", qci.Quantity,
                                               qci.Quantity == 1 ? qci.Details.Name : qci.Details.NamePlural) + " to the alchemist.", false);
                }
            }
            

            RaiseMessage("");
            RaiseMessage("You receive: ", false, Color.Green);
            RaiseMessage(quest.RewardExperiencePoints + " experience points", false, Color.DarkSalmon);
            RaiseMessage(quest.RewardGold + " gold", false, Color.DarkSalmon);
            RaiseMessage("1 " + quest.RewardItem.Name, true, Color.DarkSalmon);
            

            //if (quest.RewardItem is HealingPotion healingPotion)
            //{
            //    AddPotionToInventory(healingPotion); // Hier fügst du den Heiltrank der Inventarliste hinzu
            //}
            //else
            //{
            //    AddItemToInventory(quest.RewardItem); // Für alle anderen Items
            //}

            AddExperiencePoints(quest.RewardExperiencePoints);
            Gold += quest.RewardGold;

            RemoveQuestCompletionItems(quest);
            AddItemToInventory(quest.RewardItem);

            MarkPlayerQuestCompleted(quest);
        }

        private void MarkPlayerQuestCompleted(Quest quest)
        {
            PlayerQuest playerQuest = Quests.SingleOrDefault(pq => pq.Details.ID == quest.ID);

            if (playerQuest != null)
            {
                playerQuest.IsCompleted = true;
                CurrentLocation.CloseOffLocation = false;
                OnPropertyChanged("CloseOffLocation");
            }
            
            SortQuests();
        }

        public bool PlayerLevelUp()
        {

            var retValue = false;

            if (ExperiencePoints >= Level * 100)
            {
                Level++; // add Level to current Level
                MaximumHitPoints = Level * 5;
                retValue = true;
            }

            return retValue;

        }

        private void MoveHome()
        {
            MoveTo(World.LocationByID(World.LOCATION_ID_HOME));
        }

        public void MoveNorth()
        {
            if (CurrentLocation.LocationToNorth != null)
            {
                MoveTo(CurrentLocation.LocationToNorth);
            }
        }

        public void MoveEast()
        {
            if (CurrentLocation.LocationToEast != null)
            {
                MoveTo(CurrentLocation.LocationToEast);
            }
        }

        public void MoveSouth()
        {
            if (CurrentLocation.LocationToSouth != null)
            {
                MoveTo(CurrentLocation.LocationToSouth);
            }
        }

        public void MoveWest()
        {
            if (CurrentLocation.LocationToWest != null)
            {
                MoveTo(CurrentLocation.LocationToWest);
            }
        }

        public string ToJsonString()
        {

            var options = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                PreserveReferencesHandling = PreserveReferencesHandling.All,
                TypeNameHandling = TypeNameHandling.Auto
            };
            
            var playerToSave = JsonConvert.SerializeObject(this, options);
            DateTimeOffset dto = new DateTimeOffset(DateTime.Now);
            var fileName = $"SaveGame_{dto.ToUnixTimeMilliseconds()}.json";
            
            if (!Directory.Exists("SaveGames"))
            {
                Directory.CreateDirectory("SaveGames");
            }
            
            File.WriteAllText(Path.Combine("SaveGames", fileName), playerToSave, Encoding.UTF8);
            
            return string.Empty;

        }


    }

}
