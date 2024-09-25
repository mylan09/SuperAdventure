using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

// Was macht der Code? Er speichert alles was in dieser Spielwelt existiert

namespace Engine
{
    public class World
    {

        public static Monster _currentMonster;
        // Static list variables. similar to the properties in a class. Populate them here, then read from them in the rest of the program
        public static readonly List<Item> _items = new List<Item>();
        public static readonly List<Monster> _monsters = new List<Monster>();
        public static readonly List<Quest> _quests = new List<Quest>();
        public static readonly List<Location> _locations = new List<Location>();

        //Alle Items und deren ID
        //Constants look, and work, like variables, except for one big difference – they can never have their values changed.
        public const int ITEM_ID_RUSTY_SWORD = 1;
        public const int ITEM_ID_RAT_TAIL = 2;
        public const int ITEM_ID_PIECE_OF_FUR = 3;
        public const int ITEM_ID_SNAKE_FANG = 4;
        public const int ITEM_ID_SNAKESKIN = 5;
        public const int ITEM_ID_CLUB = 6;
        public const int ITEM_ID_HEALING_POTION = 7;
        public const int ITEM_ID_SPIDER_FANG = 8;
        public const int ITEM_ID_SPIDER_SILK = 9;
        public const int ITEM_ID_ADVENTURER_PASS = 10;
        public const int ITEM_ID_BOW = 11;
        public const int ITEM_ID_ARROW = 12;
        public const int ITEM_ID_TORCH = 13;
        public const int ITEM_ID_KEY = 14;
        public const int ITEM_ID_BONE = 15;
        public const int ITEM_ID_TOOTH = 16;
        public const int ITEM_ID_LEG = 17;
        public const int ITEM_ID_PADDLE = 18;
        public const int ITEM_ID_TUFTS_OF_HAIR = 19;
        public const int ITEM_ID_FISH_TAIL = 20;
        public const int ITEM_ID_HEALING_STONE = 21;
        public const int ITEM_ID_DRAGON_EGG = 22;
        public const int ITEM_ID_DRAGON_EYE = 23;



        public const int MONSTER_ID_RAT = 1;
        public const int MONSTER_ID_SNAKE = 2;
        public const int MONSTER_ID_GIANT_SPIDER = 3;
        public const int MONSTER_ID_SKELETON = 4; // dungeon guard post end enemy, drops bow and some arrows (Haltbarkeit bei Bogen)
        public const int MONSTER_ID_PIRANHA = 5; // pond - fish tail
        public const int MONSTER_ID_WOLF = 6; // guard post dungeon
        public const int MONSTER_ID_TROLL = 7; // dungeon
        public const int MONSTER_ID_DRAGON = 8; // dungeon
        public const int MONSTER_ID_GIANT_FROG = 9; // swamp - legs

        public const int QUEST_ID_CLEAR_ALCHEMIST_GARDEN = 1;
        public const int QUEST_ID_CLEAR_FARMERS_FIELD = 2;
        public const int QUEST_ID_CLEAR_SPIDER_FIELD = 3;
        public const int QUEST_ID_GET_PADDLE = 4; // Clear POND for the boat to pass the pond to dungeon guard post or swamp
        public const int QUEST_ID_GET_KEY = 5; // To enter dungeon
        public const int QUEST_ID_CLEAR_DUNGEON_ENTRANCE = 6;
        public const int QUEST_ID_CLEAR_DUNGEON = 7; // kill all trolls
        public const int QUEST_ID_GET_TORCH = 8; // To kill the giant frog for a special item (torch?)
        public const int QUEST_ID_DEFEAT_DRAGON = 9;
        public const int QUEST_ID_CLEAR_CAVE = 10;

        public const int LOCATION_ID_HOME = 1;
        public const int LOCATION_ID_TOWN_SQUARE = 2;
        public const int LOCATION_ID_GUARD_POST = 3;
        public const int LOCATION_ID_ALCHEMIST_HUT = 4;
        public const int LOCATION_ID_ALCHEMISTS_GARDEN = 5;
        public const int LOCATION_ID_FARMHOUSE = 6;
        public const int LOCATION_ID_FARM_FIELD = 7;
        public const int LOCATION_ID_BRIDGE = 8;
        public const int LOCATION_ID_SPIDER_FIELD = 9;
        public const int LOCATION_ID_POND = 10;
        public const int LOCATION_ID_SWAMP = 11;
        public const int LOCATION_ID_DUNGEON_GUARD_POST = 12; // drops key
        public const int LOCATION_ID_DUNGEON = 13; // needs healing stone
        public const int LOCATION_ID_DUNGEON_ENTRANCE = 14; // needs Key
        public const int LOCATION_ID_SKELETON_CAVE = 15;
        public const int LOCATION_ID_TRAINING_ROOM = 16;

