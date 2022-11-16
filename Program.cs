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
            public TodoItem(int priority, string task, string taskDescription) //konstruktor manuell inmatning
            {
                this.status = Active;
                this.priority = priority;
                this.task = task;
                this.taskDescription = taskDescription;
            }
            public TodoItem(string todoLine) //konstruktor från fil
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
        public static void ReadListFromFile(string readFromFile) //Läser från fil och skapar objekt till en lista
        {
            Console.Write($"Läser från fil {readFromFile} ... ");
            list.Clear();
            using (StreamReader sr = new StreamReader(readFromFile))
            {
                int numRead = 0;
                string line;
                while (notNullOrEmpty(line = sr.ReadLine()))
                {
                    TodoItem item = new TodoItem(line);
                    list.Add(item);
                    numRead++;
                }
                Console.WriteLine($"Läste {numRead} rader.");
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
                Console.WriteLine("Du kan inte spara innan du ens laddat en fil");
        }
        public static void AddItemToList(string subject)
        {
            string task;
            if (subject != string.Empty)
            {
                task = subject;
            }
            else
            {
                Console.WriteLine("Uppgiftens namn: ");
                task = Console.ReadLine();
            }
            Console.WriteLine("Prioritet: ");
            int priority = int.Parse(Console.ReadLine());
            Console.WriteLine("Beskrivning: ");
            string taskDescription = Console.ReadLine();
            TodoItem item = new TodoItem(priority, task, taskDescription);
            list.Add(item);
        }
        public static void AmendItemInList(string[] command)
        {
            int index = IndexInList(command);
            SetTaskPriorityDescriptionFromConsole(out string task, out int priority, out string taskDescription);
            TodoItem amendedItem = new TodoItem(priority, task, taskDescription);
            amendedItem.status = list[index].status;
            list.Insert(index, amendedItem);
            list.RemoveAt(index + 1);
        }
        public static void CopyItemInList(string[] command)
        {
            int index = IndexInList(command);
            int priority = list[index].priority;
            string task = list[index].task + ", 2";
            string taskDescription = list[index].taskDescription;
            TodoItem item = new TodoItem(priority, task, taskDescription);
            list.Insert(index + 1, item);
        }
        private static void SetTaskPriorityDescriptionFromConsole(out string task, out int priority, out string taskDescription)
        {
            Console.WriteLine("Ange (nytt) namn: ");
            task = Console.ReadLine();
            Console.WriteLine("Ange (ny) prioritet: ");
            priority = int.Parse(Console.ReadLine());
            Console.WriteLine("Ange (ny) beskrivning: ");
            taskDescription = Console.ReadLine();
        }
        private static int IndexInList(string[] command)
        {
            string temp = string.Join(" ", command.Skip(1));
            int index = 0;
            foreach (TodoItem items in list)
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
                list[index].status = Todo.Active;
            else if (command[0] == "klar")
                list[index].status = Todo.Ready;
            else
                list[index].status = Todo.Waiting;
        }
        public static bool notNullOrEmpty(string line)
        {
            return (line == null || line == string.Empty) ? false : true;
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
        public static void PrintTodoList(bool verbose = false, int status = 0) //övergripande metod som skriver ut listan
        {
            PrintHead(verbose);
            foreach (TodoItem item in list)
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
                    if (command.Length > 1)
                        Todo.AddItemToList(command[1]);
                    else
                        Todo.AddItemToList("");
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
    class MyIO
    {
        static public string[] ReadCommand(string prompt)
        {
            Console.Write(prompt);
            string[] command = Console.ReadLine().Trim().Split(" ");
            return command;
        }
    }
}