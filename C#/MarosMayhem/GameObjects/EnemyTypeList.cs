// Impressum
// Das Projekt "Maro's Mayhem" ist im Studiengang MultiMediaTechnology / FHS im Rahmen des MultiMediaProjekt 1 von Alija Suljic erstellt worden.
// The project "Maro's Mayhem" has been developed within the MultiMediaTechnology Bachelor Studies at the Fachhochschule Salzburg as part of the MultiMediaProject 1 by Alija Suljic in the year 2022.

using SFML.Graphics;

internal class EnemyTypeList
{
    public static Enemy GetEnemyData(EnemyTypes type)
    {
        Enemy enemy = new(0, 0, 0, new Sprite(AssetManager.Instance.textures["Eel01"]), AssetManager.Instance.textures["GreenProjectile"],  8, 1400, 1, 1, 1, 1, 1);
        switch(type)
        {
            // Health / Damage / Score / TextureSprite / ProjectileTexture / ProjectileAnimationLength 
            // Enemy Attackspeed / TilingX / TilingY / TextureScale / ColSizeX / ColSizeY / projectileSpeed? / hasProjectileEmitter?
            case EnemyTypes.Eel01:
                enemy = new Enemy(300, 200, 200, new Sprite(AssetManager.Instance.textures["Eel01"]), AssetManager.Instance.textures["GreenProjectile"], 8, 
                1400, 8, 3, 1.45f, 2f, 1f);
                break;
            case EnemyTypes.Abysswyrm01:
                enemy = new Enemy(2500, 400, 1000, new Sprite(AssetManager.Instance.textures["Abysswyrm01"]), AssetManager.Instance.textures["PurpleProjectile"], 8, 
                500, 8, 3, 1.45f, 1.1f, 1.3f);
                break;
            case EnemyTypes.Eel02:
                enemy = new Enemy(350, 250, 400, new Sprite(AssetManager.Instance.textures["Eel02"]), AssetManager.Instance.textures["GreenProjectile"], 8, 
                10000, 8, 3, 1f, 2f, 1f, 200f, false);
                break;
            case EnemyTypes.Jellyfish02:
                enemy = new Enemy(1250, 400, 700, new Sprite(AssetManager.Instance.textures["Jellyfish02"]), AssetManager.Instance.textures["GreenProjectile"], 8, 
                10000, 8, 3, 1f, 1.5f, 1f, 200f, false);
                break;
            case EnemyTypes.Shark03:
                enemy = new Enemy(1800, 500, 900, new Sprite(AssetManager.Instance.textures["Shark03"]), AssetManager.Instance.textures["OrangeProjectile"], 8, 
                750, 8, 3, 1f, 1.6f, 1.2f);
                break;
            case EnemyTypes.Cycrab03:
                enemy = new Enemy(4000, 850, 1500, new Sprite(AssetManager.Instance.textures["Cycrab03"]), AssetManager.Instance.textures["WhiteProjectile"], 8, 
                1000, 8, 3, 1f, 1f, 1.5f, 800f);
                break;
            case EnemyTypes.Anglerfish03:
                enemy = new Enemy(2000, 700, 1000, new Sprite(AssetManager.Instance.textures["Anglerfish03"]), AssetManager.Instance.textures["OrangeProjectile"], 8, 
                1000, 8, 3, 1f, 1f, 1.4f, 0f, false);
                break;
            case EnemyTypes.Jellyfish03:
                enemy = new Enemy(10000, 1000, 0, new Sprite(AssetManager.Instance.textures["Jellyfish03"]), AssetManager.Instance.textures["GreenProjectile"], 8, 
                10000, 8, 3, 1f, 1.5f, 1f, 200f, false);
                break;
        }
        return enemy;
    }
    public static Boss GetBossData(EnemyTypes bossType)
    {
        Boss boss = new(40000, 700, 20000, new Sprite(AssetManager.Instance.textures["HammerheadShark"]), AssetManager.Instance.textures["OrangeProjectile"], 8, 1500, 1.3f, 1.05f);
        switch(bossType)
        {
            // Health / Damage / Score / TextureSprite / ProjectileTexture / ProjectileAnimationLength
            // Enemy Attackspeed / ColSizeX? / ColSizeY? / TilingX? / TilingY?
            case EnemyTypes.HammerHeadShark01:
                boss = new HammerHeadShark(35000, 500, 20000, new Sprite(AssetManager.Instance.textures["HammerheadShark"]), AssetManager.Instance.textures["OrangeProjectile"], 8, 
                1500, 1.3f, 1.05f);
                return boss;
            case EnemyTypes.Sharknado02:
                boss = new Sharknado(95000, 550, 50000, new Sprite(AssetManager.Instance.textures["Sharknado"]), AssetManager.Instance.textures["PurpleProjectile"], 8, 
                1500, 1.4f, 1.1f, 8, 4);
                return boss;
            case EnemyTypes.Kraken03:
                boss = new Kraken(488888, 1200, 100000, new Sprite(AssetManager.Instance.textures["Kraken"]), AssetManager.Instance.textures["WhiteProjectile"], 8, 
                1000, 1.1f, 1.1f, 8, 8);
                return boss;
            default:
                return default(Boss);
        }
    }
    public enum EnemyTypes
    {
        Eel01,
        Abysswyrm01,
        Eel02,
        Jellyfish02,
        Shark03,
        Cycrab03,
        Anglerfish03,
        Jellyfish03,

        // BOSSES
        HammerHeadShark01,
        Sharknado02,
        Kraken03
    }
}