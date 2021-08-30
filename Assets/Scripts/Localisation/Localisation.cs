using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Localisation
{
    public static Language currentLanguage { get; private set; } = Language.Portuguese;

    public static Dictionary<Language, Dictionary<StringKey, string>> languageStringDictionary = new Dictionary<Language, Dictionary<StringKey, string>>(){
        {
            Language.English, new Dictionary<StringKey, string>(){
                {StringKey.Class_Ranger_Name, "Ranger"},
                {StringKey.Class_Rogue_Name, "Rogue"},
                {StringKey.Class_Warrior_Name, "Warrior"},

                {StringKey.Item_RedMushroom_Name, "Red Mushroom"},
                {StringKey.Item_SlimMushroom_Name, "Slim Mushroom"},
                {StringKey.Item_TreeMushroom_Name, "Tree Mushroom"},
                {StringKey.Item_AlgaeMushroom_Name, "Algae Mushroom"},
                {StringKey.Item_BasicMushroom_Name, "Basic Mushroom"},
                {StringKey.Item_GnomeMushroom_Name, "Gnome Mushroom"},
                {StringKey.Item_HairyMushroom_Name, "Hairy Mushroom"},
                {StringKey.Item_ShellMushroom_Name, "Shell Mushroom"},
                {StringKey.Item_DragonMushroom_Name, "Dragon Mushroom"},
                {StringKey.Item_PurpleMushroom_Name, "Purple Mushroom"},

                {StringKey.Item_Berries_Name, "Berries"},
                {StringKey.Item_CoalOre_Name, "Coal"},
                {StringKey.Item_Fish_Name, "Fish"},
                {StringKey.Item_GoldOre_Name, "Gold Ore"},
                {StringKey.Item_IronOre_Name, "Iron Ore"},
                {StringKey.Item_Meat_Name, "Meat"},
                {StringKey.Item_WoodLog_Name, "Wood Log"},

                {StringKey.Item_GoldIngot_Name, "Gold Ingot"},
                {StringKey.Item_IronIngot_Name, "Iron Ingot"},
                {StringKey.Item_CoalProcessed_Name, "Processed Coal"},
                {StringKey.Item_WoodPlank_Name, "Wood Plank"},

                {StringKey.Item_Coin_Name, "Coin"},
                {StringKey.Item_Gem_Name, "Gem"},

                {StringKey.Item_EncryptedKey_Name, "Encrypted Key"},
                {StringKey.Item_DecryptedKey_Name, "Decrypted Key"},

                {StringKey.HUD_LootFeedback, "You got $AMOUNT$x $ITEM$!"},

                {StringKey.UI_DailyQuestDescription, "Sell the number of items shown"},
                {StringKey.UI_DailyQuestReward, "Increased Damage"},

                {StringKey.UI_EndScreen_Success_Title, "You return to the shop with the loot you collected"},
                {StringKey.UI_EndScreen_Success_CollectedItems, "COLLECTED ITEMS"},
                {StringKey.UI_EndScreen_Success_Offering, "Walking back you find the King, asking for a tribute"},
                {StringKey.UI_EndScreen_Success_Continue, "Press ENTER to continue..."},
                {StringKey.UI_EndScreen_Failure_Title, "You were defeated but managed to collect some loot"},
                {StringKey.UI_EndScreen_Failure_CollectedItems, "COLLECTED ITEMS"},
                {StringKey.UI_EndScreen_Failure_Offering, "Walking back you find the King, asking for a tribute"},
                {StringKey.UI_EndScreen_Failure_Continue, "Press ENTER to continue..."},
                {StringKey.UI_ClassSelect_Title, "Select your class:"},

                {StringKey.UI_Oracle_Message, "The Oracle whispers, predicting that this item are in high demand at this time today:"},
                {StringKey.UI_SecretDoor_Message, "This door requires a code to unlock"},

                {StringKey.UI_NextMission_AbundantItem, "Abundant Item"},
                {StringKey.UI_NextMission_DiseasedItem, "Diseased Item"},

                {StringKey.UI_RawItems_Title, "Raw Items"},
                {StringKey.UI_ProcessedItems_Title, "Processed Items"},
                {StringKey.UI_MarketItems_Title, "Product Sell"},

                {StringKey.UI_AdventurerDailyQuest_Reward_Text, "Reward"},
                { StringKey.UI_AdventurerDailyQuest_Description, "Collect the exact number shown"},
                { StringKey.UI_AdventurerDailyQuest_Complete_Text, "Complete"},
                { StringKey.UI_AdventurerDailyQuest_Complete_Description, "$ITEMNAME$ unlocked"},

                { StringKey.Buff_Afraid_Name, "Afraid"},
                { StringKey.Buff_Afraid_Description, "Get speed when entering rooms."},
                { StringKey.Buff_Blessing_Name, "Blessing"},
                { StringKey.Buff_Blessing_Description, "Your enemies will be weaker!"},
                { StringKey.Buff_LuckyCharm_Name, "Lucky Charm"},
                { StringKey.Buff_LuckyCharm_Description, "Double to Loot!"},
                { StringKey.Buff_DeflectingArmor_Name, "Deflecting Armor"},
                { StringKey.Buff_DeflectingArmor_Description, "Chance of resisting turret shots."},
                { StringKey.Buff_IronBoots_Name, "Iron Boots"},
                { StringKey.Buff_IronBoots_Description, "Spikes won't hurt you!"},
                { StringKey.Buff_LifeSteal_Name, "Life Steal"},
                { StringKey.Buff_LifeSteal_Description, "Your attacks heal you back."},
                { StringKey.Buff_Poison_Name, "Poison"},
                { StringKey.Buff_Poison_Description, "Your attacks poison your enemies"},
                { StringKey.Buff_Spread_Name, "Spread"},
                { StringKey.Buff_Spread_Description, "Shoot two more arrows with your bow!"},
                {StringKey.Buff_Health_Description,"More Health"},
                {StringKey.Buff_Damage_Description,"More Damage"},
                {StringKey.Buff_Movement_Description,"More Speed"},
                {StringKey.Buff_UnderConstruction_Description, "WARNING: Under Construction"},

                { StringKey.Unlock_Buff_Altar_Name, "Altar"},
                { StringKey.Unlock_Buff_Altar_Description, "Unlocks the Altar - Giving buffs to the adventurer!"},
                { StringKey.Unlock_Buff_Afraid_Name, "Afraid"},
                { StringKey.Unlock_Buff_Afraid_Description, "Unlocks the Afraid buff - Get speed when entering rooms."},
                { StringKey.Unlock_Buff_Blessing_Name, "Blessing"},
                { StringKey.Unlock_Buff_Blessing_Description, "Unlocks the Blessing buff - Your enemies will be weaker!"},
                { StringKey.Unlock_Buff_LuckyCharm_Name, "Lucky Charm"},
                { StringKey.Unlock_Buff_LuckyCharm_Description, "Unlocks the Lucky Charm buff - Double Loot!"},
                { StringKey.Unlock_Buff_DeflectingArmor_Name, "Deflecting Armor"},
                { StringKey.Unlock_Buff_DeflectingArmor_Description, "Unlocks the Deflecting Armor buff - Chance of resisting turret shots."},
                { StringKey.Unlock_Buff_IronBoots_Name, "Iron Boots"},
                { StringKey.Unlock_Buff_IronBoots_Description, "Unlocks the Iron Boots buff - Spikes won't hurt you!"},
                { StringKey.Unlock_Buff_LifeSteal_Name, "Life Steal"},
                { StringKey.Unlock_Buff_LifeSteal_Description, "Unlocks the Life Steal buff - Your attacks heal you back."},
                { StringKey.Unlock_Buff_Poison_Name, "Poison"},
                { StringKey.Unlock_Buff_Poison_Description, "Unlocks the Poison buff - Your attacks poison your enemies"},
                { StringKey.Unlock_Buff_Spread_Name, "Spread"},
                { StringKey.Unlock_Buff_Spread_Description, "Unlocks the Spread buff - Shoot two more arrows with your bow!"},

                { StringKey.Unlock_Item_RedMushroom_Name, "Red Mushroom"},
                { StringKey.Unlock_Item_RedMushroom_Description, ""},
                { StringKey.Unlock_Item_SlimMushroom_Name, "Slim Mushroom"},
                { StringKey.Unlock_Item_SlimMushroom_Description, ""},
                { StringKey.Unlock_Item_TreeMushroom_Name, "Tree Mushroom"},
                { StringKey.Unlock_Item_TreeMushroom_Description, ""},
                { StringKey.Unlock_Item_AlgaeMushroom_Name, "Algae Mushroom"},
                { StringKey.Unlock_Item_AlgaeMushroom_Description, ""},
                { StringKey.Unlock_Item_GnomeMushroom_Name, "Gnome Mushroom"},
                { StringKey.Unlock_Item_GnomeMushroom_Description, ""},
                { StringKey.Unlock_Item_HairyMushroom_Name, "Hairy Mushroom"},
                { StringKey.Unlock_Item_HairyMushroom_Description, ""},
                { StringKey.Unlock_Item_ShellMushroom_Name, "Shell Mushroom"},
                { StringKey.Unlock_Item_ShellMushroom_Description, ""},
                { StringKey.Unlock_Item_DragonMushroom_Name, "Dragon Mushroom"},
                { StringKey.Unlock_Item_DragonMushroom_Description, ""},
                { StringKey.Unlock_Item_PurpleMushroom_Name, "Purple Mushroom"},
                { StringKey.Unlock_Item_PurpleMushroom_Description, ""},

                { StringKey.Unlock_Item_FoodMarket_Name, "Food Market"},
                { StringKey.Unlock_Item_FoodMarket_Description, "Unlocks Meat and Fish!"},
                { StringKey.Unlock_Item_Groceries_Name, "Groceries"},
                { StringKey.Unlock_Item_Groceries_Description, "Unlocks Berries!"},
                { StringKey.Unlock_Item_Jeweler_Name, "Jeweler"},
                { StringKey.Unlock_Item_Jeweler_Description, "Unlocks Gold!"},

                { StringKey.Unlock_Stats_Cooking_Name, "Cooking"},
                { StringKey.Unlock_Stats_Cooking_Description, "Increases adventurer's health"},
                { StringKey.Unlock_Stats_DancingLessons_Name, "Dancing Lessons"},
                { StringKey.Unlock_Stats_DancingLessons_Description, "Increases adventurer's speed"},
                { StringKey.Unlock_Stats_TrainingDummy_Name, "Training Dummy"},
                { StringKey.Unlock_Stats_TrainingDummy_Description, "Increases adventurer's damage"},

                { StringKey.Unlock_Class_Rogue_Name, "Rogue"},
                { StringKey.Unlock_Class_Rogue_Description, "Unlocks a new class for the adventurer!"},
                { StringKey.Unlock_Class_Warrior_Name, "Warrior"},
                { StringKey.Unlock_Class_Warrior_Description, "Unlocks a new class for the adventurer!"},

                { StringKey.Unlock_Shop_IronWorkshop_Name, "Iron Workshop"},
                { StringKey.Unlock_Shop_IronWorkshop_Description, "Get more iron ingots from processing"},
                { StringKey.Unlock_Shop_WoodWorkshop_Name, "Wood Workshop"},
                { StringKey.Unlock_Shop_WoodWorkshop_Description, "Get more wood planks from processing"},

                {StringKey.DiseasedItemTutorial, "This resource is sick and brought loss to the store."}
            }
        },
        {
    Language.Portuguese, new Dictionary<StringKey, string>(){
                {StringKey.Class_Ranger_Name, "Arqueiro"},
                {StringKey.Class_Rogue_Name, "Assassino"},
                {StringKey.Class_Warrior_Name, "Guerreiro"},

                {StringKey.Item_RedMushroom_Name, "Cogumelo Vermelho"},
                {StringKey.Item_SlimMushroom_Name, "Cogumelo Fino"},
                {StringKey.Item_TreeMushroom_Name, "Cogumelo Árvore"},
                {StringKey.Item_AlgaeMushroom_Name, "Cogumelo Alga"},
                {StringKey.Item_BasicMushroom_Name, "Cogumelo Básico"},
                {StringKey.Item_GnomeMushroom_Name, "Cogumelo Gnomo"},
                {StringKey.Item_HairyMushroom_Name, "Cogumelo Peludo"},
                {StringKey.Item_ShellMushroom_Name, "Cogumelo Concha"},
                {StringKey.Item_DragonMushroom_Name, "Cogumelo Dragão"},
                {StringKey.Item_PurpleMushroom_Name, "Cogumelo Roxo"},

                {StringKey.Item_Berries_Name, "Frutas"},
                {StringKey.Item_CoalOre_Name, "Carvão"},
                {StringKey.Item_Fish_Name, "Peixe"},
                {StringKey.Item_GoldOre_Name, "Minério de Ouro"},
                {StringKey.Item_IronOre_Name, "Minério de Ferro"},
                {StringKey.Item_Meat_Name, "Carne"},
                {StringKey.Item_WoodLog_Name, "Madeira"},

                {StringKey.Item_GoldIngot_Name, "Ouro"},
                {StringKey.Item_IronIngot_Name, "Ferro"},
                {StringKey.Item_CoalProcessed_Name, "Carvão Processado"},
                {StringKey.Item_WoodPlank_Name, "Tábua de Madeira"},

                {StringKey.Item_Coin_Name, "Moeda"},
                {StringKey.Item_Gem_Name, "Gema"},

                {StringKey.Item_EncryptedKey_Name, "Chave Encriptada"},
                {StringKey.Item_DecryptedKey_Name, "Chave Desencriptada"},

                {StringKey.HUD_LootFeedback, "Você ganhou $AMOUNT$x $ITEM$!"},

                {StringKey.UI_DailyQuestDescription, "Vende o número de itens mostrados"},
                {StringKey.UI_DailyQuestReward, "Mais Dano"},

                {StringKey.UI_EndScreen_Success_Title, "Voltas à loja com os recursos que recolheste"},
                {StringKey.UI_EndScreen_Success_CollectedItems, "RECURSOS RECOLHIDOS"},
                {StringKey.UI_EndScreen_Success_Offering, "Ao voltar, encontras o Rei, que está a pedir um tributo"},
                {StringKey.UI_EndScreen_Success_Continue, "Pressiona ENTER para continuar..."},
                {StringKey.UI_EndScreen_Failure_Title, "Foste derrotado, mas conseguiste manter alguns recursos recolhidos"},
                {StringKey.UI_EndScreen_Failure_CollectedItems, "RECURSOS RECOLHIDOS"},
                {StringKey.UI_EndScreen_Failure_Offering, "Ao voltar, encontras o Rei, que está a pedir um tributo"},
                {StringKey.UI_EndScreen_Failure_Continue, "Pressiona ENTER para continuar..."},

                {StringKey.UI_ClassSelect_Title, "Escolha a sua classe:"},

                {StringKey.UI_Oracle_Message, "O Oráculo sussura, prevendo que este recurso terá alta demanda hoje, a estas horas:"},
                {StringKey.UI_SecretDoor_Message, "Esta porta precisa de um código para abrir"},

                {StringKey.UI_NextMission_AbundantItem, "Recurso Abundante"},
                {StringKey.UI_NextMission_DiseasedItem, "Recurso Estragado"},

                {StringKey.UI_RawItems_Title, "Recursos Crús"},
                {StringKey.UI_ProcessedItems_Title, "Recursos Processados"},
                {StringKey.UI_MarketItems_Title, "Venda de Produto"},

                {StringKey.UI_AdventurerDailyQuest_Reward_Text, "Recompensa"},
                {StringKey.UI_AdventurerDailyQuest_Description, "Recolha exactamente o número especificado"},
                {StringKey.UI_AdventurerDailyQuest_Complete_Text, "Completa"},
                {StringKey.UI_AdventurerDailyQuest_Complete_Description, "$ITEMNAME$ adquirido"},

                {StringKey.Buff_Afraid_Name, "Medo"},
                {StringKey.Buff_Afraid_Description, "Ganha velocidade ao entrar numa sala"},
                {StringKey.Buff_Blessing_Name, "Benção"},
                {StringKey.Buff_Blessing_Description, "Os teus amigos são enfraquecidos"},
                {StringKey.Buff_LuckyCharm_Name, "Amuleto da Sorte"},
                {StringKey.Buff_LuckyCharm_Description, "Ganhas o dobro do recursos"},
                {StringKey.Buff_DeflectingArmor_Name, "Armadura Deflectiva"},
                {StringKey.Buff_DeflectingArmor_Description, "Chance de resistir a tiros das torres"},
                {StringKey.Buff_IronBoots_Name, "Botas de Ferro"},
                {StringKey.Buff_IronBoots_Description, "Os picos não te afectam!"},
                {StringKey.Buff_LifeSteal_Name, "Vampirismo"},
                {StringKey.Buff_LifeSteal_Description, "Ganha vida ao atacar inimigos"},
                {StringKey.Buff_Poison_Name, "Veneno"},
                {StringKey.Buff_Poison_Description, "Os teus ataques envenenam os teus inimigos"},
                {StringKey.Buff_Spread_Name, "Mais Tiros"},
                {StringKey.Buff_Spread_Description, "Dispara mais setas com o teu arco!"},
                {StringKey.Buff_UnderConstruction_Description, "ATENÇÃO: Em Construção"},
                {StringKey.Buff_Health_Description, "Mais vida"},
                {StringKey.Buff_Damage_Description, "Mais dano"},
                {StringKey.Buff_Movement_Description, "Mais velocidade"},

                {StringKey.Unlock_Buff_Altar_Name, "Altar"},
                {StringKey.Unlock_Buff_Altar_Description, "Desbloqueia o altar, oferecendo novos efeitos ao aventureiro!"},
                {StringKey.Unlock_Buff_Afraid_Name, "Medo"},
                {StringKey.Unlock_Buff_Afraid_Description, "Desbloqueia o efeito \"Medo\" - Ganha velocidade ao entrar numa sala"},
                {StringKey.Unlock_Buff_Blessing_Name, "Benção"},
                {StringKey.Unlock_Buff_Blessing_Description, "Desbloqueia o efeito \"Benção\" - Os teus amigos são enfraquecidos"},
                {StringKey.Unlock_Buff_LuckyCharm_Name, "Amuleto da Sorte"},
                {StringKey.Unlock_Buff_LuckyCharm_Description, "Desbloqueia o efeito \"Amuleto da Sorte\" - Ganhas o dobro do recursos"},
                {StringKey.Unlock_Buff_DeflectingArmor_Name, "Armadura Deflectiva"},
                {StringKey.Unlock_Buff_DeflectingArmor_Description, "Desbloqueia o efeito \"Armadura Deflectiva\" - Chance de resistir a tiros das torres"},
                {StringKey.Unlock_Buff_IronBoots_Name, "Botas de Ferro"},
                {StringKey.Unlock_Buff_IronBoots_Description, "Desbloqueia o efeito \"Botas de Ferro\" - Os picos não te afectam!"},
                {StringKey.Unlock_Buff_LifeSteal_Name, "Vampirismo"},
                {StringKey.Unlock_Buff_LifeSteal_Description, "Desbloqueia o efeito \"Vampirismo\" - Ganha vida ao atacar inimigos"},
                {StringKey.Unlock_Buff_Poison_Name, "Veneno"},
                {StringKey.Unlock_Buff_Poison_Description, "Desbloqueia o efeito \"Veneno\" - Os teus ataques envenenam os teus inimigos"},
                {StringKey.Unlock_Buff_Spread_Name, "Mais Tiros"},
                {StringKey.Unlock_Buff_Spread_Description, "Desbloqueia o efeito \"Mais Tiros\" - Dispara mais setas com o teu arco!"},

                {StringKey.Unlock_Item_RedMushroom_Name, "Cogumelo Vermelho"},
                {StringKey.Unlock_Item_RedMushroom_Description, ""},
                {StringKey.Unlock_Item_SlimMushroom_Name, "Cogumelo Fino"},
                {StringKey.Unlock_Item_SlimMushroom_Description, ""},
                {StringKey.Unlock_Item_TreeMushroom_Name, "Cogumelo Árvore"},
                {StringKey.Unlock_Item_TreeMushroom_Description, ""},
                {StringKey.Unlock_Item_AlgaeMushroom_Name, "Cogumelo Alga"},
                {StringKey.Unlock_Item_AlgaeMushroom_Description, ""},
                {StringKey.Unlock_Item_GnomeMushroom_Name, "Cogumelo Gnomo"},
                {StringKey.Unlock_Item_GnomeMushroom_Description, ""},
                {StringKey.Unlock_Item_HairyMushroom_Name, "Cogumelo Peludo"},
                {StringKey.Unlock_Item_HairyMushroom_Description, ""},
                {StringKey.Unlock_Item_ShellMushroom_Name, "Cogumelo Concha"},
                {StringKey.Unlock_Item_ShellMushroom_Description, ""},
                {StringKey.Unlock_Item_DragonMushroom_Name, "Cogumelo Dragão"},
                {StringKey.Unlock_Item_DragonMushroom_Description, ""},
                {StringKey.Unlock_Item_PurpleMushroom_Name, "Cogumelo Roxo"},
                {StringKey.Unlock_Item_PurpleMushroom_Description, ""},

                {StringKey.Unlock_Item_FoodMarket_Name, "Frescos"},
                {StringKey.Unlock_Item_FoodMarket_Description, "Desbloqueia a Carne e o Peixe!"},
                {StringKey.Unlock_Item_Groceries_Name, "Frutaria"},
                {StringKey.Unlock_Item_Groceries_Description, "Desbloqueia as Frutas!"},
                {StringKey.Unlock_Item_Jeweler_Name, "Joalheiro"},
                {StringKey.Unlock_Item_Jeweler_Description, "Desbloqueia o Ouro!"},

                {StringKey.Unlock_Stats_Cooking_Name, "Cozinhar"},
                {StringKey.Unlock_Stats_Cooking_Description, "Aumenta a vida do aventureiro"},
                {StringKey.Unlock_Stats_DancingLessons_Name, "Lições de Dança"},
                {StringKey.Unlock_Stats_DancingLessons_Description, "Aumenta a velocidade do aventureiro"},
                {StringKey.Unlock_Stats_TrainingDummy_Name, "Boneco de Treino"},
                {StringKey.Unlock_Stats_TrainingDummy_Description, "Aumenta a dano do aventureiro"},

                {StringKey.Unlock_Class_Rogue_Name, "Assassino"},
                {StringKey.Unlock_Class_Rogue_Description, "Desbloqueia uma nova classe para o aventureiro!"},
                {StringKey.Unlock_Class_Warrior_Name, "Guerreiro"},
                {StringKey.Unlock_Class_Warrior_Description, "Desbloqueia uma nova classe para o aventureiro!"},

                {StringKey.Unlock_Shop_IronWorkshop_Name, "Atelier de Ferro"},
                {StringKey.Unlock_Shop_IronWorkshop_Description, "Produz mais Ferro com o processamento"},
                {StringKey.Unlock_Shop_WoodWorkshop_Name, "Atelier de Madeira"},
                {StringKey.Unlock_Shop_WoodWorkshop_Description, "Produz mais Tábuas de Madeira com o processamento"},

                {StringKey.DiseasedItemTutorial, "Este recurso está doente e trouxe prejuízo à loja."},

            }
        }
    };

    public static void SetLanguage(Language language)
    {
        currentLanguage = language;
        PlayerPrefs.SetInt("Language", (int)language);
    }

    public static string Get(StringKey key)
    {
        return Get(key, currentLanguage);
    }
    public static string Get(StringKey key, Language language)
    {
        if (languageStringDictionary.ContainsKey(language))
        {
            if (languageStringDictionary[language].ContainsKey(key))
            {
                return languageStringDictionary[language][key];
            }
            else
            {
                Debug.LogError(string.Format("Localisation: Key {0} not found in language {1}", key, language));
            }
        }
        else
        {
            Debug.LogError("Language not found");
        }
        return "";
    }
}

