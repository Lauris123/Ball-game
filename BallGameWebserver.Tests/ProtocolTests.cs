using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiniWebserver;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using BallGame.Common;

namespace BallGameWebserver.Tests
{
    [TestClass]
    public class ProtocolTests
    {

        /// <summary>
        /// Pieslēgs vienu vienīgu jaunu spēlētāju un paprasīs kas jauns,
        /// serveris atgriezīs nop
        /// </summary>
        [TestMethod]
        public void ConnectSingleClientCheckReturnData()
        {
            TestHelper.RunWithWebserver(() => {
                bool isAborted = false;

                var comp = new ClientComponent();

                comp.RecievedEmpty += () => {
                    isAborted = true;

                    return;
                };

                //comp.Connect();

                Task.Run(() => comp.Connect());

                while (!isAborted)
                {
                    Thread.Sleep(50);
                }
            });
        }

        /// <summary>
        /// Pieslēdz divus klients
        /// </summary>
        [TestMethod]
        public void ConnectTwoClients()
        {
            TestHelper.RunWithWebserver(() =>
            {
                bool isFirstRecieved = false;
                bool isSecondRecieved = false;
                bool isFirstGameStart = false;
                bool isSecondGameStart = false;

                var client1 = new ClientComponent();
                var client2 = new ClientComponent();

                client1.RecievedEmpty += () =>
                {
                    isFirstRecieved = true;
                };

                client2.RecievedEmpty += () =>
                {
                    isSecondRecieved = true;
                };

                client1.BeginGame += () => {
                    isFirstGameStart = true;
                };

                client2.BeginGame += () =>
                {
                    isSecondGameStart = true;
                };

                Task.Run(() => {
                    client1.Connect();
                });

                Task.Run(() =>
                {
                    client2.Connect();
                });

                while (!(isFirstRecieved && isSecondRecieved))
                {
                    Thread.Sleep(50);
                }

                while (!(isFirstGameStart && isSecondGameStart))
                {
                    Thread.Sleep(50);
                }
            });
        }

        /// <summary>
        /// Viens šauj otrs kustās
        /// </summary>
        [TestMethod]
        public void TwoClientsOneMoveOtherShoot()
        {
            TestHelper.RunWithWebserver(() =>
            {
                bool isFirstRecieved = false;
                bool isSecondRecieved = false;
                bool isSecondReviecedEnemyMoved = false;

                var client1 = new ClientComponent();
                var client2 = new ClientComponent();

                client1.EnemyMove += (moveType) =>
                {
                    if (moveType == MoveType.MoveRight)
                    {
                        isFirstRecieved = true;
                    }
                };

                client2.EnemyShot += () =>
                {
                    isSecondRecieved = true;
                };

                client2.EnemyMove += (moveType) =>
                {
                    if (moveType == MoveType.MoveLeft)
                        isSecondReviecedEnemyMoved = true;
                };

                Task.Run(() =>
                {
                    client1.Connect();
                    Thread.Sleep(50);
                    client1.Shoot();
                    Thread.Sleep(50);
                    client1.Move(MoveType.MoveLeft);
                });

                Task.Run(() =>
                {
                    client2.Connect();
                    Thread.Sleep(50);
                    client2.Move(MoveType.MoveRight);
                });

                while (!(isFirstRecieved && isSecondRecieved && isSecondReviecedEnemyMoved))
                {
                    Thread.Sleep(50);
                }
            });
        }
    }
}
