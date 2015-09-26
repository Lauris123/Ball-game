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
            bool isAborted = false;

            WebServerComponent server = FakeServerHelper.CreateFakeWebServer();

            // sūtīsim ar WebClient tur datus un pārbaudīsim atbildes

            //Thread t1 = new Thread(new ThreadStart(() =>
            //{
                //server.RunUntilStoped();
            //}));

            Task.Run(() =>
            {
                var wc = new WebClient();

                string s = wc.DownloadString("http://localhost:20160/spele/?client=1");

                Assert.AreEqual(s, "nop");

                isAborted = true;
            });

            //t1.Start();

            while (!isAborted)
            {
                Thread.Sleep(200);
            }

           // t1.Abort();

            server.Stop();
        }


    }
}
