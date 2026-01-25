using System.Reflection;
using SystemModule;

namespace GameSvr.CommandSystem
{
    public class CommandManager
    {
        private static readonly Dictionary<string, BaseCommond> CommandMaps = new Dictionary<string, BaseCommond>(StringComparer.OrdinalIgnoreCase);
        
        
        
        private static readonly Dictionary<string, string> CustomCommands = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public CommandManager()
        {

        }

        public void RegisterCommand()
        {
            M2Share.CommandConf.LoadConfig();
            RegisterCommandGroups();
        }

        
        
        
        private void RegisterCommandGroups()
        {
            var cmdName = string.Empty;
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (!type.IsSubclassOf(typeof(BaseCommond))) continue;//只有继承BaseCommond，才能添加到命令对象中

                var attributes = (GameCommandAttribute[])type.GetCustomAttributes(typeof(GameCommandAttribute), true);
                if (attributes.Length == 0) continue;

                var groupAttribute = attributes[0];
                if (CommandMaps.ContainsKey(groupAttribute.Name))
                {
                    M2Share.ErrorMessage($"重复游戏命令: {groupAttribute.Name}");
                }

                if (CustomCommands.TryGetValue(groupAttribute.Name, out cmdName))
                {
                    groupAttribute.Command = groupAttribute.Command;
                    groupAttribute.Name = cmdName;
                }

                var commandGroup = (BaseCommond)Activator.CreateInstance(type);
                if (commandGroup == null)
                {
                    return;
                }
                MethodInfo methodInfo = null;
                foreach (var method in commandGroup.GetType().GetMethods())
                {
                    var methodAttributes = method.GetCustomAttribute(typeof(DefaultCommand), true);
                    if (methodAttributes != null)
                    {
                        methodInfo = method;
                        break;
                    }
                }
                if (methodInfo == null)
                {
                    return;
                }
                commandGroup.Register(groupAttribute, methodInfo);
                CommandMaps.Add(groupAttribute.Name, commandGroup);
            }
        }

        public void RegisterCommand(string command, string commandName)
        {
            CustomCommands.Add(command, commandName);
        }

        
        
        
        
        
        public void UpdataRegisterCommandGroups(GameCommandAttribute OldCmd, string sCmd)
        {
            if (CommandMaps.ContainsKey(OldCmd.Command))
            {
                
                
                
                
                

                
                
                
                
            }
        }

        
        
        
        
        
        
        public bool ExecCmd(string line, TPlayObject playObject)
        {
            var output = string.Empty;
            string command;
            string parameters;
            var found = false;

            if (playObject == null)
                throw new ArgumentException("PlayObject");
            if (!ExtractCommandAndParameters(line, out command, out parameters))
                return found;

            BaseCommond commond;
            if (CommandMaps.TryGetValue(command, out commond))
            {
                output = commond.Handle(parameters, playObject);
                found = true;
            }

            if (!found)
            {
                output = $"未知命令: {command} {parameters}";
            }

            
            if (!string.IsNullOrEmpty(output))
            {
                playObject.SysMsg(output, MsgColor.Red, MsgType.Hint);
            }
            return found;
        }

        public void ExecCmd(string line)
        {
            var output = string.Empty;
            string command;
            string parameters;
            var found = false;

            if (!ExtractCommandAndParameters(line, out command, out parameters))
                return;

            BaseCommond commond = null;
            if (CommandMaps.TryGetValue(command, out commond))
            {
                output = commond.Handle(parameters);
                found = true;
            }

            if (!found)
            {
                output = $"未知命令: {command} {parameters}";
            }
        }

        
        
        
        
        
        
        
        private bool ExtractCommandAndParameters(string line, out string command, out string parameters)
        {
            line = line.Trim();
            command = string.Empty;
            parameters = string.Empty;

            if (line == string.Empty)
                return false;

            if (line[0] != '@') 
                return false;

            line = line.Substring(1);
            command = line.Split(' ')[0]; 
            parameters = string.Empty;
            if (line.Contains(' ')) parameters = line.Substring(line.IndexOf(' ') + 1).Trim(); 
            return true;
        }

        [GameCommand("commands", "列出可用的命令")]
        public class CommandsCommandGroup : BaseCommond
        {
            public override string Fallback(string[] parameters = null, TPlayObject PlayObject = null)
            {
                var output = "Available commands: ";
                var commandList = CommandMaps.Values.ToList();
                foreach (var pair in commandList)
                {
                    if (PlayObject != null && pair.GameCommand.nPermissionMin > PlayObject.m_btPermission) continue;
                    output += pair.GameCommand.Name + ", ";
                }
                output = output.Substring(0, output.Length - 2) + ".";
                return output + "\nType 'help <command>' to get help.";
            }
        }

        [GameCommand("help", "帮助命令")]
        public class HelpCommandGroup : BaseCommond
        {
            public override string Fallback(string[] parameters = null, TPlayObject PlayObject = null)
            {
                return "usage: help <command>";
            }

            public override string Handle(string parameters, TPlayObject PlayObject = null)
            {
                if (parameters == string.Empty)
                    return this.Fallback();
                var @params = parameters.Split(' ');
                var group = @params[0];
                var command = @params.Count() > 1 ? @params[1] : string.Empty;
                var output = $"Unknown command: {group} {command}";
                return output;
            }
        }
    }
}