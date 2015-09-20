using System;
using System.Collections;
using System.Text;
using System.Net;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

namespace MiniWebserver
{
    class Program
    {
        #region Constants

        private const string EMPTY_VALUE = "nop";
        private const int MAX_PLAYER_COUNT = 2;

        private const string PROTOCOL_GAME_START = "start"; // TODO: kurš spēlētājs tu esi (labais|kreisais)

        #endregion

        #region Members


        private static int _playersConnected = 0;

        // klienta nosaukums - datu rinda
        private static Dictionary<string, Queue<string>> _clientQueues = new Dictionary<string, Queue<string>>();

        #endregion

        #region Events

        private static event EventHandler PlayerConnected;

        #endregion

        static void Main(string[] args)
        {
            PlayerConnected += PlayerConnected_Recieved;

            WebServer ws = new WebServer(SendResponse, "http://localhost:20160/spele/");
            ws.Run();
            Console.WriteLine("A simple webserver. Press a key to quit.");
            Console.ReadKey();
            ws.Stop();
        }

        #region Private methods


        internal static void PlayerConnected_Recieved(object sender, EventArgs e)
        {
            _playersConnected++;

            if (_playersConnected == MAX_PLAYER_COUNT)
            {
                foreach(KeyValuePair<string, Queue<string>> item in _clientQueues)
                {
                    item.Value.Enqueue(PROTOCOL_GAME_START);
                }
            }
        }


        #endregion

        #region Public methods

        public static string SendResponse(HttpListenerRequest request)
        {
            // NOTIKUMS: http://localhost:20160/spele/ ?client=[skaitlis]&msg=[msg]
            // VAICĀJUMS: http://localhost:20160/spele/ ?client=[skaitlis]

            // atrod kurš klients ar klienta numuru
            string sender = request.QueryString["client"];

            // vai ir atsūtīta ziņā
            if (request.QueryString.AllKeys.Contains("msg"))
            {
                // KĀDS KLIENTS NOSŪTIJA KĀDU NOTIKUMU

                // ir pienācis jauns ziņojums, tas ir jaieliek visu citu izņemot šī sūtītajā datu rindā

                foreach (KeyValuePair<string, Queue<string>> item in _clientQueues)
                {
                    if (item.Key != sender)
                    {
                        // pievienojam jaunu klucītī
                        item.Value.Enqueue(request.QueryString["msg"]);
                    }
                }
            }
            else // KĀDS KLIENTS PAJAUTĀJA KAS JAUNS?
            {
                // pārbaudam vai šīs jautātājs ir jau piereģistrēts
                if (_clientQueues.ContainsKey(sender))
                {
                    // cik daudz kluči kuri nav sūtīt, ja ir lielāks par nulli
                    if (_clientQueues[sender].Count() > 0)
                    {
                        // izņem no rindas pirmo kluci
                        return _clientQueues[sender].Dequeue();
                    }
                }
                else
                {
                    // pievieno jaunu dirsēju
                    if (_playersConnected < MAX_PLAYER_COUNT)
                    {
                        _clientQueues[sender] = new Queue<string>();
                        PlayerConnected.Invoke(null, null);
                    }
                }
            }

            return EMPTY_VALUE;
        }

        #endregion

    }

    public class WebServer
    {
        private readonly HttpListener _listener = new HttpListener();
        private readonly Func<HttpListenerRequest, string> _responderMethod;

        public WebServer(string[] prefixes, Func<HttpListenerRequest, string> method)
        {
            if (!HttpListener.IsSupported)
                throw new NotSupportedException(
                    "Needs Windows XP SP2, Server 2003 or later.");

            // URI prefixes are required, for example 
            // "http://localhost:8080/index/".
            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException("prefixes");

            // A responder method is required
            if (method == null)
                throw new ArgumentException("method");

            foreach (string s in prefixes)
                _listener.Prefixes.Add(s);

            _responderMethod = method;
            _listener.Start();
        }

        public WebServer(Func<HttpListenerRequest, string> method, params string[] prefixes)
            : this(prefixes, method)
        {
        
        }

        public void Run()
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                Console.WriteLine("Webserver running...");
                try
                {
                    while (_listener.IsListening)
                    {
                        ThreadPool.QueueUserWorkItem((c) =>
                        {
                            var ctx = c as HttpListenerContext;
                            try
                            {
                                string rstr = _responderMethod(ctx.Request);
                                byte[] buf = Encoding.UTF8.GetBytes(rstr);
                                ctx.Response.ContentLength64 = buf.Length;
                                ctx.Response.OutputStream.Write(buf, 0, buf.Length);
                            }
                            catch
                            {
                            
                            } // suppress any exceptions
                            finally
                            {
                                // always close the stream
                                ctx.Response.OutputStream.Close();
                            }
                        }, _listener.GetContext());
                    }
                }
                catch
                { 
                
                } // suppress any exceptions
            });
        }

        public void Stop()
        {
            _listener.Stop();
            _listener.Close();
        }
    }
}