public enum Language
{
    English,
    Portuguese
}

public enum StringKey
{
    // Unlocks
    Unlock_Buff_Afraid_Name,
    Unlock_Buff_Afraid_Description,
    Unlock_Buff_Blessing_Name,
    Unlock_Buff_Blessing_Description,

    Unlock_Buff_DeflectingArmor_Name,
    Unlock_Buff_DeflectingArmor_Description,
    Unlock_Buff_IronBoots_Name,
    Unlock_Buff_IronBoots_Description,
    Unlock_Buff_LifeSteal_Name,
    Unlock_Buff_LifeSteal_Description,
    Unlock_Buff_Poison_Name,
    Unlock_Buff_Poison_Description,
    Unlock_Buff_Spread_Name,
    Unlock_Buff_Spread_Description,

    Unlock_Class_Rogue_Name,
    Unlock_Class_Rogue_Description,
    Unlock_Class_Warrior_Name,
    Unlock_Class_Warrior_Description,

    Unlock_Item_AlgaeMushroom_Name,
    Unlock_Item_AlgaeMushroom_Description,
    Unlock_Item_DragonMushroom_Name,
    Unlock_Item_DragonMushroom_Description,
    Unlock_Item_GnomeMushroom_Name,
    Unlock_Item_GnomeMushroom_Description,
    Unlock_Item_HairyMushroom_Name,
    Unlock_Item_HairyMushroom_Description,
    Unlock_Item_PurpleMushroom_Name,
    Unlock_Item_PurpleMushroom_Description,
    Unlock_Item_RedMushroom_Name,
    Unlock_Item_RedMushroom_Description,
    Unlock_Item_ShellMushroom_Name,
    Unlock_Item_ShellMushroom_Description,
    Unlock_Item_SlimMushroom_Name,
    Unlock_Item_SlimMushroom_Description,
    Unlock_Item_TreeMushroom_Name,
    Unlock_Item_TreeMushroom_Description,

