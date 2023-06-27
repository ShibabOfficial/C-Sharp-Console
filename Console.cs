using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Console
{
    public class ConsoleDriver
    {
        public void write(object msg)
        {
            System.Console.Write(msg.ToString());
        }

        public void writeLine(object msg)
        {
            System.Console.WriteLine(msg.ToString());
        }

        public string readLine()
        {
            return System.Console.ReadLine();
        }

        public System.ConsoleKeyInfo readKey()
        {
            return System.Console.ReadKey();
        }

        public void close()
        {
            Environment.Exit(0);
        }

        public void close(int exitCode)
        {
            Environment.Exit(exitCode);
        }

        public void clear()
        {
            System.Console.Clear();
        }
    }

    public class NetworkingDriver
    {
        public string[] getAllIps(string hostName)
        {
            List<string> ips = new List<string>();

            foreach (IPAddress address in Dns.GetHostAddresses(hostName))
                ips.Add($"{address.AddressFamily} {address}");

            string[] ipsA = ips.ToArray();

            return ipsA;
        }

        public string getHostName()
        {
            return Dns.GetHostName();
        }

        public IPHostEntry getHostEntry(string entry)
        {
            return Dns.GetHostEntry(entry);
        }
    }

    public class Program
    {
        // -> Defining console driver //

        private static ConsoleDriver console = new ConsoleDriver();

        // -> Defining network driver //

        private static NetworkingDriver net = new NetworkingDriver();

        // -> Data struct //

        private class Data
        {
            public int cmdNL = 1;
        }

        private static Data data = new Data();

        // -> Command struct //
        private class Command
        {
            public string name;
            public string des;
            public Action<string[]> exec;

            public Command(string name, string des, Action<string[]> exec)
            {
                this.name = name.Replace(" ", "-");
                this.des = des;
                this.exec = exec;
            }
        }

        // -> Command list //

        private static List<Command> commands = new List<Command> {
            new Command("help", "shows all avaiable commands", (args) => {
                foreach (Command cmd in commands)
                {
                    string space = new StringBuilder().Insert(0, " ", data.cmdNL - cmd.name.Length).ToString();
                    console.writeLine($" > {cmd.name}{space} | {cmd.des}");
                }
            }),
            new Command("exit", "exits console", (args) => {
                console.write("Goodbye !");
                console.close();
            }),
            new Command("clear", "clears console", (args) => {
                console.clear();
            }),
            new Command("say", "prints something onto the console", (args) => {
                console.writeLine(string.Join(" ", args));
            }),

            /*new Command("command", "basic command", (args) => {
                console.write("command");
            }),*/
        };

        static void Main(string[] args)
        {
            console.writeLine("Welcome ! \n");

            foreach (Command cmd in commands)
                if (cmd.name.Length > data.cmdNL)
                    data.cmdNL = cmd.name.Length;

            // -> Starting main loop //

            MainLoop();
        }

        static void MainLoop()
        {
            // -> Executing command //

            console.write("<console>:~$ ");

            Exec(console.readLine());

            MainLoop();
        }

        static int Exec(string inp)
        {
            string cmdN = inp.Split(" ")[0];
            string[] args = inp.Replace(cmdN, "").Trim().Split(" ");

            foreach (Command cmd in commands)
            {
                if (cmd.name == cmdN)
                {
                    cmd.exec(args);

                    return 0;
                }
            }

            console.writeLine("This command doesnt exist!\nEnter 'help' for command list.");

            return 1;
        }
    }
}