        public const int UNSELLABLE_ITEM_PRICE = -1;

        //nachfolgend static constructor. when we start the game and want to display information about the player’s current location, and we try to get that data from the World class, the constructor method will be run, and the lists will get populated.
        static World()
        {
            Initialize();
        }

        public static void Initialize()
        {   
            PopulateItems();
            PopulateMonsters();
            PopulateQuests();
            PopulateLocations();
        }

        // Methode PopulateItems()
        private static void PopulateItems()
        {
            _items.Clear();
            _items.Add(new Weapon(ITEM_ID_RUSTY_SWORD, "Rusty sword", "Rusty swords", 0, 5, 5));
            _items.Add(new Item(ITEM_ID_RAT_TAIL, "Rat tail", "Rat tails", 1));
            _items.Add(new Item(ITEM_ID_PIECE_OF_FUR, "Piece of fur", "Pieces of fur", 1));
            _items.Add(new Item(ITEM_ID_SNAKE_FANG, "Snake fang", "Snake fangs", 1));
            _items.Add(new Item(ITEM_ID_SNAKESKIN, "Snakeskin", "Snakeskins", 1));
            _items.Add(new Weapon(ITEM_ID_CLUB, "Club", "Clubs", 3, 10, 50)); // quest reward
            _items.Add(new HealingPotion(ITEM_ID_HEALING_POTION, "Healing potion", "Healing potions", 5, 20));
            _items.Add(new Item(ITEM_ID_SPIDER_FANG, "Spider fang", "Spider fangs", 1));
            _items.Add(new Item(ITEM_ID_SPIDER_SILK, "Spider silk", "Spider silks", 1));
            _items.Add(new Item(ITEM_ID_ADVENTURER_PASS, "Adventurer pass", "Adventurer passes", UNSELLABLE_ITEM_PRICE));
            _items.Add(new Item(ITEM_ID_KEY, "Key", "Keys", UNSELLABLE_ITEM_PRICE)); // quest reward
            _items.Add(new Weapon(ITEM_ID_BOW, "Bow", "Bows", 3, 15, 50)); // quest reward
            _items.Add(new Item(ITEM_ID_ARROW, "Arrow", "Arrows", 1)); // quest reward
            _items.Add(new Item(ITEM_ID_TORCH, "Torch", "Torches", 1)); // quest reward
            _items.Add(new Item(ITEM_ID_BONE, "Bone", "Bones", 1));     // skelette
            _items.Add(new Item(ITEM_ID_TOOTH, "Tooth", "Teeth", 1)); // wolves
            _items.Add(new Item(ITEM_ID_LEG, "Leg", "Legs", 1)); // Frösche
            _items.Add(new Item(ITEM_ID_PADDLE, "Paddle", "Paddles", UNSELLABLE_ITEM_PRICE)); // quest reward
            _items.Add(new Item(ITEM_ID_TUFTS_OF_HAIR, "Tuft of hair", "Tufts of hair", 1)); //Trolle
            _items.Add(new Item(ITEM_ID_FISH_TAIL, "Fish tail", "Fish tails", 1)); // Piranhas
            _items.Add(new HealingPotion(ITEM_ID_HEALING_STONE, "Healing stone", "Healing stones", 150, UNSELLABLE_ITEM_PRICE)); // Trolle
            _items.Add(new Item(ITEM_ID_DRAGON_EGG, "Dragon egg", "Dragon eggs", UNSELLABLE_ITEM_PRICE)); // Dragon
            _items.Add(new Item(ITEM_ID_DRAGON_EYE, "Dragon eye", "Dragon eyes", UNSELLABLE_ITEM_PRICE));

            //For the Adventurer pass, we set the item’s value to -1. We’re going to use that as a “flag” (indicator) value.
            //In our code to display the player’s items, we won’t include any items that have a value of -1.
            //Those will be unsellable items.
        }

