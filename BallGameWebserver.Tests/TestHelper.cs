using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallGameWebserver.Tests
{
    public static class TestHelper
    {
        public static void RunWithWebserver(Action todo)
        {
            var server = FakeServerHelper.CreateFakeWebServer();

            Task.Run(() =>
            {
                todo();
                server.RequestAbort();
            });

            server.WaitForAbort();
        }
    }
}
