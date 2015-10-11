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

                //var wc = new WebClient();

                //string s = wc.DownloadString("http://localhost:20160/spele/?client=1");

                //Assert.AreEqual(s, "nop");

                var comp = new ClientComponent();

                comp.RecievedEmpty += () => {
                    isAborted = true;

                    return;
                };

                comp.Connect();

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
                var firstClient = new WebClient();
                var secondClient = new WebClient();

                string firstReply = firstClient.DownloadString("http://localhost:20160/spele/?client=1");
                string secondReply = secondClient.DownloadString("http://localhost:20160/spele/?client=2");

                Assert.AreEqual(firstReply, "nop");
                Assert.AreEqual(secondReply, "nop");

                firstReply = firstClient.DownloadString("http://localhost:20160/spele/?client=1");
                secondReply = secondClient.DownloadString("http://localhost:20160/spele/?client=2");

                Assert.AreEqual(firstReply, Constants.PROTOCOL_GAME_START);
                Assert.AreEqual(secondReply, Constants.PROTOCOL_GAME_START);
            });
        }
    }
}