        private static void PopulateMonsters()
        {
            Monster rat = new Monster(MONSTER_ID_RAT, "Rat", 5, 20, 10, 3, 3);
            rat.LootTable.Add(new LootItem(ItemByID(ITEM_ID_RAT_TAIL), 75, false));
            rat.LootTable.Add(new LootItem(ItemByID(ITEM_ID_PIECE_OF_FUR), 75, true));

            Monster snake = new Monster(MONSTER_ID_SNAKE, "Snake", 5, 20, 10, 3, 3);
            snake.LootTable.Add(new LootItem(ItemByID(ITEM_ID_SNAKE_FANG), 75, false));
            snake.LootTable.Add(new LootItem(ItemByID(ITEM_ID_SNAKESKIN), 75, true));

            Monster giantSpider = new Monster(MONSTER_ID_GIANT_SPIDER, "Giant spider", 15, 25, 40, 10, 10);
            giantSpider.LootTable.Add(new LootItem(ItemByID(ITEM_ID_SPIDER_FANG), 75, true));
            giantSpider.LootTable.Add(new LootItem(ItemByID(ITEM_ID_SPIDER_SILK), 25, false));

            Monster wolf = new Monster(MONSTER_ID_WOLF, "Wolf", 20, 20, 30, 10, 10); // 3 wolves - drops Key, after the last, spawn 4 Skeletons
            wolf.LootTable.Add(new LootItem(ItemByID(ITEM_ID_TOOTH), 75, true));

            Monster skeleton = new Monster(MONSTER_ID_SKELETON, "Skeleton", 30, 20, 30, 10, 10); // every skeleton drops 1 Arrow
            skeleton.LootTable.Add(new LootItem(ItemByID(ITEM_ID_BONE), 40, true));
            skeleton.LootTable.Add(new LootItem(ItemByID(ITEM_ID_ARROW), 40, true));

            Monster piranha = new Monster(MONSTER_ID_PIRANHA, "Piranha", 17, 20, 30, 10, 10); // Have to fight against 10, after 10 drop Paddles
            piranha.LootTable.Add(new LootItem(ItemByID(ITEM_ID_FISH_TAIL), 75, true));

            Monster giantFrog = new Monster(MONSTER_ID_GIANT_FROG, "Giant Frog", 22, 20, 30, 10, 10); // Have to fight against 6, after drop Torch
            giantFrog.LootTable.Add(new LootItem(ItemByID(ITEM_ID_LEG), 75, true));

            Monster troll = new Monster(MONSTER_ID_TROLL, "Troll", 40, 20, 30, 10, 10); // Have to fight against 10, Healing Stone? = hitPoints +150 - for Dragon Fight
            troll.LootTable.Add(new LootItem(ItemByID(ITEM_ID_TUFTS_OF_HAIR), 20, true));

            Monster dragon = new Monster(MONSTER_ID_DRAGON, "Dragon", 50, 20, 30, 10, 10); // Have to fight against 1
            dragon.LootTable.Add(new LootItem(ItemByID(ITEM_ID_DRAGON_EYE), 100, true));

            _monsters.Clear();
            _monsters.Add(rat);
            _monsters.Add(snake);
            _monsters.Add(giantSpider);
            _monsters.Add(skeleton);
            _monsters.Add(piranha);
            _monsters.Add(wolf);
            _monsters.Add(giantFrog);
            _monsters.Add(troll);
            _monsters.Add(dragon);
        }

