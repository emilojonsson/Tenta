namespace Tenta
{
    public class Todo
    {
        public static List<TodoItem> list = new List<TodoItem>();

        public const int Active = 1;
        public const int Waiting = 2;
        public const int Ready = 3;
        public static string StatusToString(int status)
        {
            switch (status)
            {
                case Active: return "aktiv";
                case Waiting: return "väntande";
                case Ready: return "avklarad";
                default: return "(felaktig)";
            }
        }
        public class TodoItem
        {
            public int status;
            public int priority;
            public string task;
            public string taskDescription;
            public TodoItem(int priority, string task) //konstruktor 1
            {
                this.status = Active;
                this.priority = priority;
                this.task = task;
                this.taskDescription = "";
            }
            public TodoItem(string todoLine) //konstruktor 2
            {
                string[] field = todoLine.Split('|');
                status = int.Parse(field[0]);
                priority = int.Parse(field[1]);
                task = field[2];
                taskDescription = field[3];
            }
            public void Print(bool verbose = false) //printar uppgifterna
            {
                string statusString = StatusToString(status);
                Console.Write($"|{statusString,-12}|{priority,-6}|{task,-20}|");
                if (verbose)
                    Console.WriteLine($"{taskDescription,-40}|");
                else
                    Console.WriteLine();
            }
        }
        public static void SaveListToFile(string saveToFile, string latestReadFile)
        {
            if (latestReadFile != string.Empty)
            {
                using (StreamWriter sw = new StreamWriter(saveToFile))
                {
                    int numRead = 0;
                    foreach (TodoItem item in list)
                    {
                        sw.WriteLine($"{item.status}|{item.priority}|{item.task}|{item.taskDescription}");
                        numRead++;
                    }
                    Console.WriteLine($"Sparar till {saveToFile} ... Sparade {numRead} uppgifter.");
                }
            }
            else
            {
                Console.WriteLine("Du kan inte spara innan du ens laddat en fil");
            } 

        }
        public static void ReadListFromFile(string readFromFile) //Läser från fil och skapar objekt till en lista
        {
            Console.Write($"Läser från fil {readFromFile} ... ");
            StreamReader sr = new StreamReader(readFromFile);
            int numRead = 0;

            list.Clear();
            string line;
            while ((line = sr.ReadLine()) != string.Empty)
            {
                TodoItem item = new TodoItem(line);
                list.Add(item);
                numRead++;
            }
            sr.Close();
            Console.WriteLine($"Läste {numRead} rader.");
        }
        private static void PrintHeadOrFoot(bool head, bool verbose)
        {
            if (head)
            {
                Console.Write("|status      |prio  |namn                |");
                if (verbose) Console.WriteLine("beskrivning                             |");
                else Console.WriteLine();
            }
            Console.Write("|------------|------|--------------------|");
            if (verbose) Console.WriteLine("----------------------------------------|");
            else Console.WriteLine();
        }
        private static void PrintHead(bool verbose) //används bara för att särskilja på om det är head eller foot
        {
            PrintHeadOrFoot(head: true, verbose);
        }
        private static void PrintFoot(bool verbose) //används bara för att särskilja på om det är head eller foot
        {
            PrintHeadOrFoot(head: false, verbose);
        }
        public static void PrintTodoList(bool verbose = false) //övergripande metod som skriver ut listan
        {
            PrintHead(verbose);
            foreach (TodoItem item in list)
            {
                item.Print(verbose);
            }
            PrintFoot(verbose);
        }
        public static void PrintHelp()
        {
            Console.WriteLine("Kommandon:");
            Console.WriteLine("hjälp                lista denna hjälp");
            Console.WriteLine("ladda                ladda listan todo.lis");
            Console.WriteLine("ladda /fil/          ladda filen fil");
            Console.WriteLine("spara                spara uppgifterna");
            Console.WriteLine("spara /fil/          spara uppgifterna på filen /fil/");
            Console.WriteLine("sluta                spara senast laddade filen och avsluta programmet!");
            Console.WriteLine("beskriv              lista alla Active uppgifter, status, prioritet, namn och beskrivning");
            Console.WriteLine("beskriv allt         lista alla uppgifter (oavsett status), status, prioritet, namn och beskrivning");
            Console.WriteLine("lista                lista alla Active uppgifter, status, prioritet, och namn på uppgiften");
            Console.WriteLine("lista allt           lista alla uppgifter (oavsett status), status, prioritet, och namn på uppgiften");
            Console.WriteLine("lista väntande       listar alla väntande uppgifter");
            Console.WriteLine("lista klara          listar alla klara uppgifter");
            Console.WriteLine("ny                   skapa en ny uppgift");
            Console.WriteLine("ny /uppgift          skapa en ny uppgift med namnet /uppgift/");
            Console.WriteLine("redigera /uppgift/   redigera en uppgift med namnet /uppgift/");
            Console.WriteLine("kopiera /uppgift/    redigera en uppgift med namnet /uppgift/ till namnet /uppgift, 2/, kopian skall ha samma prioritet, men vara Active");
            Console.WriteLine("aktivera /uppgift    sätt status på uppgift till Active");
            Console.WriteLine("klar /uppgift/       sätt status på uppgift till Ready");
            Console.WriteLine("vänta /uppgift/      sätt status på uppgift till Waiting");
        }
    }
    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Välkommen till att-göra-listan!");
            Todo.PrintHelp();
            string[] command;
            string latestReadFile = string.Empty;
            do
            {
                command = MyIO.ReadCommand("> ");
                if (command[0] == "hjälp")
                    Todo.PrintHelp();
                else if (command[0] == "ladda")
                    if (command.Length > 1)
                    {
                        Todo.ReadListFromFile(command[1]);
                        latestReadFile = command[1];
                    }
                    else
                    {
                        Todo.ReadListFromFile("todo.lis");
                        latestReadFile = "todo.lis";
                    }
                else if (command[0] == "spara")
                    if (command.Length > 1)
                        Todo.SaveListToFile(command[1], latestReadFile);
                    else
                        Todo.SaveListToFile("todo.lis", latestReadFile);
                else if (command[0] == "sluta")
                {
                    Todo.SaveListToFile(latestReadFile, latestReadFile);
                    break;
                }
                else if (command[0] == "lista")
                    if (command.Length > 1)
                        Todo.PrintTodoList(verbose: true);
                    else
                        Todo.PrintTodoList(verbose: false);
                else
                    Console.WriteLine($"Okänt kommando: {command}");
            }
            while (true);
            Console.WriteLine("Hej då!");
        }
    }
    class MyIO
    {
        static public string[] ReadCommand(string prompt)
        {
            Console.Write(prompt);
            string[] command = Console.ReadLine().Split(" ");
            return command;
        }
        static public bool Equals(string rawCommand, string expected)
        {
            string command = rawCommand.Trim();
            if (command == "") return false;
            else
            {
                string[] cwords = command.Split(' ');
                if (cwords[0] == expected) return true;
            }
            return false;
        }
        static public bool HasArgument(string rawCommand, string expected)
        {
            string command = rawCommand.Trim();
            if (command == "") return false;
            else
            {
                string[] cwords = command.Split(' ');
                if (cwords.Length < 2) return false;
                if (cwords[1] == expected) return true;
            }
            return false;
        }
    }
}