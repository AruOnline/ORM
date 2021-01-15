using System;
using ORM_DEV.Framework;
using ORM_DEV.Framework.Attributes;
using ORM_DEV.Framework.Entities;

namespace ORM_DEV.Entities
{
    [Table("users")]
    public class User : Entity<User>
    {
        public string Name { get; set; }

        public Player CurrentPlayer;
        public long P1 {
            get => CurrentPlayer.Id;
            set => CurrentPlayer = Player.Get(value);
        }
    }
}