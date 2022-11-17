namespace Tenta
{
    public class Todo
    {
        private static List<TodoItem> listOfObjects = new List<TodoItem>();

        private const int Active = 1;
        private const int Waiting = 2;
        private const int Ready = 3;
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
            public TodoItem(int priority, string task, string taskDescription)
            {
                this.status = Active;
                this.priority = priority;
                this.task = task;
                this.taskDescription = taskDescription;
            }
            public TodoItem(string todoLine)
            {
                string[] field = todoLine.Split('|');
                status = int.Parse(field[0]);
                priority = int.Parse(field[1]);
                task = field[2];
                taskDescription = field[3];
            }
            public void Print(bool verbose = false)
            {
                string statusString = StatusToString(status);
                Console.Write($"|{statusString,-12}|{priority,-6}|{task,-20}|");
                if (verbose)
                    Console.WriteLine($"{taskDescription,-40}|");
                else
                    Console.WriteLine();
            }
        }
        public static void ReadListFromFile(string[] command)
        {
            string fileName;
            if (command.Length > 1)
                fileName = command[1];
            else
                fileName = "todo.lis";
            Console.Write($"Läser från fil {fileName} ... ");
            listOfObjects.Clear();
            using (StreamReader sr = new StreamReader(fileName))
            {
                int index = 0;
                string line;
                while (!IsNullOrEmpty(line = sr.ReadLine()))
                {
                    TodoItem item = new TodoItem(line);
                    listOfObjects.Add(item);
                    index++;
                }
                Console.WriteLine($"Läste {index} rader.");
            }
        }
        private static bool IsNullOrEmpty(string line)
        {
            return (line == null || line == string.Empty);
        }
        public static void SaveListToFile(string[] command, string LastReadFile)
        {
            if (LastReadFile != string.Empty)
            {
                string fileName;
                if (command.Length < 2)
                    fileName = LastReadFile;
                else
                    fileName = command[1];
                using (StreamWriter sw = new StreamWriter(fileName))
                {
                    int index = 0;
                    foreach (TodoItem item in listOfObjects)
                    {
                        sw.WriteLine($"{item.status}|{item.priority}|{item.task}|{item.taskDescription}");
                        index++;
                    }
                    Console.WriteLine($"Sparar till {fileName} ... Sparade {index} uppgifter.");
                }
            }
            else if (command[0] == "spara")
                Console.WriteLine("Du kan inte spara innan du ens laddat en fil");
        }
        public static void EditItemInList(string[] command)
        {
            int index = IndexInList(command);
            if (index != -1)
            {
                SetTaskPrioDescFromConsole(taskInCommand: false, out string task, out int priority, out string taskDescription);
                TodoItem amendedItem = new TodoItem(priority, task, taskDescription);
                amendedItem.status = listOfObjects[index].status;
                listOfObjects.Insert(index, amendedItem);
                listOfObjects.RemoveAt(index + 1);
            }
            else
                Console.WriteLine("Uppgiften finns inte i listan");
        }
        public static void CopyItemInList(string[] command)
        {
            int index = IndexInList(command);
            if (index != -1)
            {
                int priority = listOfObjects[index].priority;
                string task = listOfObjects[index].task + ", 2";
                string taskDescription = listOfObjects[index].taskDescription;
                TodoItem item = new TodoItem(priority, task, taskDescription);
                listOfObjects.Insert(index + 1, item);
            }
            else
                Console.WriteLine("Uppgiften finns inte i listan");
        }
        public static void AddItemToList(string[] command)
        {
            bool taskInCommand;
            string taskName = string.Join(" ", command.Skip(1));
            if (command.Length < 2)
                taskInCommand = false;
            else
                taskInCommand = true;
            SetTaskPrioDescFromConsole(taskInCommand, out string task, out int priority, out string taskDescription);
            if (taskInCommand)
                task = taskName;
            TodoItem item = new TodoItem(priority, task, taskDescription);
            listOfObjects.Add(item);
        }
        private static void SetTaskPrioDescFromConsole(bool taskInCommand, out string task, out int priority, out string taskDescription)
        {
            task = string.Empty;
            if (!taskInCommand)
            {
                Console.WriteLine("Ange namn: ");
                task = Console.ReadLine();
            }
            Console.WriteLine("Ange prioritet: ");
            priority = int.Parse(Console.ReadLine());
            Console.WriteLine("Ange beskrivning: ");
            taskDescription = Console.ReadLine();
        }
        private static int IndexInList(string[] command)
        {
            string taskName = string.Join(" ", command.Skip(1));
            int index = 0;
            bool itemFound = false;
            foreach (TodoItem items in listOfObjects)
            {
                if (items.task == taskName)
                {
                    itemFound = true;
                    break;
                }
                index++;
            }
            if (!itemFound)
                index = -1;
            return index;
        }
        public static void ChangeStatus(string[] command)
        {
            int index = IndexInList(command);
            try
            {
                switch (command[0])
                {
                    case "aktivera": listOfObjects[index].status = Active; break;
                    case "klar": listOfObjects[index].status = Ready; break;
                    default: listOfObjects[index].status = Waiting; break;
                }
            }
            catch (System.ArgumentOutOfRangeException)
            {
                Console.WriteLine($"Kommandot ej korrekt skrivet tillsammans med uppgift från listan.");
            }
        }
        public static void PrintTodoList(bool verbose, string[] command)
        {
            string commandString = string.Join(" ", command);
            int status = -1;
            switch (commandString)
            {
                case "lista": status = Active; break;
                case "beskriv": status = Active; break;
                case "lista väntande": status = Waiting; break;
                case "lista klara": status = Ready; break;
                case "lista allt": status = 0; break;
                case "beskriv allt": status = 0; break;
            }
            if (status != -1)
            {
                PrintHead(verbose);
                foreach (TodoItem item in listOfObjects)
                {
                    if (status == 0)
                    {
                        item.Print(verbose);
                    }
                    else if (item.status == status)
                    {
                        item.Print(verbose);
                    }
                }
                PrintFoot(verbose);
            }
            else
                Console.WriteLine($"Okänt kommando: {string.Join(" ", command)}");
        }
        private static void PrintHead(bool verbose)
        {
            PrintHeadOrFoot(head: true, verbose);
        }
        private static void PrintFoot(bool verbose)
        {
            PrintHeadOrFoot(head: false, verbose);
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
            Console.WriteLine("ny /uppgift/         skapa en ny uppgift med namnet /uppgift/");
            Console.WriteLine("redigera /uppgift/   redigera en uppgift med namnet /uppgift/");
            Console.WriteLine("kopiera /uppgift/    redigera en uppgift med namnet /uppgift/ till namnet /uppgift, 2/, kopian skall ha samma prioritet, men vara Active");
            Console.WriteLine("aktivera /uppgift/   sätt status på uppgift till Active");
            Console.WriteLine("klar /uppgift/       sätt status på uppgift till Ready");
            Console.WriteLine("vänta /uppgift/      sätt status på uppgift till Waiting");
        }
    }
    class MainClass
    {
        private static string[] ReadCommand(string prompt)
        {
            Console.WriteLine();
            Console.Write(prompt);
            string[] command = Console.ReadLine().Trim().Split(" ");
            return command;
        }
        public static void Main(string[] args)
        {
            string[] command;
            string lastReadFile = string.Empty;
            Console.WriteLine("Välkommen till att-göra-listan!");
            Todo.PrintHelp();
            do
            {
                command = ReadCommand("> ");
                switch (command[0])
                {
                    case "hjälp": Todo.PrintHelp(); break;
                    case "ladda":
                        Todo.ReadListFromFile(command);
                        if (command.Length > 1)
                            lastReadFile = command[1];
                        else
                            lastReadFile = "todo.lis";
                        break;
                    case "spara": Todo.SaveListToFile(command, lastReadFile); break;
                    case "sluta": Todo.SaveListToFile(command, lastReadFile); break;
                    case "beskriv": Todo.PrintTodoList(verbose: true, command); break;
                    case "lista": Todo.PrintTodoList(verbose: false, command); break;
                    case "ny": Todo.AddItemToList(command); break;
                    case "redigera": Todo.EditItemInList(command); break;
                    case "kopiera": Todo.CopyItemInList(command); break;
                    case "aktivera": Todo.ChangeStatus(command); break;
                    case "klar": Todo.ChangeStatus(command); break;
                    case "vänta": Todo.ChangeStatus(command); break;
                    default: Console.WriteLine($"Okänt kommando: {string.Join(" ", command)}"); break;
                }
            }
            while (command[0] != "sluta");
            Console.WriteLine("Hej då!");
        }
    }
}
