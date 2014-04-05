using System;

namespace Assets.Scripts.Console.Commands
{
    public interface ICommand
    {
        string Execute(params string[] args);
    }

    public class Command : ICommand
    {
        private readonly Func<string[], string> _functor;
        public Command(Func<string[], string> functor)
        {
            _functor = functor;

        }
        public string Execute(params string[] args)
        {
            return _functor.Invoke(args);
        }
    }
}
