using System.Threading.Tasks;

namespace GamemodesServer.Gamemodes
{
    public abstract class GamemodeScript : GmScript
    {
        public GamemodeScript(string _name, string _eventName, int _timerSeconds)
        {
            Name = _name;
            EventName = _eventName;
            TimerSeconds = _timerSeconds;

            GamemodeManager.RegisterGamemode(this);
        }

        public string Name { get; private set; }
        public string EventName { get; private set; }
        public int TimerSeconds { get; private set; }

        public abstract Task OnStart();

        public abstract Task OnTick();

        public abstract void OnTimerUp();

        protected void Stop()
        {
            GamemodeManager.StopGamemode();
        }
    }
}
