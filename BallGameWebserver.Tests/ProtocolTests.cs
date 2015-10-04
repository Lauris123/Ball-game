using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiniWebserver;
using System.Threading.Tasks;
using System.Net;
using System.Threading;

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
                var wc = new WebClient();

                string s = wc.DownloadString("http://localhost:20160/spele/?client=1");

                Assert.AreEqual(s, "nop");
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

                Assert.AreEqual(firstReply, WebServerComponent.PROTOCOL_GAME_START);
                Assert.AreEqual(secondReply, WebServerComponent.PROTOCOL_GAME_START);
            });
        }
    }
}
