namespace Tenta
{
    public class Todo
    {
        public static List<TodoItem> listOfObjects = new List<TodoItem>();

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
        public static void ReadListFromFile(string fileName)
        {
            Console.Write($"Läser från fil {fileName} ... ");
            listOfObjects.Clear();
            using (StreamReader sr = new StreamReader(fileName))
            {
                int index = 0;
                string line;
                while (notNullOrEmpty(line = sr.ReadLine()))
                {
                    TodoItem item = new TodoItem(line);
                    listOfObjects.Add(item);
                    index++;
                }
                Console.WriteLine($"Läste {index} rader.");
            }
        }
        private static bool notNullOrEmpty(string line)
        {
            return (line == null || line == string.Empty) ? false : true;
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
        public static void AmendItemInList(string[] command)
        {
            int index = IndexInList(command);
            SetTaskPriorityDescriptionFromConsole(taskInCommand: true, out string task, out int priority, out string taskDescription);
            TodoItem amendedItem = new TodoItem(priority, task, taskDescription);
            amendedItem.status = listOfObjects[index].status;
            listOfObjects.Insert(index, amendedItem);
            listOfObjects.RemoveAt(index + 1);
        }
        public static void CopyItemInList(string[] command)
        {
            int index = IndexInList(command);
            int priority = listOfObjects[index].priority;
            string task = listOfObjects[index].task + ", 2";
            string taskDescription = listOfObjects[index].taskDescription;
            TodoItem item = new TodoItem(priority, task, taskDescription);
            listOfObjects.Insert(index + 1, item);
        }
        public static void AddItemToList(string[] command)
        {
            bool taskInCommand;
            string temp = string.Join(" ", command.Skip(1));
            if (command.Length < 2)
                taskInCommand = false;
            else
                taskInCommand = true;
            SetTaskPriorityDescriptionFromConsole(taskInCommand, out string task, out int priority, out string taskDescription);
            if (taskInCommand)
                task = temp;
            TodoItem item = new TodoItem(priority, task, taskDescription);
            listOfObjects.Add(item);
        }
        private static void SetTaskPriorityDescriptionFromConsole(bool taskInCommand, out string task, out int priority, out string taskDescription)
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
            string temp = string.Join(" ", command.Skip(1));
            int index = 0;
            foreach (TodoItem items in listOfObjects)
            {
                if (items.task == temp)
                    break;
                index++;
            }
            return index;
        }
        public static void ChangeStatus(string[] command)
        {
            int index = IndexInList(command);
            if (command[0] == "aktivera")
                listOfObjects[index].status = Todo.Active;
            else if (command[0] == "klar")
                listOfObjects[index].status = Todo.Ready;
            else
                listOfObjects[index].status = Todo.Waiting;
        }
        public static void PrintTodoList(bool verbose = false, int status = 0)
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
        public static void Main(string[] args)
        {
            Console.WriteLine("Välkommen till att-göra-listan!");
            Todo.PrintHelp();
            string[] command;
            string LastReadFile = string.Empty;
            do
            {
                command = MyIO.ReadCommand("> ");
                if (command[0] == "hjälp")
                    Todo.PrintHelp();
                else if (command[0] == "ladda")
                    if (command.Length > 1)
                    {
                        Todo.ReadListFromFile(command[1]);
                        LastReadFile = command[1];
                    }
                    else
                    {
                        Todo.ReadListFromFile("todo.lis");
                        LastReadFile = "todo.lis";
                    }
                else if (command[0] == "spara")
                    Todo.SaveListToFile(command, LastReadFile);
                else if (command[0] == "sluta")
                {
                    Todo.SaveListToFile(command, LastReadFile);
                    break;
                }
                else if (command[0] == "beskriv")
                    if (command.Length > 1)
                        Todo.PrintTodoList(verbose: true);
                    else
                        Todo.PrintTodoList(verbose: true, status: Todo.Active);
                else if (command[0] == "lista")
                    if (command.Length < 2)
                        Todo.PrintTodoList(verbose: false, status: Todo.Active);
                    else if (command[1] == "väntande")
                        Todo.PrintTodoList(verbose: false, status: Todo.Waiting);
                    else if (command[1] == "avklarad")
                        Todo.PrintTodoList(verbose: false, status: Todo.Ready);
                    else
                        Todo.PrintTodoList(verbose: false);
                else if (command[0] == "ny")
                    Todo.AddItemToList(command);
                else if (command[0] == "redigera")
                    Todo.AmendItemInList(command);
                else if (command[0] == "kopiera")
                    Todo.CopyItemInList(command);
                else if (command[0] == "aktivera" || command[0] == "klar" || command[0] == "vänta")
                    Todo.ChangeStatus(command);
                else
                    Console.WriteLine($"Okänt kommando: {string.Join(" ", command)}");
            }
            while (true);
            Console.WriteLine("Hej då!");
        }
    }
    public class MyIO
    {
        public static string[] ReadCommand(string prompt) //kanske lägga tillbaka till egen klass
        {
            Console.Write(prompt);
            string[] command = Console.ReadLine().Trim().Split(" ");
            return command;
        }
    }
}
