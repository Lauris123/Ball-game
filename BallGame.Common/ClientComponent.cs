using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BallGame.Common
{
    public class ClientComponent : IClientComponent
    {
        private const string SERVER = "http://localhost:20160/spele/";

        private readonly string _requestPath;

        private WebClient _wc;

        public ClientComponent()
        {
            this._wc = new WebClient();
            Random rnd = new Random();
            int num = rnd.Next(1, 2000000);
            this._requestPath = SERVER + "?client=" + num;
        }

        public event Action BeginGame;

        public event Action EnemyShot;

        public event Action RecievedEmpty;

        public event Action<MoveType> EnemyMove;

        public async void Connect()
        {
            while (true)
            {
                string reply = this._wc.DownloadString(this._requestPath);

                if (reply == Constants.EMPTY_VALUE)
                {
                    // saņemām tukšu atbildi -> nekas nenotikā
                    if (RecievedEmpty != null)
                        RecievedEmpty();
                }
                else if (reply == Constants.PROTOCOL_YOU_SHOOT)
                {
                    if (EnemyShot != null)
                    {
                        EnemyShot();
                    }
                }
                else if (reply == Constants.PROTOCOL_YOU_MOVE_LEFT)
                {
                    if (EnemyMove != null)
                    {
                        EnemyMove(MoveType.MoveLeft);
                    }
                }
                else if (reply == Constants.PROTOCOL_YOU_MOVE_RIGHT)
                {
                    if (EnemyMove != null)
                    {
                        EnemyMove(MoveType.MoveRight);
                    }
                }
                else if (reply == Constants.PROTOCOL_GAME_START)
                {
                    if (BeginGame != null)
                    {
                        BeginGame();
                    }
                }

                await Task.Delay(50);
            }
        }

        public void Shoot()
        {
            this._wc.DownloadString(this._requestPath + "&msg=" + Constants.PROTOCOL_I_SHOOT);
        }

        public void Move(MoveType type)
        {
            // pakustējies pa kreisi
            if (type == MoveType.MoveLeft)
            {
                this._wc.DownloadString(this._requestPath + "&msg=" + Constants.PROTOCOL_I_MOVE_LEFT);
            }
            // pakustējies pa labi
            if (type == MoveType.MoveRight)
            {
                this._wc.DownloadString(this._requestPath + "&msg=" + Constants.PROTOCOL_I_MOVE_RIGHT);
            }
        }
    }
}
