using Rage;
using Rage.Native;

namespace RAGENativeUI.Elements
{
    public class BigMessageHandler
    {
        private Scaleform _sc;
        private uint _start;
        private int _timer;

        public BigMessageHandler()
        {
        }

        public BigMessageHandler(bool load)
        {
            if(load)
                Load(true);
        }

        public bool IsLoaded
        {
            get
            {
                return _sc == null || NativeFunction.Natives.HasScaleformMovieLoaded<bool>(_sc.Handle);
            }
        }

        public void Load(bool waitToLoad)
        {
            if (_sc != null) return;
            _sc = new Scaleform(0);
            _sc.Load("MP_BIG_MESSAGE_FREEMODE");

            if (waitToLoad)
            {
                var timeout = 1000;
                var start = System.DateTime.Now;
                while (!NativeFunction.Natives.HasScaleformMovieLoaded<bool>(_sc.Handle) && System.DateTime.Now.Subtract(start).TotalMilliseconds < timeout)
                    GameFiber.Yield();
            }
        }

        public void Dispose()
        {
            unsafe
            {
                int handle = _sc.Handle;
                NativeFunction.Natives.SetScaleformMovieAsNoLongerNeeded(ref handle);
            }
            _sc = null;
        }

        public void ShowMissionPassedMessage(string msg, int time = 5000)
        {
            Load(true);
            _start = Game.GameTime;
            _sc.CallFunction("SHOW_MISSION_PASSED_MESSAGE", msg, "", 100, true, 0, true);
            _timer = time;
        }

        public void ShowColoredShard(string msg, string desc, HudColor textColor, HudColor bgColor, int time = 5000)
        {
            Load(true);
            _start = Game.GameTime;
            _sc.CallFunction("SHOW_SHARD_CENTERED_MP_MESSAGE", msg, desc, (int)bgColor, (int)textColor);
            _timer = time;
        }

        public void ShowOldMessage(string msg, int time = 5000)
        {
            Load(true);
            _start = Game.GameTime;
            _sc.CallFunction("SHOW_MISSION_PASSED_MESSAGE", msg);
            _timer = time;
        }

        public void ShowSimpleShard(string title, string subtitle, int time = 5000)
        {
            Load(true);
            _start = Game.GameTime;
            _sc.CallFunction("SHOW_SHARD_CREW_RANKUP_MP_MESSAGE", title, subtitle);
            _timer = time;
        }

        public void ShowRankupMessage(string msg, string subtitle, int rank, int time = 5000)
        {
            Load(true);
            _start = Game.GameTime;
            _sc.CallFunction("SHOW_BIG_MP_MESSAGE", msg, subtitle, rank, "", "");
            _timer = time;
        }

        public void ShowWeaponPurchasedMessage(string bigMessage, string weaponName, WeaponHash weapon, int time = 5000)
        {
            Load(true);
            _start = Game.GameTime;
            _sc.CallFunction("SHOW_WEAPON_PURCHASED", bigMessage, weaponName, unchecked((int)weapon), "", 100);
            _timer = time;
        }

        public void ShowMpMessageLarge(string msg, int time = 5000)
        {
            Load(true);
            _start = Game.GameTime;
            _sc.CallFunction("SHOW_CENTERED_MP_MESSAGE_LARGE", msg, "test", 100, true, 100);
            _sc.CallFunction("TRANSITION_IN");
            _timer = time;
        }

        public void ShowCustomShard(string funcName, params object[] paremeters)
        {
            Load(true);
            _sc.CallFunction(funcName, paremeters);
        }

        internal void Update()
        {
            if (_sc == null) return;
            _sc.Render2D();
            if (_start != 0 && Game.GameTime - _start > _timer)
            {
                _sc.CallFunction("TRANSITION_OUT");
                _start = 0;
                //Dispose();
            }
        }
    }

    public class BigMessageThread
    {
        public BigMessageHandler MessageInstance { get; set; }
        public GameFiber Fiber;

        public BigMessageThread() : this(false)
        {
        }

        public BigMessageThread(bool loadInstance)
        {
            MessageInstance = new BigMessageHandler(loadInstance);

            Fiber = GameFiber.StartNew(() =>
            {
                while (true)
                {
                    GameFiber.Yield();
                    MessageInstance.Update();
                }
            });
        }
    }
}