    Unlock_Shop_IronWorkshop_Name,
    Unlock_Shop_IronWorkshop_Description,
    Unlock_Shop_WoodWorkshop_Name,
    Unlock_Shop_WoodWorkshop_Description,

    Unlock_Stats_TrainingDummy_Name,
    Unlock_Stats_TrainingDummy_Description,
    Unlock_Stats_Cooking_Name,
    Unlock_Stats_Cooking_Description,
    Unlock_Stats_DancingLessons_Name,
    Unlock_Stats_DancingLessons_Description,

    // Buffs
    Buff_Afraid_Name,
    Buff_Afraid_Description,
    Buff_Blessing_Name,
    Buff_Blessing_Description,

    Buff_DeflectingArmor_Name,
    Buff_DeflectingArmor_Description,
    Buff_IronBoots_Name,
    Buff_IronBoots_Description,
    Buff_LifeSteal_Name,
    Buff_LifeSteal_Description,
    Buff_Poison_Name,
    Buff_Poison_Description,
    Buff_Spread_Name,
    Buff_Spread_Description,

    // Mushrooms
    Item_AlgaeMushroom_Name,
    Item_BasicMushroom_Name,
    Item_DragonMushroom_Name,
    Item_GnomeMushroom_Name,
    Item_HairyMushroom_Name,
    Item_PurpleMushroom_Name,
    Item_RedMushroom_Name,
    Item_ShellMushroom_Name,
    Item_SlimMushroom_Name,
    Item_TreeMushroom_Name,

