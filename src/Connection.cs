using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using Bearded.Utilities.SpaceTime;
using Newtonsoft.Json;
using WebSocketSharp;

namespace Game
{
    class Connection
    {
        GameState game;

        WebSocket socket;

        ConcurrentQueue<AgentUpdate> updateQueue = new ConcurrentQueue<AgentUpdate>();

        Dictionary<string, Agent> agents = new Dictionary<string, Agent>();

        Instant reconnectTime;

        bool isOpen;


        class AgentUpdate
        {
            public string Device { get; set; }
            public float X { get; set; }
            public float Y { get; set; }

            public override string ToString()
            {
                return string.Format("[AgentUpdate: Device={0}, X={1}, Y={2}]", Device, X, Y);
            }
        }


        public Connection(GameState gameState)
        {
            this.game = gameState;
            this.socket = new WebSocket("ws://tc.stfu.ee:8000/fetch");

            this.socket.OnOpen += (s, e) => { Console.WriteLine("websocket opened"); this.isOpen = true; };
            this.socket.OnClose += (s, e) => { Console.WriteLine($"websocket closed: {e.Reason}"); this.isOpen = false; };
            this.socket.OnError += (s, e) => Console.WriteLine($"websocket error: {e.Exception}, {e.Message}");
            this.socket.OnMessage += this.onMessage;
        }

        void onMessage(object sender, MessageEventArgs e)
        {
            var s = new JsonSerializer();

            Console.WriteLine("updates received:");

            var updates = s.Deserialize<List<AgentUpdate>>(new JsonTextReader(new StringReader(e.Data)));

            foreach (var update in updates)
            {
                this.updateQueue.Enqueue(update);
                Console.WriteLine(update);
            }
        }

        public void Update()
        {
            if (this.isOpen)
            {
                this.reconnectTime = this.game.Time + new Bearded.Utilities.SpaceTime.TimeSpan(5);
            }

            if (this.game.Time >= this.reconnectTime)
            {
                Console.WriteLine("Trying to connect..");
                socket.Connect();
                this.reconnectTime = this.game.Time + new Bearded.Utilities.SpaceTime.TimeSpan(2);
            }

            this.updateQueue.Enqueue(new AgentUpdate { Device = "derp", X = 0, Y = 0 });
            this.updateQueue.Enqueue(new AgentUpdate { Device = "derp2", X = 0, Y = 8 });
            this.updateQueue.Enqueue(new AgentUpdate { Device = "derp3", X = 6, Y = 0 });


            AgentUpdate update;
            while (this.updateQueue.TryDequeue(out update))
            {
                this.apply(update);
            }
        }

        private void apply(AgentUpdate update)
        {
            const float xSize = 8;
            const float ySize = 6;

            var p = new Position2(update.Y / (xSize / 1.8f) - 0.9f, update.X / (xSize / 1.8f) - 0.9f);
            Agent agent;
            if (!this.agents.TryGetValue(update.Device, out agent) || agent.Deleted)
            {
                agent = new Agent(this.game, p, update.Device);
                this.agents[update.Device] = agent;
            }

            agent.SetGoal(p);
            agent.SetDeletionTime(this.game.Time + new Bearded.Utilities.SpaceTime.TimeSpan(10));
        }
   }
}