        private static void PopulateQuests()
        {
            Quest clearAlchemistGarden =
                new Quest(
                    QUEST_ID_CLEAR_ALCHEMIST_GARDEN,
                    "Clear the alchemist's garden",
                    "Kill rats in the alchemist's garden and bring back 3 rat tails. You will receive a healing potion and 10 gold pieces.", 20, 10);

            clearAlchemistGarden.QuestCompletionItems.Add(new QuestCompletionItem(ItemByID(ITEM_ID_RAT_TAIL), 3));

            clearAlchemistGarden.RewardItem = ItemByID(ITEM_ID_HEALING_POTION);

            Quest clearFarmersField =
                new Quest(
                    QUEST_ID_CLEAR_FARMERS_FIELD,
                    "Clear the farmer's field",
                    "Kill snakes in the farmer's field and bring back 3 snake fangs. You will receive an adventurer's pass and 20 gold pieces.", 20, 20);

            clearFarmersField.QuestCompletionItems.Add(new QuestCompletionItem(ItemByID(ITEM_ID_SNAKE_FANG), 3));

            clearFarmersField.RewardItem = ItemByID(ITEM_ID_ADVENTURER_PASS);
            
            Quest clearSpiderField =
                new Quest(
                    QUEST_ID_CLEAR_SPIDER_FIELD,
                    "Clear the spider field",
                    "Kill all monsters and bring back 3 spider fangs. You will receive a club and 20 gold pieces.", 20, 20);

            clearSpiderField.QuestCompletionItems.Add(new QuestCompletionItem(ItemByID(ITEM_ID_SPIDER_FANG), 3));
            clearSpiderField.RewardItem = ItemByID(ITEM_ID_CLUB);

            Quest getBoat =
                new Quest(
                    QUEST_ID_GET_PADDLE,
                    "Get the boat",
                    "Kill all piranhas in the pond and bring back 10 fish tails. You will receive a paddle and 20 gold pieces.", 20, 20);

            getBoat.QuestCompletionItems.Add(new QuestCompletionItem(ItemByID(ITEM_ID_FISH_TAIL), 10));
            getBoat.RewardItem = ItemByID(ITEM_ID_PADDLE);

            Quest getTorch =
                new Quest(QUEST_ID_GET_TORCH,
                    "Frog King: Get the torch",
                    "Frog King: Kill all giant frogs in the swamp and bring back 6 legs. You will receive a torch and 20 gold pieces.", 20, 20);

            getTorch.QuestCompletionItems.Add(new QuestCompletionItem(ItemByID(ITEM_ID_LEG), 6));
            getTorch.RewardItem = ItemByID(ITEM_ID_TORCH);

            Quest getKey =
                new Quest(QUEST_ID_GET_KEY,
                    "Get the key",
                    "Kill all wolves at the guard post and bring back 3 teeth. You will receive the key to the dungeon and 20 gold pieces.", 20, 20);
                
            getKey.QuestCompletionItems.Add(new QuestCompletionItem(ItemByID(ITEM_ID_TOOTH), 3));
            getKey.RewardItem = ItemByID(ITEM_ID_KEY);

            Quest clearCave =
                new Quest(QUEST_ID_CLEAR_CAVE,
                    "Clear the cave",
                    "Kill all skeletons in the cave and bring back 5 bones. You will receive a bow and 20 gold pieces.", 20, 20);
            clearCave.QuestCompletionItems.Add(new QuestCompletionItem(ItemByID(ITEM_ID_BONE), 5));
            clearCave.RewardItem = ItemByID(ITEM_ID_BOW);

            Quest clearDungeonEntrance =
                new Quest(QUEST_ID_CLEAR_DUNGEON_ENTRANCE,
                    "Clear the dungeon entrance",
                    "Kill all trolls in the dungeon entrance and bring back 10 tufts of hair. You will receive the healing stone and 20 gold pieces.", 20, 20);
            clearDungeonEntrance.QuestCompletionItems.Add(new QuestCompletionItem(ItemByID(ITEM_ID_TUFTS_OF_HAIR), 10));
            clearDungeonEntrance.RewardItem = ItemByID(ITEM_ID_HEALING_STONE);

            Quest clearDungeon =
                new Quest(QUEST_ID_CLEAR_DUNGEON,
                    "Clear the dungeon",
                    "Kill the big dragon in the dungeon. You will receive 20 gold pieces.", 20, 20);
            clearDungeon.QuestCompletionItems.Add(new QuestCompletionItem(ItemByID(ITEM_ID_DRAGON_EYE), 1));
            clearDungeon.RewardItem = ItemByID(ITEM_ID_DRAGON_EGG);
            
            
            _quests.Clear();
            _quests.Add(clearAlchemistGarden);
            _quests.Add(clearFarmersField);
            _quests.Add(clearSpiderField);
            _quests.Add(getBoat);
            _quests.Add(getTorch);
            _quests.Add(getKey);
            _quests.Add(clearCave);
            _quests.Add(clearDungeonEntrance);
            _quests.Add(clearDungeon);

        }

