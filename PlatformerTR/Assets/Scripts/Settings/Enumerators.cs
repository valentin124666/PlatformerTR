namespace Settings
{
    public class Enumerators
    {
        public enum AppState
        {
            Unknown,

            AppStart,
            InGame,
        }

        public enum NamePrefabAddressable : short
        {
            HeroKnight = 0,
            Level = 1,

            EnemyGolem = 3,
            //Ui
            MainMenu = 50 
        }

        public enum SpawnEnemy : short
        {
            EnemyGolem = 3
        }
        
        public enum EntityType
        {
            Player,
            Enemy
        }

        public enum DirectionMove
        {
            Undirection,

            Right,
            Left
        }
    }
}