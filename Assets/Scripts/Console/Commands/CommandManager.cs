using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.Console.Commands
{
    public class CommandManager
    {
        private Dictionary<string, ICommand> _cmdTable = new Dictionary<string, ICommand>();

        public void RegisterCommandCallback(string commandString, ICommand command)
        {
#if !UNITY_FLASH
            _cmdTable[commandString.ToLower()] = command;
#endif
        }

        public void UnRegisterCommandCallback(string commandString)
        {
            _cmdTable.Remove(commandString.ToLower());
        }

        public bool Contains(string command)
        {
            return _cmdTable.ContainsKey(command);
        }

        public ICommand this[string name] 
        {
            get
            {
                return _cmdTable[name];
            }
        }

        public IEnumerable<string> CommandNames
        {
            get
            {
                return _cmdTable.Keys;
            }
        }

        public void RegisterDefaults()
        {
            RegisterCommandCallback("sys", new SysCommand());
            RegisterCommandCallback("/?", new Command(CmdHelp));
        }

        private string CmdHelp(params string[] args)
        {
            var output = new StringBuilder();

            output.AppendLine(":: Command List ::");

            foreach (string key in CommandNames)
            {
                output.AppendLine(key);
            }

            output.AppendLine(" ");

            return output.ToString();
        }

    

    }
}