        public static void PopulateLocations()
        {
            // Create each location
            Location home = new Location(LOCATION_ID_HOME, "Home", "Your house. You really need to clean up the place.");

            Location training = new Location(LOCATION_ID_TRAINING_ROOM, "Training field", "Here you can test your Weapons, make sure the monsters don't hurt you! In the trainingroom is no reward for killing monsters");
            training.AddMonster(MONSTER_ID_RAT, 10);
            training.AddMonster(MONSTER_ID_SNAKE, 10);
            training.AddMonster(MONSTER_ID_GIANT_SPIDER, 10);
            training.AddMonster(MONSTER_ID_GIANT_FROG, 10);
            training.AddMonster(MONSTER_ID_PIRANHA, 10);
            training.AddMonster(MONSTER_ID_WOLF, 10);
            training.AddMonster(MONSTER_ID_SKELETON, 10);
            training.AddMonster(MONSTER_ID_TROLL, 10);
            training.AddMonster(MONSTER_ID_DRAGON, 10);

            Location townSquare = new Location(LOCATION_ID_TOWN_SQUARE, "Town square", "You see a fountain.");

            Vendor bobTheRatCatcher = new Vendor("Bob the Rat-Catcher");
            bobTheRatCatcher.AddItemToInventory(ItemByID(ITEM_ID_PIECE_OF_FUR), 5);
            bobTheRatCatcher.AddItemToInventory(ItemByID(ITEM_ID_RAT_TAIL), 3);
            bobTheRatCatcher.AddItemToInventory(ItemByID(ITEM_ID_HEALING_POTION), 50);

            townSquare.VendorWorkingHere = bobTheRatCatcher;

            Location alchemistHut = new Location(LOCATION_ID_ALCHEMIST_HUT, "Alchemist's hut", "There are many strange plants on the shelves.");
            alchemistHut.QuestAvailableHere = QuestByID(QUEST_ID_CLEAR_ALCHEMIST_GARDEN);

            Location alchemistsGarden = new Location(LOCATION_ID_ALCHEMISTS_GARDEN, "Alchemist's garden", "Many plants are growing here.");
            alchemistsGarden.AddMonster(MONSTER_ID_RAT, 70);
            alchemistsGarden.AddMonster(MONSTER_ID_SNAKE, 29);
            alchemistsGarden.AddMonster(MONSTER_ID_GIANT_SPIDER, 1);

            Location farmhouse = new Location(LOCATION_ID_FARMHOUSE, "Farmhouse", "There is a small farmhouse, with a farmer in front.");
            farmhouse.QuestAvailableHere = QuestByID(QUEST_ID_CLEAR_FARMERS_FIELD);

            Location farmersField = new Location(LOCATION_ID_FARM_FIELD, "Farmer's field", "You see rows of vegetables growing here.");
            farmersField.AddMonster(MONSTER_ID_SNAKE, 80);
            farmersField.AddMonster(MONSTER_ID_RAT, 19);
            farmersField.AddMonster(MONSTER_ID_GIANT_SPIDER, 1);

            Location guardPost = new Location(LOCATION_ID_GUARD_POST, "Guard post", "There is a large, tough-looking guard here.", ItemByID(ITEM_ID_ADVENTURER_PASS));
            guardPost.QuestAvailableHere = QuestByID(QUEST_ID_CLEAR_SPIDER_FIELD);

            Location bridge = new Location(LOCATION_ID_BRIDGE, "Bridge", "A stone bridge crosses a wide river.");

            Location spiderField = new Location(LOCATION_ID_SPIDER_FIELD, "Forest", "You see spider webs covering covering the trees in this forest.");
            spiderField.AddMonster(MONSTER_ID_GIANT_SPIDER, 80);
            spiderField.AddMonster(MONSTER_ID_SNAKE, 10);
            spiderField.AddMonster(MONSTER_ID_RAT, 10);

            Location pond = new Location(LOCATION_ID_POND, "Pond", "You see a pond. You can see piranhas swimming in the water. They attack you!");
            pond.AddMonster(MONSTER_ID_PIRANHA, 100);
            pond.QuestAvailableHere = QuestByID(QUEST_ID_GET_PADDLE);

            Location swamp = new Location(LOCATION_ID_SWAMP, "Swamp", "You see a swamp. You can see giant frogs jumping around. They try to attack you!", ItemByID(ITEM_ID_PADDLE));
            swamp.AddMonster(MONSTER_ID_GIANT_FROG, 100);
            swamp.QuestAvailableHere = QuestByID(QUEST_ID_GET_TORCH);

            Location dungeonGuardPost = new Location(LOCATION_ID_DUNGEON_GUARD_POST, "Dungeon Guard Post", "You see a big guard post. You can see wolves guarding. They attack you!", ItemByID(ITEM_ID_TORCH));
            dungeonGuardPost.AddMonster(MONSTER_ID_WOLF, 100);
            dungeonGuardPost.QuestAvailableHere = QuestByID(QUEST_ID_GET_KEY);

            Location skeletonCave = new Location(LOCATION_ID_SKELETON_CAVE, "Skeleton Cave", "You see a cave. You can see skeletons guarding. They attack you!");
            skeletonCave.AddMonster(MONSTER_ID_SKELETON, 100);
            skeletonCave.QuestAvailableHere = QuestByID(QUEST_ID_CLEAR_CAVE);

            Location dungeonEntrance = new Location(LOCATION_ID_DUNGEON_ENTRANCE, "Dungeon Entrance", "You see a big entrance. You can see trolls guarding. They attack you!", ItemByID(ITEM_ID_KEY));
            dungeonEntrance.AddMonster(MONSTER_ID_TROLL, 100);
            dungeonEntrance.QuestAvailableHere = QuestByID(QUEST_ID_CLEAR_DUNGEON_ENTRANCE);

            Location dungeon = new Location(LOCATION_ID_DUNGEON, "Dungeon", "You see a big dragon. He attacks you!", ItemByID(ITEM_ID_BOW));
            dungeon.AddMonster(MONSTER_ID_DRAGON, 100);
            dungeon.QuestAvailableHere = QuestByID(QUEST_ID_CLEAR_DUNGEON);

            // Link the locations together
            home.LocationToNorth = townSquare;
            home.LocationToWest = training;
            training.LocationToEast = home;

            townSquare.LocationToNorth = alchemistHut;
            townSquare.LocationToSouth = home;
            townSquare.LocationToEast = guardPost;
            townSquare.LocationToWest = farmhouse;

            farmhouse.LocationToEast = townSquare;
            farmhouse.LocationToWest = farmersField;

            farmersField.LocationToEast = farmhouse;

            alchemistHut.LocationToSouth = townSquare;
            alchemistHut.LocationToNorth = alchemistsGarden;

            alchemistsGarden.LocationToSouth = alchemistHut;

            guardPost.LocationToEast = bridge;
            guardPost.LocationToWest = townSquare;

            bridge.LocationToWest = guardPost;
            bridge.LocationToEast = spiderField;

            spiderField.LocationToWest = bridge;
            spiderField.LocationToNorth = pond;

            pond.LocationToSouth = spiderField;
            pond.LocationToNorth = swamp;
            pond.LocationToWest = dungeonGuardPost;

            swamp.LocationToSouth = pond;

            dungeonGuardPost.LocationToEast = pond;
            dungeonGuardPost.LocationToWest = skeletonCave;

            skeletonCave.LocationToEast = dungeonGuardPost;
            skeletonCave.LocationToNorth = dungeonEntrance;

            dungeonEntrance.LocationToSouth = skeletonCave;
            dungeonEntrance.LocationToEast = dungeon;

            dungeon.LocationToWest = dungeonEntrance;

            _locations.Clear();
            // Add the locations to the static list
            _locations.Add(home);
            _locations.Add(townSquare);
            _locations.Add(guardPost);
            _locations.Add(alchemistHut);
            _locations.Add(alchemistsGarden);
            _locations.Add(farmhouse);
            _locations.Add(farmersField);
            _locations.Add(bridge);
            _locations.Add(spiderField); ;
            _locations.Add(pond);
            _locations.Add(dungeonGuardPost);
            _locations.Add(dungeon);
            _locations.Add(skeletonCave);
            _locations.Add(dungeonEntrance);
            _locations.Add(swamp);
        }

        public static Item ItemByID(int id)
        {
            return _items.SingleOrDefault(x => x.ID == id);
        }

        public static Monster MonsterByID(int id)
        {
            return _monsters.SingleOrDefault(x => x.ID == id);
        }

        public static Quest QuestByID(int id)
        {
            return _quests.SingleOrDefault(x => x.ID == id);
        }

        public static Location LocationByID(int id)
        {
            var locById = _locations.SingleOrDefault(x => x.ID == id);
            return locById;
        }

    }

}
