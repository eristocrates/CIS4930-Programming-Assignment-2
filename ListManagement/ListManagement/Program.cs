using Library.ListManagement.helpers;
using ListManagement.models;
using ListManagement.services;
using Newtonsoft.Json;
using System.Windows;
using System2 = System;

namespace ListManagement // Note: actual namespace depends on the project name.
{
    public class Program
    {
        static void Main(string[] args)
        {
            ItemService itemService = ItemService.Current;
        
            var roamingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            roamingDirectory += "ListManagement\\";
            if (!Directory.Exists(roamingDirectory))
            {
                Directory.CreateDirectory(roamingDirectory);
            }
            var filePath = Path.Combine(roamingDirectory, "itemService.json");
            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };

            try
            {
                string jsonToRead = File.ReadAllText(filePath);
                itemService = JsonConvert.DeserializeObject<ItemService>(jsonToRead, settings);
            }
            catch (Exception)
            {
                itemService = ItemService.Current;
            }
            //var listNavigator = new ListNavigator<Item>(itemService.Items, 5);
            Console.WriteLine("Welcome to the List Management App");


            int input = -1;
                while (input != 7) //==
                {
            PrintMenu();
            while(!int.TryParse(Console.ReadLine(),out input)) {
                Console.WriteLine("User did not specify a valid int!");
            PrintMenu();
            }

                    if (input == 0)
                    {
                        Console.WriteLine("Enter String to search:");
                        var searchTerm = Console.ReadLine();
                        if (searchTerm == null)
                        {
                            Console.WriteLine("User did not specify a valid search");
                            break;
                        }
                        var searchResult = from item in itemService?.Items
                                           where item.ToString().Contains(searchTerm)
                                           select item; 
                        if (!searchResult.Any())
                        {
                            Console.WriteLine("No results found");
                        }
                        foreach (var item in searchResult)
                        {
                            Console.WriteLine(item.ToString());
                        }
                    }else if (input == 1)
                    {
                        //C - Create
                        //ask for property values

                        Item nextItem; //= new Item();
                        Console.WriteLine("1. Add ToDo Task");
                        Console.WriteLine("2. Add Calendar Appointment");
                        int typeSelection;
                        if(int.TryParse(Console.ReadLine(),out typeSelection))
                        {
                            if (typeSelection == 1)
                                nextItem = new ToDo();
                            else if (typeSelection == 2)
                                nextItem = new Appointment();
                            else
                            {
                                Console.WriteLine("User did not specify a valid int!");
                                continue;
                            }

                            FillProperties(nextItem); 
                            itemService.Add(nextItem);
                        }
                        else
                        {
                            Console.WriteLine("User did not specify a valid int!");
                        }

                    }
                    else if (input == 2)
                    {
                        //D - Delete/Remove
                        if (itemService.Items.Count() != 0)
                        {
                            Console.WriteLine("Which item should I delete?");
                            try
                            {
                                if (int.TryParse(Console.ReadLine(), out int selection))
                                {
                                    var selectedItem = itemService.Items[selection - 1];
                                    itemService.Remove(selectedItem);
                                }
                                else
                                {
                                    Console.WriteLine("Sorry, I can't find that item");
                                }
                            }
                            catch (ArgumentOutOfRangeException)
                            {
                                Console.WriteLine("Sorry, I can't find that item");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Sorry, no items to delete");
                        }
                    }
                    else if (input == 3)
                    {
                        //U - Update/Edit
                        if (itemService.Items.Count() != 0)
                        {
                            Console.WriteLine("Which item should I edit?");
                            try
                            {
                                if (int.TryParse(Console.ReadLine(), out int selection))
                                {
                                    var selectedItem = itemService.Items[selection - 1];

                                    if (selectedItem != null)
                                    {
                                        FillProperties(selectedItem);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Sorry, I can't find that item!");
                                }
                            }
                            catch (ArgumentOutOfRangeException)
                            {
                                    Console.WriteLine("Sorry, I can't find that item!");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Sorry, no items to edit");
                        }
                    }
                    else if (input == 4)
                    {
                        //Complete Task
                        if (itemService.Items.Count() != 0)
                        {
                            Console.WriteLine("Which item should I complete?");
                            try
                            {
                                if (int.TryParse(Console.ReadLine(), out int selection))
                                {
                                    var selectedItem = itemService.Items[selection - 1] as ToDo;

                                    if (selectedItem != null)
                                    {
                                        selectedItem.IsCompleted = true;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Sorry, I can't find that item!");
                                }
                            }
                            catch (ArgumentOutOfRangeException)
                            {
                                Console.WriteLine("Sorry, I can't find that item!");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Sorry, no items to complete");
                        }
                    }
                    else if(input ==5)
                    {
                        //R - Read / List uncompleted tasks

                        //use LINQ
                        itemService.Items
                            .Where(i => !((i as ToDo)?.IsCompleted ?? true))
                            .ToList()
                            .ForEach(Console.WriteLine);

                    } else if (input ==6)
                    {
                        //R - Read / List all tasks
                        //itemService.Items.ForEach(Console.WriteLine);
                        var userSelection = string.Empty;
                        while(userSelection?.ToUpper() != "E")
                        {
                            if (itemService.Items.Count() != 0)
                            {
                                try
                                {
                                    foreach (var item in itemService.GetPage())
                                    {
                                        Console.WriteLine(item);
                                        /*
                                        Console.WriteLine(item.Key);
                                        Console.WriteLine(item.Value.Name);
                                        Console.WriteLine(item.Value.Description);
                                        */
                                    }
                                    userSelection = Console.ReadLine();

                                    if (userSelection?.ToUpper() == "N")
                                    {
                                        itemService.NextPage();
                                    }
                                    else if (userSelection?.ToUpper() == "P")
                                    {
                                        itemService.PreviousPage();
                                    }
                                }
                                catch (PageFaultException e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                            } else
                            {
                                Console.WriteLine("List is empty.");
                                break;
                            }
                        }


                    }
                    else if (input == 7)
                    {

                    }
                    else
                    {
                        Console.WriteLine("I don't know what you mean");
                    }

                }
            

            //Console.ReadLine();
            string jsonToWrite = JsonConvert.SerializeObject(itemService, settings);
            File.WriteAllText(filePath, jsonToWrite);
        }

        public static void PrintMenu()
        {
            Console.WriteLine("0. Search");
            Console.WriteLine("1. Add Item");
            Console.WriteLine("2. Delete Item");
            Console.WriteLine("3. Edit Item");
            Console.WriteLine("4. Complete Item");
            Console.WriteLine("5. List Outstanding");
            Console.WriteLine("6. List All");
            Console.WriteLine("7. Exit");
        }

        public static void FillProperties(Item item)
        {
            Console.WriteLine("Give me a Name");
            item.Name = Console.ReadLine();
            Console.WriteLine("Give me a Description");
            item.Description = Console.ReadLine()?.Trim();
            var newTodo = item as ToDo;
            var newAppointment = item as Appointment;
            if (newTodo != null)
            {
                Console.Write("Enter a Deadline: ");
                try
                {
                    newTodo.Deadline = DateTime.Parse(Console.ReadLine().Trim());
                }
                catch (Exception)
                {
                    newTodo.Deadline = DateTime.Today;
                }
            }else if (newAppointment != null)
            {
                try
                {
                    Console.Write("Enter a Start Time: ");
                    newAppointment.Start = DateTime.Parse(Console.ReadLine().Trim());
                }
                catch (Exception)
                {
                    newAppointment.Start = DateTime.Today;

                }
                try
                {
                    Console.Write("Enter a Stop Time: ");
                    newAppointment.Stop = DateTime.Parse(Console.ReadLine().Trim());
                }
                catch (Exception)
                {
                    newAppointment.Stop = DateTime.Today.AddDays(7);
                }
                try
                {
                    Console.Write("Enter an attendee (Enter done when finished): ");
                        var attendee = string.Empty;
                    while (attendee?.ToLower() != "done")
                    { 
                        attendee = Console.ReadLine()?.Trim();
                        if(attendee != null && attendee != "done")
                        {
                            AddString(newAppointment.Attendees, attendee);
                        } 
                    }
                }
                catch (Exception)
                {
                            AddString(newAppointment.Attendees, "");
                }
            }
        }

        public static void AddString(List<string> strList, string str)
        {
            strList.Add(str);
        }

        
    }
}