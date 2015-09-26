using MiniWebserver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallGameWebserver.Tests
{
    internal static class FakeServerHelper
    {
        public static WebServerComponent CreateFakeWebServer()
        {
            return new WebServerComponent();
        }
    }
}