    // Lootables
    Item_Berries_Name,
    Item_CoalOre_Name,
    Item_Fish_Name,
    Item_GoldOre_Name,
    Item_IronOre_Name,
    Item_Meat_Name,
    Item_WoodLog_Name,

    // Processed
    Item_GoldIngot_Name,
    Item_IronIngot_Name,
    Item_CoalProcessed_Name,
    Item_WoodPlank_Name,

    // Valuables
    Item_Coin_Name,
    Item_Gem_Name,

    // Classes
    Class_Ranger_Name,
    Class_Rogue_Name,
    Class_Warrior_Name,

    HUD_LootFeedback,

    UI_DailyQuestDescription,
    UI_DailyQuestReward,
    Buff_LuckyCharm_Name,
    Buff_LuckyCharm_Description,
    Unlock_Buff_LuckyCharm_Name,
    Unlock_Buff_LuckyCharm_Description,
    Unlock_Buff_Altar_Name,
    Unlock_Buff_Altar_Description,
    Unlock_Item_FoodMarket_Name,
    Unlock_Item_FoodMarket_Description,
    Unlock_Item_Groceries_Name,
    Unlock_Item_Groceries_Description,
    Unlock_Item_Jeweler_Name,
    Unlock_Item_Jeweler_Description,

    UI_EndScreen_Success_Title,
    UI_EndScreen_Success_CollectedItems,
    UI_EndScreen_Success_Offering,
    UI_EndScreen_Success_Continue,

    UI_EndScreen_Failure_Title,
    UI_EndScreen_Failure_CollectedItems,
    UI_EndScreen_Failure_Offering,
    UI_EndScreen_Failure_Continue,

    UI_Oracle_Message,

    UI_SecretDoor_Message,

    UI_NextMission_Title,
    UI_NextMission_AbundantItem,
    UI_NextMission_DiseasedItem,
    UI_NextMission_Easy,
    UI_NextMission_Medium,
    UI_NextMission_Hard,
    UI_AdventurerDailyQuest_Title,
    UI_AdventurerDailyQuest_Description,
    UI_AdventurerDailyQuest_Reward_Text,
    UI_AdventurerDailyQuest_Complete_Text,
    UI_AdventurerDailyQuest_Complete_Description,

    Item_EncryptedKey_Name,
    Item_DecryptedKey_Name,

    UI_RawItems_Title,
    UI_ProcessedItems_Title,
    UI_MarketItems_Title,

    Buff_UnderConstruction_Description,
    Buff_Health_Description,
    Buff_Damage_Description,
    Buff_Movement_Description,

    UI_ClassSelect_Title,
    DiseasedItemTutorial,
}
