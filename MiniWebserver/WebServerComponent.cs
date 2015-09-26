using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MiniWebserver
{
    public class WebServerComponent
    {
        #region Constants

        private const int MAX_PLAYER_COUNT = 2;

        private const string PROTOCOL_GAME_START = "start"; // TODO: kurš spēlētājs tu esi (labais|kreisais)

        private static string EMPTY_VALUE
        {
            get
            {
                return BallGame.Common.Constants.EMPTY_VALUE;
            }
        }

        #endregion

        #region Members

        static WebServer _ws;

        private int _playersConnected = 0;

        // klienta nosaukums - datu rinda
        private Dictionary<string, Queue<string>> _clientQueues = new Dictionary<string, Queue<string>>();

        #endregion

        #region Events

        private event EventHandler PlayerConnected;

        #endregion

        public WebServerComponent()
        {
            PlayerConnected += PlayerConnected_Recieved;

            _ws = new WebServer(SendResponse, "http://localhost:20160/spele/");
            _ws.Run();
            Console.WriteLine("A simple webserver. Press a key to quit.");
            
            
        }

       

        #region Private methods

        internal void PlayerConnected_Recieved(object sender, EventArgs e)
        {
            _playersConnected++;

            if (_playersConnected == MAX_PLAYER_COUNT)
            {
                foreach (KeyValuePair<string, Queue<string>> item in _clientQueues)
                {
                    item.Value.Enqueue(PROTOCOL_GAME_START);
                }
            }
        }

        #endregion

        #region Public methods

        public void RunUntilStoped()
        {
            Console.ReadKey();
        }

        public string SendResponse(HttpListenerRequest request)
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

        public void Stop()
        {
            _ws.Stop();
        }

        #endregion

    }
}
