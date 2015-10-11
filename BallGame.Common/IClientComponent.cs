using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallGame.Common
{
    public enum MoveType 
    { 
        MoveLeft = 0,
        MoveRight = 1
    };

    public interface IClientComponent
    {
        /// <summary>
        /// Notikums kurš izsaucas kad spēle sākas
        /// </summary>
        event Action BeginGame;

        /// <summary>
        /// Notikums kad pretinieks izšāva
        /// </summary>
        event Action EnemyShot;

        /// <summary>
        /// Notikums kad pretinieks pakustējas, parametrs ir uz kuru pusi
        /// </summary>
        event Action<MoveType> EnemyMove;

        /// <summary>
        /// Klients izsauc vienreiz pirmo reizi (par atkārtou spēli nav padomāts)
        /// </summary>
        void Connect();

        /// <summary>
        /// Izšaut
        /// </summary>
        void Shoot();

        /// <summary>
        /// Pakustēties
        /// </summary>
        void Move(MoveType type);
        
    }
}
