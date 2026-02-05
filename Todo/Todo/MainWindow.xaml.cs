using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.Json;
using System.IO;


namespace Todo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<TodoItem> _todos = new List<TodoItem>();
        string _bestandPad = "Todos.json";
        

        public MainWindow()
        {
            InitializeComponent();
            deadlineDatepicker.SelectedDate = DateTime.Today;
            omschrijvingChangeStackPanel.Visibility = Visibility.Collapsed;
            
            
            // Zet het opgeslagen bestand om naar een list van todos 
            // TODO: logica verplaatsen naar aparte klasse            
            string json = File.ReadAllText(_bestandPad);
            _todos = JsonSerializer.Deserialize<List<TodoItem>>(json) ?? new List<TodoItem>();


            AssignTodos();
        }

        private void addTodoButton_Click(object sender, RoutedEventArgs e)
        {
            //alle inputs moeeten ingevuld zijn voor een todo kan toegevoegd worden
            if (!string.IsNullOrWhiteSpace(nameTextBox.Text) 
                && statusComboBox.SelectedIndex != -1 
                && deadlineDatepicker.SelectedDate > DateTime.Today 
                && !string.IsNullOrWhiteSpace(omschrijvingTextBox.Text))
            {
                TodoItem newTodoItem = new TodoItem();
                newTodoItem.Title = nameTextBox.Text;
                newTodoItem.Description = omschrijvingTextBox.Text;
                newTodoItem.DueDate = (DateTime)deadlineDatepicker.SelectedDate;

                //var selectedSatus = (ComboBoxItem)statusComboBox.SelectedItem;
                //string status = selectedSatus.Content.ToString();
                newTodoItem.Status = (TodoStatus)statusComboBox.SelectedIndex;

                _todos.Add(newTodoItem);
                AssignTodos();
            }
            else
            {
                MessageBox.Show("Vul alle velden in en selecteer een datum in de toekomst");
            }
        }

        private void AssignTodos()
        {
            SaveList();
            ClearListBoxes();
            //Vult de listboxes
            foreach (TodoItem todo in _todos)
            {
                switch (todo.Status.ToString().ToLower())
                {
                    case "todo":
                        todoListBox.Items.Add(todo);
                        break;
                    case "inprogress":
                        doingListBox.Items.Add(todo);
                        break;
                    case "done":
                        doneListBox.Items.Add(todo);
                        break;
                }
            } 
        }

        //TODO: logica verplaatsen naar aparte klasse
        private void SaveList()
        {
            string jsonOpslaan = JsonSerializer.Serialize(_todos, new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText(_bestandPad, jsonOpslaan);
        }

        private void ClearListBoxes()
        {
            todoListBox.Items.Clear();
            doingListBox.Items.Clear();
            doneListBox.Items.Clear();
        }

        //private void ShowInfo(ListBox sender)
        //{
        //    if (sender.SelectedItem == null)
        //        return;

        //    Todo _todo = (TodoItem)sender.SelectedItem;
        //    infoTextBlock.Text = $"deadline: {_todo.DueDate}\n\n{_todo.Description}";
        //}
        private void ShowInfo(TodoItem todo)
        {
            if (todo == null)
                return;

            infoTextBlock.Text = $"deadline: {todo.DueDate}\n\n{todo.Description}";
        }

        private void AddToListBox(TodoItem todo, TodoStatus status)
        {
            todo.Status = status;
            SaveList();
            AssignTodos();
        }

        // TODO: Dit werkt niet 100% correct, drag&drop zou beter werken in dit scenario
        private void ListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //deselecteerd de andere listboxitems
            if (sender == todoListBox)
            {
                doingListBox.SelectedItem = null;
                doneListBox.SelectedItem = null;
            }
            else if (sender == doneListBox)
            {
                doingListBox.SelectedItem = null;
                todoListBox.SelectedItem = null;
            }
            else
            {
                todoListBox.SelectedItem = null;
                doneListBox.SelectedItem = null;
            }
            ShowInfo(((ListBox)sender).SelectedItem as TodoItem);
        }


        private void AddToButton_Click(object sender, RoutedEventArgs e)
        {
            //Als op de sender geklikt wordt status van de todo gewijzigd en in de juiste listbox gezet
            if (sender == todoAddButton)
            {
                if (doingListBox.SelectedItem != null)
                {
                    AddToListBox((TodoItem)doingListBox.SelectedItem, TodoStatus.Todo);
                }
                else if (doneListBox.SelectedItem != null)
                {
                    AddToListBox((TodoItem)doneListBox.SelectedItem, TodoStatus.Todo);
                }
            }
            else if (sender == doingAddButton)
            {
                if (todoListBox.SelectedItem != null)
                {
                    AddToListBox((TodoItem)todoListBox.SelectedItem, TodoStatus.InProgress);
                }
                else if (doneListBox.SelectedItem != null)
                {
                    AddToListBox((TodoItem)doneListBox.SelectedItem, TodoStatus.InProgress);
                }
            }
            else
            {
                if (todoListBox.SelectedItem != null)
                {
                    AddToListBox((TodoItem)todoListBox.SelectedItem, TodoStatus.Done);
                }
                else if (doingListBox.SelectedItem != null)
                {
                    AddToListBox((TodoItem)doingListBox.SelectedItem, TodoStatus.Done);
                }
            }
        }

        private void DeleteTodo_Click(object sender, RoutedEventArgs e)
        {
            //Verwijdert het item uit de geselecteerde listbox
            if(todoListBox.SelectedItem != null)
            {
                _todos.Remove((TodoItem)todoListBox.SelectedItem);
            }
            else if(doingListBox.SelectedItem != null)
            {
                _todos.Remove((TodoItem)doingListBox.SelectedItem);
            }
            else if(doneListBox.SelectedItem != null)
            {
                _todos.Remove((TodoItem)doneListBox.SelectedItem);
            }
            AssignTodos();
        }

        private void Omschrijving_Click(object sender, RoutedEventArgs e)
        {
            if (todoListBox.SelectedItem != null || doingListBox.SelectedItem != null || doneListBox.SelectedItem != null)
            {
                omschrijvingChangeStackPanel.Visibility = Visibility.Visible;
                omschrijvingChangeTextBox.Text = "";
            }
        }

        private void cancelbutton_Click(object sender, RoutedEventArgs e)
        {
            omschrijvingChangeStackPanel.Visibility = Visibility.Collapsed;
        }

        private void ChangeOmschrijvingTodo(TodoItem todo)
        {
            todo.Description = omschrijvingChangeTextBox.Text;
            AssignTodos();
        }

        private void changebutton_Click(object sender, RoutedEventArgs e)
        {
            if (todoListBox.SelectedItem != null)
            {
                ChangeOmschrijvingTodo((TodoItem)todoListBox.SelectedItem);
            }
            else if (doingListBox.SelectedItem != null)
            {
                ChangeOmschrijvingTodo((TodoItem)doingListBox.SelectedItem);
            }
            else if (doneListBox.SelectedItem != null)
            {
                ChangeOmschrijvingTodo((TodoItem)doneListBox.SelectedItem);
            }
            omschrijvingChangeStackPanel.Visibility = Visibility.Collapsed;
        }
    }
}